using UnityEngine;
using System.Runtime.InteropServices;

public class iOSInfo 
{
	public static bool isIPadMini
	{
		get 
		{
			string model = GetDeviceModel();
			if (!model.StartsWith("iPad2,"))
				return false;
			
			if (GetMinorNumber(model) >= 5)
				return true;
			
			return false;
		}
	}
	
	public static bool isIPad4Gen
	{
		get 
		{
			string model = GetDeviceModel();
			if (!model.StartsWith("iPad3,"))
				return false;
			
			if (GetMinorNumber(model) >= 4)
				return true;
			
			return false;
		}
	}
	
	public static float dpi
	{
		get { return isIPadMini ? 167.0f : Screen.dpi; }
	}
	
#if UNITY_EDITOR || !UNITY_IOS
	public static string GetDeviceModel() {return "";}
#else
	[DllImport ("__Internal")]
	public static extern string GetDeviceModel();
#endif // UNITY_EDITOR || !UNITY_IOS

	protected static int GetMinorNumber(string hwid)
	{
		return int.Parse(hwid.Substring(hwid.IndexOf(',') + 1));
	}
}
