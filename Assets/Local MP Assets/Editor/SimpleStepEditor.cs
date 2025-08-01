using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimpleStep))]
public class SimpleStepEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleStep[] simpleSteps = FindObjectsByType<SimpleStep>(FindObjectsSortMode.None);

        EditorUtilities.CheckForDuplicateOrders(simpleSteps, step => step.GetStepNumber().ToString(), "Simple Step");
    }
}
