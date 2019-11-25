using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorBase : MonoBehaviour, ISceneActor
{
    private bool constrainToGround = true;
    public bool ConstrainToGround
    {
        get
        {
            return constrainToGround;
        }
        set
        {
            constrainToGround = value;
        }
    }

	Gender gender;
	public Gender Gender
	{
		get
		{
			return gender;
		}
		set
		{
			gender = value;
		}
	}

	string actorName = "Jim";
	public string Name
	{
		get
		{
			return actorName;
		}
		set
		{
			actorName = value;
		}
	}

	bool canMove = true;
	public bool CanMove
	{
		get
		{
			return canMove;
		}
		set
		{
			canMove = value;
		}
	}

	bool canTurn = true;
	public bool CanTurn
	{
		get
		{
			return canTurn;
		}
		set
		{
			canTurn = value;
		}
	}

	bool isSeated = false;
	public bool IsSeated
	{
		get
		{
			return isSeated;
		}
		set
		{
			isSeated = value;
			if (isSeated)
			{
				CanMove = false;
				CanTurn = false;
			}
			else
			{
				CanMove = true;
				CanTurn = true;
			}
		}
	}

	void Fail( string msg )
	{
		Debug.LogWarning(msg, gameObject);
		gameObject.active = false;
	}

	Transform GetBack ()
	{
		Transform back = anim.transform.Find("hips/spine_1/spine_2");
		if (back == null)
			back = anim.transform.Find("Joint_group/hips/spine_1/spine_2");
		if (back == null)
		{
			Fail("Failed to find back transform (incompatible model hierarchy?), disabling AnimationCombine.  Attempted: hips/spine_1/spine_2 & Joint_group/hips/spine_1/spine_2");
			return null;
		} 

		return back;
	}

	void IdleRevert ()
	{
		Idle();
	}
	
	public void PlaySeatedAnimationOnce(AnimationClip clip)
	{
		CancelInvoke("IdleRevert");
		if (anim[clip.name] == null)
			anim.AddClip(clip, clip.name);
		anim[clip.name].wrapMode = WrapMode.Once;
		Invoke("IdleRevert", clip.length);

		anim[clip.name].AddMixingTransform(GetBack());

        anim[clip.name].normalizedTime = 0;
        if (this.gameObject.active)
        {
            StartCoroutine(PlayAnimationProcess(clip.name));
        }
	}

    /// <summary>
    /// Play current clip to the end, skips overriding
    /// </summary>
    /// <param name="p_clipName">Clip name to play</param>
    public IEnumerator PlayAnimationProcess(string p_clipName)
    {
        while (anim[p_clipName].normalizedTime < 1)
        {
            anim.CrossFade(p_clipName);
            yield return new WaitForSeconds(0);
        }
    }

	public void PlaySeatedLoopingAnimation( AnimationClip clip )
	{
		CancelInvoke("IdleRevert");
		if (anim[clip.name] == null)
			anim.AddClip(clip, clip.name);
		anim[clip.name].wrapMode = WrapMode.Loop;

		anim[clip.name].AddMixingTransform(GetBack());
		anim.CrossFade(clip.name);
	}
		
	AnimationClip seatedIdleAnimation;
	public AnimationClip SeatedIdleAnimation
	{
		get
		{
			return seatedIdleAnimation;
		}
		set
		{
			seatedIdleAnimation = value;
			if (seatedIdleAnimation != null)
			{				
				anim.AddClip(idleAnimation, "sit"); // alias for
													// "idle"
				if (anim["sit"] == null) Debug.LogError("Idle animation is null for " + name);

				anim["sit"].wrapMode = WrapMode.Loop;

				anim.AddClip(seatedIdleAnimation, "upperBody");
				anim["upperBody"].wrapMode = WrapMode.Loop;
				anim.clip = seatedIdleAnimation;
			}

			//if (1==1)
			{
				Transform back = anim.transform.Find("hips/spine_1/spine_2");
				if (back == null)
					back = anim.transform.Find("Joint_group/hips/spine_1/spine_2");
				if (back == null)
				{
					Fail("Failed to find back transform (incompatible model hierarchy?), disabling AnimationCombine.  Attempted: hips/spine_1/spine_2 & Joint_group/hips/spine_1/spine_2");
					return;
				}

                if (anim["upperBody"])
                {
                    anim["upperBody"].AddMixingTransform(back);

                    anim.CrossFade("upperBody");
                }
			}
			/*else
			{
				if (Gender == Gender.Male)
				{
					Transform waist = anim.transform.Find("hips/spine_1");
					Transform hipsL = anim.transform.Find("hips/leftUpLeg");
					Transform hipsR = anim.transform.Find("hips/rightUpLeg");

					if (hipsL == null) Fail("Null hipsL");
				
					anim["sit"].AddMixingTransform(hipsL, true);
					anim["sit"].AddMixingTransform(hipsR, true);
					anim["sit"].AddMixingTransform(waist);

					anim.CrossFade("sit");
				}
				else
				{
					Transform back = transform.Find("Joint_group/hips/spine_1/spine_2");
					if (back == null)
					{
						Fail("Failed to find back transform (incompatible model hierarchy?), disabling AnimationCombine.  Attempted Joint_group/hips/spine_1/spine_2");
						return;
					}
				
					Transform waist = transform.Find("Joint_group/hips/spine_1");
					Transform hipsL = transform.Find("Joint_group/hips/leftUpLeg");
					Transform hipsR = transform.Find("Joint_group/hips/rightUpLeg");

					anim["upperBody"].AddMixingTransform(back, true);

					anim["sit"].AddMixingTransform(hipsL, true);
					anim["sit"].AddMixingTransform(hipsR, true);
					anim["sit"].AddMixingTransform(waist);

					anim.CrossFade("upperBody");
				}
				}*/
		}
	}
	
	public enum MoveState {Idle, Forward, Back, TurnRight, TurnLeft, StrafeRight, StrafeLeft, Mount, Dismount};
	protected MoveState moveState;
	public float playerScale = 1f;
    protected AnimationClip walkAnimation,
                            fastWalkAnimation;

    // rider's animations
    protected AnimationClip dismountAnimation,
                            horseIdleAnimation,
                            horseGallopAnimation,
                            horseTrotAnimation;

	protected AnimationClip idleAnimation;

    protected AudioClip woodWalkSound;
    protected AudioClip dirtWalkSound;

	public float forwardMoveSpeed = 10;
	public float backwardMoveSpeed = 10;
	public float turnSpeed = 30;
	public float strafeSpeed = 5;
	public float verticalSpeed = 10;

    public Animation anim,
                     lowAnim;

	protected CharacterController controller;

	public void SetAnimation (Animation a)
	{
        if (anim && a != anim)
        {
            anim.gameObject.SetActiveRecursively(false);
        }
		anim = a;
	}
	
	void ActorException (string msg)
	{
		throw new System.Exception("Exception for " + name + ": " + msg);
	}

    public void SetAnimationClip(AnimationClip clip, string name, WrapMode mode)
    {
        if (anim[name] == null)
        {
            anim.AddClip(clip, name);
            anim[name].wrapMode = mode;
        }
    }
	
	public void SetWalkAnimation (AnimationClip w)
	{
		if (w == null) 
		{
			//ActorException("Walk animation is null");
		}
		else
		{
			walkAnimation = w;
			anim.AddClip(w, "walk");
			anim["walk"].wrapMode = WrapMode.Loop;
		}
	}

    public void SetFastWalkAnimation(AnimationClip w)
    {
        if (w == null)
        {
            ActorException("Fast Walk animation is null");
        }
        else
        {
            fastWalkAnimation = w;
            anim.AddClip(w, "fastwalk");
            anim["fastwalk"].wrapMode = WrapMode.Loop;
        }
    }

    public void SetDismountAnimation(AnimationClip w)
    {
        if (w == null)
        {
            //ActorException("Walk animation is null");
        }
        else
        {
            dismountAnimation = w;
            anim.AddClip(w, "dismount");
            anim["dismount"].wrapMode = WrapMode.Once;
        }
    }

    public void SetHorseIdleAnimation(AnimationClip w)
    {
        if (w == null)
        {
            //ActorException("Walk animation is null");
        }
        else
        {
            horseIdleAnimation = w;
            anim.AddClip(w, "horseIdle");
            anim["horseIdle"].wrapMode = WrapMode.Loop;
        }
    }

    public void SetHorseGallopAnimation(AnimationClip w)
    {
        if (w == null)
        {
            //ActorException("Walk animation is null");
        }
        else
        {
            horseGallopAnimation = w;
            anim.AddClip(w, "horseGallop");
            anim["horseGallop"].wrapMode = WrapMode.Loop;
        }
    }

    internal void SetHorseTrotAnimation(AnimationClip w)
    {
        if (w == null)
        {
            //ActorException("Walk animation is null");
        }
        else
        {
            horseTrotAnimation = w;
            anim.AddClip(w, "horseTrot");
            anim["horseTrot"].wrapMode = WrapMode.Loop;
        }
    }

    public void SetDirtWalkSound(AudioClip w)
    {
        if (w == null)
        {
            //ActorException("Walk sound is null");
        }
        else
        {
            dirtWalkSound = w;
        }
    }

    public void SetWoodWalkSound(AudioClip w)
    {
        if (w == null)
        {
            ActorException("Walk sound is null");
        }
        else
        {
            woodWalkSound = w;
        }
    }
	
	public void SetIdleAnimation(AnimationClip i)
	{
		if (i == null) 
		{
			ActorException("Idle animation is null");
		}
		else
		{
			idleAnimation = i;
			if (anim == null) ActorException("anim is null");
			anim.AddClip(i, "idle");
			anim["idle"].wrapMode = WrapMode.Loop;
		}
	}

    private PlayerController oPlayerController;

	/* ISceneActor required methods */

    public void Mount()
    {
        if (!CanMove) return;

        moveState = MoveState.Mount;
            
        if (anim["dismount"])
        {
            if (anim["dismount"].normalizedTime <= 0)
            {
                anim["dismount"].normalizedTime = 1;
            }

            anim.CrossFade("dismount", 0.0f);
            anim["dismount"].speed = -1f;
        }
    }

    public void Dismount()
    {
        if (!CanMove) return;

        moveState = MoveState.Dismount;
        if (anim["dismount"])
        {
            if (anim["dismount"].normalizedTime <= 0)
            {
                anim["dismount"].normalizedTime = 0;
            }

            anim.CrossFade("dismount", 0.0f);
            anim["dismount"].speed = 1;
        }
    }

    public void HorseIdle()
    {
        if (anim && anim["horseIdle"])
        {
            if (anim["horseIdle"])
            {
                anim.CrossFade("horseIdle");
            }
        }
    }

    public void HorseForward(bool fast)
    {
        if (fast)
        {
            if (anim && anim["horseGallop"])
            {
                if (anim["horseGallop"].normalizedTime <= 0)
                {
                    anim["horseGallop"].normalizedTime = 0;
                }

                if (anim["horseGallop"])
                {
                    anim.CrossFade("horseGallop", 0.1f);
                    anim["horseGallop"].speed = 1;
                }
            }
        }
        else
        {
            if (anim && anim["horseTrot"])
            {
                if (anim["horseTrot"].normalizedTime <= 0)
                {
                    anim["horseTrot"].normalizedTime = 0;
                }

                if (anim["horseTrot"])
                {
                    anim.CrossFade("horseTrot", 0.1f);
                    anim["horseTrot"].speed = 1;
                }
            }
        }
    }

    int lastFrame = 0;
    public void MoveForward(float speed, bool fast)
    {
        MoveForward(speed, speed, fast);
    }
	
	public void MoveForward(float speed,float AnimSpeed, bool fast)
	{
        if (!ConstrainToGround)
            return;

        if (lastFrame == Time.frameCount)
            return;
		
        lastFrame = Time.frameCount;

		if (!CanMove)
		{
			return;
		}
        
        if (tag == "Player")
        {
            if (!oPlayerController && ProjectManager.project != Projects.FortMcHenry)
                oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
			
            if (woodWalkSound != null && oPlayerController && oPlayerController.IsInsideBuilding)
                StartCoroutine(PlayWalkSound(woodWalkSound));
            else if (dirtWalkSound != null && oPlayerController && !oPlayerController.IsInsideBuilding)
                StartCoroutine(PlayWalkSound(dirtWalkSound));
        }

		moveState = MoveState.Forward;

        if (anim)
        {
            if (fast)
            {
                if (anim["fast walk"])
                {
                    anim.CrossFade("fast walk", 0);
                    anim["fast walk"].speed = AnimSpeed;
                }
            }
            else
            {
                if (anim["walk"])
                {
                    anim.CrossFade("walk", 0);
                    anim["walk"].speed = AnimSpeed;
                }

                if (lowAnim && lowAnim["lowLODwalk"])
                {
                    lowAnim.CrossFade("lowLODwalk");
                }
            }
        }

        if (controller != null)
        {
            controller.Move(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime);
        }
        else
            transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
	}

    public void MoveForward(float speed, bool fast, bool nearWall)
    {
        if (!ConstrainToGround)
            return;

        if (lastFrame == Time.frameCount)
            return;

        lastFrame = Time.frameCount;

        if (!CanMove) 
            return;

        if (tag == "Player")
        {
            if (!oPlayerController && ProjectManager.project != Projects.FortMcHenry)
                oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));

            if (woodWalkSound != null && oPlayerController && oPlayerController.IsInsideBuilding)
                StartCoroutine(PlayWalkSound(woodWalkSound));
            else if (dirtWalkSound != null && oPlayerController && !oPlayerController.IsInsideBuilding)
                StartCoroutine(PlayWalkSound(dirtWalkSound));
        }

        moveState = MoveState.Forward;

        if (anim)
        {
            if (fast)
            {
                if (nearWall)
                {
                    if (anim["wallsneak"])
                    {
                        anim.CrossFade("wallsneak", 0);
                        anim["wallsneak"].speed = speed;
                    }
                }
                else
                {
                    if (anim["sneakwalk"])
                    {
                        anim.CrossFade("sneakwalk", 0);
                        anim["sneakwalk"].speed = speed;
                    }
                }
            }
            else
            {
                if (anim["walk"])
                {
                    anim.CrossFade("walk", 0);
                    anim["walk"].speed = speed;
                }
            }
        }

        if (controller != null)
            controller.Move(transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime);
        else
            transform.position += transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
    }

    private AudioClip oldSound;
    public void PlaySound(AudioClip sound)
    {
        if (sound != null)
        {
            if (!gameObject.GetComponent<AudioSource>())
                Common.AddAudioTo(gameObject);

            if (!GetComponent<AudioSource>().isPlaying || (GetComponent<AudioSource>().isPlaying && sound != oldSound))
            {
                oldSound = sound;
                GetComponent<AudioSource>().clip = sound;
                GetComponent<AudioSource>().loop = false;
                GetComponent<AudioSource>().Play();
            }
        }
    }

    private IEnumerator PlayWalkSound(AudioClip sound)
    {
        if (!gameObject.GetComponent<AudioSource>())
            Common.AddAudioTo(gameObject);

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = sound;
            GetComponent<AudioSource>().Play();
        }        

        yield return 0;
    }
	
	public void MoveBackward(float speed)
	{
		MoveBackward(speed,speed*-1);
	}
	
	public void MoveBackward(float speed, float AnimSpeed)
	{
        if (!ConstrainToGround)
            return;

		if (!CanMove) return;

        if (tag == "Player")
        {
            if (!oPlayerController && ProjectManager.project != Projects.FortMcHenry)
            {
                oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
            }

            if (woodWalkSound != null && oPlayerController && oPlayerController.IsInsideBuilding)
            {
                StartCoroutine(PlayWalkSound(woodWalkSound));
            }
            else if (dirtWalkSound != null && oPlayerController && !oPlayerController.IsInsideBuilding)
            {
                StartCoroutine(PlayWalkSound(dirtWalkSound));
            }
        }

		moveState = MoveState.Back;
		anim.CrossFade("walk", 0.1f);
		anim["walk"].speed = AnimSpeed;
		
        
		if (controller != null)
		{
            controller.Move(transform.TransformDirection(Vector3.back) * speed * Time.deltaTime);
		}
		else
		{
			transform.position -= transform.TransformDirection(Vector3.forward) * speed * Time.deltaTime;
		}
	}

    public virtual void TurnRight(float? maxAngle, Vector3 target)
	{
		if (!ConstrainToGround)
            return;

		if (!CanTurn) return;
		
		float angleToRotate = this.turnSpeed * Time.deltaTime;

        if (maxAngle.HasValue)
        {
            if (angleToRotate > maxAngle.Value)
            {
                transform.LookAt(target);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
                return;
            }
        }

        transform.Rotate(Vector3.up, angleToRotate);

		if (anim &&  anim["turn right"])
		{
			anim["turn right"].wrapMode = WrapMode.Loop;
			anim.CrossFade("turn right");
		}
	}

    public virtual void TurnLeft(float? maxAngle, Vector3 target)
	{
		if (!ConstrainToGround)
            return;

		if (!CanTurn) return;


		float angleToRotate = this.turnSpeed * Time.deltaTime;

        if (maxAngle.HasValue)
        {
            if (angleToRotate > maxAngle.Value)
            {
                transform.LookAt(target);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
                return;
            }
        }

		transform.Rotate(Vector3.up, -angleToRotate);

		if (anim && anim["turn left"])
		{
			anim["turn left"].wrapMode = WrapMode.Loop;
			anim.CrossFade("turn left");
		}
	}
	
	public void StrafeLeft(float speed)
	{
		if (!CanMove) return;

        transform.position += transform.TransformDirection(-Vector3.right) * speed * Time.deltaTime;
	}
	
	public void StrafeRight(float speed)
	{
		if (!CanMove) return;

        transform.position += transform.TransformDirection(Vector3.right) * speed * Time.deltaTime;
	}
	
	public void MoveUp(float speed)
	{
        transform.position += Vector3.up * speed * Time.deltaTime;
	}
	
	public void MoveDown(float speed)
	{
        transform.position -= Vector3.up * speed * Time.deltaTime;
	}
	
	public bool ActorTypeCanStrafe()
	{
		return false;
	}
	
	public bool ActorTypeCanTurn()
	{
		return true;
	}
	
	public bool ActorTypeCanFly()
	{
		return false;
	}
	
	public bool IAmTarget (PCCamera cam)
	{
		if (cam == null) return false;
		Transform targ = cam.Target;
		if (targ == transform || (targ != null && targ.parent == transform))
		{
			return true;
		}
		
		return false;
	}

	public Transform lodBuildingRoot;
	public List<PlatformTrigger> platformTriggers = new List<PlatformTrigger>();
	public void OnTriggerEnter (Collider coll)
	{
		PlatformTrigger pt = (PlatformTrigger)coll.GetComponent(typeof(PlatformTrigger));
		if (pt)
		{
			Debug.Log("Adding platform trigger " + pt.name + " to actor " + name, pt.gameObject);
			platformTriggers.Add(pt);
			if (tag != "Player")
			{
				lodBuildingRoot = (pt.transform.parent != null && pt.transform.parent.parent != null) ? pt.transform.parent.parent : pt.transform.parent;
			}
			
			if (pt.switchesPCToFirstPerson)
			{
				PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
				
				if (IAmTarget(cam))
				{
					cam.Indoors = true;
					cam.FirstPersonView();
				}
			}
		}

		if (tag == "Player")
		{
            if (!oPlayerController && ProjectManager.project == Projects.FortMcHenry)
            {
                oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
            }

			EventPlayerBase ep = (EventPlayerBase)coll.GetComponent(typeof(EventPlayerBase));
            if (ep)
            {
                print("Got EventPlayer trigger");
                ep.PlayerTriggered();

                IdleRevert();

                if (oPlayerController != null)
                {
                    oPlayerController.IsInsideBuilding = true;
                }
            }
            else
            {
                if (oPlayerController != null)
                {
                    oPlayerController.IsInsideTrigger = true;
                }
            }
		}
	}
	
	
	protected PlatformTrigger platformTrigger;

    /*
    public void OnTriggerStay(Collider coll)
    {
        if (tag == "Player")
        {
            platformTrigger = (PlatformTrigger)coll.GetComponent(typeof(PlatformTrigger));
            if (platformTrigger != null && platformTrigger.switchesPCToFirstPerson)
            {
                PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
                if (IAmTarget(cam))
                {
                    cam.Indoors = true;
                    cam.FirstPersonView();
                }
            }
        }
    }
     */

    public void OnTriggerExit ( Collider coll )
	{
		lodBuildingRoot = null;
		
		PlatformTrigger pt = (PlatformTrigger)coll.GetComponent(typeof(PlatformTrigger));

        if (tag == "Player")
        {
            if (!oPlayerController && ProjectManager.project != Projects.FortMcHenry)
            {
                oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
            }

            if (oPlayerController != null)
            {
                oPlayerController.IsInsideBuilding = false;
                oPlayerController.IsInsideTrigger = false;
            }
        }

		if (coll.GetComponent(typeof(PlatformTrigger)))
		{			
			Debug.Log("Removing platform trigger " + pt.gameObject.name + " from actor " + name, pt.gameObject);
			
			platformTriggers.Remove(pt);
			
			if (platformTriggers.Count == 0)
			{
				PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
				if (IAmTarget(cam))
				{
					cam.Indoors = false;
					cam.ThirdPersonView();
				}
			}
		}
	}
	
	public void ClaimPCControl ()
	{
		pcControlled = true;
	}
	
	public void ReleasePCControl ()
	{
		pcControlled = false;
	}
	
	bool pcControlled = false;
	public bool IsPCControlled ()
	{
		return pcControlled;
	}

	public void Idle()
	{
        if (ConstrainToGround && anim && !anim.IsPlaying("idle"))
        {
            PlayAnimationLooping("idle");
        }
        /*
		if (IsSeated)
		{
			anim["upperBody"].AddMixingTransform(GetBack());
			anim.CrossFade("upperBody");
		}
		else
        {
            if (transform.parent != null && transform.parent.GetComponent(typeof(HorseController)) != null)
            {
                if (anim["horseIdle"])
                {
                    anim.CrossFade("horseIdle");
                }
            }
            else
            {
                if (anim && anim["idle"]) // anim may not be assigned if this character is being downloaded
                {
                    anim.CrossFade("idle");
                }
            }
        }
         */
	}
	
	public Transform GetTransform ()
	{
        // The object can be destroyed already
        if (this)
        {
            return transform;
        }
        else
            return null;
	}


    public void PlayAnimationLooping(AnimationClip clip)
    {
        if (anim[clip.name] == null)
            anim.AddClip(clip, clip.name);
        anim[clip.name].wrapMode = WrapMode.Loop;
        anim.CrossFade(clip.name);
    }

    public void PlayAnimationLooping(string clipName)
    {
        if (anim == null)
        {
            return;
        }

        if (anim[clipName] == null)
        {
            return;
        }
        
        if (!anim.IsPlaying(clipName))
        {
            anim[clipName].wrapMode = WrapMode.Loop;
            anim.CrossFade(clipName, .5f);
        }

        if (clipName == "idle" && lowAnim && lowAnim[clipName])
        {
            lowAnim.CrossFade(clipName, .5f);
        }
    }

    public void PlayAnimationOnce(string clipName)
    {
        if (!anim)
            return;

        if (!anim[clipName])
            return;

        CancelInvoke("IdleRevert");
        anim[clipName].wrapMode = WrapMode.Once;
        Invoke("IdleRevert", anim[clipName].length);

        anim.CrossFade(clipName, .5f);
    }

    public void Greeting(AnimationClip greetingAnimation)
    {
        CanMove = false;

        if (greetingAnimation)
            PlayAnimationOnce(greetingAnimation.name);
    }

    public void StopTalking()
    {
        CanMove = true;
    }

    public void SetLowAnimation(Animation animSource)
    {
        lowAnim = animSource;
    }
}
