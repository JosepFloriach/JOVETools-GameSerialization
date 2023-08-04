using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileDataHandler<T> where T : IGameData
{
    private string fileDirPath = "";
    private string fileName = "";

    public FileDataHandler(string fileDirPath, string fileName)
    {
        this.fileDirPath = fileDirPath;
        this.fileName = fileName;
    }

    public IGameData Load()
    {
        string fullPathFile = Path.Combine(fileDirPath, fileName);
        IGameData data = null;
        if (File.Exists(fullPathFile))
        {
            try
            {
                string dataJSon = "";
                using (FileStream stream = new(fullPathFile, FileMode.Open))
                {
                    using (StreamReader reader = new(stream))
                    {
                        dataJSon = reader.ReadToEnd();
                    }
                }

                data = JsonUtility.FromJson<T>(dataJSon);
            }
            catch (Exception e)
            {
                throw new System.Exception("Error while trying to load data from file: " + fullPathFile + "\n" + e);
            }
        }
        return data;
    }

    public void SaveData(IGameData data)
    {
        string fullPathFile = Path.Combine(fileDirPath, fileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPathFile));
            string dataJson = JsonUtility.ToJson(data, true);
            using (FileStream stream = new(fullPathFile, FileMode.Create))
            {
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(dataJson);
                }
            }
        }
        catch (Exception e)
        {
            throw new System.Exception("Error while trying to save data into file: " + fullPathFile + "\n" + e);
        }
    }

}
