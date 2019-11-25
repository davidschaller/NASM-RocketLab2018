using UnityEngine;
using System.Collections;

public class TooltipsStartup : MTIUIBase
{
	public float startupTimeout = 3;
	
	void Start ()
	{
		Invoke("TurnOffTooltips", startupTimeout);
	}
	
	void TurnOffTooltips ()
	{
		gameObject.SetActiveRecursively(false);
	}
}
