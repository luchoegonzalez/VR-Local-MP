using UnityEditor;

[CustomEditor(typeof(RecenterTarget))]
public class RecenterPointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RecenterTarget[] recenterTargets = FindObjectsOfType<RecenterTarget>();

        EditorUtilities.CheckForDuplicateOrders(recenterTargets, target => target.GetTargetOrder().ToString(), "Recenter Target");
    }
}
