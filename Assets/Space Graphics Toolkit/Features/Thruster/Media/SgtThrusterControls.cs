using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtThrusterControls))]
	public class SgtThrusterControl_Editor : SgtEditor<SgtThrusterControls>
	{
		protected override void OnInspector()
		{
			DrawDefault("Binds");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component allows you to control the specified thrusters with the specified control axes.</summary>
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Thruster Controls")]
	public class SgtThrusterControls : MonoBehaviour
	{
		[System.Serializable]
		public class Bind
		{
			[Tooltip("The control axis used for these thrusters")]
			public string Axis;

			public bool Inverse;

			public bool Bidirectional;

			public List<SgtThruster> Positive;

			public List<SgtThruster> Negative;
		}

		public List<Bind> Binds;

		protected virtual void Update()
		{
			if (Binds != null)
			{
				for (var i = Binds.Count - 1; i >= 0; i--)
				{
					var bind = Binds[i];

					if (bind != null)
					{
						var throttle = Input.GetAxisRaw(bind.Axis);

						if (bind.Inverse == true)
						{
							throttle = -throttle;
						}

						if (bind.Bidirectional == false)
						{
							if (throttle < 0.0f)
							{
								throttle = 0.0f;
							}
						}

						for (var j = bind.Positive.Count - 1; j >= 0; j--)
						{
							var thruster = bind.Positive[j];

							if (thruster != null)
							{
								thruster.Throttle = throttle;
							}
						}

						for (var j = bind.Negative.Count - 1; j >= 0; j--)
						{
							var thruster = bind.Negative[j];

							if (thruster != null)
							{
								thruster.Throttle = throttle;
							}
						}
					}
				}
			}
		}
	}
}