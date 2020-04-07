using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component scales the current Transform based on its distance to the camera.
	/// This scaling allows you to see the object from a greater distance than usual, which is very useful for star/planet/etc billboards you need to see from a distance.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtSimpleScaler")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Simple Scaler")]
	public class SgtSimpleScaler : MonoBehaviour
	{
		/// <summary>The base scale of the object.</summary>
		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		/// <summary>Scale is multiplied by this when at DistanceMin.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>The distance where the scaling begins.</summary>
		public float DistanceMin { set { distanceMin = value; } get { return distanceMin; } } [SerializeField] private float distanceMin = 1000.0f;

		/// <summary>The distance where the scaling stops.</summary>
		public float DistanceMax { set { distanceMax = value; } get { return distanceMax; } } [SerializeField] float distanceMax = 1000000.0f;

		[System.NonSerialized]
		private Transform cachedTransform;

		protected virtual void OnEnable()
		{
			cachedTransform = GetComponent<Transform>();

			SgtCamera.OnSgtCameraPreCull += HandleCameraPreCull;
		}

		protected virtual void OnDisable()
		{
			SgtCamera.OnSgtCameraPreCull -= HandleCameraPreCull;
		}

		private void HandleCameraPreCull(SgtCamera camera)
		{
			UpdateDistance(Vector3.Distance(camera.transform.position, cachedTransform.position));
		}

		private void UpdateDistance(float distance)
		{
			if (distance <= DistanceMin)
			{
				cachedTransform.localScale = Vector3.zero;
			}
			else
			{
				var distanceRange = DistanceMax - DistanceMin;

				distance -= DistanceMin;

				if (distance >= distanceRange)
				{
					distance = distanceRange * 0.5f;
				}
				else
				{
					var distance01 = distance / distanceRange;

					distance -= distance * 0.5f * distance01;
				}

				var linear = distance * Multiplier;

				cachedTransform.localScale = Scale * (float)linear;
			}
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtSimpleScaler))]
	public class SgtSimpleScaler_Editor : SgtEditor<SgtSimpleScaler>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Scale == Vector3.zero));
				DrawDefault("scale", "The scale of the object when at DistanceMin.");
			EndError();
			BeginError(Any(t => t.Multiplier <= 0.0));
				DrawDefault("multiplier", "Scale is multiplied by this, allowing you to more easily tweak large scales.");
			EndError();
			BeginError(Any(t => t.DistanceMin < 0.0 || t.DistanceMin >= t.DistanceMax));
				DrawDefault("distanceMin", "The distance where the scaling begins.");
				DrawDefault("distanceMax", "The distance where the scaling stops.");
			EndError();
		}
	}
}
#endif