using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace SpaceGraphicsToolkit
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SgtFloatingWarpPin))]
	public class SgtFloatingWarpPin_Editor : SgtEditor<SgtFloatingWarpPin>
	{
		protected override void OnInspector()
		{
			Draw("PickDistance", "The maximum distance between the tap/click point at the SgtWarpTarget in scaled screen space.");
			Draw("CurrentTarget", "The currently picked target.");

			Separator();

			Draw("FloatingCamera", "The currently picked target.");
			Draw("WorldCamera", "The currently picked target.");

			Separator();

			BeginError(Any(t => t.Parent == null));
				Draw("Parent", "The parent rect of the pin.");
			EndError();
			BeginError(Any(t => t.Rect == null));
				Draw("Rect", "The main rect of the pin that will be placed on the screen on top of the CurrentTarget.");
			EndError();
			BeginError(Any(t => t.Title == null));
				Draw("Title", "The name of the pin.");
			EndError();
			BeginError(Any(t => t.Group == null));
				Draw("Group", "The group that will control hide/show of the pin.");
			EndError();
			BeginError(Any(t => t.Warp == null));
				Draw("Warp", "The warp component that will be used.");
			EndError();
			Draw("HideIfTooClose", "The the pin if we're within warping distance?");
		}
	}
}
#endif

namespace SpaceGraphicsToolkit
{
	/// <summary>This component moves Rect above the currently picked SgtFloatingTarget. You can tap/click the screen to update the picked target.</summary>
	[ExecuteInEditMode]
	[HelpURL(SgtHelper.HelpUrlPrefix + "SgtFloatingWarpPin")]
	[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Floating Warp Pin")]
	public class SgtFloatingWarpPin : MonoBehaviour
	{
		/// <summary>The maximum distance between the tap/click point at the SgtWarpTarget in scaled screen space.</summary>
		public float PickDistance = 0.025f;

		/// <summary>The currently picked target.</summary>
		public SgtFloatingTarget CurrentTarget;

		/// <summary>The parent rect of the pin.</summary>
		public RectTransform Parent;

		/// <summary>The main rect of the pin that will be placed on the screen on top of the CurrentTarget.</summary>
		public RectTransform Rect;

		/// <summary>The name of the pin.</summary>
		public Text Title;

		/// <summary>The group that will control hide/show of the pin.</summary>
		public CanvasGroup Group;

		/// <summary>The warp component that will be used.</summary>
		public SgtFloatingWarp Warp;

		public SgtFloatingCamera FloatingCamera;

		public Camera WorldCamera;

		/// <summary>The the pin if we're within warping distance?</summary>
		public bool HideIfTooClose = true;

		[HideInInspector]
		public float Alpha;

		[System.NonSerialized]
		private SgtInputManager inputManager = new SgtInputManager();

		public void ClickWarp()
		{
			if (CurrentTarget != null)
			{
				Warp.WarpTo(CurrentTarget.CachedPoint.Position, CurrentTarget.WarpDistance);
			}
		}

		public void Pick(Vector2 pickScreenPoint)
		{
			if (FloatingCamera != null && WorldCamera != null)
			{
				var warpTarget     = SgtFloatingTarget.FirstInstance;
				var bestTarget     = default(SgtFloatingTarget);
				var bestDistance   = float.PositiveInfinity;

				for (var i = 0; i < SgtFloatingTarget.InstanceCount; i++)
				{
					var localPosition = FloatingCamera.CalculatePosition(ref warpTarget.CachedPoint.Position);
					var screenPoint   = WorldCamera.WorldToScreenPoint(localPosition);

					if (screenPoint.z >= 0.0f)
					{
						var distance = ((Vector2)screenPoint - pickScreenPoint).sqrMagnitude;

						if (distance <= bestDistance)
						{
							bestDistance = distance;
							bestTarget   = warpTarget;
						}
					}

					warpTarget = warpTarget.NextInstance;
				}

				if (bestTarget != null)
				{
					var pickThreshold = Mathf.Min(Screen.width, Screen.height) * PickDistance;

					if (bestDistance <= pickThreshold * pickThreshold)
					{
						CurrentTarget = bestTarget;
					}
				}
				else
				{
					CurrentTarget = null;
				}
			}
		}

		protected virtual void LateUpdate()
		{
			inputManager.Update();

			foreach (var finger in inputManager.Fingers)
			{
				if (finger.Down == true)
				{
					Pick(finger.Position);
				}
			}

			var targetAlpha = 0.0f;

			if (FloatingCamera != null && WorldCamera != null)
			{
				if (CurrentTarget != null)
				{
					var localPosition = FloatingCamera.CalculatePosition(ref CurrentTarget.CachedPoint.Position);
					var screenPoint   = WorldCamera.WorldToScreenPoint(localPosition);

					if (screenPoint.z >= 0.0f)
					{
						var anchoredPosition = default(Vector2);

						if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Parent, screenPoint, null, out anchoredPosition) == true)
						{
							Rect.anchoredPosition = anchoredPosition;
						}

						targetAlpha = 1.0f;

						if (HideIfTooClose == true)
						{
							if (SgtPosition.SqrDistance(ref SgtFloatingCamera.Instances.First.Value.Position, ref CurrentTarget.CachedPoint.Position) <= CurrentTarget.WarpDistance * CurrentTarget.WarpDistance)
							{
								targetAlpha = 0.0f;
							}
						}
					}
					else
					{
						Alpha = 0.0f;
					}

					Title.text = CurrentTarget.WarpName;
				}
			}

			var factor = SgtHelper.DampenFactor(10.0f, Time.deltaTime);

			Alpha = Mathf.Lerp(Alpha, targetAlpha, factor);

			Group.alpha          = Alpha;
			Group.blocksRaycasts = targetAlpha > 0.0f;
		}
	}
}