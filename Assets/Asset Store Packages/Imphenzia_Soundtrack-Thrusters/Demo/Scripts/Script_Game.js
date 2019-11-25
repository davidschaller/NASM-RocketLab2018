// THIS IS A SCRIPT FOR THE DEMO SCENE
var currentThruster = 1;

function OnGUI () {
	// Make a background box
	GUI.Box (Rect (10,10,290,150), "Imphenzia Soundtrack - Thrusters Demo");
	
	// Create a button to toggle thruster models
	if (GUI.Button (Rect (40,40,250,20), "(F1-F12) Thruster: " + (currentThruster))) {
		currentThruster += 1;
		if (currentThruster > 12) currentThruster = 1;
		GameObject.Find("Rocket").GetComponent.<Script_Rocket>().ChangeThruster(currentThruster);
	}
	
	// Create a button to reset the scene
	if (GUI.Button (Rect (40,70,250,20), "Reset Scene ")) {
		Application.LoadLevel ("Scene");
	}
	
	// Display help text
	GUI.Label (Rect (40, 110, 250, 30), "Thrust: Ctrl Key / Left Mouse Button");
	GUI.Label (Rect (40, 130, 250, 30), "Steer: Arrows / Mouse Drag");
	
}

function Update() {
}