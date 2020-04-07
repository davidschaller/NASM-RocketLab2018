using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtBeltSimple))]
	public class SgtBeltSimple_Editor : SgtBelt_Editor<SgtBeltSimple>
	{
		protected override void OnInspector()
		{
			var updateMaterial        = false;
			var updateMeshesAndModels = false;

			DrawMaterial(ref updateMaterial);

			Separator();

			DrawMainTex(ref updateMaterial, ref updateMeshesAndModels);

			Separator();

			DrawLighting(ref updateMaterial);

			Separator();

			DrawDefault("Seed", ref updateMeshesAndModels, "This allows you to set the random seed used during procedural generation.");
			DrawDefault("Thickness", ref updateMeshesAndModels, "The thickness of the belt in local coordinates.");
			BeginError(Any(t => t.ThicknessBias < 1.0f));
				DrawDefault("ThicknessBias", ref updateMeshesAndModels, "The higher this value, the less large asteroids will be generated.");
			EndError();
			BeginError(Any(t => t.InnerRadius < 0.0f || t.InnerRadius > t.OuterRadius));
				DrawDefault("InnerRadius", ref updateMeshesAndModels, "The radius of the inner edge of the belt in local coordinates.");
			EndError();
			DrawDefault("InnerSpeed", ref updateMeshesAndModels, "The speed of asteroids orbiting on the inner edge of the belt in radians.");
			BeginError(Any(t => t.OuterRadius < 0.0f || t.InnerRadius > t.OuterRadius));
				DrawDefault("OuterRadius", ref updateMeshesAndModels, "The radius of the outer edge of the belt in local coordinates.");
			EndError();
			DrawDefault("OuterSpeed", ref updateMeshesAndModels, "The speed of asteroids orbiting on the outer edge of the belt in radians.");

			Separator();

			DrawDefault("RadiusBias", ref updateMeshesAndModels, "The higher this value, the more likely asteroids will spawn on the inner edge of the ring.");
			DrawDefault("SpeedSpread", ref updateMeshesAndModels, "How much random speed can be added to each asteroid.");

			Separator();

			BeginError(Any(t => t.AsteroidCount < 0));
				DrawDefault("AsteroidCount", ref updateMeshesAndModels, "The amount of asteroids generated in the belt.");
			EndError();
			DrawDefault("asteroidColors", ref updateMeshesAndModels, "Each asteroid is given a random color from this gradient.");
			DrawDefault("AsteroidSpin", ref updateMeshesAndModels, "The maximum amount of angular velcoity each asteroid has.");
			BeginError(Any(t => t.AsteroidRadiusMin < 0.0f || t.AsteroidRadiusMin > t.AsteroidRadiusMax));
				DrawDefault("AsteroidRadiusMin", ref updateMeshesAndModels, "The minimum asteroid radius in local coordinates.");
			EndError();
			BeginError(Any(t => t.AsteroidRadiusMax < 0.0f || t.AsteroidRadiusMin > t.AsteroidRadiusMax));
				DrawDefault("AsteroidRadiusMax", ref updateMeshesAndModels, "The maximum asteroid radius in local coordinates.");
			EndError();
			DrawDefault("AsteroidRadiusBias", ref updateMeshesAndModels, "How likely the size picking will pick smaller asteroids over larger ones (0 = default/linear).");

			RequireCamera();

			serializedObject.ApplyModifiedProperties();

			if (updateMaterial        == true) DirtyEach(t => t.UpdateMaterial       ());
			if (updateMeshesAndModels == true) DirtyEach(t => t.UpdateMeshesAndModels());
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to generate an asteroid belt with a simple exponential distribution.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtBeltSimple")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Belt Simple")]
	public class SgtBeltSimple : SgtBelt
	{
		/// <summary>This allows you to set the random seed used during procedural generation.</summary>
		[SgtSeed] public int Seed; public void SetSeed(int value) { Seed = value; UpdateMeshesAndModels(); }

		/// <summary>The thickness of the belt in local coordinates.</summary>
		public float Thickness; public void SetThickness(float value) { Thickness = value; UpdateMeshesAndModels(); }

		/// <summary>The higher this value, the less large asteroids will be generated.</summary>
		public float ThicknessBias = 1.0f; public void SetThicknessBias(float value) { ThicknessBias = value; UpdateMeshesAndModels(); }

		/// <summary>The radius of the inner edge of the belt in local coordinates.</summary>
		public float InnerRadius = 1.0f; public void SetInnerRadius(float value) { InnerRadius = value; UpdateMeshesAndModels(); }

		/// <summary>The speed of asteroids orbiting on the inner edge of the belt in radians.</summary>
		public float InnerSpeed = 0.1f; public void SetInnerSpeed(float value) { InnerSpeed = value; UpdateMeshesAndModels(); }

		/// <summary>The radius of the outer edge of the belt in local coordinates.</summary>
		public float OuterRadius = 2.0f; public void SetOuterRadius(float value) { OuterRadius = value; UpdateMeshesAndModels(); }

		/// <summary>The speed of asteroids orbiting on the outer edge of the belt in radians.</summary>
		public float OuterSpeed = 0.05f; public void SetOuterSpeed(float value) { OuterSpeed = value; UpdateMeshesAndModels(); }

		/// <summary>The higher this value, the more likely asteroids will spawn on the inner edge of the ring.</summary>
		public float RadiusBias = 0.25f; public void SetRadiusBias(float value) { RadiusBias = value; UpdateMeshesAndModels(); }

		/// <summary>How much random speed can be added to each asteroid.</summary>
		public float SpeedSpread; public void SetSpeedSpread(float value) { SpeedSpread = value; UpdateMeshesAndModels(); }

		/// <summary>The amount of asteroids generated in the belt.</summary>
		public int AsteroidCount = 1000; public void SetAsteroidCount(int value) { AsteroidCount = value; UpdateMeshesAndModels(); }

		/// <summary>Each asteroid is given a random color from this gradient.</summary>
		[FormerlySerializedAs("AsteroidColors")] [SerializeField] private Gradient asteroidColors; public Gradient AsteroidColors { get { if (asteroidColors == null) asteroidColors = new Gradient(); return asteroidColors; } }

		/// <summary>The maximum amount of angular velcoity each asteroid has.</summary>
		public float AsteroidSpin = 1.0f; public void SetAsteroidSpin(float value) { AsteroidSpin = value; UpdateMeshesAndModels(); }

		/// <summary>The minimum asteroid radius in local coordinates.</summary>
		public float AsteroidRadiusMin = 0.025f; public void SetAsteroidRadiusMin(float value) { AsteroidRadiusMin = value; UpdateMeshesAndModels(); }

		/// <summary>The maximum asteroid radius in local coordinates.</summary>
		public float AsteroidRadiusMax = 0.05f; public void SetAsteroidRadiusMax(float value) { AsteroidRadiusMax = value; UpdateMeshesAndModels(); }

		/// <summary>How likely the size picking will pick smaller asteroids over larger ones (1 = default/linear).</summary>
		public float AsteroidRadiusBias = 0.0f; public void SetAsteroidRadiusBias(float value) { AsteroidRadiusBias = value; UpdateMeshesAndModels(); }

		public static SgtBeltSimple Create(int layer = 0, Transform parent = null)
		{
			return Create(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
		}

		public static SgtBeltSimple Create(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
		{
			var gameObject = SgtHelper.CreateGameObject("Belt Simple", layer, parent, localPosition, localRotation, localScale);
			var simpleBelt = gameObject.AddComponent<SgtBeltSimple>();

			return simpleBelt;
		}

#if UNITY_EDITOR
		[MenuItem(SgtHelper.GameObjectMenuPrefix + "Belt Simple", false, 10)]
		public static void CreateMenuItem()
		{
			var parent     = SgtHelper.GetSelectedParent();
			var simpleBelt = Create(parent != null ? parent.gameObject.layer : 0, parent);

			SgtHelper.SelectAndPing(simpleBelt);
		}
#endif

		protected override int BeginQuads()
		{
			SgtHelper.BeginRandomSeed(Seed);

			if (asteroidColors == null)
			{
				asteroidColors = SgtHelper.CreateGradient(Color.white);
			}

			return AsteroidCount;
		}

		protected override void NextQuad(ref SgtBeltAsteroid asteroid, int asteroidIndex)
		{
			var distance01 = SgtHelper.Sharpness(Random.value * Random.value, RadiusBias);

			asteroid.Variant       = Random.Range(int.MinValue, int.MaxValue);
			asteroid.Color         = asteroidColors.Evaluate(Random.value);
			asteroid.Radius        = Mathf.Lerp(AsteroidRadiusMin, AsteroidRadiusMax, SgtHelper.Sharpness(Random.value, AsteroidRadiusBias));
			asteroid.Height        = Mathf.Pow(Random.value, ThicknessBias) * Thickness * (Random.value < 0.5f ? -0.5f : 0.5f);
			asteroid.Angle         = Random.Range(0.0f, Mathf.PI * 2.0f);
			asteroid.Spin          = Random.Range(-AsteroidSpin, AsteroidSpin);
			asteroid.OrbitAngle    = Random.Range(0.0f, Mathf.PI * 2.0f);
			asteroid.OrbitSpeed    = Mathf.Lerp(InnerSpeed, OuterSpeed, distance01);
			asteroid.OrbitDistance = Mathf.Lerp(InnerRadius, OuterRadius, distance01);

			asteroid.OrbitSpeed += Random.Range(-SpeedSpread, SpeedSpread) * asteroid.OrbitSpeed;
		}

		protected override void EndQuads()
		{
			SgtHelper.EndRandomSeed();
		}
	}
}