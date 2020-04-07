using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtCamera))]
	public class SgtCamera_Editor : SgtEditor<SgtCamera>
	{
		protected override void OnInspector()
		{
			DrawDefault("UseOrigin", "");
			DrawDefault("RollAngle", "The amount of degrees this camera has rolled (used to counteract billboard non-rotation).");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component monitors the attached Camera for modifications in roll angle, and stores the total change.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Camera))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtCamera")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Camera")]
	public class SgtCamera : SgtLinkedBehaviour<SgtCamera>
	{
		public static event System.Action<Camera> OnCameraPreCull;

		public static event System.Action<Camera> OnCameraPreRender;

		public static event System.Action<Camera> OnCameraPostRender;

		public static event System.Action<SgtCamera> OnSgtCameraPreCull;

		public static event System.Action<SgtCamera> OnSgtCameraPreRender;

		public static event System.Action<SgtCamera> OnSgtCameraPostRender;

		public bool UseOrigin;

		/// <summary>The amount of degrees this camera has rolled (used to counteract billboard non-rotation).</summary>
		public float RollAngle;

		// A quaternion of the current roll angle
		public Quaternion RollQuaternion = Quaternion.identity;

		// A matrix of the current roll angle
		public Matrix4x4 RollMatrix = Matrix4x4.identity;

		// The change in position of this GameObject over the past frame
		[System.NonSerialized]
		public Vector3 DeltaPosition;

		// The current velocity of this GameObject per second
		[System.NonSerialized]
		public Vector3 Velocity;

		// Previous frame rotation
		[System.NonSerialized]
		public Quaternion OldRotation = Quaternion.identity;

		// Previous frame position
		[System.NonSerialized]
		public Vector3 OldPosition;

		// The camera this camera is attached to
		[System.NonSerialized]
		public Camera cachedCamera;

		[System.NonSerialized]
		public bool cachedCameraSet;

		[System.NonSerialized]
		private SgtPosition expectedPosition;

		[System.NonSerialized]
		private bool expectedPositionSet;

		public Camera CachedCamera
		{
			get
			{
				if (cachedCameraSet == false)
				{
					cachedCamera    = GetComponent<Camera>();
					cachedCameraSet = true;
				}

				return cachedCamera;
			}
		}

		static SgtCamera()
		{
			Camera.onPreCull    += (camera) =>
				{
					if (OnCameraPreCull != null) OnCameraPreCull(camera);
				};

			Camera.onPreRender  += (camera) =>
				{
					if (OnCameraPreRender != null) OnCameraPreRender(camera);
				};

			Camera.onPostRender += (camera) =>
				{
					if (OnCameraPostRender != null) OnCameraPostRender(camera);
				};
			
#if UNITY_2019_1_OR_NEWER
			UnityEngine.Rendering.RenderPipelineManager.beginCameraRendering += (context, camera) =>
				{
					if (OnCameraPreCull != null) OnCameraPreCull(camera);
					if (OnCameraPreRender != null) OnCameraPreRender(camera);

					var sgtCamera = default(SgtCamera);

					if (TryFind(camera, ref sgtCamera) == true)
					{
						if (OnSgtCameraPreCull != null) OnSgtCameraPreCull(sgtCamera);
						if (OnSgtCameraPreRender != null) OnSgtCameraPreRender(sgtCamera);
					}
				};

			UnityEngine.Rendering.RenderPipelineManager.endCameraRendering += (context, camera) =>
				{
					if (OnCameraPostRender != null) OnCameraPostRender(camera);

					var sgtCamera = default(SgtCamera);

					if (TryFind(camera, ref sgtCamera) == true)
					{
						if (OnSgtCameraPostRender != null) OnSgtCameraPostRender(sgtCamera);
					}
				};
#endif
		}

		// Find the camera attached to a specific camera, if it exists
		public static bool TryFind(Camera unityCamera, ref SgtCamera foundCamera)
		{
			var camera = FirstInstance;

			for (var i = 0; i < InstanceCount; i++)
			{
				if (camera.CachedCamera == unityCamera)
				{
					foundCamera = camera; return true;
				}

				camera = camera.NextInstance;
			}

			return false;
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			OldRotation = transform.rotation;
			OldPosition = transform.position;

			SgtFloatingCamera.OnSnap += FloatingCameraSnap;
		}

		protected override void OnDisable()
		{
			base.OnDisable();

			SgtFloatingCamera.OnSnap -= FloatingCameraSnap;
		}

		protected virtual void OnPreCull()
		{
			if (OnSgtCameraPreCull != null) OnSgtCameraPreCull(this);
		}

		protected virtual void OnPreRender()
		{
			if (OnSgtCameraPreRender != null) OnSgtCameraPreRender(this);
		}

		protected virtual void OnPostRender()
		{
			if (OnSgtCameraPostRender != null) OnSgtCameraPostRender(this);
		}

		protected virtual void LateUpdate()
		{
			var newRotation   = transform.rotation;
			var newPosition   = transform.position;
			var deltaRotation = Quaternion.Inverse(OldRotation) * newRotation;

			RollAngle      = (RollAngle - deltaRotation.eulerAngles.z) % 360.0f;
			RollQuaternion = Quaternion.Euler(0.0f, 0.0f, RollAngle);
			RollMatrix     = Matrix4x4.Rotate(RollQuaternion);

			if (UseOrigin == true)
			{
				var currentPosition = SgtFloatingOrigin.CurrentPoint.Position;

				if (expectedPositionSet == false)
				{
					expectedPosition    = currentPosition;
					expectedPositionSet = true;
				}

				var deltaPosition = SgtPosition.Vector(ref expectedPosition, ref currentPosition);

				DeltaPosition = deltaPosition;
				Velocity      = SgtHelper.Reciprocal(Time.deltaTime) * deltaPosition;

				expectedPosition = currentPosition;
			}
			else
			{
				var deltaPosition = OldPosition - newPosition;

				DeltaPosition       = deltaPosition;
				Velocity            = SgtHelper.Reciprocal(Time.deltaTime) * deltaPosition;
				expectedPositionSet = false;
			}

			OldRotation = newRotation;
			OldPosition = newPosition;
		}

		private void FloatingCameraSnap(SgtFloatingCamera floatingCamera, Vector3 delta)
		{
			if (floatingCamera.CachedCamera == CachedCamera)
			{
				OldPosition += delta;
			}
		}
	}
}