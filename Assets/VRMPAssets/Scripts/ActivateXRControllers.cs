using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateXRControllers : NetworkBehaviour {

    [SerializeField]
    private ActionBasedController[] XRControllers;

    public override void OnNetworkSpawn() {
        ActivateControllers();
        base.OnNetworkSpawn();
    }

    public void ActivateControllers() {
        for (int i = 0; i < XRControllers.Length; i++) {
            XRControllers[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateControllers() {
        for (int i = 0; i < XRControllers.Length; i++) {
            XRControllers[i].gameObject.SetActive(false);
        }
    }

}
