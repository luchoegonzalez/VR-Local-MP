using System.Collections.Generic;
using UnityEngine;
using System.IO;
using XRMultiplayer;

public static class SaveSystem
{
    // public static void SaveGame(List<SimpleStep> steps, LocalXRINetworkGameManager gameManager)
    // {
    //     string path = Application.persistentDataPath + "/gameData.json";

    //     // Create the GameData object
    //     GameData data = new GameData(XRINetworkGameManager.Instance.m_CurrentPlayerIDs.Count, steps, gameManager.GetTimeElapsed());

    //     // Convert the GameData object to a JSON string
    //     string json = JsonUtility.ToJson(data);

    //     Debug.Log(json);

    //     // // Write the JSON string to the file
    //     // using (FileStream stream = new FileStream(path, FileMode.Create))
    //     // using (StreamWriter writer = new StreamWriter(stream))
    //     // {
    //     //     writer.Write(json);
    //     // }

    //     // Write the JSON string to the file
    //     File.WriteAllText(path, json);
    // }

    // public static GameData LoadGame()
    // {
    //     string path = Application.persistentDataPath + "/gameData.json";

    //     if (File.Exists(path))
    //     {
    //         string json = File.ReadAllText(path);
    //         return JsonUtility.FromJson<GameData>(json);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No game data found at: " + path);
    //         return null;
    //     }
    // }

}
