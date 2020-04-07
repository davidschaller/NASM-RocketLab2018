using UnityEngine;

namespace SpaceGraphicsToolkit
{
	/// <summary>This class allows you to quickly exit play mode or close the application by skipping code when quitting is true.</summary>
	public abstract class SgtBehaviour : MonoBehaviour
	{
		/// <summary>This will be set to true when OnApplicationQuit is called, allowing OnDisable and OnDestroy to skip performing expensive calls.</summary>
		[System.NonSerialized] protected bool quitting;

		protected virtual void OnApplicationQuit()
		{
			quitting = true;
		}
	}
}