using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component marks the current GameObject as a camera. This means as soon as the transform.position strays too far from the origin (0,0,0), it will snap back to the origin.
	/// After it snaps back, the SnappedPoint field will be updated with the current position of the SgtFloatingOrigin component.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingCamera")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Camera")]
	public class SgtFloatingCamera : SgtFloatingPoint
	{
		public static LinkedList<SgtFloatingCamera> Instances = new LinkedList<SgtFloatingCamera>(); private LinkedListNode<SgtFloatingCamera> node;

		/// <summary>When the transform.position.magnitude exceeds this value, the position will be snapped back to the origin.</summary>
		public float SnapDistance = 100.0f;

		/// <summary>Called when this camera's position snaps back to the origin (Vector3 = delta).</summary>
		public static event System.Action<SgtFloatingCamera, Vector3> OnSnap;

		/// <summary>Every time this camera's position gets snapped, its position at that time is stored here. This allows other objects to correctly position themselves relative to this.
		/// NOTE: This requires you to use the SgtFloatingOrigin component.</summary>
		public SgtPosition SnappedPoint;

		public bool SnappedPointSet;

		/// <summary>This method will fill the instance with the first active and enabled <b>SgtFloatingCamera</b> instance in the scene and return true, or return false.</summary>
		public static bool TryGetInstance(ref SgtFloatingCamera instance)
		{
			if (Instances.Count > 0) { instance = Instances.First.Value; return true; } return false;
		}

		/// <summary>This gives you the universal SgtPosition of the input camera-relative world space position.</summary>
		public SgtPosition GetPosition(Vector3 localPosition)
		{
			var o = default(SgtPosition);

			o.LocalX = localPosition.x;
			o.LocalY = localPosition.y;
			o.LocalZ = localPosition.z;

			o.SnapLocal();

			return o;
		}

		/// <summary>This gives you the camera-relative position of the input SgtPosition in world space.</summary>
		public Vector3 CalculatePosition(ref SgtPosition input)
		{
			var x = (input.GlobalX - SnappedPoint.GlobalX) * SgtPosition.CELL_SIZE + (input.LocalX - SnappedPoint.LocalX);
			var y = (input.GlobalY - SnappedPoint.GlobalY) * SgtPosition.CELL_SIZE + (input.LocalY - SnappedPoint.LocalY);
			var z = (input.GlobalZ - SnappedPoint.GlobalZ) * SgtPosition.CELL_SIZE + (input.LocalZ - SnappedPoint.LocalZ);

			return new Vector3((float)x, (float)y, (float)z);
		}

		/// <summary>If the current <b>Transform.position</b> has strayed too far from the origin, this method will then call <b>Snap</b>.</summary>
		[ContextMenu("Try Snap")]
		public void TrySnap()
		{
			// Did we move far enough?
			if (transform.position.magnitude > SnapDistance)
			{
				Snap();
			}
		}

		/// <summary>This method will reset the current <b>Transform</b> to 0,0,0 then update all <b>SgtFloatingObjects</b> in the scene.</summary>
		[ContextMenu("Snap")]
		public void Snap()
		{
			CheckForPositionChanges();

			SnappedPoint    = Position;
			SnappedPointSet = true;

			SnappedPoint.LocalX = System.Math.Floor(SnappedPoint.LocalX);
			SnappedPoint.LocalY = System.Math.Floor(SnappedPoint.LocalY);
			SnappedPoint.LocalZ = System.Math.Floor(SnappedPoint.LocalZ);

			var oldPosition = transform.position;

			UpdatePositionNow();

			var newPosition = transform.position;

			var delta = newPosition - oldPosition;

			if (OnSnap != null)
			{
				OnSnap(this, delta);
			}
		}

		protected virtual void OnEnable()
		{
			node = Instances.AddFirst(this);
		}

		protected virtual void OnDisable()
		{
			Instances.Remove(node); node = null;
		}

		protected virtual void LateUpdate()
		{
			UpdatePosition();

			TrySnap();
		}

		private void UpdatePosition()
		{
			CheckForPositionChanges();
			UpdatePositionNow();
		}

		private void UpdatePositionNow()
		{
			transform.position = expectedPosition = CalculatePosition(ref Position);

			expectedPositionSet = true;
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingCamera))]
	public class SgtFloatingCamera_Editor : SgtFloatingPoint_Editor<SgtFloatingCamera>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			Separator();

			BeginError(Any(t => t.SnapDistance <= 0.0));
				Draw("SnapDistance", "When the transform.position.magnitude exceeds this value, the position will be snapped back to the origin.");
			EndError();
			Draw("SnappedPoint", "Every time this camera's position gets snapped, its position at that time is stored here. This allows other objects to correctly position themselves relative to this.");
		}
	}
}
#endif