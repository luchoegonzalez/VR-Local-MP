using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Maquina de estados que controla la animación de la puerta. (Networked)
/// </summary>
public class AnimatedDoorController : NetworkBehaviour
{
    [SerializeField] private Animator m_Animator;
    // Como el valor de la puerta es compartido entre todos los clientes, lo guardamos en una NetworkVariable.
    private NetworkVariable<bool> m_IsOpen = new NetworkVariable<bool>(false);

    private void Start()
    {
        m_IsOpen.OnValueChanged += OnDoorStateChanged;
    }

    public void ToggleDoor(bool toggle)
    {
        if (!toggle) return;

        if (IsServer)
        {
            m_IsOpen.Value = !m_IsOpen.Value;
        }
        else
        {
            Debug.LogWarning("Only the server can toggle the door.");
        }
    }

    private void OnDoorStateChanged(bool ikdValue, bool newValue)
    {
        m_Animator.SetBool("IsOpen", newValue);
    }

    public override void OnDestroy()
    {
        m_IsOpen.OnValueChanged -= OnDoorStateChanged;
    }
}
