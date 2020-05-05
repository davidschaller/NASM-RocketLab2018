using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This makes the current <b>Transform</b> follow the <b>Target</b> Transform as if it were a child.</summary>
	[ExecuteInEditMode]
	[DefaultExecutionOrder(200)]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFollow")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Follow")]
	public class SgtFollow : MonoBehaviour
	{
		public enum UpdateType
		{
			Update,
			LateUpdate
		}

		/// <summary>The transform that will be followed.</summary>
		public Transform Target;

		/// <summary>How quickly this Transform follows the target.
		/// -1 = instant.</summary>
		public float Dampening = 5.0f;

		/// <summary>Follow the target's rotation too?</summary>
		public bool Rotate = true;

		/// <summary>Where in the game loop should this component update?</summary>
		public UpdateType FollowIn;

		/// <summary>This allows you to specify a positional offset relative to the <b>Target</b>.</summary>
		public Vector3 LocalPosition;

		/// <summary>This allows you to specify a rotational offset relative to the <b>Target</b>.</summary>
		public Vector3 LocalRotation;

		[ContextMenu("UpdatePosition")]
		public void UpdatePosition()
		{
			if (Target != null)
			{
				var targetPosition = Target.TransformPoint(LocalPosition);
				var factor         = SgtHelper.DampenFactor(Dampening, Time.deltaTime);

				transform.position = Vector3.Lerp(transform.position, targetPosition, factor);

				if (Rotate == true)
				{
					var targetRotation = Target.rotation * Quaternion.Euler(LocalRotation);

					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, factor);
				}
			}
		}

		protected virtual void Update()
		{
			if (FollowIn == UpdateType.Update)
			{
				UpdatePosition();
			}
		}

		protected virtual void LateUpdate()
		{
			if (FollowIn == UpdateType.LateUpdate)
			{
				UpdatePosition();
			}
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFollow))]
	public class SgtFollow_Editor : SgtEditor<SgtFollow>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				Draw("Target", "The transform that will be followed.");
			EndError();
			Draw("Dampening", "How quickly this Transform follows the target.\n\n-1 = instant.");
			Draw("Rotate", "Follow the target's rotation too?");
			Draw("FollowIn", "Where in the game loop should this component update?");

			Separator();

			Draw("LocalPosition", "This allows you to specify a positional offset relative to the Target transform.");
			Draw("LocalRotation", "This allows you to specify a rotational offset relative to the Target transform.");
		}
	}
}
#endif