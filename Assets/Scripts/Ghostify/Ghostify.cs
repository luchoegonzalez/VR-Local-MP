using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Esconde los Renderers de un avatar en todos los clientes.
/// </summary>
/// <remarks>
/// Se puede usar para hacer que un avatar sea invisible para todos los clientes.
/// </remarks>
public class Ghostify : NetworkBehaviour
{

    // Arreglo de MeshRenderers que se van a esconder.
    public Renderer[] renderers;

    // Disable the renderers on all clients on Spawn
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        // if (IsHost && IsOwner)
        Debug.Log("Ghostify OnNetworkSpawn" + OwnerClientId);
        //if (OwnerClientId == 0) // Si es el host
        //{

        //}
        foreach (var renderer in renderers)
        {
            renderer.enabled = false;
        }
    }

}
