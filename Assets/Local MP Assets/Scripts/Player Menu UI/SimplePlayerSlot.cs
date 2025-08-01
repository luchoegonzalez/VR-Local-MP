using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using XRMultiplayer;

/// <summary>
/// Player slot UI for the Simple Player Menu UI prefab.
/// </summary>
public class SimplePlayerSlot : MonoBehaviour
{
    public TMP_Text playerSlotName;
    public TMP_Text playerInitial;
    public Image playerIconImage;
    // public TMP_Dropdown recenterPointDropdown;
    public Button soundButton;

    XRINetworkPlayer m_Player;
    internal ulong playerID = 0;

    // NetworkRecenterManager m_RecenterManager;
    HMDNoise m_HMDNoise;

    public void Setup(XRINetworkPlayer player)
    {
        // m_RecenterManager = FindObjectOfType<NetworkRecenterManager>();
        m_HMDNoise = FindFirstObjectByType<HMDNoise>();

        m_Player = player;
        m_Player.onColorUpdated += UpdateColor;
        m_Player.onNameUpdated += UpdateName;
        // m_RecenterManager.onRecenterPointUpdated += SwapRecenterPoint;

        // if is local player, disable recenter point dropdown and sound button
        if (playerID == 0)
        {
            // recenterPointDropdown.gameObject.SetActive(false);
            soundButton.gameObject.SetActive(false);
        }
        else
        {
            // recenterPointDropdown.ClearOptions();
            // foreach (RecenterTarget r in m_RecenterManager.GetRecenterTargets())
            // {
            //     recenterPointDropdown.options.Add(new TMP_Dropdown.OptionData(r.GetTargetOrder().ToString()));
            // }

            // recenterPointDropdown.onValueChanged.AddListener(delegate { OnRecenterPointChanged(recenterPointDropdown.value); });
            soundButton.onClick.AddListener(OnSoundButtonClicked);
        }
    }

    void OnDestroy()
    {
        m_Player.onColorUpdated -= UpdateColor;
        m_Player.onNameUpdated -= UpdateName;
    }

    void UpdateColor(Color newColor)
    {
        playerIconImage.color = newColor;
    }

    void UpdateName(string newName)
    {
        if (!newName.IsNullOrEmpty())
        {
            string playerName = newName;
            if (m_Player.IsLocalPlayer)
            {
                playerName += " (Tu)";
            }
            else if (m_Player.IsOwnedByServer)
            {
                playerName += " (Host)";
            }
            playerSlotName.text = playerName;
            playerInitial.text = newName.Substring(0, 1);
        }
    }

    // public void SwapRecenterPoint(ulong clientId, RecenterTarget recenterTarget)
    // {
    //     if (clientId == playerID)
    //     {
    //         int index = Array.IndexOf(m_RecenterManager.GetRecenterTargets(), recenterTarget);
    //         recenterPointDropdown.value = index;
    //     }
    // }

    // private void OnRecenterPointChanged(int value)
    // {
    //     // We get the Index, but we need the actual RecenterTarget
    //     string recenterTargetOrderString = recenterPointDropdown.options[value].text;
    //     int recenterTargetOrder = int.Parse(recenterTargetOrderString);
    //     Debug.Log("Recenter point changed to: " + recenterTargetOrder);
    //     m_RecenterManager.SetClientTarget(playerID, recenterTargetOrder);
    // }

    public void OnSoundButtonClicked()
    {
        m_HMDNoise.TriggerNoiseOnClient(playerID);
    }

}
