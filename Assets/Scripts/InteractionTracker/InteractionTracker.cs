using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Tracks the interactions a VR User has with the environment.
/// </summary>
[RequireComponent(typeof(InteractionRecorder), typeof(NetworkObject))]
public class InteractionTracker : NetworkBehaviour
{

    [SerializeField] private bool trackHover = false;
    [SerializeField] private bool trackSelect = true;
    [SerializeField] private bool trackActivate = true;

    private XRBaseInteractable[] m_Interactables;

    private LocalXRINetworkGameManager m_GameManager;

    public Action<int> onSelectEntered;
    public Action<int> onActivate;

    private void Awake()
    {
        // Search for all the Interactables in the scene
        m_Interactables = FindObjectsOfType<XRBaseInteractable>();
        m_GameManager = LocalXRINetworkGameManager.Instance;

        // Starts to track the interactions when the game starts
        m_GameManager.onGameStarted += CreateListeners;
    }

    private void CreateListeners()
    {
        if (IsHost) return;

        // Interactables like: XRGrabInteractable, XRSimpleInteractable, XRGazeInteractable, etc.
        foreach (var interactable in m_Interactables)
        {
            if (trackHover)
                interactable.hoverEntered.AddListener(args => OnInteractionOccurred(InteractionType.HoverEntered, args));
            if (trackSelect)
                interactable.selectEntered.AddListener(args => OnInteractionOccurred(InteractionType.SelectEntered, args));
            if (trackActivate)
                interactable.activated.AddListener(args => OnInteractionOccurred(InteractionType.Activated, args));
        }
    }

    private void OnInteractionOccurred(InteractionType interactionType, BaseInteractionEventArgs args)
    {
        ulong NetworkObjectID = args.interactableObject.transform.GetComponent<NetworkObject>().NetworkObjectId;
        string interactableName = args.interactableObject.transform.name;

        RecordInteractionRpc(NetworkObjectID, interactableName, interactionType);
    }

    // Who made the interaction, what was the interactable, and what type of interaction was it
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RecordInteractionRpc(ulong NetworkObjectID, string interactableName, InteractionType interactionType, RpcParams serverRpcParams = default)
    {
        Debug.Log("RecordInteractionRpc | Sender Client ID: " + serverRpcParams.Receive.SenderClientId + " | NetworkObjectID: " + NetworkObjectID + " | Interactable Name: " + interactableName + " | Interaction Type: " + interactionType);

        ulong senderClientId = serverRpcParams.Receive.SenderClientId;
        // XRINetworkGameManager.Instance.GetPlayerByID(senderClientId, out XRINetworkPlayer xRINetworkPlayer);
        InteractionRecorder.Instance.RecordInteraction(senderClientId, NetworkObjectID, interactableName, interactionType);
    }


    public override void OnDestroy()
    {
        foreach (var interactable in m_Interactables)
        {
            interactable.hoverEntered.RemoveListener(args => OnInteractionOccurred(InteractionType.HoverEntered, args));
            interactable.selectEntered.RemoveListener(args => OnInteractionOccurred(InteractionType.SelectEntered, args));
            interactable.activated.RemoveListener(args => OnInteractionOccurred(InteractionType.Activated, args));
        }
    }

}