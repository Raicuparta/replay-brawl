using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LevelLoader))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelLoader levelLoader = (LevelLoader)target;

        // Creates the load level button.
        if (GUILayout.Button("Load Level"))
        {
            levelLoader.Start();
        }

        // Creates the clear level button.
        if (GUILayout.Button("Clear Level"))
        {
            levelLoader.Clear();
        }
    }
}