using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DogController : MonoBehaviour
{
    public enum NPCType { Biped, Quadruped, Vehicle };

    public string npcName;
    public Gender gender;
    public Vector3 scale = new Vector3(1, 1, 1);
    public bool npcIsSeated = false;
    public AnimationClip seatedIdleAnimation;
    public bool canTalk = false;
    public AudioClip barkAudio;
    
    public EventPlayer talkEventPlayer;
    public Animation npcPrefab;
    public AnimationClip walkAnimation;
    public AnimationClip barkAnimation;
    public bool instantiateOnSceneStart = false;
    public NPCType npcType = NPCType.Biped;
    public float forwardMoveSpeed = 1;

    Animation npc;
    LOD lod = null;
    public LOD LOD
    {
        get
        {
            return lod;
        }
        private set
        {
        }
    }


    //bool inFlashback = false;

    void Awake()
    {
		GetComponent<Collider>().isTrigger = true;
		
        if (instantiateOnSceneStart)
        {
            Instantiate();
        }
        //if (transform.root.name == "Flashback Root")
           // inFlashback = true;

        if (Buildings == null)
        {
            Buildings = GameObject.FindGameObjectsWithTag("Buildings");
        }

        if (obstacles == null)
            obstacles = new List<Transform>();
    }

    /*
        This is called when an event (or other) is going to use
        the NPC, so it allows the NPC a chance to instantiate, 
        set up persistent state, etc.
    */
    public void ActivateNPC()
    {
        if (npc == null)
            Instantiate();
    }

    ActorBase actor = null;
    public ActorBase Actor
    {
        get
        {
            return actor;
        }
    }

    public void Instantiate()
    {
        //print("Instantiating NPC: " + name);
        if (npcPrefab == null)
        {
            if (Application.isEditor)
                Debug.LogWarning("Disabling this NPC.  This npc has no prefab associated with it (click this log entry to highlight the NPC in hieararchy).", gameObject);
            enabled = false;
            return;
        }

        Common.AddAudioTo(gameObject);
        
        npc = (Animation)GameObject.Instantiate(npcPrefab, transform.position, transform.rotation);
        npc.transform.localScale = scale;
        npc.transform.parent = transform;
        npc.transform.localPosition = Vector3.zero;
        npc.transform.localRotation = Quaternion.identity;
        switch (npcType)
        {
            case NPCType.Biped:
                actor = (ActorBase)gameObject.AddComponent(typeof(Biped));
                break;
            case NPCType.Quadruped:
                actor = (ActorBase)gameObject.AddComponent(typeof(Quadruped));
                break;
            case NPCType.Vehicle:
                actor = (ActorBase)gameObject.AddComponent(typeof(Vehicle));
                break;
        }
        actor.Name = npcName;
        actor.Gender = gender;
        actor.SetAnimation(npc);
        actor.SetIdleAnimation(npc.clip);
        if (npcIsSeated)
        {
            actor.IsSeated = true;
            actor.SeatedIdleAnimation = seatedIdleAnimation;
        }
        else
        {
            actor.SetWalkAnimation(walkAnimation);
            actor.forwardMoveSpeed = forwardMoveSpeed;
            if (npc.clip != null && npc[npc.clip.name] == null)
            {
                npc.AddClip(npc.clip, npc.clip.name);
            }
            npc.Play();
        }

        lod = new LOD(gameObject);
        //InvokeRepeating("CheckLOD", 0, 0.15f);
        if (!gameObject.GetComponent<Rigidbody>())
            gameObject.AddComponent(typeof(Rigidbody));
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;

    }

    public Animation GetAnimation()
    {
        return npc;
    }

    public void IdleRevert()
    {
        checkAnimations--;
        Debug.Log("IdleRevert at " + Time.time);
        actor.Idle();
    }

    public void PlayAnimationOnce(AnimationClip clip)
    {
        if (actor == null) return;

        if (actor.IsSeated)
        {
            actor.PlaySeatedAnimationOnce(clip);
        }
        else
        {
            checkAnimations++;
            CancelInvoke("IdleRevert");
            if (npc[clip.name] == null)
                npc.AddClip(clip, clip.name);
            npc[clip.name].wrapMode = WrapMode.Once;
            if (Application.isEditor) Debug.Log("NPC will revert to idle in " + clip.length + " starting from " + Time.time);
            Invoke("IdleRevert", clip.length);
            npc.CrossFade(clip.name);
        }
    }

    public void PlayAnimationLooping(AnimationClip clip)
    {
        if (actor.IsSeated)
        {
            actor.PlaySeatedLoopingAnimation(clip);
        }
        else
        {
            checkAnimations++;
            CancelInvoke("IdleRevert");
            if (npc[clip.name] == null)
                npc.AddClip(clip, clip.name);
            npc[clip.name].wrapMode = WrapMode.Loop;
            npc[clip.name].speed = speed * animationMultiplier;
            npc.CrossFade(clip.name);
        }
    }

    public void OnLODHide()
    {
        if (Application.isEditor)
        {
            GUIManager.DebugText(this, "");
        }
    }

    int checkAnimations = 0;
    //string checkText = "";

    public Collider dogCollider;

    public float maxDistanceToPlayer = 6f,
                 minDistanceToPlayer = 5f,
                 minDistanceToCollider = 4f;

    public float speed = 0.8f;
    public float animationMultiplier = 1.5f;

    private float distanceToPlayer = 0f,
                  distanceToTarget = 0f;

    public float minDistanceToTarget = 40f;

    private GameObject[] Buildings = null;
    private PlayerController oPlayerController = null;
    private GameObject playerGO = null;

    public float idleTime = 0f;
    public float timeTurnToPlayer = 6f,
                 timeToBark = 12f,
                 timeToLead = 18f,
                 timeTurnToTarget = 2f;

    public float turnSpeed = 300f;

    private bool between = true;

    private float waitOutsideDelay = 0f; // When Player comes out of the trigger, whait a couple seconds before start to move

    void Update()
    {
        if (!actor)
            return;

        if (playerGO == null)
        {
            playerGO = GameObject.FindWithTag("Player");
        }
        if (oPlayerController == null)
        {
            oPlayerController = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
        }

        if (oPlayerController.IsInsideBuilding || oPlayerController.IsInsideTrigger)
        {
            actor.Idle();
            waitOutsideDelay = 0.3f;
        }
        else
        {
            if (waitOutsideDelay > 0)
            {
                waitOutsideDelay -= Time.deltaTime;
                return;
            }

            distanceToPlayer = (transform.position - playerGO.transform.position).magnitude;
            if (GetTargetPosition() != null)
            {
                distanceToTarget = (transform.position - GetTargetPosition().position).magnitude;
            }

            if (!between && GetTargetPosition() != null)
            {
                Vector3 norm = Vector3.Normalize(GetTargetPosition().position - playerGO.transform.position);
                Vector3 target = playerGO.transform.position + norm * 5;
                target.y = playerGO.transform.position.y;

                if ((transform.position - target).magnitude > 1f)
                {
                    FollowTarget(target);
                }
                else
                {
                    if (Rotate(GetTargetPosition().position, 0, turnSpeed) > 4f)
                    {
                        PlayAnimationLooping(walkAnimation);
                    }
                    else
                    {
                        between = true;
                        actor.Idle();
                    }
                }
            }
            else if (distanceToPlayer > maxDistanceToPlayer && distanceToTarget > minDistanceToTarget)
            {
                idleTime = 0f;
                FollowTarget(playerGO.transform.position);
            }
            else if (distanceToPlayer >= minDistanceToPlayer && distanceToPlayer <= maxDistanceToPlayer && distanceToTarget > minDistanceToTarget)
            {
                idleTime += Time.deltaTime;

                if (idleTime >= timeTurnToTarget && idleTime < timeTurnToPlayer)
                {
                    between = false;
                }
                else if (idleTime >= timeTurnToPlayer)
                {
                    if (Rotate(playerGO.transform.position, 0, turnSpeed) > 4f)
                    {
                        PlayAnimationLooping(walkAnimation);
                    }
                    else
                    {
                        if (idleTime >= timeToBark && idleTime < timeToBark + 1f)
                        {
                            Bark();
                        }
                        else if (idleTime > timeToLead)
                        {
                            FollowTarget(playerGO.transform.position);
                        }
                        else
                        {
                            actor.Idle();
                        }
                    }
                }
                else
                {
                    actor.Idle();
                }
            }
            else if (distanceToPlayer <= minDistanceToPlayer && distanceToTarget > minDistanceToTarget && GetTargetPosition() != null)
            {
                idleTime = 0f;
                FollowTarget(GetTargetPosition().position);
				Debug.DrawLine(transform.position, GetTargetPosition().position, Color.red);
            }
            else
            {
                actor.Idle();
            }
            GUIManager.DebugText(this, distanceToTarget + " " + minDistanceToTarget);
        }
    }

    private void Bark()
    {
        PlayAnimationLooping(barkAnimation);

        if (!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().clip = barkAudio;
            GetComponent<AudioSource>().Play();
        }
    }

    private float lastCirrectionTime = 0f;
    private void FollowTarget(Vector3 p_Target)
    {
        if (p_Target != Vector3.zero)
        {
            if (GetCorrectionAngle(GetClosestCollider(), p_Target) != 0)
            {
                Rotate(p_Target, GetCorrectionAngle(GetClosestCollider(), p_Target), turnSpeed);
                lastCirrectionTime = Time.time;
            }
            else
            {
                if (Time.time - lastCirrectionTime > 2f)
                {
                    Rotate(p_Target, 0, turnSpeed);
                }
            }
            StartCoroutine(Walk());
        }
        else
        {
            Debug.Log("Can't follow target. It's null");
            actor.Idle();
        }
    }

    private Transform GetClosestCollider()
    {
        float minDistance = 9999f;
        int minIndex = -1;

        for (int i = 0; i < obstacles.Count; i++)
        {
            if (minDistance > (transform.position - obstacles[i].position).magnitude)
            {
                minDistance = (transform.position - obstacles[i].position).magnitude;
                minIndex = i;
            }
        }

        if (minIndex >= 0)
            return obstacles[minIndex];
        else
            return null;
    }


    private List<Transform> obstacles = null;

    private float GetCorrectionAngle(Transform p_ClosestCollider, Vector3 p_Target)
    {
        if (p_ClosestCollider == null)
            return 0;

        Vector3 relative = transform.InverseTransformPoint(p_ClosestCollider.position);
        float r_ColliderAngle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

        relative = transform.InverseTransformPoint(p_Target);
        float r_TargetAngle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

        if ((transform.position - p_ClosestCollider.position).magnitude < minDistanceToCollider)
        {
            return 180 - r_ColliderAngle;
        }

        if (r_TargetAngle < r_ColliderAngle)
        {
            return 90 - Mathf.Abs(r_TargetAngle - r_ColliderAngle);
        }
        else
            return 0;
    }

    private IEnumerator Walk()
    {
        PlayAnimationLooping(walkAnimation);
        yield return new WaitForSeconds(0);

        Vector3 moveDirection = new Vector3();
        moveDirection = new Vector3(0, 0, 0.1f);
        moveDirection = speed * moveDirection;
        moveDirection = transform.TransformDirection(moveDirection);
        transform.position += moveDirection;
    }

    private Transform GetTargetPosition()
    {
        if (GoalManager.CurrentGoal && GoalManager.CurrentGoal.Indicator)
        {
			//Debug.Log("Goal: " + GoalManager.CurrentGoal.Indicator.transform.name, GoalManager.CurrentGoal.Indicator.transform);
            return GoalManager.CurrentGoal.Indicator.transform;
        }
        else
        {
            GUIManager.DebugText(this, "GoalIndicator is null, Can't get target position for Dog");
            return null;
        }
    }

    private float Rotate(Vector3 p_Target, float p_correct, float p_rotateSpeed)
    {
        Vector3 relative = transform.InverseTransformPoint(p_Target);
        float r_angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg + p_correct;

        if (p_rotateSpeed != 0)
            transform.Rotate(0, Mathf.Clamp(r_angle, -p_rotateSpeed * Time.deltaTime, p_rotateSpeed * Time.deltaTime), 0);
        else
            transform.Rotate(0, r_angle, 0);

        return r_angle;
    }

    /*
    private Transform GetClosestBuilding(GameObject[] p_Buildings, float p_minDistanceToBuilding)
    {
        float minDistance = p_minDistanceToBuilding + 2f;
        Transform r_Closest = null;

        for (int i = 0; i < p_Buildings.Length; i++)
        {
            if (minDistance > (transform.position - p_Buildings[i].transform.position).magnitude)
            {
                minDistance = (transform.position - p_Buildings[i].transform.position).magnitude;
                r_Closest = p_Buildings[i].transform;
            }
        }

        if (r_Closest != null && Mathf.Abs((transform.position - r_Closest.position).magnitude - distanceToTarget) <= 3f)
        {
            Debug.Log(Mathf.Abs((transform.position - r_Closest.position).magnitude - distanceToTarget));

            return null;
        }

        return r_Closest;
    }
     */

    public Vector2 ScreenPosition
    {
        get
        {
            Debug.DrawRay(transform.position + Vector3.up * 1, Vector3.right, Color.blue);

            return GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1));
        }
        private set
        {
        }
    }

    void SwitchPCToFirstPerson()
    {
        PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
        cam.enabled = true;
        cam.FirstPersonView();
        MovementController.Main.ClearWalkTarget();
    }

    //bool debugLOD = false;
    public float maxTalkDistance = 5;
    public float maxNameDistance = 5;
    public float talkLabelHeight = 0;

    bool behindCamera = false;
    float lodMaxDistance = 30;
    float deactivateMaxDistance = 30;
    static float inBuildingLODDistance = 10;
    public static float InBuildingLODDistance
    {
        set
        {
            inBuildingLODDistance = value;
        }
    }

    void CheckLOD()
    {
        float cameraDistance = 1000;
        //bool npcIndoors = false;

        //if (actor.lodBuildingRoot != null)
        //    npcIndoors = true;
        

        if (Camera.main == null)
        {
            behindCamera = true;
        }
        else
        {
            behindCamera = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), transform.position - Camera.main.transform.position) < 0.40f;
            cameraDistance = (transform.position - Camera.main.transform.position).magnitude;
        }

        if (cameraDistance > lodMaxDistance || (behindCamera && cameraDistance > 10) || (actor.lodBuildingRoot && tag != "Player" && cameraDistance > inBuildingLODDistance))
        {
            if (cameraDistance > deactivateMaxDistance)
                lod.UpdateLOD(LODState.Deactivate, behindCamera);
            else
                lod.UpdateLOD(LODState.Hidden, behindCamera);
        }
        else
        {
            lod.UpdateLOD(LODState.Visible, behindCamera);
        }
    }

    public void OnTriggerEnter(Collider coll)
    {
        if (coll.tag != "Player")
        {
            Debug.Log("triggered " + coll.name);
            obstacles.Add(coll.transform);
        }
    }

    public void OnTriggerExit(Collider coll)
    {
        if (coll.tag != "Player")
        {
            Debug.Log("untriggered " + coll.name);
            obstacles.Remove(coll.transform);
        }
    }

}
