using UnityEngine;
using Unity.Netcode;

public class ActivateXRControllers : NetworkBehaviour
{

    [SerializeField]
    private GameObject[] XRControllers;

    public override void OnNetworkSpawn()
    {
        ActivateControllers();
        base.OnNetworkSpawn();
    }

    public void ActivateControllers()
    {
        foreach (var controller in XRControllers)
        {
            controller.SetActive(true);
        }
    }

    public void DeactivateControllers()
    {
        foreach (var controller in XRControllers)
        {
            controller.SetActive(false);
        }
    }
}
