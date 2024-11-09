using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Manages the data persistence of the game.
/// </summary>
/// <remarks>
/// This class is responsible for loading and saving the game data.
/// </remarks>
public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Configuration")]
    [SerializeField] private string fileName = "GameData.json";

    private GameDataList m_GameDataList;
    private GameData m_ActualGameData;
    private List<IDataPersistence> m_DataPersistenceList;
    private FileDataHandler m_FileDataHandler;

    public Action OnDataLoaded;

    public static DataPersistenceManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one DataPersistenceManager in the scene");
        }
        Instance = this;
    }

    private void Start()
    {
        // Do not run on Android
        if (Application.platform == RuntimePlatform.Android) return;

        m_FileDataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        m_DataPersistenceList = FindAllDataPersistenceObjects();
        LoadData();
    }

    public void LoadData()
    {
        m_GameDataList = m_FileDataHandler.Load() ?? new GameDataList();

        if (m_GameDataList.gameDataList.Count == 0)
        {
            Debug.LogWarning("No game data found, creating new data");
        }
        else
        {
            // m_ActualGameData = m_GameDataList.gameDataList.Last();
            // foreach (IDataPersistence dataPersistence in m_DataPersistenceList)
            // {
            //     dataPersistence.LoadData(m_ActualGameData);
            // }
            OnDataLoaded?.Invoke();
            Debug.Log("Data loaded");
        }
        m_ActualGameData = new GameData();
    }

    public void SaveData(string lobbyName, float timeElapsed, int playerCount)
    {

        m_ActualGameData.lobbyName = lobbyName;
        m_ActualGameData.gameTime = (float)Math.Round(timeElapsed, 2);
        m_ActualGameData.numberOfPlayers = playerCount;
        foreach (IDataPersistence dataPersistence in m_DataPersistenceList)
        {
            dataPersistence.SaveData(ref m_ActualGameData);
        }

        m_GameDataList.gameDataList.Add(m_ActualGameData);
        m_FileDataHandler.Save(m_GameDataList);

        Debug.Log("Data saved");
    }

    // private void OnApplicationQuit()
    // {
    //     SaveData();
    // }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        return FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>().ToList();
    }

    public List<GameData> GetGameDataList()
    {
        return m_GameDataList.gameDataList;
    }

    public GameData GetGameData(int index)
    {
        if (index >= 0 && index < m_GameDataList.gameDataList.Count)
        {
            return m_GameDataList.gameDataList[index];
        }
        else
        {
            Debug.LogError("Invalid index: " + index);
            return null;
        }
    }

}
