using UnityEngine;
using UnityEditor;
using System.Collections;

public class GPSManager : EditorWindow
{
	GPSScale scaler;
	
    [MenuItem("GPS Utils/GPS Manager")]
    static void Init()
	{
        GPSManager window = GetWindow<GPSManager>("GPS Manager");
        window.Show();
		window.scaler = (GPSScale)GameObject.FindObjectOfType(typeof(GPSScale));
		previousScale = window.scaler.scale;
    }
    
	void RepositionLocators ()
	{
		GPSAnchor[] anchors = (GPSAnchor[])GameObject.FindObjectsOfType(typeof(GPSAnchor));

		foreach(GPSAnchor a in anchors)
		{
			a.IsZeroLoc = false;
		}
			
		GPSScale.InitScale();
						
		foreach(GPSAnchor a in anchors)
		{
			Vector2 uc = GPSScale.UnityCoords(a.latitude, a.longitude);
				
			a.transform.position = new Vector3(uc.x, 0, -uc.y);
		}		
	}
	
	static float previousScale;
	void OnGUI()
	{
        if (scaler != null)
        {
            scaler.scale = EditorGUI.Slider(new Rect(0, 25, position.width, 40), scaler.scale, 13, 19);

            if (scaler.scale != previousScale)
            {
                RepositionLocators();
            }

            if (GUI.Button(new Rect(0, 40, position.width, 100), "Position Locators"))
            {
                RepositionLocators();
            }
        }
	}
}
