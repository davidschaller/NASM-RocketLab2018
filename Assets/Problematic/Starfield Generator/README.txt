# Starfield Generator

To generate a starfield, simply use the included Generator scene, or drop the "Starfield Generator" prefab into your 
own scene.

When tweaking settings, use the "Regenerate Starfield" button in the inspector to regenerate a new starfield with the
new specs. Once you're happy with the outcome, click the "Generate Skybox" button to save the starfield to a cubemap
texture and assign the cubemap to a new skybox material. You can then assign the material as the global skybox, or
add a skybox component to your scene camera.

You can assign any number of objects to be used when generating a new starfield. The settings are as follows:

Prefab - the gameObject prefab to instantiate
Weight - how likely the object is to be randomly selected. If you have one object with weight 1 and one object with 
	weight 4, the object with weight 4 will be selected four times as often
Gradient - the shader color on the instantiated prefab will be set to a value randomly sampled from the gradient
Min/Max Size - How much the instantiated prefab will be scaled (randomly selected in range between min and max)
Base Emission Color / Min/Max Emission Strength - The emission color and intensity of the instantiated prefab will be 
	set to the sampled gradient color, plus the base emission color multiplied by a random value between min and max 
	emission strength


## Notes

Adding image effects to the starfield generator's camera can add a great feel to the skybox. Import the Effects package 
and add the image effects to the camera. I recommend using Bloom, Tonemapping, Color Correction, and Antialiasing to start.
