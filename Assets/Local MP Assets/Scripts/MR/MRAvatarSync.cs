using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;

public class MRAvatarSync : NetworkBehaviour
{
    [Header("Config")]
    [Tooltip("Ocultar mi avatar local (ej. para no verme a mí mismo).")]
    [SerializeField] private bool ocultarAvatarLocal = true;

    [Tooltip("Velocidad de suavizado para avatares remotos.")]
    [SerializeField] private float smoothLerp = 15f;

    // Referencias
    private Transform sharedAnchor;        // Espacio común (mundo físico)
    private XROrigin xrOrigin;             // Rig local
    private Transform headLocal;           // Cámara (cabeza) local

    // Poses RELATIVAS al anchor (en espacio local del anchor)
    private NetworkVariable<Vector3> posLocalRel = new NetworkVariable<Vector3>(
        writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> rotLocalRel = new NetworkVariable<Quaternion>(
        writePerm: NetworkVariableWritePermission.Owner);

    void Awake()
    {
        // Buscar XR Origin (solo local lo usará de verdad, pero el warning de parenting corre para todos)
        xrOrigin = FindFirstObjectByType<XROrigin>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // TODOS los objetos (owner y no-owner) deben conocer el anchor
        TryFindSharedAnchor();

        // Si soy el dueño, consigo mi cabeza local
        if (IsOwner && xrOrigin != null && xrOrigin.Camera != null)
        {
            headLocal = xrOrigin.Camera.transform;
            if (ocultarAvatarLocal)
            {
                // Si tu avatar tiene renderers, podés ocultarlos acá
                ToggleRenderers(false);
            }
        }

        // Parentar el avatar bajo el anchor (clave para que el recenter no afecte)
        TryParentUnderAnchor();

        // Chequeo de seguridad: el anchor NO debe ser hijo del XROrigin
        if (xrOrigin && sharedAnchor && sharedAnchor.IsChildOf(xrOrigin.transform))
        {
            Debug.LogWarning("[MRAvatarSync] El SharedAnchor NO debe ser hijo del XROrigin. " +
                             "Movelo a la raíz de la escena u otro root independiente.");
        }
    }

    void Update()
    {
        if (sharedAnchor == null)
        {
            // Reintentar si el anchor apareció más tarde
            TryFindSharedAnchor();
            TryParentUnderAnchor();
            if (sharedAnchor == null) return;
        }

        if (IsOwner)
        {
            if (headLocal == null)
            {
                // Intentar reparar si la cámara no estaba lista
                if (xrOrigin != null && xrOrigin.Camera != null)
                    headLocal = xrOrigin.Camera.transform;
                if (headLocal == null) return;
            }

            // 1) Calcular pose RELATIVA al anchor
            Vector3 relPos = sharedAnchor.InverseTransformPoint(headLocal.position);
            Quaternion relRot = Quaternion.Inverse(sharedAnchor.rotation) * headLocal.rotation;

            // 2) Publicarla por red
            posLocalRel.Value = relPos;
            rotLocalRel.Value = relRot;

            // 3) (Opcional) También posicionar mi avatar local en el espacio del anchor
            //    Esto mantiene consistencia visual si decidís mostrarte a vos mismo.
            transform.localPosition = relPos;
            transform.localRotation = relRot;
        }
        else
        {
            // Reconstrucción en el otro extremo: usamos directamente localPosition/localRotation
            // porque el avatar es hijo del anchor (espacio del anchor)
            Vector3 targetPos = posLocalRel.Value;
            Quaternion targetRot = rotLocalRel.Value;

            // Suavizado para evitar jitter/teleports breves
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothLerp);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, Time.deltaTime * smoothLerp);
        }
    }

    // ----------------------------- Helpers -----------------------------

    private void TryFindSharedAnchor()
    {
        if (sharedAnchor != null) return;

        GameObject anchorObj = GameObject.FindGameObjectWithTag("SharedAnchor");
        if (anchorObj != null)
        {
            sharedAnchor = anchorObj.transform;
        }
        else
        {
            // No lo spamee cada frame, pero dejá una pista en consola
            // Debug.Log("[MRAvatarSync] Aún no encuentro objeto con tag 'SharedAnchor'.");
        }
    }

    private void TryParentUnderAnchor()
    {
        if (sharedAnchor == null) return;

        if (transform.parent != sharedAnchor)
        {
            // Guardar pose global actual
            Vector3 worldPos = transform.position;
            Quaternion worldRot = transform.rotation;

            // Parentar bajo anchor space
            transform.SetParent(sharedAnchor, worldPositionStays: true);

            // Reaplicar para quedar en el mismo lugar global y que la local quede correctamente relativa
            transform.position = worldPos;
            transform.rotation = worldRot;
        }
    }

    private void ToggleRenderers(bool enabled)
    {
        var rends = GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends) r.enabled = enabled;
    }
}
