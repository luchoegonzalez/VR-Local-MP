using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that handles timed events, it has a name, a time to wait and an UnityEvent that will be invoked when the time is right
/// </summary>
[System.Serializable]
public class TimedEvent
{
    [Tooltip("Name of the event")]
    [SerializeField] private string name;

    [Tooltip("Time to wait before invoking the event")]
    [Range(1, 100)]
    [SerializeField] private int time;

    [Tooltip("If the event can be invoked multiple times")]
    [SerializeField] private bool multipleTimes = false;

    private bool hasHappened = false;

    // recuerda que hay que hacer las vainas lo mas clean posibles y toda esa mariquera

    public UnityEvent onTimeComplete;

    public TimedEvent(string name, int time, UnityEvent onTimeComplete)
    {
        this.name = name;
        this.time = time;
        this.onTimeComplete = onTimeComplete;
    }

    public void InvokeEvent()
    {
        if (!hasHappened)
        {
            onTimeComplete.Invoke();
            if (!multipleTimes)
                hasHappened = true;
        }
    }

    public int GetTime()
    {
        return time;
    }

    public string GetName()
    {
        return name;
    }

}

