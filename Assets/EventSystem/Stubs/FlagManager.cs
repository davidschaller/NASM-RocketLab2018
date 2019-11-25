using UnityEngine;
using System.Collections;

public class FlagManager : MonoBehaviour
{
	public static FlagManager Main;
	public void SetFlagByGameobject (GameObject g)
	{
		
	}
	
	public static FlagSize CurrentflagSize
	{
		get; set;
	}
	
	public enum FlagSize
	{
		Large,
			Medium,
				Small,
					British
	}
}
