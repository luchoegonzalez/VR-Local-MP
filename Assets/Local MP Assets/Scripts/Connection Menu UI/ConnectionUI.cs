using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using XRMultiplayer;
using Unity.Netcode;

/// <summary>
/// Handles the UI and Logic for the connection menu.
/// </summary>
public class ConnectionUI : MonoBehaviour
{
    [Header("Lobby List")]
    [SerializeField] Transform m_LobbyListParent;
    [SerializeField] GameObject m_LobbyListPrefab;
    [SerializeField] Button m_RefreshButton;
    [SerializeField] Image m_CooldownImage;
    [SerializeField] float m_AutoRefreshTime = 5.0f;
    [SerializeField] float m_RefreshCooldownTime = .5f;

    [Header("Connection Texts")]
    [SerializeField] TMP_Text m_ConnectionUpdatedText;
    [SerializeField] TMP_Text m_ConnectionSuccessText;
    [SerializeField] TMP_Text m_ConnectionFailedText;

    [Header("Room Creation")]
    [SerializeField] TMP_InputField m_RoomNameText;

    [SerializeField] GameObject[] m_ConnectionSubPanels;

    VRNetworkDiscoveryUI m_VRNetworkDiscoveryUI;

    Coroutine m_UpdateLobbiesRoutine;
    Coroutine m_CooldownFillRoutine;

    bool m_Private;
    int m_PlayerCount;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += ConnectedUpdated;
        NetworkManager.Singleton.OnClientDisconnectCallback += FailedToConnect;

        m_VRNetworkDiscoveryUI = GetComponent<VRNetworkDiscoveryUI>();

        m_RefreshButton.onClick.AddListener(ShowLobbies);
        m_VRNetworkDiscoveryUI.OnServerRegistered += GetAllLobbies;

        foreach (Transform t in m_LobbyListParent)
        {
            Destroy(t.gameObject);
        }

        // StartCoroutine(ShowLobbiesWithDelay(3.0f));
    }

    private void OnDisable()
    {
        HideLobbies();
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ConnectedUpdated;
            NetworkManager.Singleton.OnClientDisconnectCallback -= FailedToConnect;
        }
        if (m_VRNetworkDiscoveryUI != null)
        {
            m_VRNetworkDiscoveryUI.OnServerRegistered -= GetAllLobbies;
        }
    }

    public void CreateLobby()
    {

        if (m_RoomNameText.text == "" || m_RoomNameText.text == "<Room Name>")
        {
            m_RoomNameText.text = $"Lobby de {XRINetworkGameManager.LocalPlayerName.Value}";
        }

        // se cambia el server name en el VRNetworkDiscoveryUI para que los usuarios puedan ver el nombre del lobby
        m_VRNetworkDiscoveryUI.ChangeServerName(m_RoomNameText.text);
        // se actualiza tambien el nombre del lobby en el XRINetworkGameManager
        LocalXRINetworkGameManager.Instance.SetLobbyName(m_RoomNameText.text);
        LocalXRINetworkGameManager.StartHost();
    }

    /// <summary>
    /// Join the first lobby in the list
    /// </summary>
    public void QuickJoinLobby()
    {
        LocalLobbyListSlotUI lobbySlot = m_LobbyListParent.GetChild(0).GetComponent<LocalLobbyListSlotUI>();
        lobbySlot.JoinLobby();
    }

    // Creo que se puede borrar
    public void UpdatePlayerCount(int count)
    {
        m_PlayerCount = Mathf.Clamp(count, 1, XRINetworkGameManager.maxPlayers);
    }

    /// <summary>
    /// Set the room name
    /// </summary>
    /// <param name="roomName">The name of the room</param>
    /// <remarks> This function is called from <see cref="XRIKeyboardDisplay"/>
    // public void SetRoomName(string roomName)
    // {
    //     if (!string.IsNullOrEmpty(roomName))
    //     {
    //         m_RoomNameText.text = roomName;
    //     }
    // }

    public void ToggleConnectionSubPanel(int panelId)
    {
        for (int i = 0; i < m_ConnectionSubPanels.Length; i++)
        {
            m_ConnectionSubPanels[i].SetActive(i == panelId);
        }
    }

    void ConnectedUpdated(ulong sender)
    {
        m_ConnectionSuccessText.text = "<b>Estado:</b> Conectado";
        m_ConnectionSuccessText.color = Color.green;
        m_ConnectionFailedText.text = "";
        m_ConnectionUpdatedText.text = "";
        ToggleConnectionSubPanel(3);
    }

    public void FailedToConnect(ulong sender)
    {
        if (sender == NetworkManager.Singleton.LocalClientId)
        {
            m_ConnectionFailedText.text = "<b>Estado:</b> Error al conectar";
            m_ConnectionFailedText.color = Color.red;
            m_ConnectionSuccessText.text = "";
            m_ConnectionUpdatedText.text = "";
            ToggleConnectionSubPanel(4);
        }
    }

    public void HideLobbies()
    {
        EnableRefresh();
        if (m_UpdateLobbiesRoutine != null) StopCoroutine(m_UpdateLobbiesRoutine);
    }

    public void ShowLobbies()
    {
        Debug.Log("Show lobbies");
        m_VRNetworkDiscoveryUI.DiscoverHosts();
        if (m_UpdateLobbiesRoutine != null) StopCoroutine(m_UpdateLobbiesRoutine);
        m_UpdateLobbiesRoutine = StartCoroutine(UpdateAvailableLobbies());
    }

    IEnumerator UpdateAvailableLobbies()
    {
        while (true)
        {
            yield return new WaitForSeconds(m_AutoRefreshTime);
            m_VRNetworkDiscoveryUI.DiscoverHosts();
        }
    }

    private IEnumerator ShowLobbiesWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowLobbies();
    }

    void EnableRefresh()
    {
        m_CooldownImage.enabled = false;
        m_RefreshButton.interactable = true;
    }

    IEnumerator UpdateButtonCooldown()
    {
        m_RefreshButton.interactable = false;

        m_CooldownImage.enabled = true;
        for (float i = 0; i < m_RefreshCooldownTime; i += Time.deltaTime)
        {
            m_CooldownImage.fillAmount = Mathf.Clamp01(i / m_RefreshCooldownTime);
            yield return null;
        }
        EnableRefresh();
    }

    void GetAllLobbies()
    {
        if (m_CooldownFillRoutine != null) StopCoroutine(m_CooldownFillRoutine);
        m_CooldownFillRoutine = StartCoroutine(UpdateButtonCooldown());

        foreach (Transform t in m_LobbyListParent)
        {
            Destroy(t.gameObject);
        }

        if (m_VRNetworkDiscoveryUI.GetDiscoveredServers().Count > 0)
        {
            foreach (var discoveredServer in m_VRNetworkDiscoveryUI.GetDiscoveredServers())
            {
                LocalLobbyListSlotUI newLobbyUI = Instantiate(m_LobbyListPrefab, m_LobbyListParent).GetComponent<LocalLobbyListSlotUI>();
                string lobbyIP = discoveredServer.Key.ToString();
                DiscoveryResponseData data = discoveredServer.Value;
                newLobbyUI.CreateLobbyUI(lobbyIP, data.Port, data.ServerName, this);
            }
        }
        else
        {
            Debug.Log("No lobbies found");
        }

    }

}
