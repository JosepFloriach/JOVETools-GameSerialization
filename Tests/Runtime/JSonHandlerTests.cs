using jovetools.gameserialization;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class JSonHandlerTests
{
    private class TestGameData : IGameData
    {
        public int Value1 = 1;
        public int Value2 = 2;

        public bool ValidateData()
        {
            return true;
        }
    }

    private JsonHandler<TestGameData> handler;

    [Test] 
    public void LoadNonExistingFile_Expect_Null()
    {
        handler = new JsonHandler<TestGameData>("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/", "NoExisting.json");
        IGameData data = handler.Load();
        Assert.IsNull(data);
    }

    [Test]
    public void LoadHealthyJson_Expect_DataLoaded()
    {
        handler = new JsonHandler<TestGameData>("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/", "Healthy.json");
        IGameData data = handler.Load();
        Assert.NotNull(data);
    }

    [Test]
    public void LoadMalformattedJson_Expect_Exception()
    {        
        handler = new JsonHandler<TestGameData>("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/", "Malformatted.json");
        Assert.Throws<System.Exception>(() => handler.Load());
    }

    [Test]
    public void SaveNullData_Expect_FileNotCreated()
    {
        handler = new JsonHandler<TestGameData>("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/", "SaveTest.json");
        bool fileExists = File.Exists("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/SaveTest.json");
        Assert.IsFalse(fileExists);
        File.Delete("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/SaveTest.json");
    }

    [Test]
    public void SaveValidData_Expect_FileCreated()
    {
        handler = new JsonHandler<TestGameData>("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/", "SaveTest.json");
        IGameData test = new TestGameData();
        handler.Save(test);
        bool fileExists = File.Exists("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/SaveTest.json");
        Assert.IsTrue(fileExists);
        File.Delete("Packages/com.jovetools.gameserialization/Tests/Runtime/Resources/SaveTest.json");
    }
}
