** IMPORTANT **************************************************************

 BEFORE starting the demo scene - lower the gravity for your project!
 
 Edit --> Project Settings --> Physics --> Gravity Y: -3.0
 
 (If you don't the rocket don't have enough force to take off...)

** IMPORTANT **************************************************************


---------------------------------------------------------------------------
 IMPHENZIA SOUNDTRACK - THRUSTERS
---------------------------------------------------------------------------

Thrusters (for Unity) is a collection of thruster Prefabs that can be
attached as children to an object. The thruster Prefabs consist of a
seamlessly looping sound effect, particle emitter, thruster light,
and a thruster script. The force applied by the thruster is customizable
along with all the properties of the particle effects and light.

Contained within the Unity package is a playable demo scene where the
thrusters can be alternated on a 3D rocket in space. The content in the
demo scene can be used commercially.

--------------------------------------------------------------------------
 CONTENT
---------------------------------------------------------------------------

Main Content
* 12 Thruster Prefabs - sound, particles, and script (\Thrusters\Prefabs)
* 12 Seamlessly looping thruster sound effects (\Thrusters\Sound)

Bonus Content
* Playable Demo Scene (\Demo)
* Seamless Skybox Textures for a "SpaceBox" (\Demo\SpaceBox)
* Textured 3D Rocket model (\Demo\Rocket)
* Scripts to control the rocket and aming camera (\Demo\Scripts)
* Music: "Spirit Within You" (Calm Loop 07 - Breakdown Without Perc)
  (See Imphenzia Soundtrack in the Unity Asset Store for more music)

---------------------------------------------------------------------------
 HELP / INSTRUCTIONS
---------------------------------------------------------------------------

** YOUTUBE TUTORIAL **
http://www.youtube.com/watch?v=ISsvDwgr298

To use a Thruster Prefab for your own objects:

1. Import the Thrusters package from the Unity Asset Store to your project

2. Drag a Thruster prefab (\Thrusters\Prefabs) onto your object so it
   becomes a child. Your object must be a rigidbody object.

3. Move the thruster to the appropriate position and rotation in relation
   to your object.

4. Modify the "Thruster Force" variable for the appropriate thrust force of
   the thruster - this variable is located in the inspector of the prefab
   under "Script_Thruster"

5. Call the startThruster() function in the Script_Thuster script to
   ignite the thruster.

6. Call the stopThruster() function in the Script_Thruster script for the
   prefab to stop the thruster.

Common Issues:

Problem: My thruster is too weak/strong
Answer: Select the thruster and change the "Thruser Force" variable.

Problem: The thruster particles are too large
Answer: Modify the "Min Size" and "Max Size" settings under Ellipsoid
        Particle emitter for the thruster prefab.

Problem: The thruster flame is too long/fast or short/slow
Answer: Modify the "Local Velocity Y" under the Ellipsoid Particle Emitter
        to decrease or increase the value.

Problem: The area from which the thruster emitts particles is too small/big
Answer: Modify the Ellipsoid X and Z values of the thruster prefab.


---------------------------------------------------------------------------
 Official Web Site: http://soundtrack.imphenzia.com
---------------------------------------------------------------------------
 Copyright 2011 Stefan Persson - Imphenzia Soundtrack
---------------------------------------------------------------------------
