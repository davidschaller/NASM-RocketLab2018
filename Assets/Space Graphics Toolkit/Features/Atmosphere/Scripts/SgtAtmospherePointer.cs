using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtAtmospherePointer))]
	public class SgtAtmospherePointer_Editor : SgtEditor<SgtAtmospherePointer>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This component points the current atmosphere to the rendering camera, allowing you to keep the atmosphere vertex density high near the camera. This should be used with the SgtAtmosphereOuterMesh component, or an exported mesh from it.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component points the current atmosphere to the rendering camera, allowing you to keep the atmosphere vertex density high near the camera. This should be used with the SgtAtmosphereOuterMesh component, or an exported mesh from it.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtAtmospherePointer))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtAtmospherePointer")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Atmosphere Pointer")]
	public class SgtAtmospherePointer : MonoBehaviour
	{
		public class CameraState : SgtCameraState
		{
			public Quaternion LocalRotation;
		}

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

		private void CameraPreCull(Camera camera)
		{
			Revert();
			{
				transform.forward = camera.transform.position - transform.position;
			}
			Save(camera);
		}

		private void CameraPreRender(Camera camera)
		{
			Restore(camera);
		}

		private void Save(Camera camera)
		{
			var cameraState = SgtCameraState.Find(ref cameraStates, camera);

			cameraState.LocalRotation = transform.localRotation;
		}

		private void Restore(Camera camera)
		{
			var cameraState = SgtCameraState.Restore(cameraStates, camera);

			if (cameraState != null)
			{
				transform.localRotation = cameraState.LocalRotation;
			}
		}

		private void Revert()
		{
		}
	}
}