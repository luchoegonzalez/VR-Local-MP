using System.Collections.Generic;
using UnityEngine;
using XRMultiplayer;
using TMPro;

/// <summary>
/// A simple UI for displaying a list of connected players.
/// </summary>
/// <remarks>
/// It is used inside the Simple Player Menu UI prefab, in the Room Panel.
/// </remarks>
public class SimplePlayerListUI : MonoBehaviour
{

    [SerializeField] TMP_Text m_PlayerCountText;
    [SerializeField] Transform m_ConnectedPlayersViewportContentTransform;
    [SerializeField] GameObject m_PlayerSlotPrefab;

    [SerializeField] bool m_AutoInitializeCallbacks = true;

    readonly Dictionary<SimplePlayerSlot, XRINetworkPlayer> m_PlayerDictionary = new();

    bool m_CallbacksInitialized = false;

    private void Awake()
    {
        if (!m_CallbacksInitialized && m_AutoInitializeCallbacks)
            InitializeCallbacks();
    }

    public void OnDestroy()
    {
        XRINetworkGameManager.Instance.playerStateChanged -= ConnectedPlayerStateChange;
        XRINetworkGameManager.Connected.Unsubscribe(OnConnected);
    }

    /// <summary>
    /// Use this function to initialize the callbacks on objects that start out disabled or inactive.
    /// </summary>
    public void InitializeCallbacks()
    {
        if (m_CallbacksInitialized) return;
        m_CallbacksInitialized = true;

        foreach (Transform t in m_ConnectedPlayersViewportContentTransform)
        {
            Destroy(t.gameObject);
        }

        XRINetworkGameManager.Instance.playerStateChanged += ConnectedPlayerStateChange;
        XRINetworkGameManager.Connected.Subscribe(OnConnected);
    }

    void OnConnected(bool connected)
    {
        if (!connected)
        {
            foreach (Transform t in m_ConnectedPlayersViewportContentTransform)
            {
                Destroy(t.gameObject);
            }

            m_PlayerDictionary.Clear();
        }
    }

    void ConnectedPlayerStateChange(ulong playerId, bool connected)
    {
        if (connected)
        {
            SetupPlayerSlotUI(playerId);
        }
        else
        {
            RemovePlayerSlotUI(playerId);
        }
    }

    void RemovePlayerSlotUI(ulong playerId)
    {
        SimplePlayerSlot slotToRemove = null;
        foreach (SimplePlayerSlot slot in m_PlayerDictionary.Keys)
        {
            if (slot.playerID == playerId)
            {
                slotToRemove = slot;
                break;
            }
        }

        if (slotToRemove != null)
        {
            m_PlayerDictionary.Remove(slotToRemove);
            Destroy(slotToRemove.gameObject);
            m_PlayerCountText.text = $"{m_PlayerDictionary.Keys.Count}/{XRINetworkGameManager.Instance.lobbyManager.connectedLobby.MaxPlayers}";
        }
    }

    /// <summary>
    /// Sets up the player slot UI in the connected players list.
    /// </summary>
    void SetupPlayerSlotUI(ulong playerId)
    {
        SimplePlayerSlot slot = Instantiate(m_PlayerSlotPrefab, m_ConnectedPlayersViewportContentTransform).GetComponent<SimplePlayerSlot>();
        slot.playerID = playerId;

        if (XRINetworkGameManager.Instance.GetPlayerByID(playerId, out XRINetworkPlayer player))
        {
            if (m_PlayerDictionary.TryAdd(slot, player))
            {
                slot.Setup(player);
                if (player.IsLocalPlayer)
                {
                    slot.playerSlotName.text += " (Tu)";
                }
                slot.playerIconImage.color = player.playerColor;
                m_PlayerCountText.text = $"{m_PlayerDictionary.Keys.Count}";
            }
        }
        else
        {
            Utils.Log($"Player with id {playerId} is null. This is a bug.", 2);
        }
    }

}
