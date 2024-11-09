using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PasswordConsole))]
public class PasswordConsoleEditor : Editor
{

    public override void OnInspectorGUI()
    {
        PasswordConsole passwordConsole = (PasswordConsole)target;
        if (passwordConsole.GetAudioSource == null)
        {
            passwordConsole.SetAudioSource(passwordConsole.GetComponent<AudioSource>());
            if (passwordConsole.GetAudioSource == null)
            {
                EditorGUILayout.HelpBox("No AudioSource found in the GameObject", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
        }
        DrawDefaultInspector();
    }

}
