using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Texture2D marker;
    public float fastspeed = 4;
    public float speed = 2;
    public float sneakSpeed = 1;
    public float backwardSpeed = 1;
    public float turnSpeed = 360;

	public string playerName;
	Gender gender;
	public Gender Gender
	{
		get
		{
			return SaveManager.Gender;
		}
	}

    private bool isInsideBuilding = false;
    public bool IsInsideBuilding
    {
        get
        {
            return isInsideBuilding;
        }
        set
        {
            isInsideBuilding = value;
        }
    }

    private bool isInsideTrigger = false;
    public bool IsInsideTrigger
    {
        get
        {
            return isInsideTrigger;
        }
        set
        {
            isInsideTrigger = value;
        }
    }

    public AudioClip woodWalkSound;
    public AudioClip dirtWalkSound;

	public static bool Mounted
	{
		get
		{
			return horse != null;
		}
	}

	static HorseController horse;
	public static HorseController Horse
	{
		get
		{
			return horse;
		}
		set
		{
			horse = value;
		}
	}
}
