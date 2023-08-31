using System.Collections;
using jovetools.gameserialization;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.TestTools;
using NSubstitute.ReturnsExtensions;

public class PersistanceManagerTests
{
    private class TestSerializable : ISerializable
    {
        public bool dataCleared = false;
        public bool dataCreated = false;
        public bool dataLoaded = false;
        public bool dataSaved = false;

        public void Reset()
        {
            dataCleared = false;
            dataCreated = false;
            dataLoaded = false;
            dataSaved = false;
    }

        public void ClearData(IGameData data)
        {
            dataCleared = true;
        }

        public void CreateData(ref IGameData data)
        {
            dataCreated = true;
        }

        public void LoadData(IGameData data)
        {
            dataLoaded = true;
        }

        public void SaveData(ref IGameData data)
        {
            dataSaved = true;
        }
    }

    private class TestGameData : IGameData
    {
        public static bool ValidData = true;

        public bool ValidateData()
        {
            return ValidData;
        }
    }

    private TestSerializable testSerializable1 = new();
    private TestSerializable testSerializable2 = new();
    private TestSerializable testSerializable3 = new();

    private TestGameData testGameData = new();

    [TearDown]
    public void Reset()
    {
        PersistanceManager<TestGameData>.Instance.Init(null);
        PersistanceManager<TestGameData>.Instance.DeregisterAllSerializables();
        testSerializable1.Reset();
        testSerializable2.Reset();
    }

    [Test, Order(0)]
    public void NoInit_Expect_ExceptionOnSave()
    {
        Assert.Throws<System.NullReferenceException>( () => PersistanceManager<TestGameData>.Instance.SaveGame());
    }

    [Test, Order(1)]
    public void NoInit_Expect_ExceptionOnLoad()
    {
        Assert.Throws<System.NullReferenceException>(() => PersistanceManager<TestGameData>.Instance.LoadGame());
    }

    [Test, Order(2)]
    public void NoInit_Expect_ExceptionOnNew()
    {
        Assert.Throws<System.NullReferenceException>(() => PersistanceManager<TestGameData>.Instance.NewGame());
    }

    [Test, Order(3)]
    public void NoInit_Expect_ExceptionOnDelete()
    {
        Assert.Throws<System.NullReferenceException>(() => PersistanceManager<TestGameData>.Instance.DeleteGame());
    }

    [Test, Order(4)]
    public void SaveGameNoGameCreated_Expect_Exception()
    {
        var dataHandler = Substitute.For<IDataHandler>();

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);

        Assert.Throws<System.Exception>(() => PersistanceManager<TestGameData>.Instance.SaveGame());
        Assert.IsTrue(!testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(!testSerializable1.dataLoaded);
        Assert.IsTrue(!testSerializable1.dataSaved);
    }

    [Test, Order(5)]
    public void DeleteGameNoGameCreated_Expect_Exception()
    {
        var dataHandler = Substitute.For<IDataHandler>();

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);

        Assert.Throws<System.Exception>(() => PersistanceManager<TestGameData>.Instance.DeleteGame());
        Assert.IsTrue(!testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(!testSerializable1.dataLoaded);
        Assert.IsTrue(!testSerializable1.dataSaved);
    }

    [Test, Order(6)]
    public void NewGame_Expect_GameCreated()
    {
        var dataHandler = Substitute.For<IDataHandler>();
        dataHandler.Load().Returns(testGameData);

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);
        PersistanceManager<TestGameData>.Instance.NewGame();

        Assert.IsTrue(testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(!testSerializable1.dataLoaded);
        Assert.IsTrue(!testSerializable1.dataSaved);
    }

    [Test, Order(7)]
    public void LoadGameNoNewGame_Expect_NewGameCreated()
    {
        var dataHandler = Substitute.For<IDataHandler>();
        dataHandler.Load().ReturnsNull();

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);
        PersistanceManager<TestGameData>.Instance.LoadGame();

        Assert.IsTrue(testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(testSerializable1.dataLoaded);
        Assert.IsTrue(!testSerializable1.dataSaved);
    }

    [Test, Order(8)]
    public void LoadGamePreviouslyCreated_Expect_GameLoaded()
    {
        var dataHandler = Substitute.For<IDataHandler>();
        dataHandler.Load().Returns(testGameData);

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);
        PersistanceManager<TestGameData>.Instance.LoadGame();

        Assert.IsTrue(!testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(testSerializable1.dataLoaded);
        Assert.IsTrue(!testSerializable1.dataSaved);
    }

    [Test, Order(9)]
    public void SaveGamePreviouslyCreated_Expect_GameSaved()
    {
        var dataHandler = Substitute.For<IDataHandler>();
        
        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);
        PersistanceManager<TestGameData>.Instance.NewGame();
        PersistanceManager<TestGameData>.Instance.SaveGame();

        Assert.IsTrue(testSerializable1.dataCreated);
        Assert.IsTrue(!testSerializable1.dataCleared);
        Assert.IsTrue(!testSerializable1.dataLoaded);
        Assert.IsTrue(testSerializable1.dataSaved);
    }

    [Test, Order(10)]
    public void DeleteGamePreviouslyCreated_Expect_GameDeletedAndSaved()
    {
        var dataHandler = Substitute.For<IDataHandler>();

        PersistanceManager<TestGameData>.Instance.Init(dataHandler);
        PersistanceManager<TestGameData>.Instance.RegisterSerializableObject(testSerializable1);
        PersistanceManager<TestGameData>.Instance.NewGame();
        PersistanceManager<TestGameData>.Instance.DeleteGame();

        Assert.IsTrue(testSerializable1.dataCreated);
        Assert.IsTrue(testSerializable1.dataCleared);
        Assert.IsTrue(!testSerializable1.dataLoaded);
        Assert.IsTrue(testSerializable1.dataSaved);
    }

}