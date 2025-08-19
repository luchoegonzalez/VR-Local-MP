using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using XRMultiplayer;

public class MRAvatarSync : NetworkBehaviour
{
    [Header("Referencia al SharedAnchor (se busca por Tag)")]
    private Transform sharedAnchor;

    [Header("XR Rig / Cabeza del jugador local")]
    private Transform m_HeadOrigin;
    private XROrigin m_XROrigin;

    private NetworkVariable<Vector3> posRelativa = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> rotRelativa = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner);

    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            XRINetworkGameManager.Instance.LocalPlayerConnected(NetworkObject.OwnerClientId);

            m_XROrigin = FindFirstObjectByType<XROrigin>();
            if (m_XROrigin != null)
            {
                Utils.Log("XR Rig Available");
                m_HeadOrigin = m_XROrigin.Camera.transform;
            }
            else
            {
                Utils.Log("No XR Rig Available", 1);
            }

            Invoke("FindSharedAnchor", 10f);
        }
    }

    void Update()
    {
        if (sharedAnchor == null) return;

        if (IsOwner)
        {
            // Calcular posición y rotación relativas al anchor
            Vector3 relPos = sharedAnchor.InverseTransformPoint(m_HeadOrigin.position);
            Quaternion relRot = Quaternion.Inverse(sharedAnchor.rotation) * m_HeadOrigin.rotation;

            // Guardar en las variables de red
            posRelativa.Value = relPos;
            rotRelativa.Value = relRot;

            // Opcional: el avatar local copia la posición de la cabeza XR (no te ves a vos mismo si ocultás el modelo)
            transform.position = m_HeadOrigin.position;
            transform.rotation = m_HeadOrigin.rotation;
        }
        else
        {
            // Reconstruir posición absoluta desde la data recibida
            Vector3 absPos = sharedAnchor.TransformPoint(posRelativa.Value);
            Quaternion absRot = sharedAnchor.rotation * rotRelativa.Value;

            transform.position = absPos;
            transform.rotation = absRot;
        }
    }

    void FindSharedAnchor()
    {
        if (sharedAnchor == null)
        {
            GameObject anchorObj = GameObject.FindGameObjectWithTag("SharedAnchor");
            if (anchorObj != null)
                sharedAnchor = anchorObj.transform;
            else
                Debug.LogError("No se encontró un objeto con el Tag 'SharedAnchor' en la escena.");
        }
    }
}
