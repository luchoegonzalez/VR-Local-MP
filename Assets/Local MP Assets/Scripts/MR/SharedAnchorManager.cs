using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

public class SharedAnchorManager : NetworkBehaviour
{
    public static SharedAnchorManager Instance;

    [Header("Dependencies")]
    [SerializeField] private ARAnchorManager anchorManager;
    [SerializeField] private Transform anchorSpawnPoint; // Posición inicial para el anchor (solo host)

    public Transform SharedAnchorTransform { get; private set; }

    // Usamos FixedString en lugar de SerializableGuid para serialización
    private readonly NetworkVariable<FixedString128Bytes> sharedAnchorGroupId =
        new(writePerm: NetworkVariableWritePermission.Server);

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient && !IsHost)
        {
            StartCoroutine(WaitAndLoadSharedAnchor());
        }
    }

    #region HOST

    public async void Host_CreateAndShareAnchor()
    {
        if (!IsHost)
            return;

        Debug.Log("[SharedAnchorManager] Creando anchor compartido...");

        // 1. Crear y setear el Group ID
        var groupGuid = Guid.NewGuid();
        var groupId = new SerializableGuid(groupGuid);
        var metaSubsystem = (MetaOpenXRAnchorSubsystem)anchorManager.subsystem;
        metaSubsystem.sharedAnchorsGroupId = groupId;

        sharedAnchorGroupId.Value = groupGuid.ToString(); // Guardamos como string

        // 2. Crear el anchor en el punto indicado
        var result = await anchorManager.TryAddAnchorAsync(new Pose(anchorSpawnPoint.position, anchorSpawnPoint.rotation));
        if (!result.status.IsSuccess())
        {
            Debug.LogError("[SharedAnchorManager] Error creando anchor.");
            return;
        }

        var anchor = result.value;
        SharedAnchorTransform = anchor.transform;

        // 3. Compartir anchor
        var shareResult = await anchorManager.TryShareAnchorAsync(anchor);
        if (shareResult.IsError())
        {
            Debug.LogError("[SharedAnchorManager] Error compartiendo anchor.");
        }
        else
        {
            Debug.Log("[SharedAnchorManager] Anchor compartido correctamente.");
        }
    }

    #endregion

    #region CLIENT

    private IEnumerator WaitAndLoadSharedAnchor()
    {
        yield return new WaitUntil(() => !sharedAnchorGroupId.Value.IsEmpty);

        Debug.Log("[SharedAnchorManager] GroupID recibido, cargando anchor...");
        LoadSharedAnchor(sharedAnchorGroupId.Value.ToString());
    }

    private async void LoadSharedAnchor(string groupIdStr)
    {
        if (!Guid.TryParse(groupIdStr, out Guid parsedGuid))
        {
            Debug.LogError("[SharedAnchorManager] GroupID inválido.");
            return;
        }

        var metaSubsystem = (MetaOpenXRAnchorSubsystem)anchorManager.subsystem;
        metaSubsystem.sharedAnchorsGroupId = new SerializableGuid(parsedGuid);

        var loadedAnchors = new List<XRAnchor>();
        var result = await anchorManager.TryLoadAllSharedAnchorsAsync(loadedAnchors, null);

        if (result.IsError())
        {
            Debug.LogError("[SharedAnchorManager] Error al cargar anchors.");
            return;
        }

        foreach (var xrAnchor in loadedAnchors)
        {
            GameObject anchorGO = new GameObject("SharedAnchor");
            anchorGO.transform.SetPositionAndRotation(xrAnchor.pose.position, xrAnchor.pose.rotation);
            SharedAnchorTransform = anchorGO.transform;
            Debug.Log("[SharedAnchorManager] Anchor cargado exitosamente.");
            break; // solo uno esperado
        }
    }

    #endregion
}
