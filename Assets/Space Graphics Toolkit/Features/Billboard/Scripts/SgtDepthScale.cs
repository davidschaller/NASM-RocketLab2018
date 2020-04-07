using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtDepthScale))]
	public class SgtDepthScale_Editor : SgtEditor<SgtDepthScale>
	{
		protected override void OnInspector()
		{
			DrawDefault("MaxScale", "This allows you to set the maximum scale when there is no depth."); // Updated automatically
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to scale the current GameObject based on optical thickness between the current camera and the current position.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtDepthScale")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Depth Scale")]
	public class SgtDepthScale : MonoBehaviour
	{
		public class CameraState : SgtCameraState
		{
			public Vector3 LocalScale;
			public float   Value;
		}

		/// <summary>This allows you to set the maximum scale when there is no depth.</summary>
		public Vector3 MaxScale = Vector3.one;

		[System.NonSerialized]
		private List<CameraState> cameraStates;

		protected virtual void OnEnable()
		{
			SgtCamera.OnCameraPreCull   += CameraPreCull;
			SgtCamera.OnCameraPreRender += CameraPreRender;
		}

		protected virtual void OnDisable()
		{
			SgtCamera.OnCameraPreCull   -= CameraPreCull;
			SgtCamera.OnCameraPreRender -= CameraPreRender;
		}

		protected virtual void LateUpdate()
		{
			if (cameraStates != null)
			{
				for (var i = cameraStates.Count - 1; i >= 0; i--)
				{
					var cameraState = cameraStates[i];

					if (cameraState.Camera != null && SgtDepth.InstanceCount > 0)
					{
						cameraState.Value = 1.0f - SgtDepth.FirstInstance.Calculate(cameraState.Camera.transform.position, transform.position);
					}
					else
					{
						cameraState.Value = 0.0f;
					}
				}
			}
		}

		private void Restore(Camera camera)
		{
			var cameraState = SgtCameraState.Restore(cameraStates, camera);

			if (cameraState != null)
			{
				transform.localScale = cameraState.LocalScale;
			}
		}

		private void CameraPreCull(Camera camera)
		{
			var cameraState = SgtCameraState.Find(ref cameraStates, camera);

			transform.localScale = MaxScale * cameraState.Value;

			// Store scale
			cameraState.LocalScale = transform.localScale;
		}

		private void CameraPreRender(Camera camera)
		{
			Restore(camera);
		}
	}
}