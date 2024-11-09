using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Scriptas that handles the recenter target. It is used to recenter the player's view in the XR experience.
/// </summary>
public class RecenterTarget : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private int targetOrder = 0;

    private LocalXRINetworkGameManager m_GameManager;

    private void Awake()
    {
        if (targetOrder == 0) Debug.LogWarning("Target order is not set for " + gameObject.name);
        if (target == null) target = transform;

        m_GameManager = LocalXRINetworkGameManager.Instance;
        m_GameManager.onGameStarted += hideRecenterSign;
    }

    private void hideRecenterSign()
    {
        Canvas canvas = GetComponentInChildren<Canvas>();
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    public void SetTarget(Transform target, int targetOrder)
    {
        this.target = target;
        this.targetOrder = targetOrder;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public int GetTargetOrder()
    {
        return targetOrder;
    }

    public bool IsTarget(Transform target)
    {
        return this.target == target;
    }

    public bool IsTargetOrder(int targetOrder)
    {
        return this.targetOrder == targetOrder;
    }

    public Vector3 GetPosition()
    {
        return target.position;
    }

    public Vector3 GetUp()
    {
        return target.up;
    }

    public Vector3 GetForward()
    {
        return target.forward;
    }

}
