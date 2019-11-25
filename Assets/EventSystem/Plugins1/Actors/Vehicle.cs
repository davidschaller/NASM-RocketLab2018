using UnityEngine;
using System;
using System.Collections.Generic;

public class Vehicle : ActorBase, ISceneActor, ISceneObject
{
	public NPCController[] engines;
	
	static Terrain terrain;
	
	static TerrainData terrainData;
	static Transform terrainTransform;
	Transform frontWheels;
	Transform rearWheels;
	
	VehicleDescriptor desc;
	void Awake ()
	{
		if (!anim)
			anim = (Animation)GetComponentInChildren(typeof(Animation));
		
		// TODO instantiate the "engines" and position them, instantiate any "minders"...
		desc = (VehicleDescriptor)gameObject.GetComponent(typeof(VehicleDescriptor));
		if (desc != null)
		{
			engines = desc.engines;
            if (engines != null)
            {
                for (int i = 0; i < engines.Length; i++)
                {
                    if (engines[i])
                    {
                        engines[i].ActivateNPC();
                    }
                }
            }

			frontWheels = anim.transform.Find(desc.frontWheelsMeshName);
			rearWheels = anim.transform.Find(desc.rearWheelsMeshName);
		}
	}
	
	void Start ()
	{
		if (terrain == null)
		{
			terrain = TerrainManager.MainTerrain;
			terrainData = terrain.terrainData;
			terrainTransform = terrain.transform;
		}
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		//print("Collided with: " + hit.gameObject.name);
	}
	
	/* ISceneObject required methods */
	public void GroundObject()
	{
		transform.position = new Vector3(transform.position.x, terrainData.GetInterpolatedHeight(transform.position.x/terrainData.size.x, transform.position.z/terrainData.size.z) + terrainTransform.position.y, transform.position.z);
	}
	
	void Update ()
	{
		if (moveState == MoveState.Forward)
		{
			if (frontWheels)
				frontWheels.transform.Rotate(Vector3.right * forwardMoveSpeed*100*Time.deltaTime);
			//else MTDebug.WatchVariable("No front wheel!", 0);
			
			if (rearWheels) 
				rearWheels.transform.Rotate(Vector3.right * forwardMoveSpeed*100*Time.deltaTime);
			//else MTDebug.WatchVariable("No front wheel!", 0);
			
		}
	}
	
	void LateUpdate ()
	{
		GroundObject();

        if (engines != null)
        {
            for (int i = 0; i < engines.Length; i++)
            {
                Vector3 horizontalSpacing = i == 0 ? transform.TransformDirection(-Vector3.right) * desc.engineHorizontalSpacing : transform.TransformDirection(Vector3.right) * desc.engineHorizontalSpacing;
                engines[i].transform.position = transform.position + transform.TransformDirection(Vector3.forward) * desc.engineForwardDistance + horizontalSpacing;

                engines[i].transform.rotation = transform.rotation;

                Animation ea = engines[i].GetAnimation();
                if (moveState == MoveState.Forward)
                {
                    ea.CrossFade("walk");
                }
            }
        }
	}
}
