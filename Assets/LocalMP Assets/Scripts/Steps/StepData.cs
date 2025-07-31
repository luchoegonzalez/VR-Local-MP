using System;
using Newtonsoft.Json;
using UnityEngine;

/// <summary>
/// Data class used to store data about a step
/// </summary>
[Serializable]
public class StepData
{

    [SerializeField, JsonProperty] private int stepNumber = 0;
    [SerializeField, JsonProperty] private string stepName = "Step";
    [JsonProperty] private bool m_Completed = false;
    [JsonProperty] private float m_TimeTaken = 0;

    // A Step could be completed but not succeeded !!!
    //private bool m_Succeeded = false;

    [JsonProperty] private ulong m_SenderClientId = 0;
    [JsonProperty] private string completedBy = "None";

    private Action onStepReset;

    public StepData(int stepNumber, string stepName)
    {
        this.stepNumber = stepNumber;
        this.stepName = stepName;
        this.m_Completed = false;
    }

    public void Complete(float timeTaken, ulong senderClientId)
    {
        if (!m_Completed)
        {
            this.m_Completed = true;
            this.m_TimeTaken = (float)Math.Round(timeTaken, 2);
            this.m_SenderClientId = senderClientId;

            XRMultiplayer.XRINetworkGameManager.Instance.GetPlayerByID(senderClientId, out XRMultiplayer.XRINetworkPlayer player);

            if (player != null)
            {
                this.completedBy = player.name;
            }
        }
        else
        {
            Debug.LogWarning("Step already completed");
        }
    }

    public void resetStep()
    {
        m_Completed = false;
        m_TimeTaken = 0;
        m_SenderClientId = 0;

        onStepReset?.Invoke();
    }

    public void SetOnStepReset(Action onStepReset)
    {
        this.onStepReset = onStepReset;
    }

    public int GetStepNumber()
    {
        return stepNumber;
    }

    public float GetTimeTaken()
    {
        return m_TimeTaken;
    }

    public ulong GetSenderClientId()
    {
        return m_SenderClientId;
    }

    public string GetCompletedBy()
    {
        return completedBy;
    }

    public bool IsCompleted()
    {
        return m_Completed;
    }

    public string GetStepName()
    {
        return stepName;
    }

    public string toJson()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static StepData GetEmptyStep()
    {
        return new StepData(0, "Empty Step");
    }

}
