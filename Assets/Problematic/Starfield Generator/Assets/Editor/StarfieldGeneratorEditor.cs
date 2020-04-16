using UnityEngine;
using UnityEditor;

namespace Problematic.StarfieldGenerator
{
	[CustomEditor (typeof(StarfieldGenerator))]
	public class StarfieldGeneratorEditor : Editor
	{
		StarfieldGenerator generator;

		void OnEnable ()
		{
			generator = target as StarfieldGenerator;
		}

		public override void OnInspectorGUI ()
		{
			DrawDefaultInspector ();
			EditorGUILayout.Space ();
			if (GUILayout.Button ("Regenerate Starfield")) {
				generator.Generate ();
			}
			if (GUILayout.Button ("Generate Skybox")) {
				generator.GenerateSkybox ();
			}
			if (GUILayout.Button ("Generate Texture")) {
				generator.GenerateTexture ();
			}
			if (GUILayout.Button ("Clear Starfield Objects")) {
				generator.Clear ();
			}
		}
	}
}
