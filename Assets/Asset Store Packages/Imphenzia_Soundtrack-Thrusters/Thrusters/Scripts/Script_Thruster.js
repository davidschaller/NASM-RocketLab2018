// MAIN SCRIPT FOR THRUSTERS

var isActive : boolean; // Boolean value to say if thruster is active or not
var thrusterForce : float; // Customizable force of the thruster

function FixedUpdate () {
	if (isActive) {
		// If the thruster is active - add the relative thruster force to the parent object
		transform.parent.rigidbody.AddRelativeForce (Vector3.up * thrusterForce);
	}
}

function Update() {
	// Alter the intensity of the child light of the thruster depending on the number of particles active (0 if no particles)
	transform.Find("ThrusterLight").light.intensity = particleEmitter.particleCount / 20;
}


// Call startThruster() function to start the thruster
function startThruster() {
	audio.volume = 1.0; // Reset audio volume (fade out in stopThruster function sets it to 0)
	audio.loop = true; // Ensure that the audio is set to be looped
	audio.Play(); // Play the sound
	particleEmitter.emit = true;// Enable the paritcle emitter for the visuals of the thruster
	isActive = true; // Set the thruster active flag to true
}

// Call stopThruster() function to stop the thruster
function stopThruster() {
	isActive = false; // Set the thruster active flag to false
	particleEmitter.emit = false; // Stop emitting particles for the thruster
	
	// Workaround: If audio.Stop() is called directly it will result in a audible click - therefore we need to fade it out instead
	while(audio.volume > 0.01) {
		audio.volume -= 0.1;
		if (isActive) {
			// If the thruster ignites again before it has faded out - exit this function
			return;
		}
		yield WaitForSeconds(0.01); // Wait one hundred of a second
	}
	audio.Stop(); // Stop the audio playing
}
