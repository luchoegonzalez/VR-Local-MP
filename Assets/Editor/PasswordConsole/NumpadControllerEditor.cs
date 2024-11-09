using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(NumpadController))]
public class NumpadControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NumpadController numpadController = (NumpadController)target;

        if (GUILayout.Button("Auto-Assign Numpad Buttons"))
        {
            Button[] buttons = numpadController.GetComponentsInChildren<Button>();

            if (buttons.Length > 10)
            {
                EditorGUILayout.HelpBox("The numpad can only have 10 buttons", MessageType.Warning);
                return;
            }

            NumpadButton[] numpadButtons = new NumpadButton[buttons.Length];

            for (int i = 0; i < buttons.Length; i++)
            {
                // We want the button number to be 1-9, then 0
                char buttonNumber = (i + 1) % 10 == 0 ? '0' : (char)('0' + (i + 1) % 10);
                numpadButtons[i] = new NumpadButton(buttons[i], buttonNumber);
            }

            // Assign to the editor properties
            SerializedProperty numpadButtonsProperty = serializedObject.FindProperty("numpadButtons");
            numpadButtonsProperty.arraySize = numpadButtons.Length;
            for (int i = 0; i < numpadButtons.Length; i++)
            {
                SerializedProperty element = numpadButtonsProperty.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("m_Button").objectReferenceValue = numpadButtons[i].Button;
                element.FindPropertyRelative("m_Value").intValue = numpadButtons[i].Value;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
