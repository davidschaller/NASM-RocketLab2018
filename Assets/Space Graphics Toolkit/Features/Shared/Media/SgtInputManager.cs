using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SpaceGraphicsToolkit
{
	/// <summary>This component converts mouse and touch inputs into a single interface.</summary>
	public class SgtInputManager
	{
		public class Finger
		{
			public int     Index;
			public float   Pressure;
			public bool    LastSet;
			public bool    Set;
			public Vector2 LastPosition;
			public Vector2 Position;

			public bool Down
			{
				get
				{
					return Set == true && LastSet == false;
				}
			}

			public bool Up
			{
				get
				{
					return Set == false && LastSet == true;
				}
			}
		}

		private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

		private static PointerEventData tempPointerEventData;

		private static EventSystem tempEventSystem;

		private List<Finger> fingers = new List<Finger>();

		private static Stack<Finger> pool = new Stack<Finger>();

		public static float ScaleFactor
		{
			get
			{
				var dpi = Screen.dpi;

				if (dpi <= 0)
				{
					dpi = 200.0f;
				}

				return 200.0f / dpi;
			}
		}

		public List<Finger> Fingers
		{
			get
			{
				return fingers;
			}
		}

		public Vector2 GetAverageDeltaScaled()
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				total += finger.Position - finger.LastPosition;
				count += 1;
			}

			if (count > 0)
			{
				total *= ScaleFactor;
				total /= count;

			}

			return total;
		}

		public Vector2 GetLastScreenCenter()
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.LastPosition;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public Vector2 GetScreenCenter()
		{
			var total = Vector2.zero;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += finger.Position;
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public float GetScreenDistance(Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.Position);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public float GetLastScreenDistance(Vector2 center)
		{
			var total = 0.0f;
			var count = 0;

			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger != null)
				{
					total += Vector2.Distance(center, finger.LastPosition);
					count += 1;
				}
			}

			return count > 0 ? total / count : total;
		}

		public float GetPinchScale(float wheelSensitivity = 0.0f)
		{
			var center       = GetScreenCenter();
			var lastCenter   = GetLastScreenCenter();
			var distance     = GetScreenDistance(center);
			var lastDistance = GetLastScreenDistance(lastCenter);

			if (lastDistance > 0.0f)
			{
				return distance / lastDistance;
			}

			if (wheelSensitivity != 0.0f)
			{
				var scroll = Input.mouseScrollDelta.y;

				if (scroll > 0.0f)
				{
					return 1.0f - wheelSensitivity;
				}

				if (scroll < 0.0f)
				{
					return 1.0f + wheelSensitivity;
				}
			}

			return 1.0f;
		}

		public static bool PointOverGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition).Count > 0;
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition)
		{
			return RaycastGui(screenPosition, 1 << 5);
		}

		public static List<RaycastResult> RaycastGui(Vector2 screenPosition, LayerMask layerMask)
		{
			tempRaycastResults.Clear();

			var currentEventSystem = EventSystem.current;

			if (currentEventSystem != null)
			{
				// Create point event data for this event system?
				if (currentEventSystem != tempEventSystem)
				{
					tempEventSystem = currentEventSystem;

					if (tempPointerEventData == null)
					{
						tempPointerEventData = new PointerEventData(tempEventSystem);
					}
					else
					{
						tempPointerEventData.Reset();
					}
				}

				// Raycast event system at the specified point
				tempPointerEventData.position = screenPosition;

				currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

				// Loop through all results and remove any that don't match the layer mask
				if (tempRaycastResults.Count > 0)
				{
					for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
					{
						var raycastResult = tempRaycastResults[i];
						var raycastLayer  = 1 << raycastResult.gameObject.layer;

						if ((raycastLayer & layerMask) == 0)
						{
							tempRaycastResults.RemoveAt(i);
						}
					}
				}
			}

			return tempRaycastResults;
		}

		public void Update(KeyCode key = KeyCode.Mouse0)
		{
			// Discard old fingers that went up
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Up == true)
				{
					fingers.RemoveAt(i); pool.Push(finger);
				}
				else
				{
					finger.LastSet = finger.Set;
					finger.Set     = false;
				}
			}

			// Update real fingers
			if (Input.touchCount > 0)
			{
				for (var i = 0; i < Input.touchCount; i++)
				{
					var touch = Input.GetTouch(i);

					if (touch.phase == TouchPhase.Began)
					{
						CreateFinger(touch.fingerId, touch.position, touch.pressure);
					}
					else
					{
						UpdateFinger(touch.fingerId, touch.position, touch.pressure, touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
					}
				}
			}
			// If there are no real touches, simulate some from the mouse?
			else
			{
				var mousePosition = (Vector2)Input.mousePosition;

				if (Input.GetKeyDown(key) == true)
				{
					CreateFinger(-1, mousePosition, 1.0f);
				}
				else
				{
					UpdateFinger(-1, mousePosition, 1.0f, Input.GetKey(key) == true);
				}
			}
		}

		private void CreateFinger(int index, Vector2 screenPosition, float pressure)
		{
			if (PointOverGui(screenPosition) == false)
			{
				var finger = pool.Count > 0 ? pool.Pop() : new Finger();

				finger.Index        = index;
				finger.Pressure     = pressure;
				finger.LastSet      = false;
				finger.Set          = true;
				finger.LastPosition = screenPosition;
				finger.Position     = screenPosition;

				fingers.Add(finger);
			}
		}

		private void UpdateFinger(int index, Vector2 screenPosition, float pressure, bool set)
		{
			for (var i = fingers.Count - 1; i >= 0; i--)
			{
				var finger = fingers[i];

				if (finger.Index == index)
				{
					finger.Pressure     = pressure;
					finger.Set          = set;
					finger.LastPosition = finger.Position;
					finger.Position     = screenPosition;

					break;
				}
			}
		}
	}
}