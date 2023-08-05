using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Persistance manager allows the client to serialize game data. For now just into a JSon file. Future development on that is planned to allow
/// several files (to have different saves), and in different formats and outputs (like xml or in a DDBB). A serializable object can be any 
/// piece of code that holds some data that needs to be serialized. For example, let's assume a monobehaviour that holds the total amount of 
/// money that player has. Let's call it CurrencyController.
/// 
/// The client needs to inherit from the ISerializable interface on CurrencyController and implement the required methods (more info on ISerializable).
/// Then, all ISerializable objects need to be registered into PersistanceManager through RegisterSerializableObject method. 
/// Each of these methods will be called automatically by PersistanceManager.
/// 
/// Usage: first of all, just call the Init function specifing the output file. Then, register serializable objects
/// </summary>
/// <typeparam name="T"></typeparam>
public class PersistanceManager<T> where T : IGameData, new()
{
    private static PersistanceManager<T> instance;
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

    public static event Action DataReseted;
    public static event Action DataCreated;
    public static event Action DataLoaded;
    public static event Action DataSaved;

    private IGameData gameData;
    private FileDataHandler<T> fileDataHandler;

    private List<ISerializable> serializableObjects = new();

    public void Init(string path, string fileName)
    {
        fileDataHandler = new FileDataHandler<T>(path, fileName);
    }

    public void RegisterSerializableObject(ISerializable serializable)
    {
        serializableObjects.Add(serializable);
    }

    public void DeregisterSerializableObject(ISerializable serializable)
    {
        serializableObjects.Remove(serializable);
    }

    public void NewGame()
    {
        this.gameData = new T();
        foreach (ISerializable obj in serializableObjects)
        {
            obj.CreateData(ref gameData);
        }
        DataCreated?.Invoke();
    }

    public void SaveGame()
    {       
        foreach (ISerializable obj in serializableObjects)
        {
            obj.SaveData(ref gameData);
        }
        
        if (!gameData.ValidateData())
        {
            throw new Exception("Corrupted game data. No data was saved.");
        }

        fileDataHandler.SaveData(gameData);
        DataSaved?.Invoke();
    }

    public void LoadGame()
    {
        gameData = fileDataHandler.Load();        
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

    public void DeleteGame()
    {
        foreach (ISerializable obj in serializableObjects)
        {
            obj.ClearData(gameData);
        }
        SaveGame();
        DataReseted?.Invoke();
    }
}
