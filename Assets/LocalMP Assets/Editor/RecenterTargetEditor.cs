using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RecenterTarget))]
public class RecenterPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RecenterTarget[] recenterTargets = FindObjectsByType<RecenterTarget>(FindObjectsSortMode.None);

        EditorUtilities.CheckForDuplicateOrders(recenterTargets, target => target.GetTargetOrder().ToString(), "Recenter Target");
    }
}
