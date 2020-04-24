using UnityEngine;

class SpyGlassRunner : IEvent
{
    public Camera guiCamera;
    public Camera camera;
    public float rotateYSpeed = 10;
    public float rotateXSpeed = 5;
    public float timeLeft = 10;
    public float minX = 190,
                 maxX = 230,
                 minY = -2,
                 maxY = 5;
    public GameObject ezGUIPrefab;

    public Transform epSuccess,
                     epFailed;

    public bool specialOutcome = false;

    public Transform epSeeTheFlag,
                     epNotSeeWon,
                     epSeeBritishLoose,
                     epNotSeeBritishLoose;

    public float distanceToCamera;
    public bool won = false;

    public bool canSeeSmallFlag = false,
                canSeeMedFlag = false,
                canSeeLargeFlag = false;

    public float afterGotFlagDelay = 3;
    public float detectionTime = 1;
	public Transform flag;

    float x, y;
    UIProgressBar progressBar;

    #region IEvent Members

    public void OnEnter()
    {
        if (!camera)
        {
            eventFinished = true;
            Debug.LogError("Camera is not assigned");
            return;
        }

        camera.gameObject.SetActiveRecursively(true);

        if ((!specialOutcome && epSuccess && epFailed) || (specialOutcome && epNotSeeWon && epSeeBritishLoose && epSeeTheFlag))
        {
            x = camera.transform.rotation.eulerAngles.y;
            y = camera.transform.rotation.eulerAngles.x;

            if (guiCamera && ezGUIPrefab)
            {
                InstantiateEzGUI();
            }

            currentTime = timeLeft;

            ManagementTrigger.DisableAll();

            if (!flag)
            {
                GameObject goFlag = GameObject.Find("HorzCollider");
                if (goFlag)
                    flag = goFlag.transform;
            }
        }
        else
        {
            if (specialOutcome)
            {
                Debug.LogError("It is special outcome but some event players are not set:");
                if (!epNotSeeWon)
                    Debug.LogError("epNotSeeWon is missing");
                if (!epSeeBritishLoose)
                    Debug.LogError("epSeeBritishLoose is missing");
                if (!epSeeTheFlag)
                    Debug.LogError("epSeeTheFlag is missing");
            }
            else
            {
                Debug.LogError("It is regular outcome but some event players are not set:");
                if (!epSuccess)
                    Debug.LogError("epSuccess is missing");
                if (!epFailed)
                    Debug.LogError("epFailed is missing");
            }

            eventFinished = true;
            return;
        }
    }

    float currentTime = 0,
          detectTimer = 0;

    bool doISeeTheFlag = false;

    float delayTimer = 0;

    public void OnExecute()
    {
        if (!flag)
        {
            GameObject goFlag = GameObject.Find("HorzCollider");
            if (goFlag)
                flag = goFlag.transform;
        }

        if (Input.GetMouseButton(0))
        {
            x += Input.GetAxis("Mouse X") * rotateXSpeed * Time.deltaTime;
            x = ClampAngle(x, minX, maxX);

            y -= Input.GetAxis("Mouse Y") * rotateYSpeed * Time.deltaTime;
            y = ClampAngle(y, minY, maxY);
        }

        Quaternion rotation = Quaternion.Euler(y, x, 0);
        camera.transform.rotation = rotation;

        if (detectTimer >= detectionTime || currentTime <= 0)
        {
            if (detectTimer >= detectionTime)
            {
                if (!doISeeTheFlag)
                {
                    doISeeTheFlag = true;
                    if (specialOutcome)
                    {
                        if (won)
                        {
                            Debug.Log("Player sees the flag -- but only if it is Medium or Large.");
                            epSeeTheFlag.GetComponent<EventPlayer>().PlayerTriggered();
                        }
                        else
                        {
                            Debug.Log("See a British flag--the player has lost the game/battle.");
                            epSeeBritishLoose.GetComponent<EventPlayer>().PlayerTriggered();
                        }
                    }
                    else
                        epSuccess.GetComponent<EventPlayer>().PlayerTriggered();
                }
            }
            else
            {
                if (specialOutcome)
                {
                    if (won)
                    {
                        Debug.Log("Starting 'Don't See The Flag won'");
                        epNotSeeWon.GetComponent<EventPlayer>().PlayerTriggered();
                    }
                    else
                    {
                        Debug.Log("Starting 'Don't See The British Flag loose'");
                        epNotSeeBritishLoose.GetComponent<EventPlayer>().PlayerTriggered();
                    }
                }
                else
                {
                    epFailed.GetComponent<EventPlayer>().PlayerTriggered();
                }
                
            }

            eventFinished = true;
            return;
        }

        if (instantiatedGUI)
        {
            foreach (Transform tr in instantiatedGUI.transform)
            {
                if (tr.GetComponent<SpriteRoot>())
                    tr.GetComponent<SpriteRoot>().SetCamera(guiCamera);
                else if (tr.GetComponent<SpriteText>())
                    tr.GetComponent<SpriteText>().SetCamera(guiCamera);
            }
        }

        if (flag)
        {
            float angle = Vector3.Angle(camera.transform.TransformDirection(Vector3.forward), flag.position - camera.transform.position);

            if (angle <= 1)
            {
                switch (FlagManager.CurrentflagSize)
                {
                    case FlagManager.FlagSize.Large:
                        if (canSeeLargeFlag)
                            detectTimer += Time.deltaTime;
                        else
                            detectTimer = 0;
                        break;
                    case FlagManager.FlagSize.Medium:
                        if (canSeeMedFlag)
                            detectTimer += Time.deltaTime;
                        else
                            detectTimer = 0;
                        break;
                    case FlagManager.FlagSize.Small:
                        if (canSeeSmallFlag)
                            detectTimer += Time.deltaTime;
                        else
                            detectTimer = 0;
                        break;
                    case FlagManager.FlagSize.British:
                        if (canSeeMedFlag)
                            detectTimer += Time.deltaTime;
                        else
                            detectTimer = 0;
                        break;
                }
            }
            else
                detectTimer = 0;
        }

        currentTime -= Time.deltaTime;

        progressBar.value = currentTime / timeLeft;
    }

    public void OnExit()
    {
        if (camera)
        {
            camera.gameObject.SetActive(false);
        }

        if (instantiatedGUI)
        {
            GameObject.Destroy(instantiatedGUI);
        }
    }

    bool eventFinished = false;
    public bool EventIsFinished()
    {
        return eventFinished;
    }

    #endregion

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    GameObject instantiatedGUI;

    void InstantiateEzGUI()
    {
        Vector3 pos = guiCamera.transform.position + guiCamera.transform.TransformDirection(Vector3.forward) * distanceToCamera + Vector3.up * 170;
        instantiatedGUI = (GameObject)GameObject.Instantiate(ezGUIPrefab, pos, guiCamera.transform.rotation);

        progressBar = instantiatedGUI.GetComponentInChildren<UIProgressBar>();
        progressBar.value = 1;
    }

    public void OnReset()
    {
    }

    public void OnTerminate()
    {
    }
}

public class SpyGlassEvent : EventBase
{
    public Camera guiCamera;
    public Camera camera;
    public float rotateYSpeed = 10;
    public float rotateXSpeed = 5;
    public float timeLeft = 10;
    public float minX = 190,
                 maxX = 230,
                 minY = -2, 
                 maxY = 5;
    public GameObject ezGUIPrefab;

    public Transform epSuccess,
                     epFailed;

    public float distanceToCamera = 20;

    public bool canSeeSmallFlag = false,
                canSeeMedFlag = false,
                canSeeLargeFlag = false;

    public float afterGotFlagDelay = 3;
    public float detectionTime = 1;

    public bool specialOutcome = false;
    public bool won = false;

    public Transform epSeeTheFlag,
                     epNotSeeWon,
                     epSeeBritishLoose,
                     epNotSeeBritishLoose;

	public Transform flag;

    public override IEvent CreateRunner()
    {
        SpyGlassRunner runner = new SpyGlassRunner();
        runner.guiCamera = guiCamera;
        runner.camera = camera;
        runner.rotateXSpeed = rotateXSpeed;
        runner.rotateYSpeed = rotateYSpeed;
        runner.timeLeft = timeLeft;
        runner.minX = minX;
        runner.maxX = maxX;
        runner.minY = minY;
        runner.maxY = maxY;
        runner.ezGUIPrefab = ezGUIPrefab;
        runner.distanceToCamera = distanceToCamera;

        runner.epSuccess = epSuccess;
        runner.epFailed = epFailed;

        runner.canSeeSmallFlag = canSeeSmallFlag;
        runner.canSeeMedFlag = canSeeMedFlag;
        runner.canSeeLargeFlag = canSeeLargeFlag;
        runner.afterGotFlagDelay = afterGotFlagDelay;
        runner.detectionTime = detectionTime;

        runner.specialOutcome = specialOutcome;
        runner.epSeeTheFlag = epSeeTheFlag;
        runner.epNotSeeWon = epNotSeeWon;
        runner.epSeeBritishLoose = epSeeBritishLoose;
        runner.epNotSeeBritishLoose = epNotSeeBritishLoose;

        runner.won = won;
		runner.flag = flag;

        return runner;
    }

    string eventName = "Spy Glass";
    public override string GetEventName()
    {
        return eventName;
    }
}
