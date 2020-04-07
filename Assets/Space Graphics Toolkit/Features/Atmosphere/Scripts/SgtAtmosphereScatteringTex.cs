using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmosphereScatteringTex))]
	public class SgtAtmosphereScatteringTex_Editor : SgtEditor<SgtAtmosphereScatteringTex>
	{
		protected override void OnInspector()
		{
			var updateTexture = false;

			BeginError(Any(t => t.Width < 1));
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
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to generate the SgtAtmosphere.ScatteringTex field.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmosphere))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmosphereScatteringTex")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere ScatteringTex")]
	public class SgtAtmosphereScatteringTex : SgtBehaviour
	{
		/// <summary>The width of the generated texture. A higher value can result in a smoother transition.</summary>
		[FormerlySerializedAs("width")] public int Width = 64; public void SetWidth(int value) { Width = value; UpdateTexture(); }

		/// <summary>The format of the generated texture.</summary>
		[FormerlySerializedAs("format")] public TextureFormat Format = TextureFormat.ARGB32; public void SetFormat(TextureFormat value) { Format = value; UpdateTexture(); }

		/// <summary>The transition style between the day and night.</summary>
		[FormerlySerializedAs("sunsetEase")] public SgtEase.Type SunsetEase = SgtEase.Type.Smoothstep; public void SetSunsetEase(SgtEase.Type value) { SunsetEase = value; UpdateTexture(); }

		/// <summary>The start point of the day/sunset transition (0 = dark side, 1 = light side).</summary>
		[FormerlySerializedAs("sunsetStart")] [Range(0.0f, 1.0f)] public float SunsetStart = 0.35f; public void SetSunsetStart(float value) { SunsetStart = value; UpdateTexture(); }

		/// <summary>The end point of the sunset/night transition (0 = dark side, 1 = light side).</summary>
		[FormerlySerializedAs("sunsetEnd")] [Range(0.0f, 1.0f)] public float SunsetEnd = 0.6f; public void SetSunsetEnd(float value) { SunsetEnd = value; UpdateTexture(); }

		/// <summary>The sharpness of the sunset red channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessR")] public float SunsetSharpnessR = 2.0f; public void SetSunsetSharpnessR(float value) { SunsetSharpnessR = value; UpdateTexture(); }

		/// <summary>The sharpness of the sunset green channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessG")] public float SunsetSharpnessG = 2.0f; public void SetSunsetSharpnessG(float value) { SunsetSharpnessG = value; UpdateTexture(); }

		/// <summary>The sharpness of the sunset blue channel transition.</summary>
		[FormerlySerializedAs("sunsetSharpnessB")] public float SunsetSharpnessB = 2.0f; public void SetSunsetSharpnessB(float value) { SunsetSharpnessB = value; UpdateTexture(); }

		[System.NonSerialized]
		private Texture2D generatedTexture;

		[System.NonSerialized]
		private SgtAtmosphere cachedAtmosphere;

		[System.NonSerialized]
		private bool cachedAtmosphereSet;

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
		/// Once done, you can remove this component, and set the <b>SgtAtmosphere</b> component's <b>ScatteringTex</b> setting using the exported asset.</summary>
		[ContextMenu("Export Texture")]
		public void ExportTexture()
		{
			var importer = SgtHelper.ExportTextureDialog(generatedTexture, "Atmosphere Scattering");

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

		[ContextMenu("Update Texture")]
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
					generatedTexture = SgtHelper.CreateTempTexture2D("Scattering (Generated)", Width, 1, Format);

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
			if (CachedAtmosphere.ScatteringTex != generatedTexture)
			{
				cachedAtmosphere.SetScatteringTex(generatedTexture);
			}
		}

		[ContextMenu("Remove Texture")]
		public void RemoveTexture()
		{
			if (CachedAtmosphere.ScatteringTex == generatedTexture)
			{
				cachedAtmosphere.SetScatteringTex(null);
			}
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
			color.a = (color.r + color.g + color.b) / 3.0f;

			generatedTexture.SetPixel(x, 0, color);
		}
	}
}