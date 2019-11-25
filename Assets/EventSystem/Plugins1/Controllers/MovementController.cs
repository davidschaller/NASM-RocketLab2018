//#define CharacterCustomizer

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MovementController : MonoBehaviour
{
	public Biped playerPrefab;
	public Animation playerCharacterPrefab;
	public AnimationClip walkAnimation;
    public Animation girlCharacterPrefab,
                     girlCharacterPrefabOnHorse; // hack or girl on hors animation

	public AnimationClip girlWalkAnimation;

    public AnimationClip dismountAnimationClip;
    public AnimationClip girlDismountAnimationClip;

    public AnimationClip horseIdleAnimationClip;
    public AnimationClip girlHorseIdleAnimationClip;

    public AnimationClip horseTrotAnimationClip;
    public AnimationClip girlHorseTrotAnimationClip;

    public AnimationClip horseGallopAnimationClip;
    public AnimationClip girlHorseGallopAnimationClip;
    public static GameObject prefab;

	public float playerScale = 1f;
	public float indoorWalkMultiplier = 0.5f;

    public bool debugMode = false;

    private float mouseDownTime,mouseUpTime;
    private ISceneActor controlTarget;
	public static ISceneActor ControlTarget
	{
		get
		{
			return main == null ? null : main.controlTarget;
		}
		set
		{
			main.controlTarget = value;
		}
	}

	static MovementController main;
	public bool fakeController = false;

    private HorseController rememberMyHorse;
    public HorseController RememberMyHorse
    {
        get
        {
            return rememberMyHorse;
        }
        private set
        {
        }
    }

    /*
     * Can't use SuspendPCControl function in banjo game, 
     * but we have to lock player's movement or at the end of the game he
     * can be somewhere outside of the building
     */

    private bool isInBanjoGame = false;  
    public bool IsInBanjoGame
    {
        get
        {
            return isInBanjoGame;
        }
        set
        {
            isInBanjoGame = value;
        }
    }

    Animation playerBody = null,
              girlPlayerBodyOnHorse = null;
    PlayerController controller = null;
    public PlayerController CharacterController
    {
        get
        {
            return controller;
        }
    }

	private void Awake ()
	{
		MTDebug.Construct ();

		if (!main)
			main = this;
	}

    private bool hasInstantiatePlayer = false;
    public bool HasInstantiatePlayer
    {
        get
        {
            return hasInstantiatePlayer;
        }
    }

    public void InstantiatePlayerTo(string playersPositions)
    {
        List<string> positions = null;

        if (!string.IsNullOrEmpty(playersPositions))
        {
            positions = new List<string>();
            positions.AddRange(playersPositions.Split('|'));
        }

        if (positions != null)
            SetPlayerControllerPos(positions);

        hasInstantiatePlayer = true;
        StartCoroutine(InstantiateCharacter());
    }

    private void SetPlayerControllerPos(List<string> positions)
    {
        string set = positions.Find(delegate(string s) { return s.Split(':')[0].Equals(CryptoGUI.PlayerName); });

        if (!string.IsNullOrEmpty(set))
        {
            Vector3 pos = RivalSynchronizer.StringToVector3(set.Split(':')[1]);

            float height = TerrainManager.GetInterpolatetHeight(pos);

            if (height <= pos.y && pos != Vector3.zero)
            {
                GameObject.Find("Player Controller").transform.position = pos;
            }
        }
    }

    Biped player;
	public bool isCustomizedCharacter = false;
	private IEnumerator InstantiateCharacter ()
	{
        controller = GetComponent(typeof(PlayerController)) as PlayerController;
        player = GameObject.Instantiate(playerPrefab, transform.position, transform.rotation) as Biped;
        player.turnSpeed = controller.turnSpeed;
	
		player.Name = controller.playerName;
		player.Gender = controller.Gender;
	
		player.tag = "Player";
        
        MapCameraController.AddMarker(player.transform, controller.marker);

		if (isCustomizedCharacter)
		{
			bool playerManagerIsReady = true;
#if Crypto
			playerManagerIsReady = CryptoPlayerManager.IsReady;
#endif
			
#if CharacterCustomizer
			while (!CharacterGenerator.ReadyToUse || (!playerManagerIsReady && !debugMode))
				yield return 0;
#endif
			
#if Crypto
            if (!string.IsNullOrEmpty(CryptoPlayerManager.Config))
            {
                Customizer.Main.IsConfiguredAlready = true;
                Customizer.Main.Generator = CharacterGenerator.CreateWithConfig(CryptoPlayerManager.Config);
            }
            else
#endif
            {
                Debug.Log("Craeting character");

#if CharacterCustomizer
                Customizer.Main.IsConfiguredAlready = false;
                Customizer.Main.Generator = CharacterGenerator.CreateWithRandomConfig("malebasic");
#endif
            }

#if CharacterCustomizer
            while (!Customizer.Main.Generator.ConfigReady)
                yield return 0;

            Customizer.Main.Parent = player.transform;
            Customizer.Main.Character = Customizer.Main.Generator.Generate();
            Customizer.Main.Character.transform.localPosition = Vector3.zero;
            Customizer.Main.Character.transform.localRotation = Quaternion.identity;
#endif
			
            player.anim = player.GetComponentInChildren<Animation>();
            playerCharacterPrefab = player.anim;
		}
		
        if (player.Gender == Gender.Female && girlCharacterPrefabOnHorse != null)
        {
			girlPlayerBodyOnHorse =
                (Animation)GameObject.Instantiate (girlCharacterPrefabOnHorse, transform.position, transform.rotation);
			girlPlayerBodyOnHorse.transform.localScale = new Vector3 (playerScale, playerScale, playerScale);
			girlPlayerBodyOnHorse.transform.parent = player.transform;


            girlPlayerBodyOnHorse.AddClip (girlHorseIdleAnimationClip, "horseIdle");
			girlPlayerBodyOnHorse.AddClip (girlHorseTrotAnimationClip, "horseTrot");
			girlPlayerBodyOnHorse.AddClip (girlHorseGallopAnimationClip, "horseGallop");

            player.SetAnimation (girlPlayerBodyOnHorse);
		
        }
		
		bool sameBody = false;
		if (player.Gender == Gender.Male)
        {
			if (playerPrefab.transform == playerCharacterPrefab.transform || isCustomizedCharacter)
			{
				if (!isCustomizedCharacter)
					player.anim = player.GetComponent<Animation>();
				playerBody = player.anim;
				playerBody.clip = player.anim.GetClip ("idle");
				sameBody = true;
			}
			else
			{
				playerBody = (Animation)GameObject.Instantiate (playerCharacterPrefab, transform.position + Vector3.up * 0.08f, transform.rotation);
			}
		}
        else
        {
			playerBody = (Animation)GameObject.Instantiate (girlCharacterPrefab, transform.position + Vector3.up * 0.08f, transform.rotation);
		}
		
		if (!sameBody)
		{
			playerBody.transform.localScale = new Vector3 (playerScale, playerScale, playerScale);
			playerBody.transform.parent = player.transform;
		}
		player.SetAnimation (playerBody);
		
		player.SetIdleAnimation (playerBody.clip);
		if (player.Gender == Gender.Male)
        {
			player.SetWalkAnimation (walkAnimation);

            player.SetDismountAnimation (dismountAnimationClip);
			player.SetHorseIdleAnimation (horseIdleAnimationClip);
			player.SetHorseGallopAnimation (horseGallopAnimationClip);
			player.SetHorseTrotAnimation (horseTrotAnimationClip);
		}
        else
        {
			player.SetWalkAnimation (girlWalkAnimation);
			player.SetDismountAnimation (girlDismountAnimationClip);
		}

        player.SetWoodWalkSound (controller.woodWalkSound);
		player.SetDirtWalkSound (controller.dirtWalkSound);

        yield return new WaitForSeconds(3);
		
#if Crypto
        if (!debugMode)
            TicTacManager.Main.enabled = true;
#endif
	}


    /// <summary>
    /// Get every animation from Animals root and restart it with random delay
    /// </summary>
    /// 
    private void DesyncAnimals()
    {
        Animation[] animList = (Animation[])GameObject.FindObjectsOfType(typeof(Animation));
        
        
        foreach (Animation item in animList)
        {

            if (item && item.transform && item.transform.parent)
            {
                if (item.transform.parent.name.Contains("Animal") ||
                        item.transform.parent.name.Contains("Cow") ||
                            item.transform.parent.name.Contains("Horse"))
                {
                    if (item && item.clip)
                    {
                        item.Stop();
                        StartCoroutine(RestartAnimal(item, UnityEngine.Random.Range(0f, 3.5f)));
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Usend by DesyncAnimals to de-synch the animal's default animations 
    /// </summary>
    /// <param name="p_anim"></param>
    /// <param name="p_time">delay</param>
    /// <returns></returns>
    private IEnumerator RestartAnimal(Animation p_anim, float p_time)
    {
        p_anim[p_anim.clip.name].normalizedTime = UnityEngine.Random.Range(0f, 1.0f);

        yield return new WaitForSeconds(p_time);
        p_anim.Play();
    }
	
	TerrainData terrainData;
	Terrain activeTerrain;
	private void Start ()
	{
        flame = GameObject.Find("Flame");
        if (Terrain.activeTerrain != null)
        {
            terrainData = Terrain.activeTerrain.terrainData;
            activeTerrain = Terrain.activeTerrain;
        }
		if (fakeController)
			return;
		
		MTDebug.AddButton("Man", Man);
		MTDebug.AddButton("Fly", Fly);
		MTDebug.AddButton("Find CTs", FindAllControlTargets);
		MTDebug.WatchVariable("Test expire", 30, 2.0f);

        DesyncAnimals();

        if (!pcCam)
            pcCam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));

        if (debugMode)
        {
            InstantiatePlayerTo(null);
        }
        else
            LockPCControl();
        mouseDownTime = 0;
	}

	public static MovementController Main
	{
		get 
		{
			return main;
		}
		private set {}
	}

	public static MovementController GetMovementController ()
	{
		return main;
	}

    /*
	bool clickToWalk = true;
	public static bool ClickToWalkEnabled
	{
		set
		{
			Main.clickToWalk = value;
			Debug.Log("Click to walk " + (value ? "enabled" : "disabled"));
		}
		get
		{
			return Main.clickToWalk;
		}
	}
     */

    bool isWritingLetter = false;
    public static bool IsWritingLetter
    {
        set
        {
            Main.isWritingLetter = value;
        }
        get
        {
            return Main.isWritingLetter;
        }
    }

	public void UnlockPCControl (bool breakLock)
	{
		if (breakLock)
			ControlLock = 0;
		else
			ControlLock--;
		SetPCControl();
	}
	
	public void SetPCControl ()
	{
		if (controlLock > 0) 
            return;
		
		Man("me");
		//ClickToWalkEnabled = true;
	}
	
	public bool PCControlled ()
	{
		if (controlTarget != null) return true;
		
		return false;
	}

	private int controlLock = 0;
	private int ControlLock
	{
		get
		{
			return controlLock;
		}
		set
		{
			controlLock = value < 0 ? 0 : value;
		}
	}

    public bool IsLocked
    {
        get
        {
            return controlLock > 0;
        }
    }

	public void LockPCControl ()
	{
		ControlLock++;
		SuspendPCControl();
	}
	
	private void SuspendPCControl ()
	{
		Debug.Log("SuspendPCControl at " + Time.time);
		if (fakeController)
			return;


        if (controlTarget != null)
        {
            controlTarget.Idle();
            controlTarget.ReleasePCControl();
        }
		controlTarget = null;
		//ClickToWalkEnabled = false;

		Camera.main.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);
	}

	float vertical = 0;
	float horizontal = 0;
	private static Vector3 clickTarget = Vector3.zero;

    // used to show on-click animation
    private static GameObject  flame;


    public static Vector3 ClickTarget
    {
        get
        {
            return clickTarget;
        }
        set
        {
            clickTarget = value;
            //if (clickTarget != Vector3.zero)
            {
                if (clickAnim == null)
                {
                    //clickAnim = (GameObject)Instantiate(prefab, clickTarget, Quaternion.identity);
                    //clickAnim.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    clickAnim = flame;
                }
                clickAnim.transform.localPosition = clickTarget;
                
            }
            /*else
            {
                if (clickAnim != null)
                {
                    GameObject.Destroy(clickAnim);
                }
                clickAnim = null;
            }*/
        }
    }
    private static GameObject clickAnim;

	public void ClearWalkTarget ()
	{
		ClickTarget = Vector3.zero;
	}
	
	Ray clickRay;
	public float maxClickDistance = 80;
	
	enum TargetOrientation {Aligned, Left, Right};
	enum FacingOrientation {Beside, Ahead, Behind};
	FacingOrientation facingOrientation;
	TargetOrientation targetOrientation;

    private PCCamera pcCam;

    private float oldTargetDistance = 0;
    private bool isNearWall = false; // need that for sneakwalls animation

    enum ClosestWall
    {
        Left = 0,
        Right,
        None, 
    }

	void Update ()
	{
        Debug.DrawRay(wallRayLeft.origin, wallRayLeft.direction, Color.green);
        Debug.DrawRay(wallRayRight.origin, wallRayRight.direction, Color.red);

        if (!pcCam)
			pcCam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));

		// Shouldn't move if player is in a banjo game
        if (fakeController || isInBanjoGame || isWritingLetter || mountingInProgress || !player || ControlLock > 0)
			return;

        if(Input.GetMouseButtonDown(0))
        {
       		mouseDownTime = Time.time;
        }

        // Disable the click-on-ground-to-walk for Texas  
		if (GUIManager.Project != GUIManager.Projects.Texas)
		{
			
			if (/*ClickToWalkEnabled && */!(Time.time - mouseDownTime > 1) && Input.GetMouseButtonUp(0))
			{
				mouseDownTime = 0;
				clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
                if (Physics.Raycast(clickRay.origin, clickRay.direction, out hit, maxClickDistance, 1 << LayerMask.NameToLayer("Default")))
				{
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Default") && hit.transform.gameObject.layer != LayerMask.NameToLayer("Terrain"))
                    {
                        ClickTarget = Vector3.zero;
                        return;
                    } 

                    bool isTerrain = false;
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                        isTerrain = true;

					ClickTarget = hit.point;
					/*
					if (terrainData == null) 
                        Debug.Log("NULL TERRAINDATA");
					if (activeTerrain == null) 
                        Debug.Log("NULL ACTIVE TERRAIN");
					*/

                    if (isTerrain)
                        ClickTarget = new Vector3(ClickTarget.x, terrainData.GetInterpolatedHeight(ClickTarget.x / terrainData.size.x, ClickTarget.z / terrainData.size.z) + activeTerrain.transform.position.y, ClickTarget.z);

                    //Debug.Log("ClickTarget (" + hit.collider.gameObject.name +", layer: " + hit.collider.gameObject.layer + "): " + ClickTarget + " (" + Input.mousePosition + ") at " + Time.time);
				}
				else
				{
					//Debug.Log("Click but no target, terrain layer " + Terrain.activeTerrain.gameObject.layer + " via layermask: " + LayerMask.NameToLayer("Terrain"));
					ClickTarget = Vector3.zero;
				}
			}
		}

		if (Input.GetKeyDown("c"))
		{
			pcCam.ToggleViewPoint();
		}

        if (controlTarget != null && controlTarget.GetTransform() != null && controlTarget.GetTransform().childCount > 0)
		{
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ClosestWall closest = GetClosestWall(controlTarget.GetTransform());

                switch (closest)
                {
                    case ClosestWall.Left:
                        RotateCharacterMesh(controlTarget.GetTransform().GetChild(0), new Vector3(0, 90, 0));
                        isNearWall = true;
                        break;
                    case ClosestWall.Right:
                        // I thin we should use different animation here but rotation should be (0, -90, 0)
                        RotateCharacterMesh(controlTarget.GetTransform().GetChild(0), new Vector3(0, 90, 0));
                        isNearWall = true;
                        break;
                    default:
                        RotateCharacterMesh(controlTarget.GetTransform().GetChild(0), new Vector3(0, 0, 0));
                        isNearWall = false;
                        break;
                }
            }
            else
            {
                RotateCharacterMesh(controlTarget.GetTransform().GetChild(0), Vector3.zero);
                isNearWall = false;
            }

            if (ClickTarget != Vector3.zero)
			{
                Debug.DrawRay(ClickTarget, Vector3.up, Color.green);
				Debug.DrawRay(clickRay.origin, clickRay.direction * maxClickDistance, Color.green);
				float targetDistance = (ClickTarget - controlTarget.GetTransform().position).magnitude;

                float forwardDot = Vector3.Dot(controlTarget.GetTransform().TransformDirection(Vector3.forward), ClickTarget - controlTarget.GetTransform().position);
                if (forwardDot > 0)
                {
                    facingOrientation = FacingOrientation.Ahead;
                }
                else if (forwardDot < 0)
                {
                    facingOrientation = FacingOrientation.Behind;
                }
                else
                {
                    facingOrientation = FacingOrientation.Beside;
                }

				Vector3 vecTo = clickTarget - controlTarget.GetTransform().position;

                float rightDot = Vector3.Dot(controlTarget.GetTransform().TransformDirection(Vector3.right).normalized, vecTo.normalized);

                if (rightDot > 0)
                {
                    targetOrientation = TargetOrientation.Right;
                }
                else if (rightDot < 0f)
                {
                    targetOrientation = TargetOrientation.Left;
                }
                else
                {
                    targetOrientation = TargetOrientation.Aligned;
                }

                /* Click-on-ground-to-walk shouldn't work in Texas
                 * 
                 */

                if (GUIManager.Project != GUIManager.Projects.Texas)
                {
                    if (targetOrientation != TargetOrientation.Aligned && targetDistance > 1)
                    {
                        //player.turnSpeed = controller.turnSpeed / targetDistance;
                        float maxAngle = Vector3.Angle(controlTarget.GetTransform().TransformDirection(Vector3.forward), vecTo);

                        maxAngle /= targetDistance;

                        if (targetOrientation == TargetOrientation.Right)
                        {
                            controlTarget.TurnRight(maxAngle, ClickTarget);
                        }
                        else
                        {
                            controlTarget.TurnLeft(maxAngle, ClickTarget);
                        }
                    }

                    if (targetDistance > 2)
                    {
                        if (facingOrientation == FacingOrientation.Ahead)
                        {
                            // If target is unreachable -> set idle
                            if (targetDistance != oldTargetDistance)
                            {
                                oldTargetDistance = targetDistance;
            					
                                if (Input.GetKey(KeyCode.LeftShift))
                                {
                                    controlTarget.MoveForward(controller.sneakSpeed, true, isNearWall);
                                }
                                else
                                {
                                	if(targetDistance > 5)
                                    	controlTarget.MoveForward(controller.fastspeed, false);
                                	else
                                    	controlTarget.MoveForward(controller.speed, false);
                                }
                            }
                            else 
                                controlTarget.Idle();
                        }
                    }
                }

                if (targetDistance <= 2 || targetOrientation == TargetOrientation.Aligned)
                {
                    ClickTarget = Vector3.zero;
                }
			}
			else
			{
				vertical = Input.GetAxis("Vertical");
				horizontal = Input.GetAxis("Horizontal");

				if (vertical == 0 && horizontal == 0)
				{
                    if (controlTarget.GetTransform().Find("Player(Clone)") != null)
                    {
                        // player horse idle animation
                        ((HorseController)controlTarget.GetTransform().GetComponent(typeof(HorseController))).CanShowDismountButton = true;
                    }

                    controlTarget.Idle();
				}
				else
				{
                    if (controlTarget.GetTransform().Find("Player(Clone)") != null)
                    {
                        // hide mount/dismount button
                        if (PlayerController.Horse)
                        {
                            PlayerController.Horse.CanShowDismountButton = false;
                        }
                    }

					if (controlTarget.ActorTypeCanFly() && (Input.GetKey("left shift") || Input.GetKey("right shift")))
					{
						if (vertical > 0)
						{
							controlTarget.MoveUp(vertical);
						}
						else if (vertical < 0)
						{
							controlTarget.MoveDown(vertical);
						}
					}
					else
					{
						if (pcCam && pcCam.Indoors) 
                            vertical *= indoorWalkMultiplier;
						
						if (vertical > 0)
						{
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                controlTarget.MoveForward(controller.sneakSpeed, true, isNearWall);
                            }
                            else
                                controlTarget.MoveForward(controller.speed, false);                            
						}
						else if (vertical < 0)
						{
                            if (controlTarget.GetTransform().Find("Player(Clone)") != null)
                            {
                                // horse can't move back
                            }
                            else
                            {
                                if (!controller)
                                    controller = (PlayerController)GetComponent(typeof(PlayerController));
                                
                                controlTarget.MoveBackward(controller.speed);
                            }
						}
					}

					if (controlTarget.ActorTypeCanStrafe() && (Input.GetKey("left alt") || Input.GetKey("right alt")))
					{
						if (horizontal > 0)
						{
							controlTarget.StrafeRight(horizontal);
						}
						if (horizontal < 0)
						{
							controlTarget.StrafeLeft(horizontal);
						}
					}
					else
					{
						if (horizontal > 0)
						{
                            if (vertical == 0)
                            {
                                if (controlTarget.GetTransform().Find("Player(Clone)") != null)
                                {
                                    controlTarget.Idle();
                                }
                            }

                            controlTarget.TurnRight(null, Vector3.zero);
						}
						if (horizontal < 0)
						{
                            if (vertical == 0)
                            {
                                if (controlTarget.GetTransform().Find("Player(Clone)") != null)
                                {
                                    controlTarget.Idle();
                                }
                            }

                            controlTarget.TurnLeft(null, Vector3.zero);                            
						}
					}
				}
			}
		}

		
		MTDebug.WatchVariable("Control: {0}", String.Format("V/H: {0:0.#}/{1:0.#}\nCanFly: {2}\nCanStrafe: {3}", 
						vertical, horizontal, 
						controlTarget != null ? controlTarget.ActorTypeCanFly() : false,
						controlTarget != null ? controlTarget.ActorTypeCanStrafe() : false
						));
	}

    private void RotateCharacterMesh(Transform character, Vector3 vector3)
    {
        character.localEulerAngles = vector3;
    }

    Ray wallRayLeft, wallRayRight;
    private ClosestWall GetClosestWall(Transform me)
    {
        wallRayLeft = new Ray(me.position + Vector3.up, me.TransformDirection(Vector3.left));
        wallRayRight = new Ray(me.position + Vector3.up, me.TransformDirection(Vector3.right));
        
	    RaycastHit[] hits = new RaycastHit[2];

        Physics.Raycast(wallRayLeft, out hits[0], 2);

        Physics.Raycast(wallRayRight, out hits[1], 2);


        float minDistance = 9999;
        int minIndex = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform != null)
            {
                float distance = (me.position - hits[i].point).magnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
                    minIndex = i;
                }
            }
        }

        if (minIndex >= 0)
        {
            return (ClosestWall)minIndex;
        }
        else
            return ClosestWall.None;
    }

    /*
    private void MoveForward()
    {
        // means control horse
        if (controlTarget.GetTransform().FindChild("Player(Clone)") != null)
        {
            if ((Input.GetKey("left shift") || Input.GetKey("right shift")))
            {
                controlTarget.MoveForward(PlayerController.Horse.forwardGallopSpeed, true);
            }
            else
            {
                controlTarget.MoveForward(PlayerController.Horse.forwardMoveSpeed, false);
            }
        }
        else
        {
            if (!controller)
                controller = (PlayerController)GetComponent(typeof(PlayerController));

            if ((Input.GetKey("left shift") || Input.GetKey("right shift")))
            {
                controlTarget.MoveForward(controller.fastForwardMoveSpeed, true);
            }
            else
                controlTarget.MoveForward(controller.forwardMoveSpeed, false);
        }
    }
     */

    public void Mount(Transform p_Rider, Transform p_Horse, bool p_skipMountingAnimation)
    {
        ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).ConstrainToGround = false;
        ((HorseController)p_Horse.GetComponent(typeof(HorseController))).Mounted = true;
        ((HorseController)p_Horse.GetComponent(typeof(HorseController))).MyRider = p_Rider;

        p_Rider.rotation = p_Horse.rotation;
		if (p_Rider.tag == "Player")
		{
			PlayerController.Horse = p_Horse.GetComponent<HorseController>();
            rememberMyHorse = PlayerController.Horse;
		}
		
        if (p_skipMountingAnimation)
        {
            p_Rider.position = p_Horse.position + Vector3.up * ((HorseController)p_Horse.GetComponent(typeof(HorseController))).horseRiderHeight;
            p_Rider.rotation = p_Horse.rotation;

            p_Rider.parent = p_Horse;

            if (p_Rider.tag == "Player")
            {
                p_Rider.gameObject.SetActiveRecursively(false);
                ControlTarget = (ISceneActor)p_Horse.GetComponent(typeof(ActorBase));
            }
            ((ActorBase)p_Horse.GetComponent(typeof(ActorBase))).CanMove = true;

            if (p_Rider.Find("Girl_Horserider(Clone)") && p_Rider.tag == "Player")
            {
                ((Biped)p_Rider.GetComponent(typeof(Biped))).SetAnimation(girlPlayerBodyOnHorse);
                p_Rider.Find("Girl_Horserider(Clone)").gameObject.SetActiveRecursively(true);
            }

        }
        else
        {
            StartCoroutine(Mounting(p_Rider, p_Horse));
        }
    }

    public void Dismount(Transform p_Rider, Transform p_Horse)
    {
		if (p_Rider.tag == "Player")
		{
			PlayerController.Horse = null;
            ControlTarget = (ISceneActor)p_Rider.GetComponent(typeof(ActorBase));
		}
        p_Rider.parent = null;

        ((HorseController)p_Horse.GetComponent(typeof(HorseController))).Mounted = false;
        ((HorseController)p_Horse.GetComponent(typeof(HorseController))).MyRider = null;

        StartCoroutine(Dismounting(p_Rider, p_Horse));
    }

    bool mountingInProgress = false;

    private IEnumerator Mounting(Transform p_Rider, Transform p_Horse)
    {
        float t = 0;
        while (t < 1.70f)
        {
            mountingInProgress = true;
            ((HorseController)p_Horse.GetComponent(typeof(HorseController))).CanShowDismountButton = false;

            if (t == 0f)
            {
                p_Rider.position = p_Horse.position;
            }

            if (t < 1.50f)
            {
                ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).Mount();
            }

            if (p_Rider.position.y < (p_Horse.position + Vector3.up * 0.90f).y)
            {
                ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).MoveUp(t * 2.5f);
            }

            t += Time.deltaTime;
            yield return new WaitForSeconds(0f);
        }

        p_Rider.position = p_Horse.position + Vector3.up * ((HorseController)p_Horse.GetComponent(typeof(HorseController))).horseRiderHeight;
        p_Rider.rotation = p_Horse.rotation;

        if (p_Rider.Find("Girl_Horserider(Clone)") && p_Rider.tag == "Player")
        {
            ((Biped)p_Rider.GetComponent(typeof(Biped))).SetAnimation(girlPlayerBodyOnHorse);
            p_Rider.Find("Girl_Horserider(Clone)").gameObject.SetActiveRecursively(true);
        }

        p_Rider.parent = p_Horse;

        if (p_Rider.tag == "Player")
        {
            p_Rider.gameObject.SetActiveRecursively(false);
            ControlTarget = (ISceneActor)p_Horse.GetComponent(typeof(ActorBase));
        }
        ((ActorBase)p_Horse.GetComponent(typeof(ActorBase))).CanMove = true;
        mountingInProgress = false;
    }

    private IEnumerator Dismounting(Transform p_Rider, Transform p_Horse)
    {
        if (p_Rider.tag == "Player")
        {
            if (p_Rider.Find("Girl_Horserider(Clone)"))
            {
                ((Biped)p_Rider.GetComponent(typeof(Biped))).SetAnimation(playerBody);
                p_Rider.Find("Girl-Player(Clone)").gameObject.SetActiveRecursively(true);
            }

            //SuspendPCControl();
        }

        if (p_Rider.tag == "Player")
        {
            p_Rider.gameObject.SetActiveRecursively(true);
            UnlockPCControl(true);
        }

        float normalizeTime = 0;

        ActorBase riderActor = (ActorBase)p_Rider.GetComponent(typeof(ActorBase));

        Vector3 oldRidersPosition = p_Rider.position;

        while (normalizeTime < 0.95f)
        {
            //normalizeTime = riderActor.Dismount();

            p_Rider.position = Vector3.Lerp(oldRidersPosition, p_Horse.position, normalizeTime > 0.35f ? normalizeTime : 0);
            yield return 0;
        }


        p_Rider.position = p_Horse.position + p_Horse.TransformDirection(new Vector3(-0.6f, 0, 0));

        if (p_Rider.Find("Girl_Horserider(Clone)"))
        {
            while (p_Rider.Find("Girl-Player(Clone)").Find("Joint_group").localPosition != new Vector3(0, -0.2f, -0.08f))
            {
                p_Rider.Find("Girl-Player(Clone)").Find("Joint_group").localPosition = new Vector3(0, -0.2f, -0.08f);
                yield return 0;
            }
        }
      


        riderActor.ConstrainToGround = true;
        
        

        /*
        if (p_Rider.FindChild("Girl_Horserider(Clone)") && p_Rider.tag == "Player")
        {
            ((Biped)p_Rider.GetComponent(typeof(Biped))).SetAnimation(playerBody);
            p_Rider.FindChild("Girl-Player(Clone)").gameObject.SetActiveRecursively(true);
        }

        if (p_Rider.tag == "Player")
        {
            p_Rider.active = true;
            UnlockPCControl(true);
        }

        float t = 0;
        while (t < 1.70f)
        {
            ((HorseController)p_Horse.GetComponent(typeof(HorseController))).CanShowDismountButton = false;

            if (t < 1.50f)
            {
                ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).Dismount();
                if (p_Rider.FindChild("Girl-Player(Clone)") != null)
                {
                    girlsDismountPosition = p_Rider.FindChild("Girl-Player(Clone)").FindChild("Joint_group").position;
                }
            }
            else
            {
                if (p_Rider.FindChild("Girl-Player(Clone)") != null)
                {
                    if (p_Rider.FindChild("Girl-Player(Clone)").FindChild("Joint_group") != null)
                    {
                        if (p_Rider.FindChild("Girl-Player(Clone)").FindChild("Joint_group").position == p_Rider.position)
                        {
                            p_Rider.FindChild("Girl-Player(Clone)").FindChild("Joint_group").position = p_Rider.position;
                            t = 10;
                            p_Rider.position = girlsDismountPosition;
                        }
                        else
                        {
                            p_Rider.FindChild("Girl-Player(Clone)").FindChild("Joint_group").position = p_Rider.position;
                        }
                        
                        yield return new WaitForSeconds(0f);
                    }
                }
                else
                {
                    ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).StrafeLeft(3.8f);
                }
            }
            
            if (t > 0.5f && p_Rider.position.y > p_Horse.position.y)
            {
                ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).MoveDown(t * 1.3f);
            }
             

            t += Time.deltaTime;
            
            yield return new WaitForSeconds(0f);
        }

        // Player was inactive and some platformtriggers couldn't be added/removed 
        //((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).platformTriggers =
            //((ActorBase)p_Horse.GetComponent(typeof(ActorBase))).platformTriggers;

        ((ActorBase)p_Rider.GetComponent(typeof(ActorBase))).ConstrainToGround = true;
         */
    }
	
	private void Man (string buttonName)
	{
		if (controlTarget != null) 
            controlTarget.ReleasePCControl();

		StartCoroutine(ActorUtility.WaitForPlayerAndSetControl());
	}

	void Fly (string buttonName)
	{
		if (controlTarget != null) controlTarget.ReleasePCControl();
		controlTarget = (ISceneActor)GameObject.FindObjectOfType(typeof(CameraController));
		PCCamera pc = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
		pc.enabled = false;
		Camera.main.transform.localEulerAngles = new Vector3(0, Camera.main.transform.localEulerAngles.y, 0);
	}
	
	void FindAllControlTargets (string bname)
	{
		ActorBase[] actors = (ActorBase[])GameObject.FindObjectsOfType(typeof(ActorBase));
		string actorList = "";
		foreach (ActorBase actor in actors)
		{
			actorList += "\t"+actor.gameObject.name+"\n";
		}
		MTDebug.WatchVariable("Actors: \n{0}", actorList, 5);
		
	}
	
	void OnGUI ()
	{
		if (controlTarget == null)
		{
			GUI.Box( new Rect(Screen.width-150-10, 0, 150, 40), "NO Control Target");
		}
	}

    /*
    private Vector3 savedPosition = Vector3.zero;
    internal void SavePosition()
    {
        if (ControlTarget != null)
        {
            savedPosition = ControlTarget.GetTransform().position;
        }
    }
	*/
}

