using UnityEngine;
using System.Collections;


public class HorseController : NPCController
{
    public bool canMount = false;
    public AnimationClip gallopAnimation;

    public float forwardGallopSpeed = 1;

    public AudioClip GallopAudio;
    public AudioClip TrotAudio;

    public float horseRiderHeight = 0.77f;

    public float turnSpeed = 3f;

    private GameObject playerGO = null;

    private bool canShowDismountButton = false;
    public bool CanShowDismountButton
    {
        get
        {
            return canShowDismountButton;
        }
        set
        {
            canShowDismountButton = value;
        }
    }


    private bool mounted = false;
    public bool Mounted
    {
        get
        {
            return mounted;
        }
        set
        {
            mounted = value;
        }
    }

    private Transform myRider;
    public Transform MyRider
    {
        get
        {
            return myRider;
        }
        set
        {
            myRider = value;
        }

    }

    void Awake()
    {
		if (!transform.Find("CameraTarget"))
		{
			GameObject go = new GameObject();
			go.name = "CameraTarget";
			go.transform.parent = transform;
			go.transform.localPosition = Vector3.zero + Vector3.up * 2;
			go.transform.localRotation = Quaternion.identity;
		}
        // Oleg: NPC reconstruction
		/*
        if (instantiateOnSceneStart)
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
            Instantiate();
            Actor.SetFastWalkAnimation(gallopAnimation);
        }
         */
        //if (transform.root.name == "Flashback Root")
          //  InFlashback = true;
    }

    ActorBase myRiderActor = null;

    void Update()
    {
        if (mounted)
        {
            if (!myRiderActor)
            {
                myRiderActor = ((ActorBase)myRider.GetComponent(typeof(ActorBase)));
            }

            if (Actor.anim.IsPlaying("fastwalk"))
            {
                Actor.PlaySound(GallopAudio);
                if (myRiderActor)
                {
                    myRiderActor.HorseForward(true);
                }
            }
            else if (Actor.anim.IsPlaying("walk"))
            {
                Actor.PlaySound(TrotAudio);

                if (myRiderActor)
                {
                    myRiderActor.HorseForward(false);
                }
            }
            else if(myRiderActor)
            {
                myRiderActor.HorseIdle();
            }
        }
    }

	public void DismountForDialog ()
	{
		Actor.CanMove = false;
		MovementController.Main.ClearWalkTarget();
        if (MovementController.ControlTarget != null)
        {
            MovementController.ControlTarget.Idle();
        }
							
		tag = "Untagged";
		MovementController oMovementController = MovementController.GetMovementController();
		oMovementController.Dismount(playerGO.transform, transform);

		Actor.Idle();

		GUIManager.PlayButtonSound();
	}
	
    public float maxMountDistance = 5;

    void OnGUI()
    {
        if (!playerGO)
        {
            playerGO = GameObject.FindGameObjectWithTag("Player");
        }

        GUI.skin = GUIManager.Skin;

        // Oleg: NPC reconstruction
        /*
        if (DebugLOD && !BehindCamera)
        {
            Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2));

            GUI.Label(new Rect(screenPos.x, Screen.height - screenPos.y, 100, 30), LOD.CurrentLOD.ToString());
        }
         */

        float dist = playerGO == null ? 999 : ((transform.position - transform.TransformDirection(-Vector3.left)) - playerGO.transform.position).magnitude;
        if (mounted)
            dist = 0;

        /*
        if (!BehindCamera && npcName != "" && dist < 10 && !InFlashback)
        {
            Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2));

            if (canMount && instantiateOnSceneStart)
            {
                if (dist < maxMountDistance || mounted)
                {
                    string tstr = string.Empty;

                    if (mounted && canShowDismountButton)
                    {
                        tstr = "Dismount " + npcName;
                    }
                    else if (!mounted)
                    {
                        tstr = "Mount " + npcName;
                    }

                    if (((mounted && canShowDismountButton) || !mounted) && !MapCamera.MapViewActive)
                    {
                        float w = GUI.skin.GetStyle(GUIManager.NPCNameButtonStyle).CalcSize(new GUIContent(tstr)).x;

                        if (GUI.Button(new Rect(screenPos.x, Screen.height - screenPos.y - talkLabelHeight, w, 50), tstr, GUIManager.NPCNameButtonStyle))
                        {
                            Actor.CanMove = false;
                            if (MovementController.ControlTarget != null)
                            {
                                MovementController.Main.ClearWalkTarget();
                                MovementController.ControlTarget.Idle();
                            }


							
                            if (mounted)
                            {
								tag = "Untagged";
                                MovementController oMovementController = MovementController.GetMovementController();
								oMovementController.Dismount(myRider, transform);
                            }
                            else
                            {
								tag = "Player";
								ignoreLOD = true;
								MovementController oMovementController = MovementController.GetMovementController();
								oMovementController.Mount(playerGO.transform.transform, transform, false);
                            }

                            Actor.Idle();

                            GUIManager.PlayButtonSound();
                        }
                    }
                }
            }
        }
         */
         
    }

}

