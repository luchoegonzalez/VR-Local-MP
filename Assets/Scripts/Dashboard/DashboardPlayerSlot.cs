using TMPro;
using UnityEngine;
using XRMultiplayer;

/// <summary>
/// Player Slot UI for the Players List.
/// </summary>
public class DashboardPlayerSlot : MonoBehaviour
{
    private XRINetworkPlayer m_Player;
    private InteractionData m_InteractionData;

    [SerializeField] private TMP_Text playerIDSlot;
    [SerializeField] private TMP_Text playerTextSlot;
    [SerializeField] private TMP_Text hoverSlot;
    [SerializeField] private TMP_Text selectSlot;
    [SerializeField] private TMP_Text activateSlot;

    public void Setup(XRINetworkPlayer player, InteractionData interactionData)
    {
        this.m_Player = player;
        this.m_InteractionData = interactionData;
        this.m_Player.onNameUpdated += UpdateName;

        // playerIDSlot.text = player.OwnerClientId.ToString();
        playerIDSlot.text = player.OwnerId.ToString();
        playerTextSlot.text = player.playerName;

        m_InteractionData.onInteractionRecorded += OnInteractionRecordedCallback;

        int hover = interactionData.GetInteractionCountByType(InteractionType.HoverEntered);
        int select = interactionData.GetInteractionCountByType(InteractionType.SelectEntered);
        int activated = interactionData.GetInteractionCountByType(InteractionType.Activated);

        hoverSlot.text = hover.ToString();
        selectSlot.text = select.ToString();
        activateSlot.text = activated.ToString();
    }

    public void Deactivate()
    {
        this.gameObject.transform.Find("Button").gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (m_Player != null)
        {
            m_Player.onNameUpdated -= UpdateName;
        }
        if (m_InteractionData != null)
        {
            m_InteractionData.onInteractionRecorded -= OnInteractionRecordedCallback;
        }
    }

    void UpdateName(string newName)
    {
        playerTextSlot.text = newName;
    }

    void OnInteractionRecordedCallback(InteractionType interactionType, int count)
    {
        switch (interactionType)
        {
            case InteractionType.HoverEntered:
                UpdateHoverCount(count);
                break;
            case InteractionType.SelectEntered:
                UpdateSelectCount(count);
                break;
            case InteractionType.Activated:
                UpdateActivateCount(count);
                break;
        }
    }

    void UpdateHoverCount(int count)
    {
        hoverSlot.text = count.ToString();
    }

    void UpdateSelectCount(int count)
    {
        selectSlot.text = count.ToString();
    }

    void UpdateActivateCount(int count)
    {
        activateSlot.text = count.ToString();
    }

    public ulong GetPlayerID()
    {
        return m_Player.OwnerClientId;
    }


}
