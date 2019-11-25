using UnityEngine;

public class CannonGUI : MonoBehaviour
{
	public GUISkin skin;
	public static GUISkin GUISkin
	{
		get
		{
			return main.skin;
		}
		private set
		{
		}
	}

	float offsetFromBottom = 20;
	float boxWidth = 400;
	float boxHeight = 150;

	public enum DisplayState
	{
		Init,
		Controls,
		Feedback,
	}
	DisplayState displayState;
	public DisplayState CurrentState
	{
		get
		{
			return displayState;
		}
		set
		{
			displayState = value;
			if (displayState == DisplayState.Controls)
			{
				Debug.Log("Reset full text");
				
				fullText = "";
			}
		}
	}

	bool controlsEnabled = false;
	public bool ControlsEnabled
	{
		get
		{
			return controlsEnabled;
		}
		set
		{
			controlsEnabled = value;
		}
	}

	public static CannonGUI main;
	
	CannonController cannon;
	void Awake ()
	{
		cannon = GetComponent<CannonController>();
		main = this;
		cannon.TotalScore = PlayerPrefs.GetInt("CannonScore", 0);
	}

	public string degrees = "d";
	public float pitchOffset = 0;
	void DrawPitchControls ()
	{
		GUILayout.BeginHorizontal();
	
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Tilt:", "tilt/rotate text");
		GUILayout.Label((cannon.PitchAngle + pitchOffset) + degrees, "tilt/rotate text");
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();

		GUILayout.BeginVertical();
		if (GUILayout.Button("Up", "pitch up button"))
			cannon.PitchUp();		
		if (GUILayout.Button("Down", "pitch down button"))
			cannon.PitchDown();
		GUILayout.EndVertical();
	
		GUILayout.EndHorizontal();
	}

	void DrawYawControls ()
	{
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		if(GUILayout.Button("L", "rotate left button"))
			cannon.RotateLeft();
		if (GUILayout.Button("R", "rotate right button"))
			cannon.RotateRight();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Rotate: " + cannon.YawAngle + degrees + " " + cannon.YawDirection, "tilt/rotate text");
		GUILayout.EndHorizontal();

		GUILayout.EndVertical();
	}


	public DistanceClassification[] distanceClassifications;
	public DistanceClassification[] leftRightDistanceClassifications;

	string GetDistanceClassification(DistanceClassification[] cl, float dist)
	{
		foreach(DistanceClassification d in cl)
		{
			if (dist < d.maxDistance && dist > d.minDistance)
				return d.textVersion;
		}

		return "Indeed!";
	}
	
	string fullText = "";
	public void RegisterFeedbackData ( Vector3 closestTarget, Vector3 impactPoint )
	{
		if (impactPoint == Vector3.zero)
		{
			fullText = "A hit! A very palpable hit! Now strike them again!";
		}
		else
		{
			Vector3 vectorToTarget = closestTarget - cannon.transform.position;
			Vector3 vectorToImpact = impactPoint - cannon.transform.position;
			float targetToImpactAngle = Vector3.Angle(vectorToTarget, vectorToImpact);
			
			float distToTarget = vectorToTarget.magnitude;
			float distToImpact = vectorToImpact.magnitude;
			float diff = Mathf.Abs(distToTarget - distToImpact);
		
			string distanceText = "";
			fullText = "A miss!  ";

			string diffText = GetDistanceClassification(distanceClassifications, diff);
			if (distToTarget > distToImpact)
			{
				distanceText = " " + diffText + " ";
			}
			else
			{
				distanceText = " " + diffText + " ";
			}

			string directionText = " and perhaps adjust your aim slightly rightward.";
			diffText = GetDistanceClassification(distanceClassifications, diff);
			// formula for triangle?  use distance to closest target
			// as the same for 2 legs.  Use the angle between those 2
			// legs to find the 3rd side (opposite the known angle)
			float oppLength = distToTarget*distToTarget + distToTarget*distToTarget - 2*distToTarget*distToTarget * Mathf.Cos(targetToImpactAngle*Mathf.Deg2Rad);
			oppLength = Mathf.Sqrt(oppLength);
			Debug.Log(" " + oppLength +"to batter the enemy! (angle: " + targetToImpactAngle + "), distToTarget: " + distToTarget);
			string side = "right";
			if (Vector3.Dot(cannon.transform.TransformDirection(Vector3.right), (impactPoint - closestTarget).normalized) > 0)
				side = "left";

			diffText = GetDistanceClassification(leftRightDistanceClassifications, diff);
			directionText = " " + diffText +" to the " + side + " to batter the enemy!";

			
			fullText += distanceText + directionText;
			Debug.Log("Register full: " + fullText);
		}
	}

	void DebugWindow(int ind)
	{
		GUILayout.BeginArea( new Rect(20, 40, 600, 100) );
			
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Finish this level"))
		{
			cannon.SendMessage("FinishGame");
			GUIManager.PlayButtonSound();
		}
		if (GUILayout.Button("Fail this level"))
		{
			cannon.CannonBallCount = 0;
			GUIManager.PlayButtonSound();
		}
		if (GUILayout.Button("Delete Prefs"))
		{
			PlayerPrefs.DeleteKey("CannonScore");
			PlayerPrefs.DeleteKey("CannonCompleted");
			cannon.Score = 0;
			cannon.TotalScore = 0;
			GUIManager.PlayButtonSound();
		}
		GUILayout.Label("Multiplier: " +TargetRegistry.CurrentMultiplier);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
			
		GUILayout.EndArea();

		GUI.DragWindow (new Rect (0,0,10000,10000));
	}

	public static void DrawHeader ( GUISkin skin, string quitURL )
	{
		GUIStyle header = skin.GetStyle("header");
		GUIStyle footer = skin.GetStyle("footer");

		Rect headerRect = new Rect(0, 0, Screen.width, header.fixedHeight);
		GUI.Box(headerRect, "", header);
		GUI.Box(new Rect(0, Screen.height - footer.fixedHeight, Screen.width, footer.fixedHeight), "", footer);

		GUILayout.BeginArea(headerRect);
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Quit"))
		{
			Application.OpenURL(quitURL);
			GUIManager.PlayButtonSound();
		}
		GUILayout.Space(10);
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndArea();
	}
	
	public Vector2 feedbackBoxLocationBL = new Vector2(5, 5);
	public string levelDescription = "Destroy British Cannons";
	public int levelNumber = 1;
	public string quitURL = "http://google.com";
	
	Rect windowRect0 = new Rect(100, 100, 600, 100);
	void OnGUI ()
	{
		GUI.skin = skin;

		DrawHeader(skin, quitURL);
		
		if (Application.isEditor)
		{
			 windowRect0 = GUI.Window (0, windowRect0, DebugWindow, "Debug Window");
			 GUI.FocusWindow(0);
		}

		if (displayState != DisplayState.Init)
		{
			GUIStyle scoreBox = skin.GetStyle("score box");
			GUILayout.BeginArea(new Rect(Screen.width-scoreBox.fixedWidth,
										 Screen.height - scoreBox.fixedHeight,
										 scoreBox.fixedWidth,
										 scoreBox.fixedHeight));
			GUILayout.Label("BRITISH DAMAGE:", "score label");
			GUILayout.Label(cannon.HitPercentage + "%", "score text");

			GUILayout.Label("REMAINING\nCANNONBALLS:", "score label");
			GUILayout.Label("" + cannon.CannonBallCount, "score text");

			GUILayout.Label("LEVEL SCORE", "score label");
			GUILayout.Label(cannon.Score + " pts", "score text");

			GUILayout.Label("TOTAL SCORE:", "score label");
			GUILayout.Label(cannon.TotalScore + " pts", "score text");

			GUILayout.EndArea();
		}
		
		if (displayState == DisplayState.Controls && controlsEnabled)
		{		
			GUIStyle controlBanner = skin.GetStyle("control banner");
			boxWidth = controlBanner.fixedWidth;
			boxHeight = controlBanner.fixedHeight;
		
			Rect rect = new Rect(Screen.width/2 - boxWidth/2,
								 Screen.height - boxHeight - offsetFromBottom,
								 boxWidth,
								 boxHeight);
			GUI.Box(rect, "", "control banner");
			GUILayout.BeginArea(rect);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Level " + levelNumber + ": ", "level label");
			GUILayout.Label(levelDescription, "level description");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			DrawPitchControls();

			GUILayout.FlexibleSpace();
		
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Fire!", "fire button"))
			{
				cannon.Fire();
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();

			DrawYawControls();
	
			GUILayout.EndHorizontal();
	
			GUILayout.EndArea();
		}
		else if (displayState == DisplayState.Feedback)
		{
			GUIStyle textBox = skin.GetStyle("text box");
			
			Rect boxArea = new Rect(feedbackBoxLocationBL.x,
									Screen.height - textBox.fixedHeight - feedbackBoxLocationBL.y - textBox.overflow.top - textBox.overflow.bottom,
									textBox.fixedWidth,
									textBox.fixedHeight);
			GUI.Box( boxArea, "", textBox);
			GUILayout.BeginArea(boxArea);
			GUILayout.Label("General Washington says:", "text box title");
			GUILayout.Label(fullText, "text box text");

			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Continue"))
			{
				CurrentState = DisplayState.Controls;
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea();
		}
	}

}

[System.Serializable]
public class DistanceClassification
{
	public float minDistance = 0;
	public float maxDistance = 2.5f;
	
	public string textVersion = "2-5 meters";
}