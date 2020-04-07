using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmosphereLightingTex))]
	public class SgtAtmosphereLightingTex_Editor : SgtEditor<SgtAtmosphereLightingTex>
	{
		protected override void OnInspector()
		{
			var updateTexture = false;
			var updateApply   = false;

			BeginError(Any(t => t.Width <= 1));
				DrawDefault("Width", ref updateTexture, "The width of the generated texture. A higher value can result in a smoother transition.");
			EndError();
			DrawDefault("Format", ref updateTexture, "The format of the generated texture.");

			Separator();

			DrawDefault("SunsetEase", ref updateTexture, "The transition style between the day and night.");
			BeginError(Any(t => t.SunsetStart >= t.SunsetEnd));
				DrawDefault("SunsetStart", ref updateTexture, "The start point of the day/sunset transition (0 = dark side, 1 = light side).");
				DrawDefault("SunsetEnd", ref updateTexture, "The end point of the sunset/night transition (0 = dark side, 1 = light side).");
			EndError();
			DrawDefault("SunsetSharpnessR", ref updateTexture, "The sharpness of the sunset red channel transition.");
			DrawDefault("SunsetSharpnessG", ref updateTexture, "The sharpness of the sunset green channel transition.");
			DrawDefault("SunsetSharpnessB", ref updateTexture, "The sharpness of the sunset blue channel transition.");

			if (updateTexture == true) DirtyEach(t => t.UpdateTexture());
			if (updateApply   == true) DirtyEach(t => t.ApplyTexture ());
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to generate the SgtAtmosphere.LightingTex field.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmosphere))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmosphereLightingTex")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere LightingTex")]
	public class SgtAtmosphereLightingTex : MonoBehaviour
	{
		/// <summary>The width of the generated texture. A higher value can result in a smoother transition.</summary>
		[FormerlySerializedAs("width")] public int Width = 256; public void SetWidth(int value) { Width = value; }

		/// <summary>The format of the generated texture.</summary>
		[FormerlySerializedAs("format")] public TextureFormat Format = TextureFormat.ARGB32; public void SetFormat(TextureFormat value) { Format = value; }

		/// <summary>The transition style between the day and night.</summary>
		[FormerlySerializedAs("sunsetEase")] public SgtEase.Type SunsetEase = SgtEase.Type.Smoothstep; public void SetSunsetEase(SgtEase.Type value) { SunsetEase = value; }

		/// <summary>The start point of the day/sunset transition (0 = dark side, 1 = light side).</summary>
		[FormerlySerializedAs("sunsetStart")] [Range(0.0f, 1.0f)] public float SunsetStart = 0.45f; public void SetSunsetStart(float value) { SunsetStart = value; }

		/// <summary>The end point of the sunset/night transition (0 = dark side, 1 = light side).</summary>
		[FormerlySerializedAs("sunsetEnd")] [Range(0.0f, 1.0f)] public float SunsetEnd = 1.0f; public void SetSunsetEnd(float value) { SunsetEnd = value; }

		/// <summary>The sharpness of the sunset red channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessR")] public float SunsetSharpnessR = 3.0f; public void SetSunsetSharpnessR(float value) { SunsetSharpnessR = value; }

		/// <summary>The sharpness of the sunset green channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessG")] public float SunsetSharpnessG = 2.0f; public void SetSunsetSharpnessG(float value) { SunsetSharpnessG = value; }

		/// <summary>The sharpness of the sunset blue channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessB")] public float SunsetSharpnessB = 2.0f; public void SetSunsetSharpnessB(float value) { SunsetSharpnessB = value; }

		[System.NonSerialized]
		private Texture2D generatedTexture;

		[System.NonSerialized]
		private SgtAtmosphere cachedAtmosphere;

		[System.NonSerialized]
		private bool cachedAtmosphereSet;

		[System.NonSerialized]
		private bool quitting;

		public SgtAtmosphere CachedAtmosphere
		{
			get
			{
				if (cachedAtmosphereSet == false)
				{
					cachedAtmosphere    = GetComponent<SgtAtmosphere>();
					cachedAtmosphereSet = true;
				}

				return cachedAtmosphere;
			}
		}

		public Texture2D GeneratedTexture
		{
			get
			{
				return generatedTexture;
			}
		}

#if UNITY_EDITOR
		/// <summary>This method allows you to export the generated texture as an asset.
		/// Once done, you can remove this component, and set the <b>SgtAtmosphere</b> component's <b>LightingTex</b> setting using the exported asset.</summary>
		[ContextMenu("Export Texture")]
		public void ExportTexture()
		{
			var importer = SgtHelper.ExportTextureDialog(generatedTexture, "Atmosphere Lighting");

			if (importer != null)
			{
				importer.textureCompression  = TextureImporterCompression.Uncompressed;
				importer.alphaSource         = TextureImporterAlphaSource.FromInput;
				importer.wrapMode            = TextureWrapMode.Clamp;
				importer.filterMode          = FilterMode.Trilinear;
				importer.anisoLevel          = 16;
				importer.alphaIsTransparency = true;

				importer.SaveAndReimport();
			}
		}
#endif

		[ContextMenu("Update Textures")]
		public void UpdateTexture()
		{
			if (Width > 0)
			{
				// Destroy if invalid
				if (generatedTexture != null)
				{
					if (generatedTexture.width != Width || generatedTexture.height != 1 || generatedTexture.format != Format)
					{
						generatedTexture = SgtHelper.Destroy(generatedTexture);
					}
				}

				// Create?
				if (generatedTexture == null)
				{
					generatedTexture = SgtHelper.CreateTempTexture2D("Lighting (Generated)", Width, 1, Format);

					generatedTexture.wrapMode = TextureWrapMode.Clamp;

					ApplyTexture();
				}

				var stepU = 1.0f / (Width  - 1);

				for (var x = 0; x < Width; x++)
				{
					WritePixel(stepU * x, x);
				}

				generatedTexture.Apply();
			}
		}

		[ContextMenu("Apply Texture")]
		public void ApplyTexture()
		{
			if (CachedAtmosphere.LightingTex != generatedTexture)
			{
				cachedAtmosphere.SetLightingTex(generatedTexture);
			}
		}

		[ContextMenu("Remove Texture")]
		public void RemoveTexture()
		{
			if (CachedAtmosphere.LightingTex == generatedTexture)
			{
				cachedAtmosphere.SetLightingTex(null);
			}
		}

		protected virtual void OnApplicationQuit()
		{
			quitting = true;
		}

		protected virtual void OnEnable()
		{
			UpdateTexture();
			ApplyTexture();
		}

		protected virtual void OnDisable()
		{
			if (quitting == false)
			{
				RemoveTexture();
			}
		}

		protected virtual void OnDestroy()
		{
			if (quitting == false)
			{
				SgtHelper.Destroy(generatedTexture);
			}
		}

		private void WritePixel(float u, int x)
		{
			var sunsetU = Mathf.InverseLerp(SunsetEnd, SunsetStart, u);
			var color   = default(Color);

			color.r = SgtEase.Evaluate(SunsetEase, 1.0f - SgtHelper.Sharpness(sunsetU, SunsetSharpnessR));
			color.g = SgtEase.Evaluate(SunsetEase, 1.0f - SgtHelper.Sharpness(sunsetU, SunsetSharpnessG));
			color.b = SgtEase.Evaluate(SunsetEase, 1.0f - SgtHelper.Sharpness(sunsetU, SunsetSharpnessB));
			color.a = 0.0f;

			generatedTexture.SetPixel(x, 0, color);
		}
	}
}