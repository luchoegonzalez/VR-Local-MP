using System;
using System.Collections.Generic;

/// <summary>
/// Represents the data of a game that is going to be saved.
/// </summary>
[System.Serializable]
public class GameData
{

    public DateTime dateOfGame;
    public string lobbyName;
    public int numberOfPlayers;
    public float gameTime;
    public List<StepData> stepsList;
    public List<InteractionData> interactionData;

    public GameData()
    {
        this.dateOfGame = DateTime.Now;
        this.lobbyName = "";
        this.numberOfPlayers = 0;
        this.gameTime = 0;
        this.stepsList = new List<StepData>();
        this.interactionData = new List<InteractionData>();
    }
}

[System.Serializable]
public class GameDataList
{
    public List<GameData> gameDataList;

    public GameDataList()
    {
        gameDataList = new List<GameData>();
    }
}
