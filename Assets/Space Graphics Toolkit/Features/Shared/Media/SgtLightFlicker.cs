using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will prcoedurally change the <b>Intensity</b> of the attached <b>Light</b> to simulate flickering.</summary>
	[RequireComponent(typeof(Light))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtLightFlicker")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Light Flicker")]
	public class SgtLightFlicker : MonoBehaviour
	{
		/// <summary>The minimum <b>Intensity</b> value.</summary>
		public float IntensityMin { set { intensityMin = value; } get { return intensityMin; } } [SerializeField] private float intensityMin = 0.9f;

		/// <summary>The maximum <b>Intensity</b> value.</summary>
		public float IntensityMax { set { intensityMax = value; } get { return intensityMax; } } [SerializeField] private float intensityMax = 1.1f;

		/// <summary>The current animation position.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>The current animation speed.</summary>
		public float Speed { set { speed = value; } get { return speed; } } [SerializeField] private float speed = 5.0f;

		[System.NonSerialized]
		private Light cachedLight;

		[System.NonSerialized]
		private float[] points;

		protected virtual void OnEnable()
		{
			cachedLight = GetComponent<Light>();
		}

		protected virtual void Update()
		{
			if (points == null)
			{
				points = new float[128];

				for (var i = points.Length - 1; i >= 0; i--)
				{
					points[i] = Random.value;
				}
			}

			offset += speed * Time.deltaTime;

			cachedLight.intensity = Mathf.Lerp(intensityMin, intensityMax, Sample());
		}

		private float Sample()
		{
			var noise  = Mathf.Repeat(offset, points.Length);
			var index  = (int)noise;
			var frac   = noise % 1.0f;
			var pointA = points[index];
			var pointB = points[(index + 1) % points.Length];
			var pointC = points[(index + 2) % points.Length];
			var pointD = points[(index + 3) % points.Length];

			return SgtHelper.CubicInterpolate(pointA, pointB, pointC, pointD, frac);
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtLightFlicker))]
	public class SgtLightFlicker_Editor : SgtEditor<SgtLightFlicker>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.IntensityMin == t.IntensityMax));
				Draw("intensityMin", "The minimum Intensity value.");
				Draw("intensityMax", "The maximum Intensity value.");
			EndError();
			Draw("offset", "The current animation position.");
			BeginError(Any(t => t.Speed == 0.0f));
				Draw("speed", "The current animation speed.");
			EndError();
		}
	}
}
#endif