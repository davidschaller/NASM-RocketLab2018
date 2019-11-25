using UnityEngine;
using System.Collections;

public class GPSScale : MonoBehaviour
{
	public float scale = 17;
	public GPSAnchor unityZeroLocation;

	static GPSAnchor currentZeroLoc;
	static Vector2 baseOffset;
	static float currentScale;
	
	void Start ()
	{
		InitScale();
	}
	
	public static Vector2 UnityCoords ( float latitude, float longitude)
	{
		Vector2 coords = Vector2.zero;
		
		float sinLatitude = Mathf.Sin(latitude * Mathf.PI/180);
		coords.x = ((longitude + 180) / 360) * 256 * Mathf.Pow(2, currentScale);
		coords.y = (0.5f - Mathf.Log((1+sinLatitude)/(1-sinLatitude)) / (4*Mathf.PI)) * 256 * Mathf.Pow(2, currentScale);
		
		coords.x -= baseOffset.x;
		coords.y -= baseOffset.y;
		
		return coords;
	}
	
	public static void InitScale ()
	{
		GPSScale main = (GPSScale)GameObject.FindObjectOfType(typeof(GPSScale));
		
		currentScale = main.scale;
		
		currentZeroLoc = main.unityZeroLocation;
		currentZeroLoc.IsZeroLoc = true;
		
		baseOffset = Vector2.zero;
		baseOffset = UnityCoords(main.unityZeroLocation.latitude, main.unityZeroLocation.longitude);
		
		Debug.Log("Base offset x: " + baseOffset.x + ", y: " + baseOffset.y);
		
	}
}
