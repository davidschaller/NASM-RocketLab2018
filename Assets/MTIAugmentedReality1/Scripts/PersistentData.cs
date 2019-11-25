using UnityEngine;
using System.Collections;

public class PersistentData : MonoBehaviour
{
	public bool restorePosition = false;
	public Vector3 storedPosition;
	public Quaternion storedRotation;
	public bool virtualWalkthrough = false;
	public bool unityWalkButtons = false;
	public bool unityTurnButtons = false;
	
	public Texture2D virtualLeft;
	public Texture2D virtualRight;
	public Texture2D virtualForward;
	public Texture2D virtualBack;
	
	static PersistentData main;
	public static PersistentData Main
	{
		get
		{
			InitIfNotAlready();
			
			return main;
		}
	}
	void Awake ()
	{
		if (main == null)
			main = this;
		DontDestroyOnLoad(gameObject);
	}
	
	void OnLevelWasLoaded (int lvl)
	{
		if (lvl == 0)
		{
			foreach(MovementButton m in MovementButton.registry)
			{
				m.enabled = false;
			}
		}
		else
		{
			foreach(MovementButton m in MovementButton.registry)
			{
				m.enabled = true;
			}			
		}
	}
	
	static void InitIfNotAlready()
	{
		if (main == null)
		{
			GameObject go = new GameObject("PersistentData");
			main = go.AddComponent<PersistentData>();
			DontDestroyOnLoad(main.gameObject);
			
			if (UIController.Main.virtualLeft != null)
				main.virtualLeft = UIController.Main.virtualLeft;
			else
			{
				Debug.LogWarning("UI Controller's stuff is null", main.gameObject);
				Debug.Break();
			}
			if (UIController.Main.virtualRight != null)
				main.virtualRight = UIController.Main.virtualRight;
			if (UIController.Main.virtualForward != null)
				main.virtualForward = UIController.Main.virtualForward;
			if (UIController.Main.virtualBack != null)
				main.virtualBack = UIController.Main.virtualBack;
		}
	}
}
