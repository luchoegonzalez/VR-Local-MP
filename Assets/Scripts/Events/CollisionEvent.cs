using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is used to detect collision events and trigger a UnityEvent when a collision is detected.
/// </summary>
public class CollisionEvent : MonoBehaviour
{
    [Tooltip("If true, only the tag object especified in the configuration in tagToTrigger will trigger the event.")]
    [SerializeField] public bool onlyTag;
    [SerializeField] public string objectTag;

    [Header("Events")]
    public UnityEvent<bool> onCollisionEntered;
    public UnityEvent<bool> onCollisionExited;
    public UnityEvent<bool> onCollisionStay;


    private void OnCollisionEnter(Collision other)
    {
        Detection(other, onCollisionEntered);
    }

    private void OnCollisionExit(Collision other)
    {
        Detection(other, onCollisionExited);
    }

    private void OnCollisionStay(Collision other)
    {
        Detection(other, onCollisionStay);
    }

    private void Detection(Collision other, UnityEvent<bool> unityEvent)
    {
        if (onlyTag)
        {
            if (other.gameObject.tag == objectTag)
            {
                unityEvent.Invoke(true);
            }
            return;
        }
        else
        {
            unityEvent.Invoke(true);
        }
    }
}
