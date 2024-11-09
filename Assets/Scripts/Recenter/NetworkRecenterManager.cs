using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// NetworkRecenterManager is a NetworkBehaviour that manages the recentering of VR rigs in a networked environment.
/// </summary>
/// <remarks>
/// This script is intended to be attached to a GameObject in the scene.
/// </remarks>
public class NetworkRecenterManager : NetworkBehaviour
{

    [SerializeField] RecenterTarget[] recenterTargets;
    [SerializeField] CharacterRecenter characterRecenter;

    private LocalXRINetworkGameManager m_LocalXRINetworkGameManager;

    // ClientID, TargetIndex 
    private Dictionary<ulong, RecenterTarget> clientRecenterTargets = new Dictionary<ulong, RecenterTarget>();

    /// <summary>
    /// Action called when the recenter point is swapped
    /// </summary>
    public Action<ulong, RecenterTarget> onRecenterPointUpdated;

    public void Awake()
    {
        m_LocalXRINetworkGameManager = LocalXRINetworkGameManager.Instance;

        if (m_LocalXRINetworkGameManager == null)
        {
            Debug.LogError("LocalXRINetworkGameManager not found in the scene");
        }

        m_LocalXRINetworkGameManager.onGameStarted += RecenterAll;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost) return;

        if (recenterTargets.Length == 0)
        {
            Debug.LogWarning("No recenter targets assigned to NetworkRecenterManager");
        }
        else
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (!IsHost) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
    }

    private void OnClientConnectedCallback(ulong ClientId)
    {
        if (clientRecenterTargets.Count < recenterTargets.Length && ClientId != NetworkManager.ServerClientId)
        {
            RecenterTarget target = recenterTargets[clientRecenterTargets.Count];

            Debug.Log("Recenter target " + target.GetTargetOrder() + " added to client: " + ClientId);
            clientRecenterTargets.Add(ClientId, target);
            onRecenterPointUpdated?.Invoke(ClientId, target);
        }
    }


    // if target is already in use, swap targets
    private void TrySetClientTarget(ulong clientId, RecenterTarget newTarget, RecenterTarget actualTarget)
    {
        bool hasBeenSwapped = false;
        ulong keySwapped = 0;
        if (clientRecenterTargets.ContainsValue(newTarget))
        {
            foreach (var client in clientRecenterTargets)
            {
                if (client.Value == newTarget)
                {
                    clientRecenterTargets[client.Key] = actualTarget;
                    hasBeenSwapped = true;
                    keySwapped = client.Key;
                    break;
                }
            }
        }
        clientRecenterTargets[clientId] = newTarget;
        // this is how we avoid the action being called many times
        if (hasBeenSwapped) onRecenterPointUpdated?.Invoke(keySwapped, actualTarget);
    }

    [ContextMenu("ShowClientTargets")]
    public void ShowClientTargets()
    {
        foreach (var client in clientRecenterTargets)
        {
            Debug.Log("Client " + client.Key + " is using target " + client.Value);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void RecenterRpc(Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        Debug.Log(NetworkManager.Singleton.LocalClientId + "Rig recentered to " + NetworkManager.Singleton.LocalClientId);
        characterRecenter.Recenter(position, rotation);

        // eliminar la pantalla negra luego de recenter


    }

    [ContextMenu("RecenterAll")]
    public void RecenterAll()
    {
        if (IsServer)
        {
            foreach (var client in clientRecenterTargets)
            {
                Transform targetTransform = client.Value.transform;
                Vector3 position = targetTransform.position;
                Quaternion rotation = targetTransform.rotation;

                RecenterRpc(position, rotation, RpcTarget.Single(client.Key, RpcTargetUse.Temp));
            }
        }
    }

    [ContextMenu("List ClientRecenterTargets")]
    public void ListClientRecenterTargets()
    {
        foreach (var client in clientRecenterTargets)
        {
            Debug.Log("Client " + client.Key + " is using target " + client.Value.GetTargetOrder());
        }
    }

    private void OnClientDisconnectCallback(ulong ClientId)
    {
        if (IsServer)
        {
            clientRecenterTargets.Remove(ClientId);
        }
    }

    public RecenterTarget[] GetRecenterTargets()
    {
        return recenterTargets;
    }

    public Dictionary<ulong, RecenterTarget> GetClientRecenterTargets()
    {
        return clientRecenterTargets;
    }

    public int GetNumberOfTargets()
    {
        return recenterTargets.Length;
    }

    public RecenterTarget GetClientRecenterTarget(ulong clientId)
    {
        return clientRecenterTargets[clientId];
    }

    public void SetClientTarget(ulong clientId, RecenterTarget newTarget)
    {
        RecenterTarget actualTarget = GetClientRecenterTarget(clientId);

        TrySetClientTarget(clientId, newTarget, actualTarget);

    }

    public void SetClientTarget(ulong clientId, int recenterTargetOrder)
    {
        foreach (RecenterTarget target in recenterTargets)
        {
            if (target.IsTargetOrder(recenterTargetOrder))
            {
                SetClientTarget(clientId, target);
                break;
            }
        }
    }

}
