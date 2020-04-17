using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(RandomStarfield))]
public class RandomStarfieldEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh"))
            (target as RandomStarfield).Refresh();
    }
}
