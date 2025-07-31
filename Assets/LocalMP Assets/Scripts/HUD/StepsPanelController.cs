using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller for the steps panel
/// </summary>
public class StepsPanelController : IPanelController
{
    [SerializeField] private TMPro.TextMeshProUGUI m_StepCounterText;
    private StepsManager stepsManager;

    private void Awake()
    {
        stepsManager = StepsManager.Instance;
        if (m_StepCounterText == null || stepsManager == null)
        {
            Debug.LogError("StepsPanelController: Missing references");
        }

        stepsManager.onManagerStepCompleted += UpdateSteps;
        stepsManager.onAllStepsCompleted += OnAllStepsCompleted;
        stepsManager.onSceneStepsLoaded += OnSceneStepsLoadedCallback;
    }

    public override void Initiate()
    {
        m_StepCounterText.text = "0/X";
    }

    private void OnSceneStepsLoadedCallback(List<StepData> steps)
    {
        m_StepCounterText.text = "0/" + stepsManager.GetTotalSteps().ToString();
    }


    public void UpdateSteps(StepData stepData)
    {
        m_StepCounterText.text = stepData.GetStepNumber() + "/" + stepsManager.GetTotalSteps().ToString();
    }

    public void OnAllStepsCompleted()
    {
        m_StepCounterText.color = Color.green;
    }

    public override void Reset()
    {
        m_StepCounterText.text = "0";
    }
}
