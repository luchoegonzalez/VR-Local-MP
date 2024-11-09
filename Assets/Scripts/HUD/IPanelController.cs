
using UnityEngine;

/// <summary>
/// Abstract class for Panel Controllers
/// </summary>
/// <remarks>
/// This class needs to be abstract (Not an interface) to be shown in the Unity Editor
/// </remarks>
/// <remarks>
/// Interfaces are not serialized by Unity.
public abstract class IPanelController : MonoBehaviour
{
    public abstract void Initiate();
    public abstract void Reset();
}
