using jovetools.gameserialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataHandler
{
    IGameData Load();
    void Save(IGameData data);
}
