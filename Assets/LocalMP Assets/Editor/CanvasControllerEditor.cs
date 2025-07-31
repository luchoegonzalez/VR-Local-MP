using UnityEditor;

[CustomEditor(typeof(CanvasController))]
public class CanvasControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CanvasController controller = (CanvasController)target;

        GameObjectKeyBinding[] bindings = controller.GetGameObjectKeyBindings();

        EditorUtilities.CheckForDuplicateOrders(bindings, binding => binding.key.ToString(), "Canvas Key Binding");
    }
}
