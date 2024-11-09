using System;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;
using UnityEngine.UI;

/// <summary>
/// Extends the NetworkGameManager to manage a LOCAL networked game session.
/// </summary>
public class LocalXRINetworkGameManager : XRINetworkGameManager
{

    private static LocalXRINetworkGameManager _instance;

    public static new LocalXRINetworkGameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LocalXRINetworkGameManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<LocalXRINetworkGameManager>();
                    singletonObject.name = typeof(LocalXRINetworkGameManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    const string k_DebugPrepend = "<color=#FAC00C>[Local Network Game Manager]</color> ";

    private TimeTracker m_TimeTracker;
    private DataPersistenceManager m_DataPersistenceManager;

    [Header("Game Settings")]
    [SerializeField]
    private int m_MinutesToFinish = 10;

    private string m_lobbyName = "Lobby";
    private bool m_GameStarted = false;
    private bool m_GameFinished = false;

    [Header("UI")]
    [SerializeField] Button startGameButton;

    [Header("Events")]
    public Action onGameStarted;
    public Action onGameFinished;
    public Action onGamePaused;
    public Action onGameResumed;

    protected override void Awake()
    {
        if (_instance == null)
        {
            _instance = this as LocalXRINetworkGameManager;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();

        if (!enabled) return;

        m_TimeTracker = FindObjectOfType<TimeTracker>();
        startGameButton.onClick.AddListener(StartGame);
        m_DataPersistenceManager = DataPersistenceManager.Instance;
    }

    /// <summary>
    /// Start the local networked game session as a host.
    /// </summary>
    public static void StartHost()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Utils.Log($"{k_DebugPrepend}No NetworkManager found in scene.", 2);
            return;
        }
        else
        {
            // Mockeamos el lobby, ya que no estamos utilizando el servicio de lobbies.
            // m_LobbyManager.MockLobby("0");
            networkManager.StartHost();
        }
    }

    /// <summary>
    /// Start the local networked game session as a client.
    /// </summary>
    public static void StartClient()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Utils.Log($"{k_DebugPrepend}No NetworkManager found in scene.", 2);
            return;
        }
        else
        {
            // Mockeamos el lobby, ya que no estamos utilizando el servicio de lobbies.
            // m_LobbyManager.MockLobby("0");
            networkManager.StartClient();
        }
    }

    public static void LocalDisconnect()
    {
        NetworkManager networkManager = NetworkManager.Singleton;
        if (networkManager == null)
        {
            Utils.Log($"{k_DebugPrepend}No NetworkManagerVRMultiplayer found in scene.", 2);
            return;
        }
        else
        {
            Utils.Log($"{k_DebugPrepend}Stopping Client.");
            ulong clientId = networkManager.LocalClientId;
            networkManager.DisconnectClient(clientId);
        }
    }

    private void Update()
    {
        if (!IsHost) return;

        if (!m_GameFinished && m_TimeTracker.GetTimeElapsed() >= m_MinutesToFinish * 60)
        {
            FinishGameRpc();
        }
    }

    /// <summary>
    /// Start the game, if there are enough players.
    /// </summary>
    public void StartGame()
    {
        if (!checkForPlayers()) return;
        StartGameRpc();
    }

    [Rpc(SendTo.Everyone)]
    public void StartGameRpc()
    {
        m_GameStarted = true;
        startGameButton.interactable = false;
        m_TimeTracker.StartTracking();
        onGameStarted?.Invoke();

        Debug.Log("Game Started");
    }


    // FinishGame is not a RPC, because the steps are synced, so the game will finish at the same time for all players!!!
    public void FinishGame()
    {
        m_GameFinished = true;
        m_TimeTracker.StopTracking();
        onGameFinished?.Invoke();
        if (IsHost)
        {
            int numberOfPlayers = XRINetworkGameManager.Instance.m_CurrentPlayerIDs.Count;
            m_DataPersistenceManager.SaveData(m_lobbyName, m_TimeTracker.GetTimeElapsed(), numberOfPlayers);
        }
        Debug.Log("Game Finished");
    }

    // this is called by events that abort the game, like when the time is up, or when the host wants to finish the game.
    [Rpc(SendTo.Everyone)]
    public void FinishGameRpc(RpcParams rpcParams = default)
    {
        FinishGame();
    }

    public bool checkForPlayers()
    {
        // cambiar a 2!!
        if (NetworkManager.Singleton.ConnectedClientsList.Count < 1)
        {
            Debug.Log("Not enough players to start the game");
            return false;
        }
        return true;
    }

    public void PauseGame()
    {
        m_TimeTracker.StopTracking();
        onGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        m_TimeTracker.StartTracking();
        onGameResumed?.Invoke();
    }

    public bool IsGameStarted() => m_GameStarted;

    public bool IsGameFinished() => m_GameFinished;

    public float GetMinutesToFinish() => m_MinutesToFinish;

    public float GetTimeElapsed() => m_TimeTracker.GetTimeElapsed();

    public float GetTimeRemaining() => m_MinutesToFinish * 60 - m_TimeTracker.GetTimeElapsed();

    public float GetTimeElapsedPercentage() => m_TimeTracker.GetTimeElapsed() / (m_MinutesToFinish * 60);

    public float GetTimeRemainingPercentage() => 1 - GetTimeElapsedPercentage();

    public string GetLobbyName() => m_lobbyName;

    public void SetLobbyName(string lobbyName) => m_lobbyName = lobbyName;
    // tambien esta connectedRoomName en el XRINetworkGameManager

}