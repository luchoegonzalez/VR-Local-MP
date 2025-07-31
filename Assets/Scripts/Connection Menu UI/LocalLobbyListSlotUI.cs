using TMPro;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Unity.Netcode.Transports.UTP;
using XRMultiplayer;

/// <summary>
/// Handles the UI for every local lobby slot in the lobby list.
/// </summary>
public class LocalLobbyListSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_RoomNameText;
    [SerializeField] private TMP_Text m_PlayerCountText;
    [SerializeField] private Button m_JoinButton;
    [SerializeField] private GameObject m_JoinImage;

    NetworkManagerVRMultiplayer m_NetworkManager;

    ConnectionUI m_ConnectionUI;

    string m_LobbyIP;
    ushort m_LobbyPort;

    private void Awake()
    {
        m_NetworkManager = FindFirstObjectByType<NetworkManagerVRMultiplayer>();
    }

    public void CreateLobbyUI(string lobbyIP, int lobbyPort, string lobbyName, ConnectionUI connectionUI)
    {
        m_ConnectionUI = connectionUI;
        m_JoinButton.onClick.AddListener(JoinLobby);
        m_RoomNameText.text = lobbyName;

        // Tener el conteo de Jugadores
        // m_PlayerCountText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";

        m_LobbyIP = lobbyIP;
        m_LobbyPort = (ushort)lobbyPort;

        m_JoinImage.SetActive(false);
    }

    public void CreateNonJoinableLobbyUI(Lobby lobby, ConnectionUI connectionUI, string statusText)
    {
        m_JoinButton.interactable = false;
        // m_Lobby = lobby;
        m_ConnectionUI = connectionUI;
        m_RoomNameText.text = lobby.Name;
        m_JoinImage.SetActive(false);
    }

    public void ToggleHover(bool toggle)
    {
        if (toggle)
        {
            m_JoinImage.SetActive(true);
            m_JoinButton.interactable = true;
        }
        else
        {
            m_JoinImage.SetActive(false);
            m_JoinButton.interactable = false;
        }
    }

    public void JoinLobby()
    {
        UnityTransport transport = (UnityTransport)m_NetworkManager.NetworkConfig.NetworkTransport;
        transport.SetConnectionData(m_LobbyIP, m_LobbyPort);

        LocalXRINetworkGameManager.StartClient();
    }

    private void OnDestroy()
    {
        m_JoinButton.onClick.RemoveListener(JoinLobby);
    }
}
