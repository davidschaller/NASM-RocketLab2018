using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Problematic.StarfieldGenerator
{
	public class StarfieldGenerator : MonoBehaviour
	{
		[System.Serializable]
		public class StarfieldObject
		{
			public GameObject prefab;
			public float weight = 1.0f;
			public Gradient gradient;
			public float minSize = 0.1f;
			public float maxSize = 1.0f;
			public Color baseEmissionColor = Color.white;
			public float minEmissionStrength = 0.1f;
			public float maxEmissionStrength = 1.0f;
		}

		public Camera captureCam;
		public StarfieldObject[] starfieldObjects;
		public float minDistance = 10f;
		public float maxDistance = 25f;
		public int objectCount = 500;
		public Transform container;
		public int textureDimensions = 1024;
		public Material skyboxTemplate;
		public bool saveAssets = true;

		public void Generate ()
		{
			Clear ();

			var propertyBlock = new MaterialPropertyBlock ();
			int _color = Shader.PropertyToID ("_Color");
			int _emissionColor = Shader.PropertyToID ("_EmissionColor");

			float sigmaP = 0;
			for (int i = 0; i < starfieldObjects.Length; i++) {
				sigmaP += starfieldObjects [i].weight;
			}

			for (int i = 0; i < objectCount; i++) {
				Vector3 position = Random.onUnitSphere * Random.Range (minDistance, maxDistance);
				Vector3 dir = position - transform.position;

				StarfieldObject obj = null;
				float r = Random.Range (0f, 1f);
				float acc = 0;
				for (var j = 0; j < starfieldObjects.Length; j++) {
					acc += starfieldObjects [j].weight / sigmaP;
					if (r <= acc) {
						obj = starfieldObjects [j];
						break;
					}
				}

				var star = Instantiate (obj.prefab, position, Quaternion.LookRotation (dir), container) as GameObject;
				star.transform.localScale = Vector3.one * Random.Range (obj.minSize, obj.maxSize);
				star.transform.Rotate (star.transform.forward, Random.Range (0f, 360f), Space.World);

				propertyBlock.Clear ();

				var color = obj.gradient.Evaluate (Random.Range (0f, 1f));
				propertyBlock.SetColor (_color, color);
				propertyBlock.SetColor (_emissionColor, color + obj.baseEmissionColor * Random.Range (obj.minEmissionStrength, obj.maxEmissionStrength));
				star.GetComponent<MeshRenderer> ().SetPropertyBlock (propertyBlock);
			}
		}

		public Material GenerateSkybox ()
		{
			var cubemap = new Cubemap (textureDimensions, TextureFormat.RGB24, true);
			captureCam.RenderToCubemap (cubemap);

			var material = new Material (skyboxTemplate);
			material.SetTexture ("_Tex", cubemap);

			#if UNITY_EDITOR
			if (saveAssets) {
				AssetDatabase.CreateAsset (cubemap, AssetDatabase.GenerateUniqueAssetPath ("Assets/Starfield Cubemap.cubemap"));
				AssetDatabase.CreateAsset (material, AssetDatabase.GenerateUniqueAssetPath ("Assets/Starfield Skybox.mat"));
				AssetDatabase.Refresh ();
			}
			#endif

			return material;
		}

		public Texture2D GenerateTexture ()
		{
			var renderTex = RenderTexture.GetTemporary (textureDimensions, textureDimensions);
			RenderTexture.active = renderTex;
			captureCam.targetTexture = renderTex;
			captureCam.Render ();

			var tex = new Texture2D (textureDimensions, textureDimensions);
			tex.ReadPixels (new Rect (0, 0, textureDimensions, textureDimensions), 0, 0);
			tex.Apply ();

			#if UNITY_EDITOR
			if (saveAssets) {
				System.IO.File.WriteAllBytes (AssetDatabase.GenerateUniqueAssetPath ("Assets/Starfield Texture.png"), tex.EncodeToPNG ());
				AssetDatabase.Refresh ();
			}
			#endif

			RenderTexture.ReleaseTemporary (renderTex);
			captureCam.targetTexture = null;

			return tex;
		}

		public void Clear ()
		{
			for (int i = container.childCount - 1; i >= 0; i--) {
				DestroyImmediate (container.GetChild (i).gameObject);
			}
		}
	}
}
