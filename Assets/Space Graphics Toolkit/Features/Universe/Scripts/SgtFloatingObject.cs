using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component turns a normal <b>GameObject + Transform</b> into one that works with the floating origin system.
	/// Keep in mind the <b>Transform.position</b> value will be altered based on camera movement, so certain components may not work correctly without modification.
	/// For example, if you make this <b>Transform.position</b> between two positions, then those positions will be incorrect when the floating origin snaps to a new position.
	/// To correctly handle this scenario, you must hook into the either this component's <b>OnSnap</b> event, or the static <b>SgtFloatingCamera.OnPositionChanged</b> event, and offset your position values from the given <b>Vector3</b> delta.</summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingObject")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Object")]
	public class SgtFloatingObject : SgtFloatingPoint
	{
		[SerializeField]
		private bool positionSet;

		public bool PositionSet
		{
			set
			{
				positionSet = value;
			}

			get
			{
				return positionSet;
			}
		}

		/// <summary>This allows you to set the random seed used during procedural generation. If this object is spawned from an <b>SgtFloatingSpawner___</b> component, then this will automatically be set.</summary>
		[SgtSeed] public int Seed;

		/// <summary>If this object is spawned from an <b>SgtFloatingSpawner___</b> component, then it will be given a seed and this event will be invoked.
		/// int = Seed.</summary>
		public event System.Action<int> OnSpawn;

		/// <summary>This event is called every <b>Update</b> with the current distance to the camera.</summary>
		public event System.Action<double> OnDistance;

		/// <summary>This method allows you to reset the <b>Position</b>, which will then be calculated in <b>Start</b>, or when you manually call the <b>DerivePosition</b> method.
		/// You should use this if your object is part of a prefab, and you want to spawn it using <b>Transform.position</b> values.</summary>
		[ContextMenu("Reset Position")]
		public void ResetPosition()
		{
			Position            = default;
			positionSet         = false;
			expectedPositionSet = false;
		}

		/// <summary>This method will calculate the <b>Position</b> based on the current <b>Transform.position</b> value relative to the current <b>SgtFloatingCamera</b>.</summary>
		[ContextMenu("Derive Position")]
		public void DerivePosition()
		{
			var floatingCamera = default(SgtFloatingCamera);

			expectedPosition = transform.position;

			if (SgtFloatingCamera.TryGetInstance(ref floatingCamera) == true && floatingCamera.SnappedPointSet == true)
			{
				SetPosition(floatingCamera.SnappedPoint + expectedPosition);
			}
			else
			{
				SetPosition(new SgtPosition(expectedPosition));
			}

			positionSet = true;
		}

		/// <summary>You can call this method to invoke the OnSpawn method.
		/// NOTE: This should only be called from an <b>SgtFloatingSpawner___</b> component.</summary>
		public void InvokeOnSpawn()
		{
			if (OnSpawn != null)
			{
				OnSpawn(Seed);
			}
		}

		public override void PositionChanged()
		{
			base.PositionChanged();

			CheckForPositionChanges();

			UpdatePositionNow();
		}

		protected virtual void OnEnable()
		{
			SgtFloatingCamera.OnSnap += HandleSnap;
		}

		protected virtual void Start()
		{
			if (positionSet == false)
			{
				DerivePosition();
			}

			UpdatePositionNow();
		}

		protected virtual void Update()
		{
			CheckForPositionChanges();

			if (OnDistance != null && SgtFloatingCamera.Instances.Count > 0)
			{
				var floatingCamera = SgtFloatingCamera.Instances.First.Value;
				var distance       = SgtPosition.Distance(ref Position, ref floatingCamera.Position);

				OnDistance.Invoke(distance);
			}
		}

		protected virtual void OnDisable()
		{
			SgtFloatingCamera.OnSnap -= HandleSnap;
		}

		private void HandleSnap(SgtFloatingCamera floatingCamera, Vector3 delta)
		{
			CheckForPositionChanges();
			UpdatePositionNow(floatingCamera);
		}

		private void UpdatePositionNow()
		{
			if (SgtFloatingCamera.Instances.Count > 0)
			{
				UpdatePositionNow(SgtFloatingCamera.Instances.First.Value);
			}
		}

		private void UpdatePositionNow(SgtFloatingCamera floatingCamera)
		{
			transform.position = expectedPosition = floatingCamera.CalculatePosition(ref Position);

			expectedPositionSet = true;
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingObject))]
	public class SgtFloatingObject_Editor : SgtFloatingPoint_Editor<SgtFloatingObject>
	{
		protected override void OnInspector()
		{
			if (Any(t => t.PositionSet == true))
			{
				base.OnInspector();
			}

			if (Any(t => t.PositionSet == false))
			{
				EditorGUILayout.HelpBox("This SgtFloatingObject's Position hasn't been set yet. It will be calculated in Start or when you manually call the DerivePosition method.", MessageType.Info);
			}

			Separator();

			Draw("Seed", "This allows you to set the random seed used during procedural generation. If this object is spawned from an SgtFloatingSpawner___ component, then this will automatically be set.");
		}
	}
}
#endif