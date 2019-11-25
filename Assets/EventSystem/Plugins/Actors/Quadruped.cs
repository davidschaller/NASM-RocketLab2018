using UnityEngine;
using System;

public class Quadruped : ActorBase, ISceneObject
{
	public AnimationClip grazeAnimation;


	static Terrain terrain;
	
	static TerrainData terrainData;
	static Transform terrainTransform;
	
	void Start ()
	{
		if (terrain == null && TerrainManager.MainTerrain != null)
		{
			terrain = TerrainManager.MainTerrain;
			terrainData = terrain.terrainData;
			terrainTransform = terrain.transform;
		}
		
		controller = (CharacterController)GetComponent(typeof(CharacterController));
	}


	void GroundToPlatform ()
	{
		transform.position = new Vector3( transform.position.x,
										  platformTriggers[0].GetFloorLevel(),
										  transform.position.z);
	}
	
	/* ISceneObject required methods */
	public void GroundObject()
	{
		if (platformTriggers.Count == 0)
		{
			Debug.DrawRay(transform.position, Vector3.up*4, Color.red);
			
			if (terrainData != null)
			{
				transform.position = new Vector3(transform.position.x,
												 terrainData.GetInterpolatedHeight(transform.position.x/terrainData.size.x,
																				   transform.position.z/terrainData.size.z) +
												 terrainTransform.position.y,
												 transform.position.z);				
			}
		}
		else
		{
			GroundToPlatform();
		}
		transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
	}
	
	void LateUpdate ()
	{
        /*
         * [Oleg: I'm not sure we need this part of code. Due to this ground works incorrectly,
         *  because can't move character controller under the round and Move function doesn't work]
         * 
        if (controller != null)
        {
            controller.Move(Vector3.down);
            Debug.Log(controller.transform.name);
        }
        else
         */
            GroundObject();
	}
}