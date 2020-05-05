using UnityEngine;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component scales the current SgtFloatingObject based on its distance to the SgtFloatingOrigin.
	/// This scaling allows you to see the object from a greater distance than usual, which is very useful for star/planet/etc billboards you need to see from a distance.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(SgtFloatingObject))]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingScaler")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Scaler")]
	public class SgtFloatingScaler : MonoBehaviour
	{
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		public Vector3 Scale { set { scale = value; } get { return scale; } } [SerializeField] private Vector3 scale = Vector3.one;

		public double Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private double multiplier = 0.0f;

		public SgtLength Range { set { range = value; } get { return range; } } [SerializeField] private SgtLength range = 1000000.0;

		[System.NonSerialized]
		private SgtFloatingObject cachedObject;

		[System.NonSerialized]
		private bool cachedObjectSet;

		/// <summary>The <b>SgtFloatingObject</b> component alongside this component.</summary>
		public SgtFloatingObject CachedObject
		{
			get
			{
				if (cachedObjectSet == false)
				{
					cachedObject    = GetComponent<SgtFloatingObject>();
					cachedObjectSet = true;
				}

				return cachedObject;
			}
		}

		protected virtual void OnEnable()
		{
			CachedObject.OnDistance += HandleDistance;
		}

		protected virtual void OnDisable()
		{
			cachedObject.OnDistance -= HandleDistance;
		}

		private void HandleDistance(double distance)
		{
			var finalTarget = target;

			if (finalTarget == null)
			{
				finalTarget = transform;
			}

			var map = math.log10(math.max(1.0, distance));
			var sca = math.saturate(1.0 - map / math.log10(range)) * distance * multiplier;

			finalTarget.localScale = scale * (float)sca;
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingScaler))]
	public class SgtFloatingScaler_Editor : SgtEditor<SgtFloatingScaler>
	{
		protected override void OnInspector()
		{
			Draw("target", "");
			BeginError(Any(t => t.Scale == Vector3.zero));
				Draw("scale", "The scale of the object when at DistanceMin.");
			EndError();
			BeginError(Any(t => t.Multiplier <= 0.0));
				Draw("multiplier", "");
			EndError();
			BeginError(Any(t => t.Range <= 0.0));
				Draw("range", "");
			EndError();
		}
	}
}
#endif