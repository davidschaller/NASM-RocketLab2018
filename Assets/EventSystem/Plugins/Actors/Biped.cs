using UnityEngine;
using System;
using System.Collections.Generic;

public class Biped : ActorBase, ISceneActor, ISceneObject
{
	static Terrain terrain;
	
	static TerrainData terrainData;
	static Transform terrainTransform;

	static Terrain musicTerrain;
	static TerrainData musicTerrainData;
	static Transform musicTerrainTransform;

    public AnimationClip climbRampart,
                         jumpOffRampart;

    public float moveDownSpeed = 1,
                 jumpOffForward = .3f;

    public bool easyMode = false;
	
	void Awake ()
	{
        controller = (CharacterController)GetComponent(typeof(CharacterController));

        if (!controller)
            enabled = false;

        GroundObject();
	}
	
	void Start ()
	{
		if (terrain == null)
		{
			terrain = TerrainManager.MainTerrain;
			if (terrain != null)
			{
				terrainData = terrain.terrainData;
				terrainTransform = terrain.transform;
			}
		}
		
		if (terrain != null)
		{
			if (transform.root.name == "Music Scene Root" && musicTerrain == null)
			{
				musicTerrain = transform.root.Find("Terrain").GetComponent<Terrain>();
				musicTerrainData = musicTerrain.terrainData;
				musicTerrainTransform = musicTerrain.transform;
			}
		}
		
		

        GetYWithRaycast(transform.position);

        if (!anim)
            anim = transform.GetComponentInChildren<Animation>();

        if (climbRampart)
        {
            if (anim && anim[climbRampart.name] == null)
                anim.AddClip(climbRampart, "climb");
        }

        if (jumpOffRampart)
        {
            if (anim && anim[jumpOffRampart.name] == null)
                anim.AddClip(jumpOffRampart, "jumpOff");
        }
	}

    void GetYWithRaycast(Vector3 cachdPosition)
    {
        RaycastHit hit;

        Ray ray = new Ray(cachdPosition + Vector3.up * 40, Vector3.down);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Walkable")))
        {
            cachedPos = new Vector3(cachdPosition.x, hit.point.y, cachdPosition.z);
        }
        else
            cachedPos = Vector3.zero;		
    }

    Vector3 cachedPos = Vector3.zero;
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		//print("Collided with: " + hit.gameObject.name);
	}
	

	void GroundToPlatform ()
	{
		transform.position = new Vector3( transform.position.x,
										  platformTriggers[0].GetFloorLevel(),
										  transform.position.z);
	}

    Vector3 lastPos = Vector3.zero;
	bool musicSceneRoot = false;
	/* ISceneObject required methods */
	Vector3 lastGroundedPosition;
	Vector3 cachedPosition;
	Vector3 cachedEuler;
	public void GroundObject()
	{
		/* PERFORMANCE: This function is incredibly performance-critical for large numbers of NPCs, 
		 * so transform.position and transform.eulerAngles are cached to avoid multiple lookups/de-references.
		 * The profiler makes these look less harmful than they actually seem to be.
		 */
		
		//Profiler.BeginSample("Ground object");

		bool positionUpdate = false;
		bool eulerUpdate = false;
		
		if (platformTriggers.Count == 0)
		{
			//Debug.DrawRay(cachedPosition, Vector3.up*4, Color.red);
			
			/* PERFORMANCE: string compare is too expensive with large numbers of Bipeds */
			//if (transform.root.name == "Music Scene Root")
			if (musicSceneRoot)
			{
				Debug.LogWarning("Music scene root");
				cachedPosition = new Vector3(cachedPosition.x,
												 musicTerrainData.GetInterpolatedHeight(cachedPosition.x/musicTerrainData.size.x,
																						cachedPosition.z/musicTerrainData.size.z) +
												 musicTerrainTransform.position.y, cachedPosition.z);
			}
			else
			{
				//Approximately is very difficult
                //if (!Mathf.Approximately(cachedPosition.x, transform.position.x) || !Mathf.Approximately(cachedPosition.z, transform.position.z))
                if (!(cachedPosition.x==transform.position.x) || !(cachedPosition.z==transform.position.z))
                {
                    GetYWithRaycast(transform.position);
                    cachedPosition = cachedPos;
                    positionUpdate = true;
                }

                /*
				if (cachedPos != Vector3.zero)
                {
					//Profiler.BeginSample("cache not zero");
					cachedPosition = transform.position;
					
					if (cachedPos.y != cachedPosition.y)
					{
						Vector3 newpos = new Vector3(cachedPosition.x, cachedPos.y, cachedPosition.z);
                    	cachedPosition = newpos;
						
						cachedPos = cachedPosition;
						positionUpdate = true;
					}
					
					
                    if (cachedPosition != cachedPos)
                    {
						//Profiler.BeginSample("Get y w/raycast");
                        GetYWithRaycast(cachedPosition);
						//Profiler.EndSample();
						positionUpdate = true;
                    }
					//Profiler.EndSample();
                }
                else
				{
					SetTerrain("Terrain-Fort");
					positionUpdate = true;
					cachedPosition = transform.position;
				}
                 */
			}
		}
		else
		{
			Debug.LogWarning("Grounding to platform...", gameObject);
			GroundToPlatform();
			positionUpdate = true;
		}
		
		//Profiler.BeginSample("Euler angles and assign last grounded");
		cachedEuler = transform.eulerAngles;
		if(cachedEuler.x != 0 || cachedEuler.z != 0)
		{
			// Debug.LogWarning("Correcting euler angles to zero, x: " + cachedEuler.x + ", z: " + cachedEuler.z);
			cachedEuler = new Vector3(0, cachedEuler.y, 0);
			eulerUpdate = true;
		}
		
		//lastGroundedPosition = cachedPosition;
		//Profiler.EndSample();
		
		//Profiler.BeginSample("Reassign cached vars");
		if (positionUpdate)
			transform.position = cachedPosition;

		if (eulerUpdate)
			transform.eulerAngles = cachedEuler;
		//Profiler.EndSample();
		
		//Profiler.EndSample();
	}

	public float UpdateDeltaTime = 0.5f;
	private float LastUpdateTime = 0;
	
	void LateUpdate ()
	{
        if (easyMode)
        {
            Destroy(GetComponent<Rigidbody>());
            enabled = false;
            return;
        }

        if (controller)
        {
            CollisionFlags flags = controller.Move(Vector3.down * moveDownSpeed);

            if (anim.IsPlaying("jumpOff"))
            {
                ConstrainToGround = false;
            }
            else
                ConstrainToGround = (flags & CollisionFlags.CollidedBelow) != 0;
            
            if (!ConstrainToGround)
            {
                if (jumpOffRampart)
                {
                    controller.Move(transform.TransformDirection(moveState == MoveState.Forward ? Vector3.forward : Vector3.back) * jumpOffForward);

                    anim["jumpOff"].wrapMode = WrapMode.Once;

                    if (!anim.IsPlaying("jumpOff"))
                    {
                        anim.CrossFade("jumpOff");
                        anim.CrossFadeQueued("idle");
                    }
                }
            }
        }
        else if (ConstrainToGround)
		{
	        if (Time.time - LastUpdateTime > UpdateDeltaTime)
			{
				LastUpdateTime = Time.time;
        	    GroundObject();
			}
		}
	}

    Terrain terrainByName;
    void SetTerrain(string terrainName)
    {
        if (terrainByName == null)
        {
            GameObject goTerrain = GameObject.Find(terrainName);
			if (goTerrain)
            	terrainByName = goTerrain.GetComponent<Terrain>();
        }
		
		if (terrainByName != null)
		{
	        transform.position = new Vector3(transform.position.x,
	                                         terrainByName.terrainData.GetInterpolatedHeight((transform.position.x - terrainByName.transform.position.x) / terrainByName.terrainData.size.x,
	                                                                                    (transform.position.z - terrainByName.transform.position.z) / terrainByName.terrainData.size.z) + terrainByName.transform.position.y,
	                                         transform.position.z);
	
	        cachedPos = transform.position;
		}
    }

    /// <summary>
    ///  It's expensive to move a collider if it doesn't have a rigidbody attached...
    /// </summary>
    public void AddRigidbody()
    {
        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
            enabled = true;
        }
    }
}
