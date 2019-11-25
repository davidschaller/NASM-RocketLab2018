using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public delegate void MTDebugButton(string name);

public class MTDebug : MonoBehaviour
{
	static MTDebug main;
	static MTDebug Main
	{
		get 
			{
				if (main == null)
					{
						Construct();
					}
				
				return main;
			}
		set
			{
				main = value;
			}
	}
	
	public static MTDebug Construct()
	{
		if (!main)
		{
			GameObject go = new GameObject();
			go.name = "Debug";
			main = (MTDebug)go.AddComponent(typeof(MTDebug));
			WatchVariable("Game Time: {0:0.0}", Time.time);
		}
		
		return main;
	}

	bool windowActive = false;
	void Update ()
	{
		WatchVariable("Game Time: {0:0.0}", Time.time);
		
		if (Input.GetKeyDown("home"))
		{
			windowActive = !windowActive;
		}
	}
	
	Rect windowRect = new Rect(10, 30, 200, 1);
	void OnGUI ()
	{
		if (windowActive)
			windowRect = GUILayout.Window (0, windowRect, DebugWindow, "Debug Window");
	}
	
	void DebugWindow(int id)
	{
		foreach (KeyValuePair<string, object> myEntry in watchVars)
		{
			GUILayout.Label(String.Format((string)myEntry.Key, myEntry.Value));
		}

		GUILayout.BeginHorizontal();
		foreach (KeyValuePair<string, MTDebugButton> myEntry in buttons)
		{
			if (GUILayout.Button(myEntry.Key))
				myEntry.Value(myEntry.Key);
		}
		GUILayout.EndHorizontal();
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
	}
	
	Dictionary<string,object> watchVars = new Dictionary<string,object>();
	public static void WatchVariable(string name, object var)
	{
		Main.watchVars[name] = var;
	}
	
	public static void RemoveVariable(string name)
	{
		Main.watchVars.Remove(name);
	}
	
	IEnumerator ExpireVar(string name, float expire)
	{
		yield return new WaitForSeconds(expire);
		RemoveVariable(name);
	}
	
	public static void WatchVariable(string name, object var, float expireTime)
	{
		WatchVariable(name, var);
		Main.StartCoroutine(Main.ExpireVar(name, expireTime));
	}
	
	Dictionary<string,MTDebugButton> buttons = new Dictionary<string,MTDebugButton>();
	public static void AddButton(string name, MTDebugButton d)
	{
		Main.buttons[name] = d;
	}
	
	public static void RemoveButton(string name)
	{
		Main.buttons.Remove(name);
	}
}
