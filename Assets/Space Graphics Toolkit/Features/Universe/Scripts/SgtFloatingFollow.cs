using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component makes the current <b>SgtPoint</b> follow the <b>Target SgtPoint</b>.
	/// This is useful because you can't parent/child <b>SgtPoint</b> components like <b>SgtFloatingCamera</b> and <b>SgtFloatingObject</b>.</summary>
	[DefaultExecutionOrder(200)]
	[RequireComponent(typeof(SgtFloatingPoint))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingFollow")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Follow")]
	public class SgtFloatingFollow : MonoBehaviour
	{
		/// <summary></summary>
		public SgtFloatingPoint Target { set { target = value; } get { return target; } } [SerializeField] private SgtFloatingPoint target;

		/// <summary>How quickly this point follows the target.
		/// -1 = instant.</summary>
		public float Dampening { set { dampening = value; } get { return dampening; } } [SerializeField] private float dampening = -1.0f;

		/// <summary></summary>
		public bool Rotate { set { rotate = value; } get { return rotate; } } [SerializeField] private bool rotate;

		/// <summary>This allows you to specify a positional offset relative to the <b>Target</b>.</summary>
		public Vector3 LocalPosition { set { localPosition = value; } get { return localPosition; } } [SerializeField] private Vector3 localPosition;

		/// <summary>This allows you to specify a rotational offset relative to the <b>Target</b>.</summary>
		public Vector3 LocalRotation { set { localRotation = value; } get { return localRotation; } } [SerializeField] private Vector3 localRotation;

		[System.NonSerialized]
		private SgtFloatingPoint cachedPoint;

		protected virtual void OnEnable()
		{
			cachedPoint = GetComponent<SgtFloatingPoint>();
		}

		protected virtual void Update()
		{
			if (target != null)
			{
				var currentPosition = cachedPoint.Position;
				var targetPosition  = target.Position + localPosition;
				var factor          = SgtHelper.DampenFactor(Dampening, Time.deltaTime);

				cachedPoint.SetPosition(SgtPosition.Lerp(ref currentPosition, ref targetPosition, factor));

				if (Rotate == true)
				{
					var targetRotation = target.transform.rotation * Quaternion.Euler(LocalRotation);

					transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, factor);
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingFollow))]
	public class SgtFloatingFollow_Editor : SgtEditor<SgtFloatingFollow>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Target == null));
				Draw("target", "");
			EndError();
			Draw("dampening", "How quickly this point follows the target.\n\n-1 = instant.");
			Draw("rotate", "");
			Draw("localPosition", "");
			Draw("localRotation", "");
		}
	}
}
#endif