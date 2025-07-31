using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using XRMultiplayer;

/// <summary>
/// Data class that stores the user interactions with the interactables
/// </summary>
[DataContract]
public class InteractionData// : MonoBehaviour
{

    [DataMember(Name = "Player")]
    private XRINetworkPlayer m_Player;

    /// <summary>
    /// Dictionary that stores the user interactions with the interactables, by type
    /// </summary>
    [DataMember(Name = "InteractionRecords")]
    private Dictionary<InteractionType, List<InteractionRecord>> m_InteractionRecords = new Dictionary<InteractionType, List<InteractionRecord>>();

    [DataMember(Name = "InteractionCount")]
    private int m_InteractionCount = 0;

    public Action<InteractionType, int> onInteractionRecorded;

    public InteractionData(XRINetworkPlayer player)
    {
        m_Player = player;
        InitializeInteractionRecords();
    }

    public InteractionData()
    {
        InitializeInteractionRecords();
    }

    public static InteractionData CreateEmptyInteractionData()
    {
        return new InteractionData();
    }


    /// <summary>
    /// Records the interaction of the user with an interactable
    /// </summary>
    /// <param name="interactableID">ID of the interactable</param>
    /// <param name="interactableName">Name of the interactable</param>
    /// <param name="interactionType">Type of interaction</param>
    public void RecordInteraction(ulong interactableID, string interactableName, InteractionType interactionType)
    {
        List<InteractionRecord> interactionRecords = m_InteractionRecords[interactionType];
        InteractionRecord interactionRecord = interactionRecords.Find(x => x.GetInteractableID() == interactableID);

        if (interactionRecord == null)
        {
            interactionRecord = new InteractionRecord(interactableName, interactableID);
            interactionRecords.Add(interactionRecord);
        }
        else
        {
            interactionRecord.IncrementCount();
        }
        m_InteractionCount++;
        onInteractionRecorded?.Invoke(interactionType, this.GetInteractionCountByType(interactionType));
    }


    public int GetInteractionCountByType(InteractionType interactionType)
    {
        int count = 0;
        List<InteractionRecord> interactionRecords = m_InteractionRecords[interactionType];
        foreach (InteractionRecord record in interactionRecords)
        {
            count += record.GetCount();
        }
        return count;
    }

    public int GetInteractionCount()
    {
        return m_InteractionCount;
    }

    public InteractionRecord GetInteractionRecord(ulong interactableID, InteractionType interactionType)
    {
        List<InteractionRecord> interactionRecords = m_InteractionRecords[interactionType];
        return interactionRecords.Find(x => x.GetInteractableID() == interactableID);
    }

    public Dictionary<InteractionType, List<InteractionRecord>> GetInteractionRecords()
    {
        return m_InteractionRecords;
    }

    public string PrintInteractionData()
    {
        string json = "";
        foreach (KeyValuePair<InteractionType, List<InteractionRecord>> interactionType in m_InteractionRecords)
        {
            json += "\"" + interactionType.Key.ToString() + "\": [";
            foreach (InteractionRecord record in interactionType.Value)
            {
                json += "{ \"InteractableName\": \"" + record.GetInteractableName() + "\", \"InteractableID\": " + record.GetInteractableID() + ", \"Interactions\": " + record.GetCount() + " }";
                json += ",";
            }
            json += "],";
        }
        return json;
    }

    /// <summary>
    /// Initializes the interaction records dictionary with all the interaction types
    /// </summary>
    public void InitializeInteractionRecords()
    {
        foreach (InteractionType interactionType in System.Enum.GetValues(typeof(InteractionType)))
        {
            m_InteractionRecords.Add(interactionType, new List<InteractionRecord>());
        }
    }

    public void ClearInteractionRecords()
    {
        m_InteractionRecords.Clear();
    }

    public void SetPlayer(XRINetworkPlayer player) => m_Player = player;

    public XRINetworkPlayer GetPlayer() => m_Player;

}

/// <summary>
/// Data class that stores the interactions of the user with an interactable
/// </summary>
[Serializable]
public class InteractionRecord
{
    [JsonProperty]
    private string InteractableName;
    [JsonProperty]
    private ulong InteractableID;
    [JsonProperty]
    private int Count;

    public InteractionRecord(string interactableName, ulong interactableID)
    {
        InteractableName = interactableName;
        InteractableID = interactableID;
        Count = 1;
    }

    public void IncrementCount()
    {
        Count++;
    }

    public string GetInteractableName()
    {
        return InteractableName;
    }

    public ulong GetInteractableID()
    {
        return InteractableID;
    }

    public int GetCount()
    {
        return Count;
    }
}

[Serializable]
public enum InteractionType
{
    HoverEntered,
    SelectEntered,
    Activated
}


// {
//     "Hover": [
//         { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 10 },
//         { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 5 }
//     ],
//     "Select": [
//         { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 5 },
//         { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 2 }
//     ],
//     "Activate": [
//         { "InteractableName": "Cube", "InteractableID": 1, "Interactions": 2 },
//         { "InteractableName": "Sphere", "InteractableID": 2, "Interactions": 1 }
//     ]
// }