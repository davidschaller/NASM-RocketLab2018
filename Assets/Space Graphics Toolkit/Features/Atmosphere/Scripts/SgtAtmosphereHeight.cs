using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmosphereHeight))]
	public class SgtAtmosphereHeight_Editor : SgtEditor<SgtAtmosphereHeight>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.DistanceMin > t.DistanceMax));
				DrawDefault("DistanceMin", "The minimum distance between the atmosphere center and the camera position.");
				DrawDefault("DistanceMax", "The maximum distance between the atmosphere center and the camera position.");
			EndError();

			Separator();

			DrawDefault("HeightClose", "The SgtAtmosphere.Height value that will be set when at or below DistanceMin.");
			DrawDefault("HeightFar", "The SgtAtmosphere.Height value that will be set when at or above DistanceMax.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component modifies the SgtAtmosphere.Height based on camera proximity.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmosphere))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmosphereHeight")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere Height")]
	public class SgtAtmosphereHeight : MonoBehaviour
	{
		/// <summary>The minimum distance between the atmosphere center and the camera position in local space.</summary>
		[FormerlySerializedAs("distanceMin")] public float DistanceMin = 1.1f; public void SetDistanceMin(float value) { DistanceMin = value; }

		/// <summary>The maximum distance between the atmosphere center and the camera position in local space.</summary>
		[FormerlySerializedAs("distanceMax")] public float DistanceMax = 1.2f; public void SetDistanceMax(float value) { DistanceMax = value; }

		/// <summary>The SgtAtmosphere.Height value that will be set when at or below DistanceMin.</summary>
		[FormerlySerializedAs("heightClose")] public float HeightClose = 0.1f; public void SetHeightClose(float value) { HeightClose = value; }

		/// <summary>The SgtAtmosphere.Height value that will be set when at or above DistanceMax.</summary>
		[FormerlySerializedAs("heightFar")] public float HeightFar = 0.01f; public void SetHeightFar(float value) { HeightFar = value; }

		[System.NonSerialized]
		private SgtAtmosphere cachedAtmosphere;

		protected virtual void OnEnable()
		{
			SgtCamera.OnCameraPreCull += PreCull;

			if (cachedAtmosphere == null) cachedAtmosphere = GetComponent<SgtAtmosphere>();
		}

		protected virtual void OnDisable()
		{
			SgtCamera.OnCameraPreCull -= PreCull;
		}

		private void PreCull(Camera camera)
		{
			if (camera != null)
			{
				var cameraPoint = transform.InverseTransformPoint(camera.transform.position);
				var distance01  = Mathf.InverseLerp(DistanceMin, DistanceMax, cameraPoint.magnitude);
				var height      = Mathf.Lerp(HeightClose, HeightFar, distance01);

				if (cachedAtmosphere.Height != height)
				{
					cachedAtmosphere.SetHeight(height);
				}
			}
		}
	}
}