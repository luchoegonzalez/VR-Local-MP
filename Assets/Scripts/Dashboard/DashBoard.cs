using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

/// <summary>
/// Dashboard Controller
/// </summary>
public class Dashboard : MonoBehaviour
{
    private InteractionRecorder m_InteractionRecorder;
    private DataPersistenceManager m_DataPersistenceManager;

    [Header("Previous Games")]
    [SerializeField] private TMP_Dropdown previousGamesDropdown;

    [Header("Steps List")]
    [SerializeField] private GameObject stepRowPrefab;
    [SerializeField] private Transform stepRowContainerTransform;

    [Header("Players List")]
    [SerializeField] private GameObject playerRowPrefab;
    [SerializeField] private Transform playerRowContainerTransform;

    [Header("Interaction Counter")]
    [SerializeField] private TMP_Text interactionCounter;

    [Header("Time Panel")]
    [SerializeField] private TimePanelController timePanelController;

    public Action<StepData> onPreviousGameSelected;

    void Start()
    {
        m_InteractionRecorder = InteractionRecorder.Instance;
        m_InteractionRecorder.onPlayerInteractionDataCreated += onPlayerInteractionDataCreatedCallback;
        StepsManager.Instance.onSceneStepsLoaded += OnSceneStepsLoadedCallback;
        InteractionRecorder.Instance.onInteractionRecorded += OnInteractionRecordedCallback;
        // LocalXRINetworkGameManager.Instance.onGameStarted += OnGameStartedCallback;
        NetworkManager.Singleton.OnServerStarted += OnServerStartedCallback;

        interactionCounter.text = "0";

        m_DataPersistenceManager = DataPersistenceManager.Instance;
        m_DataPersistenceManager.OnDataLoaded += LoadPreviousGames;
        GetComponent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Clear the Dashboard when the Server/Host is started.
    /// </summary>
    void OnServerStartedCallback()
    {
        previousGamesDropdown.gameObject.SetActive(false);
        CLearList(stepRowContainerTransform);
        CLearList(playerRowContainerTransform);
        interactionCounter.text = "0";
        timePanelController.Reset();
    }

    /// <summary>
    /// Load the previous games to the Dropdown when the persistence data is loaded.
    /// </summary>
    void LoadPreviousGames()
    {
        List<GameData> gameDataList = m_DataPersistenceManager.GetGameDataList();

        foreach (GameData gameData in gameDataList)
        {
            string date = gameData.dateOfGame.ToString();
            string lobbyName = gameData.lobbyName;
            string text = date + " - " + lobbyName;

            previousGamesDropdown.options.Add(new TMP_Dropdown.OptionData(text));
        }

        previousGamesDropdown.onValueChanged.AddListener(delegate
        {
            PreviousGameSelected(previousGamesDropdown.value, gameDataList);
        });
    }

    /// <summary>
    /// Callback for when a previous game is selected from the dropdown.
    /// </summary>
    /// <remarks>
    /// It will setup the Step List and Players List in the dashboard.
    /// </remarks>
    void PreviousGameSelected(int index, List<GameData> gameDataList)
    {
        if (index == 0) return;
        GameData gameData = gameDataList[index - 1];

        CLearList(stepRowContainerTransform);
        CLearList(playerRowContainerTransform);

        foreach (StepData step in gameData.stepsList)
        {
            DashboardStepSlot stepRow = Instantiate(stepRowPrefab, stepRowContainerTransform).GetComponent<DashboardStepSlot>();
            stepRow.Setup(step);
        }

        int playerCount = gameData.interactionData.Count;
        int interactionCount = 0;
        foreach (InteractionData interactionData in gameData.interactionData)
        {
            DashboardPlayerSlot playerRow = Instantiate(playerRowPrefab, playerRowContainerTransform).GetComponent<DashboardPlayerSlot>();
            playerRow.Setup(interactionData.GetPlayer(), interactionData);
            interactionCount += interactionData.GetInteractionCount();

            Debug.Log("Player: " + interactionData.GetPlayer().OwnerId + " - " + interactionData.GetPlayer().playerName);
            Debug.Log(interactionData.PrintInteractionData());
        }

        interactionCounter.text = interactionCount.ToString();
        timePanelController.UpdateTime(gameData.gameTime);
        onPreviousGameSelected?.Invoke(gameData.stepsList[0]);
    }

    private void CLearList(Transform container)
    {
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Callback for when the game is started.
    /// </summary>
    /// <remarks>
    /// It will setup the Step List in the dashboard.
    /// </remarks>
    void OnSceneStepsLoadedCallback(List<StepData> stepsList)
    {

        CLearList(stepRowContainerTransform);

        foreach (StepData step in stepsList)
        {
            DashboardStepSlot stepRow = Instantiate(stepRowPrefab, stepRowContainerTransform).GetComponent<DashboardStepSlot>();
            stepRow.Setup(step);
            StepsManager.Instance.onManagerStepCompleted += stepRow.OnStepCompletedCallBack;
        }
    }

    /// <summary>
    /// Callback for when a player interaction data is created.
    /// </summary>
    /// <remarks>
    /// It will add a new player to the Players List in the dashboard.
    /// </remarks>
    void onPlayerInteractionDataCreatedCallback(ulong playerId, bool added)
    {
        if (added)
        {
            if (playerId == 0) return;
            XRINetworkGameManager.Instance.GetPlayerByID(playerId, out XRINetworkPlayer player);
            DashboardPlayerSlot playerRow = Instantiate(playerRowPrefab, playerRowContainerTransform).GetComponent<DashboardPlayerSlot>();
            playerRow.Setup(player, m_InteractionRecorder.GetInteractionDataByPlayer(player.OwnerClientId));
        }
        else
        {
            foreach (Transform t in playerRowContainerTransform)
            {
                DashboardPlayerSlot playerRow = t.GetComponent<DashboardPlayerSlot>();
                if (playerRow.GetPlayerID() == playerId)
                {
                    playerRow.Deactivate();
                }
            }
        }
    }

    void OnInteractionRecordedCallback(int interactionsCount)
    {
        interactionCounter.text = interactionsCount.ToString();
    }

}

