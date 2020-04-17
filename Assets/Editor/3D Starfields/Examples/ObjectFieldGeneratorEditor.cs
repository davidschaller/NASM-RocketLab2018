using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ObjectFieldGenerator))]
public class ObjectFieldGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
            (target as ObjectFieldGenerator).Regenerate();
        
        if (GUILayout.Button("Clear"))
            (target as ObjectFieldGenerator).DeleteAllChildren();
    }
}
