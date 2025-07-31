using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

/// <summary>
/// Handles the writing and reading of data to and from a file.
/// </summary>
/// <remarks>
/// This class uses the Newtonsoft.Json library to serialize and deserialize the data.
/// For more information, see the <see href="https://www.newtonsoft.com/json">Newtonsoft.Json documentation</see>.
/// </remarks>
public class FileDataHandler
{
    private string m_FilePath;
    private string m_FileName;

    public FileDataHandler(string filePath, string fileName)
    {
        m_FilePath = filePath;
        m_FileName = fileName;
    }

    public GameDataList Load()
    {
        string fullPath = Path.Combine(m_FilePath, m_FileName);
        Debug.Log("Loading data from: " + fullPath);
        GameDataList loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                Debug.Log("Data to load: " + dataToLoad);

                loadedData = JsonConvert.DeserializeObject<GameDataList>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading file: " + e.Message + " On path: " + fullPath);
            }
        }
        return loadedData;
    }

    public void Save(GameDataList gameData)
    {
        string fullPath = Path.Combine(m_FilePath, m_FileName);
        Debug.Log("Saving data to: " + fullPath);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonConvert.SerializeObject(gameData, Formatting.Indented
            // ,new JsonSerializerSettings
            // {
            //     ContractResolver = new IgnorePropertiesResolver(new List<string> { "m_XROrigin", "m_InitialConnected", "usingVoiceChat", "m_VoiceChat", "selfMuted", "m_VivoxParticipant", "m_PrevHeadPos", "m_LeftHandOrigin", "m_RightHandOrigin", "m_HeadOrigin", "playerVoiceId", "playerVoiceAmp", "squelched", "onDisconnected", "onSpawnedAll", "onSpawnedLocal", "onColorUpdated", "onNameUpdated", "rightHand", "leftHand", "head", "LocalPlayer", "k_VoiceAmplitudeSpeed", "playerColor", "NetworkManager", "NetworkObject" }),
            // }
            );

            Debug.Log("Data to store: " + dataToStore);

            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving file: " + e.Message + " On path: " + fullPath);
        }
    }

    public void AddGameData(GameData gameData)
    {
        GameDataList gameDataList = Load() ?? new GameDataList();
        gameDataList.gameDataList.Add(gameData);
        Save(gameDataList);
    }

    public GameData GetGameData(int index)
    {
        GameDataList gameDataList = Load();
        if (gameDataList != null && index >= 0 && index < gameDataList.gameDataList.Count)
        {
            return gameDataList.gameDataList[index];
        }
        else
        {
            Debug.LogError("Invalid index: " + index);
            return null;
        }
    }
}
