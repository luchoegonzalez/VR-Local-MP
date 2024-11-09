using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : NetworkBehaviour {

    [Header("Input Actions")]
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;

    // [Header("Hand Animator")]
    [SerializeField]
    private Animator handAnimator;
    // [SerializeField]
    // private NetworkAnimator networkAnimator;
    // // Podemos usar el NetworkAnimator para sincronizar la animacion en la red con un RPC

    private void Update() {
        if (!IsOwner) return;

        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);

        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);

    }

    // [ServerRpc]
    // public void AnimateHandOnServerServerRpc(float triggerValue, float gripValue) {
    //     handAnimator.SetFloat("Trigger", triggerValue);
    //     handAnimator.SetFloat("Grip", gripValue);
    // }

}
