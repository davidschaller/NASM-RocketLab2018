using UnityEngine;
using System.Collections;


/// <summary>
/// 
/// </summary>
/// 
public class NPCBase : MonoBehaviour
{
    public Gender gender = Gender.Male;
    
    public int femaleBasicMaxNumber = 12;

    protected const string MALE_BASIC_NAME = "malebasic";
    protected const string FEMALE_BASIC_TEMPLATE = "femalebasic{0}";

    protected LOD lod = null;
    protected bool thirdPersonView = false;
    protected GameObject character = null;

    private bool isLocked = false;
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }
    }

    protected virtual void Awake()
    {
        // Can be instantiated already if called ActivateNPC()
        if (character == null)
            StartCoroutine(InstantiateCharacter());
    }
    
    protected IEnumerator InstantiateCharacter()
    {
        character = new GameObject();

#if CharacterCustomizer
        while (!CharacterGenerator.ReadyToUse)
            yield return 0;
#endif

        string basicName = MALE_BASIC_NAME;
        if (gender == Gender.Female)
        {
            int random = Random.Range(1, femaleBasicMaxNumber);
            basicName = string.Format(FEMALE_BASIC_TEMPLATE, random);
        }

#if CharacterCustomizer
        CharacterGenerator generator = CharacterGenerator.CreateWithRandomConfig(basicName);

        // Wait for the assets to be downloaded
        while (!generator.ConfigReady)
            yield return 0;

        // Create the character.
        character = generator.Generate();
#endif
        // Attach the character to this prefab so we can move it around.
        character.transform.parent = transform;
        character.transform.localPosition = Vector3.zero;
        character.transform.localRotation = Quaternion.identity;

        lod = new LOD(gameObject);
        InvokeRepeating("CheckLOD", 0, 0.15f);
		
#if !CharacterCustomizer
		yield return 0;
#endif
    }

    protected void CheckLOD()
    {
        float cameraDistance = 1000;

        if (Camera.main == null)
        {
            thirdPersonView = true;
        }
        else
        {
            thirdPersonView = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), transform.position - Camera.main.transform.position) < 0.40f;
            cameraDistance = (transform.position - Camera.main.transform.position).magnitude;
        }

        if (cameraDistance > PCCamera.Main.lodMaxDistance || (thirdPersonView && cameraDistance > PCCamera.Main.behindLodMaxDistance))
        {
            if (GetComponent<AudioSource>())
            {
                GetComponent<AudioSource>().clip = null;
            }

            if (cameraDistance > PCCamera.Main.deactivateMaxDistance)
            {
                lod.UpdateLOD(LODState.Deactivate, thirdPersonView);
            }
            else
            {
                lod.UpdateLOD(LODState.Hidden, thirdPersonView);
            }
        }
        else
        {
            lod.UpdateLOD(LODState.Visible, thirdPersonView);
        }
    }

    public void Lock(string p_reason)
    {
        if (isLocked)
        {
            Debug.Log("NPC is loked alrady");
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
            Debug.Log("NPC is unloked alrady");
        }
        else
        {
            isLocked = false;
            Debug.Log("NPC is unlocked due to " + p_reason);
        }
    }
}
