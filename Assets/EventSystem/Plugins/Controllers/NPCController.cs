#define NGUI

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCController : MonoBehaviour
{
    public enum NPCType { Biped, Quadruped, Vehicle, Bust };
    public string npcName;
    public bool canRotateForSalute = false;
    public bool rotateBackAfterSalute = false;

#if EZGUI
    public UIButton talkToButtonPrefab;
#endif
	
#if NGUI
	public GameObject talkToButtonPrefab;
#endif

    public EventPlayer talkEventPlayer;

    public int turnSpeed = 30;
    public Gender gender;
    public Vector3 scale = new Vector3(1, 1, 1);
    public bool npcIsSeated = false;
    public AnimationClip seatedIdleAnimation;
    public bool canTalk = false,
                canRepeatTalk = false;
    public AudioClip standardGreeting,
                     defaultSound; // Need this in scene 5 for Mr. Weedon with Axe

    public float timeToPlayDefaultSound = 0; // default animation normalized time [0..1] 

    public GameObject npcPrefab;
    public AnimationClip idleAnimation;
    public AnimationClip walkAnimation;
    public AnimationClip greetingAnimation;
    public bool instantiateOnSceneStart = false;
    public NPCType npcType = NPCType.Biped;
    public float forwardMoveSpeed = 1;
    public float backwardMoveSpeed = 1;

    public float[] minMaxDesync = new float[2] { 0, 0.9f };

    public string greetingText = "Hello";

    public bool skipRotationAfterDialog = false;


    public AnimationClip dismountAnimationClip;
    public AnimationClip girlDismountAnimationClip;

    public AnimationClip horseIdleAnimationClip;
    public AnimationClip girlHorseIdleAnimationClip;

    public AnimationClip horseTrotAnimationClip;
    public AnimationClip girlHorseTrotAnimationClip;

    public AnimationClip horseGallopAnimationClip;
    public AnimationClip girlHorseGallopAnimationClip;

    public bool enableLOD = false;
    public Texture2D marker;

    public Animation LowAnim { get; private set; }

    public bool saluteUponApproach = false;

    public float saluteOnApproachBonus = 5;

    public bool setupAudioSource = false,
                addRigidBody = false;



    public Animation Npc
    {
        get
        {
            return actor != null ? actor.anim : null;
        }
        private set
        {
            actor.anim = value;
        }
    }

    LOD lod = null;
    public LOD LOD
    {
        get
        {
            return lod;
        }
    }

    private bool inFlashback = false;
    protected bool InFlashback
    {
        get
        {
            return inFlashback;
        }
        set
        {
            inFlashback = value;
        }
    }
    
    private NPCController myWagon = null;
    float lastTimeSalute = 0;

    GameObject talkToButton;

    public bool IsTalking { get; set; }

    void Awake()
    {
        if (instantiateOnSceneStart)
        {
            StartCoroutine(Init());
        }

        if (transform.root.name == "Flashback Root")
            inFlashback = true;

        // Need to fine all vecniles for oxes and horses

        if (transform.parent != null)
        {
            NPCController[] npcList = (NPCController[])GameObject.FindObjectsOfType(typeof(NPCController));
            if (npcType == NPCType.Quadruped)
            {
                foreach (NPCController item in npcList)
                {
                    if (item.transform.IsChildOf(transform.parent) && item.npcType == NPCType.Vehicle)
                    {
                        myWagon = item;
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
        if (canTalk && talkToButtonPrefab)
        {
            StartCoroutine(InitTalkToButton());
        }
        else
            enabled = false;
    }

    IEnumerator InitTalkToButton()
    {
		#if EZGUI
        talkToButton = (UIButton)Instantiate(talkToButtonPrefab);

        talkToButton.transform.parent = transform;
        talkToButton.transform.localRotation = Quaternion.identity;
        talkToButton.transform.localPosition = new Vector3(0, 2.5f, 0);

        talkToButton.text = "Talk to " + npcName;
        while (talkToButton.RenderCamera != Camera.mainCamera)
        {
            talkToButton.SetCamera(Camera.mainCamera);
            yield return 0;
        }
        SpriteText spriteText = talkToButton.GetComponentInChildren<SpriteText>();

        while (spriteText == null)
        {
            spriteText = talkToButton.GetComponentInChildren<SpriteText>();

			Debug.Log("Yielding... at " + Time.time);
            yield return 0;
        }

        while (spriteText.RenderCamera != Camera.mainCamera)
        {
            spriteText.SetCamera(Camera.mainCamera);
            yield return 0;
        }

        talkToButton.scriptWithMethodToInvoke = this;
        talkToButton.methodToInvoke = "OnTalkToClick";
        talkToButton.gameObject.SetActiveRecursively(false);

		#elif NGUI // NGUI implementation
		
		yield return 5;
		
		while (NPCNameButton.main == null)
		{
			NPCNameButton.FindMain();
			yield return 0;
		}
		NPCNameButton talkToButton = NPCNameButton.CreateInstance(this);		
		#endif
    }


    Vector3 lookAtPos;
    public void OnTalkToClick()
    {
        lookAtPos = Camera.main.transform.position;

        Actor.Greeting(greetingAnimation);

        IsTalking = true;
        //talkToButton.gameObject.SetActiveRecursively(false);
        enabled = false;

		if (talkEventPlayer != null)
			talkEventPlayer.PlayerTriggered();

        LODController lodController = transform.GetComponentInChildren<LODController>();
        if (lodController)
        {
            lodController.FakeUpdate(1);
        }

        if (canRepeatTalk)
            StartCoroutine(WaitForTalkFinish());
    }

	public static bool waitingForNPCDialogButton = false;
    IEnumerator WaitForTalkFinish()
    {
		if (talkEventPlayer != null)
		{
	        while (!talkEventPlayer.CompletelyFinished)
	            yield return 0.2;
		}
		else
		{
	        while (!waitingForNPCDialogButton)
	            yield return 0.2;
		}

        enabled = true;

        Actor.StopTalking();
    }

    /// <summary>
    /// This is called when an event (or other) is going to use
    ///	the NPC, so it allows the NPC a chance to instantiate, 
    ///	set up persistent state, etc.
    /// </summary>
    /// 
    public void ActivateNPC()
    {
        if (!Npc && npcPrefab)
        {
            gameObject.SetActiveRecursively(true);
            StartCoroutine(Init());
        }
    }


    ActorBase actor = null;
    public ActorBase Actor
    {
        get
        {
            return actor;
        }
        protected set
        {
            actor = value;
        }
    }

    GameObject goInstantiated;

    IEnumerator Init()
    {
        switch (npcType)
        {
            case NPCType.Biped:
                actor = (ActorBase)gameObject.AddComponent(typeof(Biped));
                if (addRigidBody)
                {
                    ((Biped)actor).AddRigidbody();
                }
                break;
            case NPCType.Quadruped:
                actor = (ActorBase)gameObject.AddComponent(typeof(Quadruped));
                break;
            case NPCType.Vehicle:
                actor = (ActorBase)gameObject.AddComponent(typeof(Vehicle));
                break;
            case NPCType.Bust:
                actor = (ActorBase)gameObject.AddComponent(typeof(Bust));
                break;
        }

        if (!GetComponent<AudioSource>() && setupAudioSource)
            Common.AddAudioTo(gameObject);

        if (npcPrefab)
        {
            goInstantiated = (GameObject)Instantiate(npcPrefab, transform.position, transform.rotation);
            yield return goInstantiated;

            goInstantiated.transform.localScale = scale;
            goInstantiated.transform.parent = transform;
            goInstantiated.transform.localPosition = Vector3.zero;
            goInstantiated.transform.localRotation = Quaternion.identity;

            LODController lodController = goInstantiated.GetComponent<LODController>();
            if (lodController)
            {
                if (addRigidBody)
                    lodController.CanMove();
            }

            while (!Npc)
            {
                Npc = GetAnimation();

                if (!Npc)
                    Debug.Log("animation is missing", transform);

                yield return 0;
            }

            actor.Name = npcName;
            actor.Gender = gender;

            if (Npc != null)
            {
                actor.SetAnimation(Npc);
                if (Npc.clip != null)
                    actor.SetIdleAnimation(Npc.clip);

                if (LowAnim)
                    actor.SetLowAnimation(LowAnim);
            }

            if (npcIsSeated)
            {
                actor.IsSeated = true;
                actor.SeatedIdleAnimation = seatedIdleAnimation;
            }
            else
            {
                if (idleAnimation)
                    actor.SetIdleAnimation(idleAnimation);

                actor.SetWalkAnimation(walkAnimation);
                actor.forwardMoveSpeed = forwardMoveSpeed;   // Used for Wheels speed rotation
                actor.backwardMoveSpeed = backwardMoveSpeed; // We don't ever use this actually
                if (Npc.clip != null && Npc[Npc.clip.name] == null)
                {
                    Npc.AddClip(Npc.clip, Npc.clip.name);
                }

                if (greetingAnimation)
                    actor.SetAnimationClip(greetingAnimation, greetingAnimation.name, WrapMode.Once);

                IdleRevert();

                if (actor.Gender == Gender.Male)
                {
                    actor.SetDismountAnimation(dismountAnimationClip);
                    actor.SetHorseIdleAnimation(horseIdleAnimationClip);
                    actor.SetHorseGallopAnimation(horseGallopAnimationClip);
                    actor.SetHorseTrotAnimation(horseTrotAnimationClip);
                }
                else
                {
                    actor.SetDismountAnimation(girlDismountAnimationClip);
                    actor.SetHorseIdleAnimation(girlHorseIdleAnimationClip);
                    actor.SetHorseGallopAnimation(girlHorseGallopAnimationClip);
                    actor.SetHorseTrotAnimation(girlHorseTrotAnimationClip);
                }
            }

            if (enableLOD)
            {
                lod = new LOD(gameObject);
                InvokeRepeating("CheckLOD", 0, 0.15f);
            }
        }
    }

    public Animation GetAnimation()
    {
        if (Npc != null)
            return Npc;

        Animation result = null;

        if (goInstantiated && goInstantiated.transform.childCount > 0)
        {
            if (goInstantiated.transform.GetComponent<Animation>())
                return goInstantiated.transform.GetComponent<Animation>();

            foreach (Transform tr in goInstantiated.transform)
                if (tr.gameObject.active && tr.GetComponent<Animation>())
                {
                    if (tr.name == "Mesh" || tr.name.ToLower().Contains("lod"))
                        LowAnim = tr.GetComponent<Animation>();
                    else
                        result = tr.GetComponent<Animation>();
                }
                else
                {
                    foreach (Transform lodChild in tr)
                    {
                        if (lodChild.name == "Mesh" || lodChild.name.ToLower().Contains("lod"))
                        {
                            LowAnim = lodChild.GetComponent<Animation>();
                        }
                        else
                        {
                            Animation anim = lodChild.GetComponent<Animation>();
                            if (anim)
                                result = anim;
                        }
                    }
                }
        }

        if (!result)
        {
            if (LowAnim)
                return LowAnim;
        }

        return result;
    }

    public void IdleRevert()
    {
        //checkAnimations--;
        //Debug.Log("IdleRevert at " + Time.time);

        if (minMaxDesync.Length == 2)
        {
            float animDelay = Random.Range(minMaxDesync[0], minMaxDesync[1]);

            if (Npc.clip != null)
                Npc[Npc.clip.name].normalizedTime = animDelay;
        }

        actor.Idle();
    }

    public void RevertClip(string clipName)
    {
        if (minMaxDesync.Length == 2)
        {
            float animDelay = Random.Range(minMaxDesync[0], minMaxDesync[1]);

			if (Npc[clipName] != null)
				Npc[clipName].normalizedTime = animDelay;
			else
				Debug.LogWarning("Missing animation '" + clipName + "' in RevertClip", gameObject);
        }

        actor.PlayAnimationLooping(clipName);
    }
    
    /*
    public void PlayAnimationOnce(AnimationClip clip)
    {
        if (actor == null)
        {
            Debug.Log("Actor is null");
            return;
        }

        if (actor.IsSeated)
        {
            actor.PlaySeatedAnimationOnce(clip);
        }
        else
        {
            checkAnimations++;
            CancelInvoke("IdleRevert");
            if (Npc[clip.name] == null)
                Npc.AddClip(clip, clip.name);
            Npc[clip.name].wrapMode = WrapMode.Once;
            if (Application.isEditor) Debug.Log("NPC will revert to idle in " + clip.length + " starting from " + Time.time);
            Invoke("IdleRevert", clip.length);
            
            Npc.animation[clip.name].normalizedTime = 0;
            if (this.gameObject.active)
            {
                StartCoroutine(actor.PlayAnimationProcess(clip.name));
            }
        }
    }
*/
    public void PlayAnimationLooping(AnimationClip clip)
    {
        if (actor != null && actor.IsSeated)
        {
            actor.PlaySeatedLoopingAnimation(clip);
        }
        else
        {
            CancelInvoke("IdleRevert");
            if (Npc[clip.name] == null)
                Npc.AddClip(clip, clip.name);

            Npc[clip.name].wrapMode = WrapMode.Loop;
            Npc.CrossFade(clip.name);

        }
    }

    void Update ()
    {
        if (Time.deltaTime == 0)
            return;

        if (!Camera.main || !PCCamera.Main || !McHenryCommander.Main)
            return;

        if (canTalk)// && talkEventPlayer)
        {
            float dist = (transform.position - PCCamera.Main.Target.transform.position).magnitude;
            
            if (dist <= maxTalkDistance)
            {
                if (!autoDialogStart || IsTalking)
                {
					#if !NGUI
                    if (talkToButton)
                    {
                        if (!talkToButton.gameObject.active)
                        {
                            talkToButton.gameObject.SetActiveRecursively(true);
                        }
                        else
                        {
                            talkToButton.transform.rotation = Quaternion.LookRotation(
                                Vector3.Cross(Camera.mainCamera.transform.TransformDirection(Vector3.right),
                                Camera.mainCamera.transform.TransformDirection(Vector3.up)));
                        }
                    }
                    else
                        enabled = false;
					#endif
                }
                else if (!IsTalking)
                {
                    OnTalkToClick();
                }
            }
            else
            {
                if (!autoDialogStart)
                {
					#if !NGUI
                    if (talkToButton)
                    {
                        if (talkToButton.gameObject.active)
                            talkToButton.gameObject.SetActiveRecursively(false);
                    }
                    else
                        enabled = false;
					#endif
                }
            }
        }

        if (saluteUponApproach)
        {
            float distToBody = (transform.position - McHenryCommander.Main.transform.position).magnitude;

            if (distToBody <= saluteRadius)
            {
                if (!isSaluting)
                {
                    isSaluting = true;

                    if (lastTimeSalute == 0 || (Time.time - lastTimeSalute) > (saluteDelaySecs))
                    {
                        MakeSureSaluteExisting();

                        if (!effectPrefab)
                        {
                            effectPrefab = (FortitudeEffect)Resources.Load("Fortitude Effect", typeof(FortitudeEffect));
                        }

                        FortitudeEffect effect = (FortitudeEffect)Instantiate(effectPrefab, transform.position/* + Vector3.up * 2*/, Quaternion.identity);
                        effect.SetFortitudeAmount(saluteOnApproachBonus);

                        McHenryCommander.Main.SaluteBack += SimpleSalute;

                        Actor.PlayAnimationOnce("salute");
                        lastTimeSalute = Time.time;
                    }
                }
            }
            else
                isSaluting = false;
        }
    }

    bool isSaluting = false;

    public float saluteDelaySecs = 30,
                 saluteRadius = 5;

    static FortitudeEffect effectPrefab;

    public void MakeSureSaluteExisting()
    {
        if (Npc["salute"] == null)
        {
            if (McHenryCommander.Main.npcSaluteClip)
            {
                Npc.AddClip(McHenryCommander.Main.npcSaluteClip, "salute");
            }
            else
                Debug.LogError("Mc Henry Salute clip is missing. Can't add this clip to the officer");
        }
    }

    public void SubscribeToSaluteBack()
    {
        McHenryCommander.Main.SaluteBack += CommanderSaluteBack;
    }
    
    void CommanderSaluteBack()
    {
        bool isManagementMode = (ManagementBar.Main != null && ManagementBar.Main.enabled);

        McHenryCommander.Main.SaluteBack = null;

        if (McHenryCommander.Main)
        {
            SimpleSalute();
        }
    }

    public void SimpleSalute()
    {
        if (!effectPrefab)
            effectPrefab = (FortitudeEffect)Resources.Load("Fortitude Effect", typeof(FortitudeEffect));

        FortitudeEffect effect = (FortitudeEffect)Instantiate(effectPrefab, transform.position + Vector3.up, Quaternion.identity);
        effect.SetFortitudeAmount(saluteOnApproachBonus);
    }

    public Vector2 ScreenPosition
    {
        get
        {
            Debug.DrawRay(transform.position + Vector3.up*1, Vector3.right, Color.blue);
			
            return GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up*1));
        }
        private set
        {
        }
    }

    void SwitchPCToFirstPerson ()
    {
        PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
        cam.enabled = true;
        cam.FirstPersonView();
        MovementController.Main.ClearWalkTarget();
    }

    public void DialogFinished ()
    {
        if (actor == null) return;
		
        actor.Idle();
        if (!actor.IsSeated)
            actor.CanMove = true;
    }
	
    private bool debugLOD = false;

    protected bool DebugLOD
    {
        get
        {
            return debugLOD;
        }
        set
        {
            debugLOD = value;
        }
    }

    public bool autoDialogStart = false;

    public float maxTalkDistance = 5;
    public float maxNameDistance = 5;
    public float talkLabelHeight = 0;

    private Quaternion oldRotation;
    
    //void OnGUI ()
    //{
    // Shouldn't show any buttons/labels if extraStuff is not
    // active AND we're part of extra stuff hierarchy
    /*
    if (extraStuff && !extraStuff.active && transform.root.name == "Extra Stuff")
        return;
            

    GUI.skin = GUIManager.Skin;
		
    if (debugLOD && !behindCamera)
    {
        Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up*2));
			
        GUI.Label(new Rect(screenPos.x, Screen.height-screenPos.y, 100, 30), lod.CurrentLOD.ToString());
    }
    
		
    float dist = (transform.position - Camera.main.transform.position).magnitude;
    if (!behindCamera && npcName != "" && dist < 10 && !inFlashback)
    {
        Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up*2));

        if (dist < maxNameDistance && instantiateOnSceneStart)
        {
            GUIStyle talkTo = new GUIStyle("NPCLabel");
            Vector2 talkToSize = talkTo.CalcSize(new GUIContent(npcName));

            GUI.Label(new Rect(screenPos.x - talkToSize.x / 2, Screen.height - screenPos.y - talkLabelHeight, talkToSize.x, 30), npcName, talkTo);
        }

        if (canTalk && dist < maxTalkDistance && talkEventPlayer != null)
        {
            string tstr = "Talk to " + npcName;
            if (PlayerController.Mounted)
                tstr = "Dismount to talk";
            float w = GUI.skin.GetStyle(GUIManager.NPCNameButtonStyle).CalcSize(new GUIContent(tstr)).x;
				
            if (GUI.Button(new Rect(screenPos.x - w / 2, Screen.height-screenPos.y - talkLabelHeight, w, 50), tstr, GUIManager.NPCNameButtonStyle))
            {
                if (PlayerController.Mounted)
                {
                    PlayerController.Horse.DismountForDialog();
                }
                else
                {
                    actor.CanMove = false;
                    MovementController.Main.ClearWalkTarget();
                    if (MovementController.ControlTarget != null)
                    {
                        MovementController.ControlTarget.Idle();
                    }

                    Invoke("SwitchPCToFirstPerson", 0.1f);

                    oldRotation = transform.rotation;
                        

                    if (talkEventPlayer != null) talkEventPlayer.PlayerTriggered();
                    DialogRenderer.DialogNPC = this;

                    actor.Idle();

                    if (greetingAnimation)
                    {
                        PlayAnimationOnce(greetingAnimation);
                    }

                    if (audio && standardGreeting)
                    {
                        audio.PlayOneShot(standardGreeting);
                    }
                    
                    GUIManager.PlayButtonSound();
                }
            }
        }
    }
     */
    //}
    /*
    public void RestoreCameraRotation()
    {
        // Don't restore old rotation during banjo game or skip selected
        if (transform.root.name.Contains("banjo") || skipRotationAfterDialog)
            return;
        
        transform.rotation = oldRotation;
    }

    public void RestoreCameraRotation(Quaternion p_oldRotation)
    {
        // Don't restore old rotation during banjo game or skip selected
        if (!transform.root.name.Contains("banjo") || skipRotationAfterDialog)
            return;

        transform.rotation = p_oldRotation;
    }
     */

    private bool behindCamera = false;

    protected bool BehindCamera
    {
        get
        {
            return behindCamera;
        }
        set
        {
            behindCamera = value;
        }
    }

    void CheckLOD()
    {
        if (!PCCamera.Main && !UIController.Main)
        {
            return;
        }

        float cameraDistance = 1000;
		float lodMaxDistance = 50;
		float behindLodMaxDistance = 5;
		float inBuildingLODDistance = 5;
		float deactivateMaxDistance = 100;
		
		if (PCCamera.Main)
		{
			lodMaxDistance = PCCamera.Main.lodMaxDistance;
			behindLodMaxDistance = PCCamera.Main.behindLodMaxDistance;
			inBuildingLODDistance = PCCamera.Main.inBuildingLODDistance;
			deactivateMaxDistance = PCCamera.Main.deactivateMaxDistance;
		}
		else if (UIController.Main)
		{
			lodMaxDistance = UIController.Main.lodMaxDistance;
			behindLodMaxDistance = UIController.Main.behindLodMaxDistance;
			inBuildingLODDistance = UIController.Main.inBuildingLODDistance;
			deactivateMaxDistance = UIController.Main.deactivateMaxDistance;
		}

        if (Camera.main == null)
        {
            behindCamera = true;
        }
        else
        {
            behindCamera = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), transform.position - Camera.main.transform.position) < 0.40f;
            cameraDistance = (transform.position - Camera.main.transform.position).magnitude;
        }

        if (cameraDistance > lodMaxDistance ||
            (behindCamera && cameraDistance > behindLodMaxDistance) ||
            (actor.lodBuildingRoot && tag != "Player" && cameraDistance > inBuildingLODDistance))
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().clip = null;
            }

            if (cameraDistance > deactivateMaxDistance)
            {
                lod.UpdateLOD(LODState.Deactivate, behindCamera);
            }
            else
            {
                lod.UpdateLOD(LODState.Hidden, behindCamera);
            }
        }
        else
        {
            lod.UpdateLOD(LODState.Visible, behindCamera);
        }

        if (npcType == NPCType.Quadruped)
        {
            if (myWagon != null && myWagon.LOD != null)
            {
                lod.UpdateLOD(myWagon.LOD.CurrentLOD, behindCamera);
            }
        }
    }

    private bool isLocked = false;
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }

    public void Lock(string p_reason)
    {
        if (isLocked)
        {
            Debug.Log("NPC is loked already");
        }
        else
        {
            isLocked = true;
            Debug.Log("NPC is locked due to " + p_reason);
        }
    }

    public void Unlock(string p_reason)
    {
        if (!isLocked)
        {
            Debug.Log("NPC is unloked already");
        }
        else
        {
            isLocked = false;
            Debug.Log("NPC is unlocked due to " + p_reason);
        }
    }

    public void AnimateWithSound(string animationClipName, AudioClip audioClip)
    {
        Actor.PlayAnimationOnce(animationClipName);

        if (audioClip)
		{
			if (!GetComponent<AudioSource>().enabled)
				GetComponent<AudioSource>().enabled = true;
					
            GetComponent<AudioSource>().PlayOneShot(audioClip);
		}
    }

    public bool Killed { get; set; }

    public void YouKilled()
    {
        Killed = true;
    }
}
