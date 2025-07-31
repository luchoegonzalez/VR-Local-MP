using TMPro;
using UnityEngine;

/// <summary>
/// Step Slot UI for the Step List.
/// </summary>
public class DashboardStepSlot : MonoBehaviour
{
    private StepData step;

    [SerializeField] private TMP_Text stepTextSlot;
    [SerializeField] private TMP_Text stepNameSlot;
    [SerializeField] private TMP_Text timeTextSlot;
    [SerializeField] private TMP_Text playerTextSlot;
    [SerializeField] private GameObject checkmark;

    public void Setup(StepData step)
    {
        this.step = step;
        stepTextSlot.text = step.GetStepNumber().ToString();
        stepNameSlot.text = step.GetStepName();
        timeTextSlot.text = step.GetTimeTaken() == 0 ? "-" : $"{(int)step.GetTimeTaken() / 60}:{(int)step.GetTimeTaken() % 60:00}";
        playerTextSlot.text = step.GetCompletedBy() == "" ? "-" : step.GetCompletedBy();

        if (step.IsCompleted())
            checkmark.SetActive(true);
        else
            checkmark.SetActive(false);
    }

    public void SetStep(StepData step)
    {
        this.step = step;
    }

    public void OnStepCompletedCallBack(StepData stepData)
    {
        if (stepData.GetStepNumber() == step.GetStepNumber())
        {
            timeTextSlot.text = $"{(int)stepData.GetTimeTaken() / 60}:{(int)stepData.GetTimeTaken() % 60:00}";
            playerTextSlot.text = stepData.GetCompletedBy();
            checkmark.SetActive(true);
        }
    }

    public StepData GetStep()
    {
        return step;
    }
}
