using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLazyFolow : MonoBehaviour
{
    [SerializeField] private Transform targetPos;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothness = 0.1f;
    private void Start()
    {
        if(targetPos == null)
            targetPos = Camera.main.transform;
    }

    private void Update()
    {
        if (targetPos == null) return;

        Vector3 flatForward = targetPos.forward;
        flatForward.y = 0;
        flatForward.Normalize();


        Vector3 desiredPosition = targetPos.position
                                + flatForward * offset.z
                                + targetPos.right * offset.x;

        desiredPosition.y = targetPos.position.y + offset.y;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothness);

        Quaternion desiredRotation = Quaternion.Euler(0, targetPos.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothness);
    }

}
