using UnityEngine;

public class TimePanelController : IPanelController
{
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    private TimeTracker timeTracker;

    private void Start()
    {
        timeTracker = FindFirstObjectByType<TimeTracker>();
        if (timeText == null || timeTracker == null)
        {
            Debug.LogError("TimePanelController: Missing references");
        }

        timeTracker.onTimeUpdated += UpdateTime;
    }

    public override void Initiate()
    {
        timeText.text = "00:00";
    }

    public void UpdateTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public override void Reset()
    {
        timeText.text = "00:00";
    }


}
