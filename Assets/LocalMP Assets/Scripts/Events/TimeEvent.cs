using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that handles time events, it has a list of TimedEvents that will be invoked when the time is right
/// </summary>
public class TimeEvent : MonoBehaviour
{
    bool timerStarted = false;
    private float timer;
    public TimedEvent[] timedEvents;

    // para conocer el tiempo de espera mas largo
    private TimedEvent longestEvent = new TimedEvent("longestEvent", 0, new UnityEvent());

    private void Start()
    {
        foreach (TimedEvent events in timedEvents)
        {

            if (events.GetTime() < 0)
            {
                Debug.LogError("TimeEvent: " + events.GetName() + " has a negative time, it will be set to 0");
            }

            if (events.GetTime() > longestEvent.GetTime())
            {
                longestEvent = events;
            }
        }
    }

    public void StartTimer()
    {
        timer = 0;
        timerStarted = true;
    }

    private void Update()
    {
        if (timerStarted)
        {
            timer += Time.deltaTime;
            foreach (TimedEvent events in timedEvents)
            {
                if (timer >= events.GetTime())
                {
                    events.InvokeEvent();
                }
            }

            // cuando el tiempo de espera sea mayor al tiempo de espera del evento con mayor tiempo de espera
            // se desactiva el timer y se resetea el tiempo de espera
            if (timer >= longestEvent.GetTime())
            {
                timerStarted = false;
                timer = 0;
            }

        }
    }
}
