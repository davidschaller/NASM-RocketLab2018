using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtSimpleOrbit))]
	public class SgtSimpleOrbit_Editor : SgtEditor<SgtSimpleOrbit>
	{
		protected override void OnInspector()
		{
			BeginError(Any(t => t.Radius == 0.0f));
				DrawDefault("Radius", "The radius of the orbit in local coordinates.");
			EndError();
			DrawDefault("Scale", "How squashed the orbit is.");

			Separator();

			DrawDefault("Offset", "The local position offset of the orbit.");
			DrawDefault("Tilt", "The local rotation offset of the orbit in degrees.");

			Separator();

			DrawDefault("Angle", "The curent position along the orbit in degrees.");
			DrawDefault("DegreesPerSecond", "The orbit speed.");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component makes the current gameObject orbit around its parent in a basic circle or ellipse shape.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Simple Orbit")]
	public class SgtSimpleOrbit : MonoBehaviour
	{
		/// <summary>The radius of the orbit in local coordinates.</summary>
		public float Radius = 1.0f;

		/// <summary>How squashed the orbit is.</summary>
		public Vector2 Scale = Vector2.one;

		/// <summary>The local position offset of the orbit.</summary>
		public Vector3 Offset;

		/// <summary>The local rotation offset of the orbit in degrees.</summary>
		public Vector3 Tilt;

		/// <summary>The curent position along the orbit in degrees.</summary>
		public float Angle;

		/// <summary>The orbit speed.</summary>
		public float DegreesPerSecond = 10.0f;

		protected virtual void Update()
		{
			if (Application.isPlaying == true)
			{
				Angle += DegreesPerSecond * Time.deltaTime;
			}

			var localPosition = Offset;

			localPosition.x += Mathf.Sin(Angle * Mathf.Deg2Rad) * Radius * Scale.x;
			localPosition.z += Mathf.Cos(Angle * Mathf.Deg2Rad) * Radius * Scale.y;

			transform.localPosition = Quaternion.Euler(Tilt) * localPosition;
		}
	
#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			if (SgtHelper.Enabled(this) == true)
			{
				if (transform.parent != null)
				{
					Gizmos.matrix = transform.parent.localToWorldMatrix;
				}

				var rotation = Quaternion.Euler(Tilt);

				SgtHelper.DrawCircle(Offset, rotation * Vector3.right * Radius * Scale.x, rotation * Vector3.forward * Radius * Scale.y);

				Gizmos.DrawLine(Vector3.zero, transform.localPosition);
			}
		}
#endif
	}
}