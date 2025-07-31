using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// A simple step that needs to be triggered to move to the next step
/// </summary>
/// <remarks>
/// Remember to trigger the StepCompleted method when the step is completed
/// </remarks>
[RequireComponent(typeof(NetworkObject))]
public class SimpleStep : NetworkBehaviour
{
    [Tooltip("If true, the step will be deactivated at the start")]
    [SerializeField] private bool deactivate = false;
    [SerializeField] private StepData stepData;

    StepsManager stepsManager;

    public Action<StepData> onStepCompleted;

    private void Awake()
    {
        if (deactivate)
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        stepsManager = StepsManager.Instance;
        if (stepsManager == null)
        {
            Debug.LogError("StepsManager not found in parent object");
        }

        // Creo que no es necesario!
        this.stepData = new StepData(this.GetStepNumber(), this.GetStepName());
    }

    public void CompleteStep()
    {
        if (!stepData.IsCompleted())
        {
            Debug.Log("Current Step: " + (stepsManager.GetCurrentStep().GetStepNumber() + 1));
            Debug.Log("Activated Step: " + stepData.GetStepNumber());

            if (stepsManager.GetCurrentStep().GetStepNumber() + 1 == stepData.GetStepNumber())
            {
                CompleteStepRpc();
            }
            else
            {
                Debug.Log("Step " + stepData.GetStepNumber() + " not completed in order");
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void CompleteStepRpc(RpcParams rpcParams = default)
    {
        float timeTaken = FindFirstObjectByType<TimeTracker>().GetTimeElapsed();
        ulong senderClientId = rpcParams.Receive.SenderClientId;
        this.stepData.Complete(timeTaken, senderClientId);

        onStepCompleted?.Invoke(this.stepData);
    }

    public int GetStepNumber()
    {
        return stepData.GetStepNumber();
    }

    public string GetStepName()
    {
        return stepData.GetStepName();
    }

    public float GetTimeTaken()
    {
        return stepData.GetTimeTaken();
    }

    public ulong GetSenderClientId()
    {
        return stepData.GetSenderClientId();
    }

    public StepData GetStepData()
    {
        return stepData;
    }

}