using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component will smoothly warp to the target, where the speed will slow down near the start of the travel, and near the end.</summary>
	[DefaultExecutionOrder(100)]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingWarpSmoothstep")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Warp Smoothstep")]
	public class SgtFloatingWarpSmoothstep : SgtFloatingWarp
	{
		/// <summary>Seconds it takes to complete a warp.</summary>
		public double WarpTime = 10.0;

		/// <summary>Warp smoothstep iterations.</summary>
		public int Smoothness = 3;

		/// <summary>Currently warping?</summary>
		public bool Warping;

		/// <summary>Current warp progress in seconds.</summary>
		public double Progress;

		/// <summary>Start position of the warp.</summary>
		public SgtPosition StartPosition;

		/// <summary>Target position of the warp.</summary>
		public SgtPosition TargetPosition;

		public override bool CanAbortWarp
		{
			get
			{
				return Warping;
			}
		}

		public override void WarpTo(SgtPosition position)
		{
			Warping        = true;
			Progress       = 0.0;
			StartPosition  = Point.Position;
			TargetPosition = position;
		}

		public override void AbortWarp()
		{
			Warping = false;
		}

		protected virtual void Update()
		{
			if (Warping == true)
			{
				Progress += Time.deltaTime;

				if (Progress > WarpTime)
				{
					Progress = WarpTime;
				}

				var bend = SmoothStep(Progress / WarpTime, Smoothness);

				if (Point != null)
				{
					Point.Position = SgtPosition.Lerp(ref StartPosition, ref TargetPosition, bend);

					Point.PositionChanged();
				}

				if (Progress >= WarpTime)
				{
					Warping = false;
				}
			}
		}

		private static double SmoothStep(double m, int n)
		{
			for (int i = 0 ; i < n ; i++)
			{
				m = m * m * (3.0 - 2.0 * m);
			}

			return m;
		}
	}
}

#if UNITY_EDITOR
namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingWarpSmoothstep))]
	public class SgtFloatingWarpSmoothstep_Editor : SgtFloatingWarp_Editor<SgtFloatingWarpSmoothstep>
	{
		protected override void OnInspector()
		{
			base.OnInspector();

			Separator();

			BeginError(Any(t => t.WarpTime < 0.0));
				Draw("WarpTime", "Seconds it takes to complete a warp.");
			EndError();
			BeginError(Any(t => t.Smoothness < 1));
				Draw("Smoothness", "Warp smoothstep iterations.");
			EndError();

			Separator();

			BeginDisabled();
				Draw("Warping", "Currently warping?");
				Draw("Progress", "Current warp progress in seconds.");
				Draw("StartPosition", "Start position of the warp.");
				Draw("TargetPosition", "Target position of the warp.");
			EndDisabled();
		}
	}
}
#endif