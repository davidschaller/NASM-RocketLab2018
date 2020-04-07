using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmosphereDepthTex))]
	public class SgtAtmosphereDepthTex_Editor : SgtEditor<SgtAtmosphereDepthTex>
	{
		protected override void OnInspector()
		{
			var updateTexture = false;
			var updateApply   = false;

			BeginError(Any(t => t.Width < 1));
				DrawDefault("Width", ref updateTexture, "The width of the generated texture. A higher value can result in a smoother transition.");
			EndError();
			DrawDefault("Format", ref updateTexture, "The format of the generated texture.");
			DrawDefault("HorizonColor", ref updateTexture, "This allows you to set the color that appears on the horizon.");

			Separator();

			DrawDefault("InnerColor", ref updateTexture, "The base color of the inner texture.");
			DrawDefault("InnerEase", ref updateTexture, "The transition style between the surface and horizon.");
			DrawDefault("InnerColorSharpness", ref updateTexture, "The strength of the inner texture transition.");
			DrawDefault("InnerAlphaSharpness", ref updateTexture, "The strength of the inner texture transition.");

			Separator();

			DrawDefault("OuterColor", ref updateTexture, "The base color of the outer texture.");
			DrawDefault("OuterEase", ref updateTexture, "The transition style between the sky and horizon.");
			DrawDefault("OuterColorSharpness", ref updateTexture, "The strength of the outer texture transition.");
			DrawDefault("OuterAlphaSharpness", ref updateTexture, "The strength of the outer texture transition.");

			if (updateTexture == true) DirtyEach(t => t.UpdateTextures());
			if (updateApply   == true) DirtyEach(t => t.ApplyTextures   ());
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to generate the SgtAtmosphere.InnerDepthTex and SgtAtmosphere.OuterDepthTex fields.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmosphere))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmosphereDepthTex")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere DepthTex")]
	public class SgtAtmosphereDepthTex : SgtBehaviour
	{
		/// <summary>The width of the generated texture. A higher value can result in a smoother transition.</summary>
		[FormerlySerializedAs("width")] public int Width = 256; public void SetWidth(int value) { Width = value; UpdateTextures(); }

		/// <summary>The format of the generated texture.</summary>
		[FormerlySerializedAs("format")] public TextureFormat Format = TextureFormat.ARGB32; public void SetFormat(TextureFormat value) { Format = value; UpdateTextures(); }

		/// <summary>This allows you to set the color that appears on the horizon.</summary>
		[FormerlySerializedAs("horizonColor")] public Color HorizonColor = Color.white; public void SetHorizonColor(Color value) { HorizonColor = value; UpdateTextures(); }

		/// <summary>The base color of the inner texture.</summary>
		[FormerlySerializedAs("innerColor")] public Color InnerColor = new Color(0.15f, 0.54f, 1.0f); public void SetInnerColor(Color value) { InnerColor = value; UpdateTextures(); }

		/// <summary>The transition style between the surface and horizon.</summary>
		[FormerlySerializedAs("innerEase")] public SgtEase.Type InnerEase = SgtEase.Type.Exponential; public void SetInnerEase(SgtEase.Type value) { InnerEase = value; UpdateTextures(); }

		/// <summary>The strength of the inner texture transition.</summary>
		[FormerlySerializedAs("innerColorSharpness")] public float InnerColorSharpness = 2.0f; public void SetInnerColorSharpness(float value) { InnerColorSharpness = value; UpdateTextures(); }

		/// <summary>The strength of the inner texture transition.</summary>
		[FormerlySerializedAs("innerAlphaSharpness")] public float InnerAlphaSharpness = 3.0f; public void SetInnerAlphaSharpness(float value) { InnerAlphaSharpness = value; UpdateTextures(); }

		/// <summary>The base color of the outer texture.</summary>
		[FormerlySerializedAs("outerColor")] public Color OuterColor = new Color(0.29f, 0.73f, 1.0f); public void SetOuterColor(Color value) { OuterColor = value; UpdateTextures(); }

		/// <summary>The transition style between the sky and horizon.</summary>
		[FormerlySerializedAs("outerEase")] public SgtEase.Type OuterEase = SgtEase.Type.Quadratic; public void SetOuterEase(SgtEase.Type value) { OuterEase = value; UpdateTextures(); }

		/// <summary>The strength of the outer texture transition.</summary>
		[FormerlySerializedAs("outerColorSharpness")] public float OuterColorSharpness = 2.0f; public void SetOuterColorSharpness(float value) { OuterColorSharpness = value; UpdateTextures(); }

		/// <summary>The strength of the outer texture transition.</summary>
		[FormerlySerializedAs("outerAlphaSharpness")] public float OuterAlphaSharpness = 3.0f; public void SetOuterAlphaSharpness(float value) { OuterAlphaSharpness = value; UpdateTextures(); }

		[System.NonSerialized]
		private Texture2D generatedInnerTexture;

		[System.NonSerialized]
		private Texture2D generatedOuterTexture;

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

		public Texture2D GeneratedInnerTexture
		{
			get
			{
				return generatedInnerTexture;
			}
		}

		public Texture2D GeneratedOuterTexture
		{
			get
			{
				return generatedOuterTexture;
			}
		}

#if UNITY_EDITOR
		/// <summary>This method allows you to export the generated texture as an asset.
		/// Once done, you can remove this component, and set the <b>SgtAtmosphere</b> component's <b>InnerDepth</b> setting using the exported asset.</summary>
		[ContextMenu("Export Inner Texture")]
		public void ExportInnerTexture()
		{
			var importer = SgtHelper.ExportTextureDialog(generatedOuterTexture, "Atmosphere InnerDepth");

			if (importer != null)
			{
				importer.textureType         = TextureImporterType.SingleChannel;
				importer.textureCompression  = TextureImporterCompression.Uncompressed;
				importer.alphaSource         = TextureImporterAlphaSource.FromInput;
				importer.wrapMode            = TextureWrapMode.Clamp;
				importer.filterMode          = FilterMode.Trilinear;
				importer.anisoLevel          = 16;
				importer.alphaIsTransparency = true;

				importer.SaveAndReimport();
			}
		}

		/// <summary>This method allows you to export the generated texture as an asset.
		/// Once done, you can remove this component, and set the <b>SgtAtmosphere</b> component's <b>OuterDepth</b> setting using the exported asset.</summary>
		[ContextMenu("Export Outer Texture")]
		public void ExportOuterTexture()
		{
			var importer = SgtHelper.ExportTextureDialog(generatedOuterTexture, "Atmosphere OuterDepth");

			if (importer != null)
			{
				importer.textureType         = TextureImporterType.SingleChannel;
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
		public void UpdateTextures()
		{
			if (Width > 0)
			{
				ValidateTexture(ref generatedInnerTexture, "InnerDepth (Generated)");
				ValidateTexture(ref generatedOuterTexture, "OuterDepth (Generated)");

				var stepU = 1.0f / (Width - 1);

				for (var x = 0; x < Width; x++)
				{
					var u = stepU * x;

					WritePixel(generatedInnerTexture, u, x, InnerColor, InnerEase, InnerColorSharpness, InnerAlphaSharpness);
					WritePixel(generatedOuterTexture, u, x, OuterColor, OuterEase, OuterColorSharpness, OuterAlphaSharpness);
				}

				generatedInnerTexture.Apply();
				generatedOuterTexture.Apply();
			}
		}

		[ContextMenu("Apply Textures")]
		public void ApplyTextures()
		{
			if (CachedAtmosphere.InnerDepthTex != generatedInnerTexture)
			{
				cachedAtmosphere.SetInnerDepthTex(generatedInnerTexture);
			}

			if (cachedAtmosphere.OuterDepthTex != generatedOuterTexture)
			{
				cachedAtmosphere.SetOuterDepthTex(generatedOuterTexture);
			}
		}

		[ContextMenu("Remove Textures")]
		public void RemoveTextures()
		{
			if (CachedAtmosphere.InnerDepthTex == generatedInnerTexture)
			{
				cachedAtmosphere.SetInnerDepthTex(null);
			}

			if (cachedAtmosphere.OuterDepthTex == generatedOuterTexture)
			{
				cachedAtmosphere.SetOuterDepthTex(null);
			}
		}

		protected virtual void OnEnable()
		{
			UpdateTextures();
			ApplyTextures();
		}

		protected virtual void OnDisable()
		{
			if (quitting == false)
			{
				RemoveTextures();
			}
		}

		protected virtual void OnDestroy()
		{
			if (quitting == false)
			{
				SgtHelper.Destroy(generatedInnerTexture);
				SgtHelper.Destroy(generatedOuterTexture);
			}
		}

		private void ValidateTexture(ref Texture2D texture2D, string createName)
		{
			// Destroy if invalid
			if (texture2D != null)
			{
				if (texture2D.width != Width || texture2D.height != 1 || texture2D.format != Format)
				{
					texture2D = SgtHelper.Destroy(texture2D);
				}
			}

			// Create?
			if (texture2D == null)
			{
				texture2D = SgtHelper.CreateTempTexture2D(createName, Width, 1, Format);

				texture2D.wrapMode = TextureWrapMode.Clamp;

				ApplyTextures();
			}
		}

		private void WritePixel(Texture2D texture2D, float u, int x, Color baseColor, SgtEase.Type ease, float colorSharpness, float alphaSharpness)
		{
			var colorU = SgtHelper.Sharpness(u, colorSharpness); colorU = SgtEase.Evaluate(ease, colorU);
			var alphaU = SgtHelper.Sharpness(u, alphaSharpness); alphaU = SgtEase.Evaluate(ease, alphaU);
			var color  = Color.Lerp(baseColor, HorizonColor, colorU);

			color.a = alphaU;

			texture2D.SetPixel(x, 0, color);
		}
	}
}