using System;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Tracks the time elapsed in the game
/// </summary>
/// <remarks>
/// Usefull for tracking the time spent to complete steps in the game.
/// </remarks>
public class TimeTracker : NetworkBehaviour
{
    private float timeElapsed = 0;
    private bool isTracking = false;

    public Action<float> onTimeUpdated;

    public void StartTracking()
    {
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
    }

    public void ResetTracking()
    {
        timeElapsed = 0;
    }

    private void Update()
    {
        if (isTracking)
        {
            timeElapsed += Time.deltaTime;
            onTimeUpdated?.Invoke(timeElapsed);
        }
    }

    public float GetTimeElapsed()
    {
        return timeElapsed;
    }
}
