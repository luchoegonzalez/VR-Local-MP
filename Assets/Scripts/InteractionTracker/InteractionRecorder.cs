using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

using Newtonsoft.Json;


/// <summary>
/// Records the interactions that every player has with the game.  
/// </summary>
/// <remarks>
/// This class is a singleton, and the interactions are only recorded by the server.
/// </remarks>
[RequireComponent(typeof(InteractionTracker), typeof(NetworkObject))]
public class InteractionRecorder : MonoBehaviour, IDataPersistence
{
    private static InteractionRecorder _instance;

    public static InteractionRecorder Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<InteractionRecorder>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<InteractionRecorder>();
                    singletonObject.name = typeof(InteractionRecorder).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Dictionary that stores the interactions of the players in a centralized way.
    /// </summary>
    // Podemos agregar InteractionData como atributo de XRINetworkPlayer, y asi no necesitamos un diccionario
    // pero entonces el atributo dentro de XRINetworkPlayer debe ser un NetworkVariable (para que se sincronice con el admin automaticamente)
    //, es mucho mas complicado, y no da mucha ventaja.
    // Podriamos hacer que no se sincronice, y que solo se mande la info al admin cuando se termine el juego. Pero si un jugador se desconecta, se pierde la info.
    // private Dictionary<XRINetworkPlayer, InteractionData> m_InteractionData = new Dictionary<XRINetworkPlayer, InteractionData>();

    private List<InteractionData> m_InteractionData = new List<InteractionData>();

    private int m_TotalInteractionsCount = 0;

    // Creamos este evento para que lo use el Dashboard (y evitamos una condicion de carrera)
    public Action<ulong, bool> onPlayerInteractionDataCreated;

    public Action<int> onInteractionRecorded;

    public void Start()
    {
        XRINetworkGameManager.Instance.playerStateChanged += OnClientConnected;
    }

    public void RecordInteraction(ulong senderClientId, ulong interactableNetworkObjectID, string interactableName, InteractionType interactionType)
    {
        XRINetworkGameManager.Instance.GetPlayerByID(senderClientId, out XRINetworkPlayer player);

        if (player == null) return;

        InteractionData interactionData = m_InteractionData.Find(x => x.GetPlayer().OwnerClientId == senderClientId);
        interactionData.RecordInteraction(interactableNetworkObjectID, interactableName, interactionType);
        m_TotalInteractionsCount++;

        onInteractionRecorded?.Invoke(m_TotalInteractionsCount);
    }

    /// <summary>
    /// Creates a new InteractionData for the player when they connect to the server.
    /// </summary>
    private void OnClientConnected(ulong clientId, bool connected)
    {
        if (connected)
        {   // if is Host return
            if (clientId == 0) return;
            XRINetworkGameManager.Instance.GetPlayerByID(clientId, out XRINetworkPlayer player);
            if (player == null || CheckIfPlayerExists(player))
            {
                return;
            }

            m_InteractionData.Add(new InteractionData(player));
            onPlayerInteractionDataCreated?.Invoke(clientId, true);
        }
        else
        {
            onPlayerInteractionDataCreated?.Invoke(clientId, false);
        }
    }

    public bool CheckIfPlayerExists(XRINetworkPlayer player)
    {
        foreach (var interactionData in m_InteractionData)
        {
            if (interactionData.GetPlayer() == player)
            {
                return true;
            }
        }
        return false;
    }


    public int GetInteractionCountByPlayer(XRINetworkPlayer player)
    {
        return m_InteractionData.Find(x => x.GetPlayer() == player).GetInteractionCount();
    }


    public int GetInteractionCountByType(XRINetworkPlayer player, InteractionType interactionType)
    {
        return m_InteractionData.Find(x => x.GetPlayer() == player).GetInteractionCountByType(interactionType);
    }


    public int GetInteractionCount()
    {
        int count = 0;
        foreach (var interactionData in m_InteractionData)
        {
            count += interactionData.GetInteractionCount();
        }
        return count;
    }

    public InteractionData GetInteractionDataByPlayer(ulong clientId)
    {
        return m_InteractionData.Find(x => x.GetPlayer().OwnerClientId == clientId);
    }

    public List<InteractionData> GetInteractionData()
    {
        return m_InteractionData;
    }

    // [ContextMenu("Print Interaction Data")]
    // public void PrintInteractionData()
    // {
    //     string json = "{";
    //     foreach (var interactionData in m_InteractionData)
    //     {
    //         json += "\"" + interactionData.Key.OwnerClientId + "\": {";
    //         json += "\"Name\": \"" + interactionData.Key.playerName + "\",";
    //         json += "\"Interactions\": {";
    //         json += interactionData.Value.PrintInteractionData();
    //         json += "}";
    //         json += "},";
    //     }
    //     if (m_InteractionData.Count > 0)
    //     {
    //         json = json.TrimEnd(','); // Remove the last comma
    //     }
    //     json += "}";
    //     Debug.Log(json);
    // }

    public void LoadData(GameData data)
    {
        m_InteractionData = data.interactionData;
    }

    public void SaveData(ref GameData data)
    {
        data.interactionData = m_InteractionData;
    }

}



// Example JSON structure matching m_InteractionData type
// {
//     "1": { // ClientID
//         "Name": "PlayerName",
//         "Interactions": {
//             "Hover": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 10 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 5 }
//             ],
//             "Select": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 5 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 2 }
//             ],
//             "Activate": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 2 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 1 }
//             ]
//         }
//     },
//     "2": { // Another ClientID
//         "Name": "AnotherPlayerName",
//         "Interactions": {
//             "Hover": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 8 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 3 }
//             ],
//             "Select": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 4 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 1 }
//             ],
//             "Activate": [
//                 { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 1 },
//                 { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 0 }
//             ]
//         }
//     }
// }

