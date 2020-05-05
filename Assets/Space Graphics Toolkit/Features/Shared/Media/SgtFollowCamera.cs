using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFollowCamera))]
	public class SgtFollowCamera_Editor : SgtEditor<SgtFollowCamera>
	{
		protected override void OnInspector()
		{
			EditorGUILayout.HelpBox("This makes the current GameObject snap to the currently rendering camera.", MessageType.Info);
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This makes the current GameObject snap to the currently rendering camera.</summary>
	public class SgtFollowCamera : MonoBehaviour
	{
		public class CameraState : SgtCameraState
		{
			public Vector3 LocalPosition;
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

		private void Save(Camera camera)
		{
			var cameraState = SgtCameraState.Find(ref cameraStates, camera);

			cameraState.LocalPosition = transform.position;
		}

		private void Restore(Camera camera)
		{
			var cameraState = SgtCameraState.Restore(cameraStates, camera);

			if (cameraState != null)
			{
				transform.localPosition = cameraState.LocalPosition;
			}
		}

		private void Revert()
		{
			transform.localPosition = Vector3.zero;
		}

		private void CameraPreCull(Camera camera)
		{
			Revert();
			{
				transform.position = camera.transform.position;
			}
			Save(camera);
		}

		private void CameraPreRender(Camera camera)
		{
			Restore(camera);
		}
	}
}