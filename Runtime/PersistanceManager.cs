using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace jovetools.gameserialization
{
    /// <summary>
    /// Central script for Game Serialization. This class contains all required methods for creating, loading, saving and deleting game data.
    /// The class works as a singleton so it just need to be called through the Instance property to access it, by providing an implementation
    /// of IGameData as generic type. 
    /// 
    /// First step should always be to call Init function by providing a dataHandler. The DataHandler will be the responsible to process 
    /// (load an save) data in the prefered format (xml, json, inserting into a data base...). For now just Json data is allowed. But more data
    /// handlers are planned for development. In any case, a custom implementation can be done by implementing IDataHandler interface.
    /// 
    /// After Initializing, next step should be to register ISerializable implementations. ISerializable types must implement a bunch of methods 
    /// that will allow PersistanceManager to collect and provide information for serialization. Those ISerializable implementations are the scripts
    /// that will contain the data that will be used in the client application.
    /// 
    /// After these two steps the PersistanceManager will be ready to create game data through NewGame(), load data through LoadGame(), save data through
    /// SaveGame(), and delete data through DeleteGame(). Also, some events are provided so the client can subscribe to each of those actions and execute
    /// client code on demand.
    /// 
    /// Have in mind that LoadingGame without creating a new one, will create a new one before anything else (by calling NewGame()).
    /// 
    /// </summary>
    public class PersistanceManager<T> where T : IGameData, new()
    {
        private static PersistanceManager<T> instance;
        
        /// <summary>        
        /// Singleton instance of PersistanceManager.
        /// </summary>
        public static PersistanceManager<T> Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PersistanceManager<T>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Called after deleting game data.
        /// </summary>
        public static event Action DataReseted;
        /// <summary>
        /// Called after creating game data.
        /// </summary>
        public static event Action DataCreated;
        /// <summary>
        /// Called after loading game data.
        /// </summary>
        public static event Action DataLoaded;
        /// <summary>
        /// Called after saving game data.
        /// </summary>
        public static event Action DataSaved;

        private IGameData gameData;
        private IDataHandler dataHandler;
        private List<ISerializable> serializableObjects = new();

        /// <summary>
        /// Initializes PersistanceManager by providing a data handler. This data handler will be the responsible one
        /// to save and load data in the corresponding format or platform (file, database, or some kind of external service).
        /// </summary>
        /// <param name="dataHandler">IDatahandler used for managing the data serialization.</param>
        public void Init(IDataHandler dataHandler)
        {
            this.dataHandler = dataHandler;
        }

        /// <summary>
        /// Register a ISerializable to PersistanceManager. This needs to be done for any script that wants to be subscribed 
        /// to main PersistanceManager methods (create, save, load or delete). Only registered ISerilizables will be able to 
        /// collect and provide data from/for PersistanceManager.
        /// </summary>
        /// <param name="serializable">ISerializable implementation to be registered.</param>
        public void RegisterSerializableObject(ISerializable serializable)
        {
            serializableObjects.Add(serializable);
        }

        /// <summary>
        /// Deregister a ISerializable from PersistanceManager. After this, that script won't be able to collect or provide any 
        /// data from PersistanceManager until is not registered again.
        /// </summary>
        /// <param name="serializable">ISerializable implementation to be unregistered.</param>
        public void DeregisterSerializableObject(ISerializable serializable)
        {
            serializableObjects.Remove(serializable);
        }

        /// <summary>
        /// Deregister all registered ISerializables. After this, nothing will be processed on game data serialization.
        /// </summary>        
        public void DeregisterAllSerializables()
        {
            serializableObjects.Clear();
        }

        /// <summary>
        /// Creates a new game. CreateData will be called in all registered ISerializables so any data can be feed into the
        /// new IGameData created and provided by reference. DataCreated will be invoked at after creation.
        /// </summary>
        public void NewGame()
        {
            this.gameData = new T();
            foreach (ISerializable obj in serializableObjects)
            {
                obj.CreateData(ref gameData);
            }
            DataCreated?.Invoke();
        }

        /// <summary>
        /// Saves the current state of the game. SaveData will be called in all registered ISerializables so any data can be feed into 
        /// the IGameData provided by reference and being managed by the PersistanceManager. If there is no game created previously, or if 
        /// data is not valid, it will throw an Exception. DataSaved will be invoked after saving.
        /// </summary>
        public void SaveGame()
        {
            if (gameData == null)
            {
                throw new Exception("Trying to save non-existing game data. Call NewGame or LoadGame before");
            }

            foreach (ISerializable obj in serializableObjects)
            {
                obj.SaveData(ref gameData);
            }
            
            if (!gameData.ValidateData())
            {
                throw new Exception("Corrupted game data. No data was saved.");
            }

            dataHandler.Save(gameData);
            DataSaved?.Invoke();
        }

        /// <summary>
        /// Loads the saved state into the game. LoadData will be called in all registered ISerializables so they can load any data needed.
        /// If there is no game created previously, a new game will be created by default. Otherwise it will load the one already created. 
        /// If data is not valid it will throw an Exception. DataLoaded will be invoked after saving.
        /// </summary>
        public void LoadGame()
        {
            gameData = dataHandler.Load();
            if (gameData == null)
            {
                NewGame();
            }

            if (!gameData.ValidateData())
            {
                throw new Exception("Corrupted game data from file. No data was loaded.");
            }

            foreach (ISerializable obj in serializableObjects)
            {
                obj.LoadData(gameData);
            }
            DataLoaded?.Invoke();
        }

        /// <summary>
        /// Deletes all game data. ClearData will be called in all registered ISerializables so they can reset any data needed.
        /// If there is no game created previously, an exception will be thrown. DataReseted will be called after deletion.
        /// </summary>
        public void DeleteGame()
        {
            if (gameData == null)
            {
                throw new Exception("Trying to delete non-existing game data. Call NewGame or LoadGame before");
            }

            foreach (ISerializable obj in serializableObjects)
            {
                obj.ClearData(gameData);
            }
            SaveGame();
            DataReseted?.Invoke();
        }
    }
}