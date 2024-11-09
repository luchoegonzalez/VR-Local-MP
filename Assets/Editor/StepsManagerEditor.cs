using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StepsManager))]
public class StepsManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        StepsManager stepsManager = (StepsManager)target;

        DrawDefaultInspector();

        if (stepsManager.makeSound)
        {
            // If the stepCompletedSound is not set, create an AudioSource component
            if (stepsManager.stepCompletedSound == null)
            {
                stepsManager.stepCompletedSound = stepsManager.gameObject.GetComponent<AudioSource>();
                if (stepsManager.stepCompletedSound == null)
                {
                    stepsManager.stepCompletedSound = stepsManager.gameObject.AddComponent<AudioSource>();
                }
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("stepCompletedSound"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}