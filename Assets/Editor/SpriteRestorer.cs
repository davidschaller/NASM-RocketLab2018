using UnityEngine;
using UnityEditor;
using System.Collections;

public class SpriteRestorer : EditorWindow
{
    [MenuItem ("Custom/Sprite Restorer")]
    static void DoRestore()
    {
        EditorWindow.GetWindow<SpriteRestorer>();
    }

    [MenuItem("Custom/Move Labels Closer")]
    static void MoveCloser()
    {
        foreach (Transform tr in Selection.transforms)
        {
            if (tr.GetComponent<UILabel>())
            {
                if (tr.localPosition.z == 0)
                {
                    tr.localPosition = new Vector3(tr.localPosition.x, tr.localPosition.y, -1);
                }
            }
        }
    }

    UIAtlas atlas;
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Parent Panel:");
        atlas = EditorGUILayout.ObjectField(atlas, typeof(UIAtlas), true) as UIAtlas;
        GUILayout.EndHorizontal();

        GUILayout.Label("Select sprites with missing Atlas and press the button");
        if (GUILayout.Button("Assign Atlas"))
        {
            foreach (Transform tr in Selection.transforms)
            {
                if (tr.GetComponent<UISprite>())
                {
                    if (tr.GetComponent<UISprite>().atlas != null)
                    {
                        tr.GetComponent<UISprite>().atlas = atlas;
                    }
                }
            }
        }
    }
}