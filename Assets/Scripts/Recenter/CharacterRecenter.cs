using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class CharacterRecenter : MonoBehaviour
{
    // public Transform[] targets;

    public void Recenter(Vector3 position, Quaternion rotation)
    {
        XROrigin xrOrigin = GetComponent<XROrigin>();
        Vector3 targetPosition = position;
        targetPosition.y += 1f;
        xrOrigin.MoveCameraToWorldLocation(targetPosition);
        xrOrigin.MatchOriginUpCameraForward(rotation * Vector3.up, rotation * Vector3.forward);
    }

    // public void Recenter(Transform target)
    // {
    //     XROrigin xrOrigin = GetComponent<XROrigin>();
    //     Vector3 targetPosition = target.position;
    //     targetPosition.y += 1f;
    //     xrOrigin.MoveCameraToWorldLocation(targetPosition);
    //     xrOrigin.MatchOriginUpCameraForward(target.up, target.forward);
    // }

    public void Recenter(Transform target)
    {
        Recenter(target.position, target.rotation);
    }

    public void Recenter(RecenterTarget target)
    {
        Recenter(target.GetTarget());
    }


}
