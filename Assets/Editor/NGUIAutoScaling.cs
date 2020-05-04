using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class NGUIAutoScaling : MonoBehaviour
{
	[MenuItem("NGUI/Scale 2D Panel")]
	static void Scale2D()
    {
		if (Selection.activeGameObject == null) {
			Debug.Log("Please select an object in the hierarchy");
			return;
		}

		GameObject go = Selection.activeGameObject;
		UIPanel panel = go.GetComponent<UIPanel>() as UIPanel;
		if (panel == null) {
			Debug.Log("Please select a UIPanel in the hierarchy");
			return;
		}

//		Undo.RegisterCompleteObjectUndo(panel.transform, "Panel scale");

		panel.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

		foreach (Transform t in go.transform) {
			if (t.gameObject.layer == 8) {
				Undo.RegisterCompleteObjectUndo(t, "Child position");
				Vector3 pos = t.localPosition;
				t.localPosition = new Vector3(pos.x * 100, pos.y * 10, pos.z);
			}
		}
	}

	[MenuItem("NGUI/Scale 3D Panel")]
	static void Scale3D()
	{
		if (Selection.activeGameObject == null) {
			Debug.Log("Please select an object in the hierarchy");
			return;
		}

		GameObject go = Selection.activeGameObject;
		UIPanel panel = go.GetComponent<UIPanel>() as UIPanel;
		if (panel == null) {
			Debug.Log("Please select a UIPanel in the hierarchy");
			return;
		}

//		Undo.RegisterCompleteObjectUndo(panel.transform, 'Panel scale');

		panel.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

		foreach (Transform t in go.transform) {
			if (t.gameObject.layer == 9) {
				Undo.RegisterCompleteObjectUndo(t, "Child position");
				Vector3 pos = t.localPosition;
				t.localPosition = new Vector3(pos.x * 100, pos.y * 100, pos.z);
			}
		}
	}
}
