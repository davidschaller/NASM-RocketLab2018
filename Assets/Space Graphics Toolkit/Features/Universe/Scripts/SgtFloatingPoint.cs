using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component wraps SgtPosition into a component, and defines a single point in the floating origin system.
	/// Normal transform position coordinates are stored using floats (Vector3), but SgtPosition coordinates are stored using a long and a double pair.
	/// The long is used to specify the current grid cell, and the double is used to specify the high precision relative offset to the grid cell.
	/// Combined, these values allow simulation of the whole observable universe.</summary>
	[DisallowMultipleComponent]
	public abstract class SgtFloatingPoint : MonoBehaviour
	{
		/// <summary>The position wrapped by this component.
		/// NOTE: If you modify this then you must call the <b>PositionChanged</b> method after.</summary>
		public SgtPosition Position;

		/// <summary>Whenever the <b>Position</b> values are modified, this gets called. This is useful for components that depend on this position being known at all times (e.g. SgtFloatingOrbit).</summary>
		public event System.Action OnPositionChanged;

		[SerializeField]
		protected Vector3 expectedPosition;

		[SerializeField]
		protected bool expectedPositionSet;

		/// <summary>Call this method after you've finished modifying the <b>Position</b>, and it will notify all event listeners.</summary>
		public virtual void PositionChanged()
		{
			if (OnPositionChanged != null)
			{
				OnPositionChanged();
			}
		}

		/// <summary>This method allows you to change the whole <b>Position</b> state, and it will automatically call the <b>PositionChanged</b> method if the position is different.</summary>
		public void SetPosition(SgtPosition newPosition)
		{
			if (SgtPosition.Equal(ref newPosition, ref Position) == false)
			{
				Position = newPosition;

				PositionChanged();
			}
		}

		protected void CheckForPositionChanges()
		{
			var position = transform.position;

			if (expectedPositionSet == true)
			{
				if (expectedPosition.x != position.x || expectedPosition.y != position.y || expectedPosition.z != position.z)
				{
					Position.LocalX += position.x - expectedPosition.x;
					Position.LocalY += position.y - expectedPosition.y;
					Position.LocalZ += position.z - expectedPosition.z;

					Position.SnapLocal();

					expectedPosition = position;

					PositionChanged();
				}
			}
			else
			{
				expectedPositionSet = true;
				expectedPosition    = position;
			}
		}

#if UNITY_EDITOR
		protected virtual void OnValidate()
		{
			PositionChanged();
		}
#endif

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			Position = new SgtPosition(transform.position);
		}
#endif
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingPoint))]
	public class SgtFloatingPoint_Editor<T> : SgtEditor<T>
		where T : SgtFloatingPoint
	{
		delegate void DoubleDel(ref SgtPosition position, double value);

		protected override void OnInspector()
		{
			if (Any(t => t.GetComponentsInParent<SgtFloatingPoint>().Length > 1))
			{
				EditorGUILayout.HelpBox("This component is parented to a SgtFloatingObject/Camera, which will not work. Detach it, and use the SgtFollow component instead.", MessageType.Error);
			}

			var modified = false;

			modified |= Draw("Position.LocalX", "The position in meters along the X axis, relative to the current global cell position.");
			modified |= Draw("Position.LocalY", "The position in meters along the Y axis, relative to the current global cell position.");
			modified |= Draw("Position.LocalZ", "The position in meters along the Z axis, relative to the current global cell position.");
			modified |= Draw("Position.GlobalX", "The current grid cell along the X axis. Each grid cell is equal to 50000000 meters.");
			modified |= Draw("Position.GlobalY", "The current grid cell along the Y axis. Each grid cell is equal to 50000000 meters.");
			modified |= Draw("Position.GlobalZ", "The current grid cell along the Z axis. Each grid cell is equal to 50000000 meters.");

			if (modified == true)
			{
				serializedObject.ApplyModifiedProperties();

				DirtyEach(t => { t.PositionChanged(); });
			}
		}
	}
}
#endif