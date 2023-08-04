using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a type that holds all the data of the progress game.
/// </summary>
public interface IGameData
{
    /// <summary>
    /// Validates the data. Used to make sure that anything corrupted is loaded 
    /// or saved.
    /// </summary>
    /// <returns>True if the data is consistent. False otherwise.</returns>
    bool ValidateData();
}
