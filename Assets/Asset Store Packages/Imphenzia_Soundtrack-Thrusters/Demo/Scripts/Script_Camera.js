// THIS IS A SCRIPT FOR THE DEMO SCENE

private var target : Transform; // The target to point the camera at (which will be the rocket)

function Start() {
	// Set the target to the Rocket object transform (position)
	this.target = GameObject.Find("Rocket").transform;
}

function Update () {
	// Always keep looking at the target
	transform.LookAt(target);
}
