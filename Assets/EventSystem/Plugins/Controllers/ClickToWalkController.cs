using UnityEngine;

public class ClickToWalkController : MonoBehaviour
{
	static ClickToWalkController main;
	public static ClickToWalkController Main
	{
		get 
		{
			return main;
		}
		private set {}
	}
	
    public float maxClickDistance = 80;

    Ray clickRay;

    float mouseDownTime = 0;


    Vector3 clickTarget = Vector3.zero;
    Vector3 ClickTarget
    {
        get
        {
            return clickTarget;
        }
        set
        {
            clickTarget = value;
            if (updateClickTarget != null)
                updateClickTarget(clickTarget);
        }
    }
	Vector3 overTarget = Vector3.zero;
    Vector3 OverTarget
    {
        get
        {
            return overTarget;
        }
        set
        {
            overTarget = value;
            if (updateOverTarget != null)
                updateOverTarget(overTarget);
        }
    }

    TerrainData terrainData;
    Terrain activeTerrain;

    void Awake()
    {
		if (!main)
			main = this;
    }

    void Start()
    {
        if (Terrain.activeTerrain != null)
        {
            terrainData = Terrain.activeTerrain.terrainData;
            activeTerrain = Terrain.activeTerrain;
        }
        else
            Debug.LogError("Terrain.activeTerrain == null");
    }

    void Update()
    {
        if (PCCamera.Main == null)
            enabled = false;
        
    	if (controlLock > 0)
    		return;
		
		if (Camera.main)
		{
        	clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
	        RaycastHit ohit;
	        if (Physics.Raycast(clickRay.origin, clickRay.direction, out ohit, maxClickDistance, 1 << LayerMask.NameToLayer("Walkable")))
	            OverTarget = ohit.point;
	        else
	            OverTarget = Vector3.zero;
		}
		else
			return;

        if (PCCamera.Main.CurrentView == PCCamera.Views.ThirdPerson)
        {
            if (Input.GetMouseButton(0))
            {
                mouseDownTime += Time.deltaTime;

                if (mouseDownTime > .2f)
                {
                    if (!PCCamera.Main.SetStandMode())
                    {
                        mouseDownTime = 0;
                    }
                }
            }
            else
            {
                PCCamera.Main.StandMode = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mouseDownTime <= .2f)
            {
                clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(clickRay.origin, clickRay.direction, out hit, maxClickDistance, 1 << LayerMask.NameToLayer("Walkable")))
                {
                    bool isTerrain = false;
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                        isTerrain = true;

                    ClickTarget = hit.point;

                    if (isTerrain)
                        clickTarget = new Vector3(clickTarget.x, terrainData.GetInterpolatedHeight(clickTarget.x / terrainData.size.x, clickTarget.z / terrainData.size.z) + activeTerrain.transform.position.y, clickTarget.z);
                }
                else
                    ClickTarget = Vector3.zero;
            }

            mouseDownTime = 0;
        }

        /*
        if (clickTarget != Vector3.zero)
        {
            Debug.DrawRay(clickTarget, Vector3.up, Color.green);
            Debug.DrawRay(clickRay.origin, clickRay.direction * maxClickDistance, Color.green);
        }
         */
    }

    public delegate void ClickCallback(Vector3 clickTarget);
    ClickCallback updateClickTarget;
    ClickCallback updateOverTarget;

    public void SetListener(ClickCallback callbackListener)
    {
        updateClickTarget = callbackListener;
    }
	
    public void SetOverListener(ClickCallback callbackListener)
    {
        updateOverTarget = callbackListener;
    }
    
    int controlLock = 0;
    public bool IsLockControl()
    {
    	return controlLock > 0;
    }
    
    public void UnlockControl ()
	{
		controlLock--;
		if(controlLock < 0)
			controlLock = 0;
	}
    public void LockControl ()
	{
		controlLock++;
	}
}
