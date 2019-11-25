// THIS IS A SCRIPT FOR THE DEMO SCENE

var turnTorque : float; // Turn rate of the rocket - customize by altering value in the inspector for Rocket
private var thrusterList = new Array(); // Private array of the thrusters in the demo
var currentThruster : GameObject; // Link to the current thruster game object

function Start() {
	
	// Populate Array of Thrusters - this is needed since GameObjects cannot be
	// located using the Unity "Find" functions once a thruster is disabled
	// All thrusters are tagged as "Thrusters" - add them all to the thrusterList array
	thrusterList = GameObject.FindGameObjectsWithTag("Thruster");
	
	ChangeThruster(1); // Set the current thruster to the first thruster
}

function Update () {
	// Code to allow function keys F1-F12 to change the thruster model for the rocket
	if (Input.GetKey(KeyCode.F1)) ChangeThruster(1);
	if (Input.GetKey(KeyCode.F2)) ChangeThruster(2);
	if (Input.GetKey(KeyCode.F3)) ChangeThruster(3);
	if (Input.GetKey(KeyCode.F4)) ChangeThruster(4);
	if (Input.GetKey(KeyCode.F5)) ChangeThruster(5);
	if (Input.GetKey(KeyCode.F6)) ChangeThruster(6);
	if (Input.GetKey(KeyCode.F7)) ChangeThruster(7);
	if (Input.GetKey(KeyCode.F8)) ChangeThruster(8);
	if (Input.GetKey(KeyCode.F9)) ChangeThruster(9);
	if (Input.GetKey(KeyCode.F10)) ChangeThruster(10);
	if (Input.GetKey(KeyCode.F11)) ChangeThruster(11);
	if (Input.GetKey(KeyCode.F12)) ChangeThruster(12);
}

function FixedUpdate() {
	if (Input.GetButton("Fire1")) {
		// If fire button is pressed - start the thruster - or leave it running if already started
		
		// Handle thrusting of the rocket ----------------------------------
		if (!currentThruster.GetComponent.<Script_Thruster>().isActive) {
			// Thruster is not active (checking flag in the "Script_Thruster" script)
			//  call the function to start the thruster.
			currentThruster.GetComponent.<Script_Thruster>().startThruster();
		}
		
	} else {
		// Fire button is not pressed - stop the thruster if it is running
		if (currentThruster.GetComponent.<Script_Thruster>().isActive) {
			currentThruster.GetComponent.<Script_Thruster>().stopThruster();
		}
		
	}
	
	/* Handle turning of the rocket -----------------------------------
	The "turnTorque" variable (customizable strength in the inspector
	for the rocket) controls how fast the rocket turns.
	The vector for the Camera is used so controlling the rocket is relative
	to the camera, left is always left in the camera view etc.
	*/
	//  By Mouse
	if (Input.GetMouseButton(0)) {
		rigidbody.AddTorque(GameObject.Find("Camera").transform.TransformDirection(Vector3(0,0,-Input.GetAxisRaw("Mouse X")*turnTorque)));
		rigidbody.AddTorque(GameObject.Find("Camera").transform.TransformDirection(Vector3(Input.GetAxisRaw("Mouse Y")*turnTorque,0,0)));
	}
	// By Keyboard
	rigidbody.AddTorque(GameObject.Find("Camera").transform.TransformDirection(Vector3(0,0,-Input.GetAxis("Horizontal")*turnTorque)));
	rigidbody.AddTorque(GameObject.Find("Camera").transform.TransformDirection(Vector3(Input.GetAxis("Vertical")*turnTorque,0,0)));
}

function ChangeThruster(id :int) {
	// Function to change the thruster for the rocket
	// Loop through all the thrusters and enable the appropriate thruster and disable all the other ones
	for (var t : GameObject in thrusterList) {
		if (t.name == "Prefab_Thruster_"+id) {
			t.active = true; // Set GameObject for the thruster prefab to active = true
			currentThruster = t; // Set the currentThruster variable to reference the active thruster prefab
		} else {
			t.transform.Find("ThrusterLight").light.intensity = 0; // Ensure light is switched off for the inactive thrusters
			t.active = false; // Set GameObject for the thruster prefab to active = false
		}
	}
	// Update the GUI since we may have changed the thruster using a function key
	GameObject.Find("GameEngine").GetComponent.<Script_Game>().currentThruster = id;
}