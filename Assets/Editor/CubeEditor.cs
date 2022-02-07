using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CubeScript)), CanEditMultipleObjects]
public class CubeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CubeScript _cs = (CubeScript)target;

        GUILayout.Label("Cube editor");
        //base.OnInspectorGUI();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Generate Color"))
        {
            _cs.Generator();
        }

        if (GUILayout.Button("Reset"))
        {
            _cs.Reset();
        }
        GUILayout.EndHorizontal();
        _cs.size = EditorGUILayout.Slider("Size",_cs.size, 0.1f, 5f);
        _cs.transform.localScale = _cs.size * Vector3.one;
    }
}
