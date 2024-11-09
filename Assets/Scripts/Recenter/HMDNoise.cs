using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// HMDNoise is a NetworkBehaviour that triggers a noise on a specific client.
/// </summary>
public class HMDNoise : NetworkBehaviour
{
    [SerializeField] private AudioSource audioSource;

    /// <summary>
    /// Triggers a noise on a specific client using an RPC.
    /// </summary>
    /// <param name="clientId">The client id to trigger the noise on</param>
    public void TriggerNoiseOnClient(ulong clientId)
    {
        if (IsServer)
        {
            NoiseSpecifiedRpc(RpcTarget.Single(clientId, RpcTargetUse.Temp));
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void NoiseSpecifiedRpc(RpcParams rpcParams = default)
    {
        PlayNoise();
    }

    private void PlayNoise()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource or AudioClip is not set.");
        }
    }
}
