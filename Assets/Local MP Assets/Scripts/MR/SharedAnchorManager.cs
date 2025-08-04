using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.XR.CoreUtils.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.Meta;

public class SharedAnchorManager : NetworkBehaviour
{
    public ARAnchorManager anchorManager;
    public TMP_Text debugText;

    private MetaOpenXRAnchorSubsystem metaSubsystem => (MetaOpenXRAnchorSubsystem)anchorManager.subsystem;

    private readonly NetworkVariable<FixedString128Bytes> sharedGroupId =
        new(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GenerateAndSetGroupID();
            HostCreateAndShareAnchor();
        }
        else
        {
            //TODO: separar logica de compartir
            LoadSharedAnchors();
        }
    }

    private void GenerateAndSetGroupID()
    {
        var generatedGuid = Guid.NewGuid();
        var serializableGuid = new SerializableGuid(generatedGuid);

        metaSubsystem.sharedAnchorsGroupId = serializableGuid;
        sharedGroupId.Value = generatedGuid.ToString();

        Debug.Log($"[Host] Generated Shared Group ID: {generatedGuid}");
        debugText.text = $"Shared Group ID: {generatedGuid}";
    }

    private void OnGroupIdChanged(FixedString128Bytes oldValue, FixedString128Bytes newValue)
    {
        if (Guid.TryParse(newValue.ToString(), out Guid parsedGuid))
        {
            var serializableGuid = new SerializableGuid(parsedGuid);
            metaSubsystem.sharedAnchorsGroupId = serializableGuid;

            Debug.Log($"[Client] Received and set Shared Group ID: {parsedGuid}");
            LoadSharedAnchors();
        }
        else
        {
            Debug.LogError("[Client] Failed to parse Shared Group ID");
        }
    }

    public async void HostCreateAndShareAnchor()
    {
        if (!IsServer) return;

        Pose pose = new Pose(anchorManager.transform.position, anchorManager.transform.rotation);

        var addResult = await anchorManager.TryAddAnchorAsync(pose);
        if (addResult.status.IsError())
        {
            Debug.LogError($"[Host] Failed to add anchor: {addResult.status}");
            return;
        }

        var anchor = addResult.value;

        var shareResult = await anchorManager.TryShareAnchorAsync(anchor);
        if (shareResult.IsError())
        {
            debugText.text = $"Failed to share anchor: {shareResult.statusCode}";
            Debug.LogError($"[Host] Failed to share anchor: {shareResult.statusCode}");
        }
        else
        {
            debugText.text = $"Anchor shared successfully with ID: {anchor.trackableId}";
            Debug.Log($"[Host] Anchor shared successfully.");
        }
    }

    private async void LoadSharedAnchors()
    {
        var loadedAnchors = new List<XRAnchor>();

        var result = await anchorManager.TryLoadAllSharedAnchorsAsync(
            loadedAnchors,
            OnIncrementalAnchorsLoaded
        );

        if (result.IsError())
        {
            Debug.LogError($"[Client] Failed to load shared anchors: {result}");
            debugText.text = $"Failed to load shared anchors: {result}";
            return;
        }

        Debug.Log($"[Client] Loaded {loadedAnchors.Count} anchors from shared group.");
        debugText.text = $"Loaded {loadedAnchors.Count} anchors from shared group.";
    }

    private void OnIncrementalAnchorsLoaded(ReadOnlyListSpan<XRAnchor> anchors)
    {
        foreach (var xrAnchor in anchors)
        {
            Debug.Log($"[Client] Incrementally loaded shared anchor: {xrAnchor.trackableId}");
            // TODO: Instanciar contenido sobre el anchor
        }
    }
}
