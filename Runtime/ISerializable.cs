using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace jovetools.gameserialization
{
    /// <summary>
    /// Represents a type for which some data can be serialized into a IGameData. 
    /// </summary>
    public interface ISerializable
    {
        /// <summary>
        /// Creates the data for the provided IGameData with the data object. This will be called just in the case there is
        /// no data created yet.
        /// </summary>
        /// <param name="data">IGameData where to save all the data</param>
        void CreateData(ref IGameData data);

        /// <summary>
        /// Loads the data into the current ISerializable object. 
        /// </summary>
        /// <param name="data">The IGameData that holds all game data</param>
        void LoadData(IGameData data);

        /// <summary>
        /// Save the data into the provided IGameData. 
        /// </summary>
        /// <param name="data">IGameData where to save all the data</param>
        void SaveData(ref IGameData data);

        /// <summary>
        /// Resets all data for this ISerializable. Use this to reset game data in memory (this won't affect data in disk).
        /// </summary>
        void ClearData(IGameData data);
    }
}