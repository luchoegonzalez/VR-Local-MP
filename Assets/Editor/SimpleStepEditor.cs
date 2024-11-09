using UnityEditor;

[CustomEditor(typeof(SimpleStep))]
public class SimpleStepEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SimpleStep[] simpleSteps = FindObjectsOfType<SimpleStep>();

        EditorUtilities.CheckForDuplicateOrders(simpleSteps, step => step.GetStepNumber().ToString(), "Simple Step");
    }
}
