using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCreatorScript)), CanEditMultipleObjects]
public class LevelCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        LevelCreatorScript script = (LevelCreatorScript)target;
        if(GUILayout.Button("CreateWorld"))
        {
            script.CreateWorld();
        }

        if(GUILayout.Button("GenerateWalls"))
        {
            script.GenerateWalls();
        }

        if(GUILayout.Button("PlaceWalls"))
        {
            script.SetAllWallsIntoPlace();
        }
    }
}
