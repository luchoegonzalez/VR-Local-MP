using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

[RequireComponent(typeof(ConnectionUI))]
public class VRNetworkDiscoveryUI : MonoBehaviour
{
    [SerializeField, HideInInspector]
    VRNetworkDiscovery m_Discovery;

    NetworkManager m_NetworkManager;

    ConnectionUI m_ConnectionUI;

    Dictionary<IPAddress, DiscoveryResponseData> discoveredServers = new Dictionary<IPAddress, DiscoveryResponseData>();

    public Action OnServerRegistered;

    void Awake()
    {
        Debug.Log("VRNetworkDiscoveryUI Awake");
        m_Discovery = FindFirstObjectByType<VRNetworkDiscovery>();
        m_NetworkManager = GetComponent<NetworkManager>();
        m_ConnectionUI = GetComponent<ConnectionUI>();
        m_Discovery.OnServerFound.AddListener(OnServerFound);
    }

    void Start()
    {
    }

    public void DiscoverHosts()
    {
        if (m_Discovery.IsRunning)
        {
            Debug.Log("Stopping discovery");
            discoveredServers.Clear();
            m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
        }
        else
        {
            Debug.Log("Starting discovery");
            m_Discovery.StartClient();
            m_Discovery.ClientBroadcast(new DiscoveryBroadcastData());
        }
    }

    public void StartDiscoveryAsHost()
    {
        if (!m_Discovery.IsRunning)
        {
            m_Discovery.StartServer();
        }
    }

    public void StopDiscoveryAsHost()
    {
        if (m_Discovery.IsRunning)
        {
            m_Discovery.StopDiscovery();
        }
    }

    public void ChangeServerName(string serverName)
    {
        m_Discovery.ServerName = serverName;
    }

    void OnServerFound(IPEndPoint sender, DiscoveryResponseData response)
    {
        discoveredServers[sender.Address] = response;
        Debug.Log($"Server found: {response.ServerName} at {sender.Address}:{response.Port}");
        OnServerRegistered?.Invoke();
    }

    public Dictionary<IPAddress, DiscoveryResponseData> GetDiscoveredServers()
    {
        return discoveredServers;
    }

}
