using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// This class is used to detect trigger events and trigger a UnityEvent when a trigger is detected.
/// </summary>
public class TriggerEvent : MonoBehaviour
{
    [Tooltip("If true, only the tag object especified in the configuration in tagToTrigger will trigger the event.")]
    [SerializeField] public bool onlyTag;
    [SerializeField] public string objectTag;

    [Header("Events")]
    public UnityEvent<bool> onTriggerEntered;
    public UnityEvent<bool> onTriggerExited;
    public UnityEvent<bool> onTriggerStay;


    private void OnTriggerEnter(Collider other)
    {
        Detection(other, onTriggerEntered);
    }

    private void OnTriggerExit(Collider other)
    {
        Detection(other, onTriggerExited);
    }

    private void OnTriggerStay(Collider other)
    {
        Detection(other, onTriggerStay);
    }

    private void Detection(Collider other, UnityEvent<bool> unityEvent)
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
