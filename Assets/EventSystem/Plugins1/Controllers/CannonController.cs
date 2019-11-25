using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonController : MonoBehaviour
{
	public ParticleSystem smokeParticles;
	public ParticleSystem fireParticles;
	public Transform barrel;
	public Transform barrelFirePoint;
	public Rigidbody projectilePrefab;
	
	public float yawDegreesPerStep = 5;
	public float maxYaw = 15;
	public float pitchDegreesPerStep = 2;
	public float maxPitch = 15;
	public float fireForce = 4000;
	public AudioClip fireSound;
	public float fireSmokeDelay = 0;
	public float fireFireDelay = 0;
	public EventPlayerBase loadCannonEvent;
	public EventPlayerBase afterLoadEvent;

	public EventPlayerBase goodShotEvent;
	public EventPlayerBase badShotEvent;

	public int cannonBallCount = 40;
	public float projectileLifeAfterImpact = 0.01f;

	public Transform[] cannonballWatchers;

	public AudioClip buttonUpAudio;
	public AudioClip buttonDownAudio;
	public AudioClip buttonRightAudio;
	public AudioClip buttonLeftAudio;
	public float clickVolume;
	

	float cannonStartYaw = 0;
	Quaternion cannonStartRotation;
	Quaternion barrelStartRotation;
	int targetCount = 0;
	int targetsHit = 0;
	CannonGUI gui = null;

	enum GameState
	{
		Initialized,
		Started,
		Finished,
	}
	GameState gameState;

	public bool Finished
	{
		get
		{
			return gameState == GameState.Finished;
		}
		private set
		{
		}
	}

	AudioSource clickSource;
	void Awake ()
	{
		clickSource = Camera.main.GetComponent<AudioSource>();
		gui = GetComponent<CannonGUI>();
		gameState = GameState.Initialized;
		totalScore = PlayerPrefs.GetInt("CannonScore", 0);
	}

	int pitchCorrection = 0;
	Quaternion cameraStartRotation;
	List<Quaternion> watchersStartRotation = new List<Quaternion>();
	public void StartGame ()
	{
		gameState = GameState.Started;
		gui.enabled = true;
		gui.CurrentState = CannonGUI.DisplayState.Controls;
		
		ProjectileTarget[] targets = (ProjectileTarget[])GameObject.FindObjectsOfType(typeof(ProjectileTarget));
		foreach(ProjectileTarget targ in targets)
		{
			if (targ.pointValue != 0 && targ.gameObject.active)
				targetCount++;
		}
		Debug.Log(targetCount + " projectile targets found");

		cameraStartRotation = Camera.main.transform.rotation;
		cannonStartRotation = transform.rotation;
		cannonStartYaw = transform.eulerAngles.y;
		barrelStartRotation = barrel.rotation;
		smokeParticles.Stop();
		fireParticles.Stop();
		pitchCorrection = UncorrectedPitch * -1;
		
		StartCoroutine(LoadCannon());

		for(int i=0;i<cannonballWatchers.Length;i++)
			watchersStartRotation.Add(cannonballWatchers[i].transform.rotation);
	}

	bool firing = false;
	void Update ()
	{
		if (Input.GetKey("tab") && Input.GetKeyDown("j"))
		{
			int lvl = Application.loadedLevel+1;
			if (lvl >= Application.levelCount)
				lvl = 0;
			FinishGame();
			Application.LoadLevel(Application.loadedLevel+1);
		}
		
		if (gameState != GameState.Started || gameState == GameState.Finished || !gui.ControlsEnabled)
			return;

		if (targetsHit == targetCount)
			FinishGame();
		else if (cannonBallCount == 0 && !firing)
			StartCoroutine(FailLevel());
		
		if (Input.GetKeyDown("left"))
		{
			RotateLeft();
		}
		else if (Input.GetKeyDown("right"))
		{
			RotateRight();
		}
		else if (Input.GetKeyDown("up"))
		{
			PitchUp();
		}
		else if (Input.GetKeyDown("down"))
		{
			PitchDown();
		}
		else if (Input.GetKeyDown("space"))
		{
			Fire();
		}
	}

	void LateUpdate ()
	{
		if (projectileWatchTarget)
		{
			if (projectileWatchTarget.position.y > originalWatchTargetY && Camera.main.WorldToScreenPoint(projectileWatchTarget.position).y > Screen.height - 100)
			{
				//float dist = (projectileWatchTarget.position - Camera.main.position).magnitude;
				
				//projectileWatchTarget.position = new Vector3(projectileWatchTarget.position.x, originalWatchTargetY - 25, projectileWatchTarget.position.z);
			}
			foreach(Transform c in cannonballWatchers)
			{
				c.LookAt(projectileWatchTarget);
			}
			Camera.main.transform.LookAt(projectileWatchTarget);
		}
	}

	public EventPlayerBase failureEvent;
	IEnumerator FailLevel ()
	{
		Debug.Log("Failing game");
		
		gameState = GameState.Finished;
		gui.ControlsEnabled = false;

		if (failureEvent != null)
			yield return StartCoroutine(DoEventPlayer(failureEvent, true, false));

		Debug.Log("Reloading level");
		Application.LoadLevel(Application.loadedLevel);
	}

	public EventPlayerBase successEvent;
	IEnumerator DoSuccessEvent()
	{
		if (successEvent != null)
			yield return StartCoroutine(DoEventPlayer(successEvent, true, false));
	}
	
	void FinishGame ()
	{
		gameState = GameState.Finished;
		gui.ControlsEnabled = false;
		PlayerPrefs.SetInt("CannonScore", totalScore);
		PlayerPrefs.SetInt("CannonCompleted", Application.loadedLevel);

		StartCoroutine(DoSuccessEvent());
	}

	int score = 0;
	void IncrementScore (int amt)
	{
		score += amt * TargetRegistry.CurrentMultiplier;
		totalScore += amt * TargetRegistry.CurrentMultiplier;
	}

	int consecutiveHits = 0;
	void RegisterTargetType (ProjectileTarget.TargetType tp)
	{
		if (tp == ProjectileTarget.TargetType.Emplacement || tp == ProjectileTarget.TargetType.EarthWorks)
			consecutiveHits++;
		else
			consecutiveHits = 0;

		if (consecutiveHits >= 6)
			TargetRegistry.CurrentMultiplier = 3;
		else if (consecutiveHits >= 3)
			TargetRegistry.CurrentMultiplier = 2;
		else
			TargetRegistry.CurrentMultiplier = 1;
	}

	public int Score
	{
		get
		{
			return score;
		}
		set
		{
			score = value;
		}
	}

	int totalScore = 0;
	public int TotalScore
	{
		get
		{
			return totalScore;
		}
		set
		{
			totalScore = value;
		}
	}
	
	public int CannonBallCount
	{
		get
		{
			return cannonBallCount;
		}
		set
		{
			cannonBallCount = value;
		}
	}
	
	IEnumerator SmokeParticles ()
	{
		yield return new WaitForSeconds(fireSmokeDelay);
		smokeParticles.Play();

		yield return new WaitForSeconds(0.5f);
		smokeParticles.Stop();
	}
	
	IEnumerator FireParticles ()
	{
		yield return new WaitForSeconds(fireFireDelay);
		fireParticles.Play();

		yield return new WaitForSeconds(0.5f);
		fireParticles.Stop();
	}


	public EventPlayerBase idleEvent;
	IEnumerator LoadCannon ()
	{
		loadDepth++;
		Debug.LogWarning("LoadDepth: " + loadDepth);
		yield return new WaitForSeconds(1);
		if (targetsHit != targetCount)
		{
			if (loadCannonEvent != null)
			{
				float elapsed = Time.time;
				gui.ControlsEnabled = false;
				Debug.Log("Starting load cannon...");
				loadCannonEvent.PlayerTriggered();
				while(!loadCannonEvent.Finished)
					yield return 0;
				Debug.Log("Finished loading cannon in " + (Time.time - elapsed) + " seconds");
				gui.ControlsEnabled = true;
			}

			if (afterLoadEvent != null)
			{
				Debug.Log("Playing after load cannon event");
				afterLoadEvent.PlayerTriggered();
				while (!afterLoadEvent.Finished)
					yield return 0;
			}
			else
			{
				Debug.Log("No event specified after load cannon");
			}

			if (idleEvent != null)
			{
				Debug.Log("Playing idle event after 'after load cannon' event");
				idleEvent.PlayerTriggered();
			}
			else
			{
				Debug.Log("No event specified for idle (after loading)");
			}
		}
		
		yield return 0;
		loadDepth--;
	}

	int loadDepth = 0;
	void ResetCannon ()
	{
		if (loadDepth < 1 && gameState != GameState.Finished && cannonBallCount != 0 && targetsHit != targetCount)
			StartCoroutine(LoadCannon());
		transform.rotation = cannonStartRotation;
		barrel.rotation = barrelStartRotation;
	}

	public float hitFeedbackDelay = 2;
	void ProjectileImpact (Projectile projectile)
	{
		targetsHit += projectile.GetTargets().Count;
		projectile.GetTargets().Clear();
		
		Debug.Log(targetsHit + " hits out of " + targetCount);
		ResetCannon();

		CancelInvoke("HitFeedback");
		if (cannonBallCount != 0 && targetsHit != targetCount)
			Invoke("HitFeedback", hitFeedbackDelay);
		else
			firing = false;
	}

	void HitFeedback ()
	{
		gui.CurrentState = CannonGUI.DisplayState.Feedback;
	}

	Transform ClosestTargetTo(Vector3 pt)
	{
		float closestDist = 10000;
		Transform closest = null;
		
		ProjectileTarget[] remainingTargets = (ProjectileTarget[])FindObjectsOfType(typeof(ProjectileTarget));
		foreach( ProjectileTarget c in remainingTargets )
		{
			float dist = (pt - c.transform.position).magnitude;
			if (closestDist > dist)
			{
				closestDist = dist;
				closest = c.transform;
			}
		}

		return closest;
	}

	IEnumerator DoEventPlayer (EventPlayerBase ep, bool waitForFinish, bool resetFiring)
	{
		if (ep != null)
		{
			ep.PlayerTriggered();
			if (waitForFinish)
			{
				while(!ep.Finished)
					yield return 0;
			}
		}
		else
		{
			yield return 0;
		}

		if (resetFiring)
			firing = false;
	}

	IEnumerator CameraToStart ()
	{
		float ti = 1;
		while (ti >= 0)
		{
			Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, cameraStartRotation, 0.15f);
			ti -= Time.deltaTime;
			yield return 0;
		}
		
		//Camera.main.transform.rotation = cameraStartRotation;
	}
	
	void FirstImpactAt ( Vector3 impactPoint )
	{
		StartCoroutine(CameraToStart());
		for(int i=0;i<cannonballWatchers.Length;i++)
		{
			cannonballWatchers[i].transform.rotation = watchersStartRotation[i];
		}
		Destroy(projectileWatchTarget);
		
		projectileWatchTarget = null;
		if (impactPoint == Vector3.zero)
		{
			gui.RegisterFeedbackData(Vector3.zero, impactPoint);
			if (cannonBallCount != 0 && targetsHit != targetCount)
				StartCoroutine(DoEventPlayer(goodShotEvent, true, true));
			else
				firing = false;
		}
		else
		{
			Transform closestTarget = ClosestTargetTo(impactPoint);
			gui.RegisterFeedbackData(closestTarget.position, impactPoint);
			Debug.DrawRay(impactPoint, Vector3.up, Color.red);
			GameObject go = new GameObject();
			go.name = "IMPACT";
			go.transform.position = impactPoint;
			
			//closestTarget.rigidbody.AddForce(Vector3.up*2000);

			if (cannonBallCount != 0 && targetsHit != targetCount)
				StartCoroutine(DoEventPlayer(badShotEvent, true, true));
			else
				firing = false;
		}
	}

	public int HitPercentage
	{
		get
		{
			return (int)((float)targetsHit/(float)targetCount*100);
		}
		private set
		{
		}
	}

	Transform projectileWatchTarget;
	float originalWatchTargetY;
	public void Fire ()
	{
		if (cannonBallCount == 0 || targetsHit == targetCount) return;
		if (!gui.enabled || gui.CurrentState == CannonGUI.DisplayState.Feedback) return;

		firing = true;
		cannonBallCount--;
		Vector3 startPos = barrelFirePoint.transform.position;
		Rigidbody rb = (Rigidbody)Instantiate(projectilePrefab, startPos, Quaternion.identity);
		if (!rb.GetComponent<Collider>())
			rb.gameObject.AddComponent<SphereCollider>();
		
		Projectile projectile = rb.GetComponent<Projectile>();		
		if (projectile == null)
			projectile = rb.gameObject.AddComponent<Projectile>();
		
		projectile.MaxLifetime = projectileLifeAfterImpact;
		projectile.NotifyOnImpact(gameObject);

		GameObject go = new GameObject();
		go.transform.parent = projectile.transform;
		go.transform.localPosition = Vector3.zero;
		go.name = "Projectile Watch Target";
		originalWatchTargetY = go.transform.position.y;
		projectileWatchTarget = go.transform;
		
		rb.AddForce(barrelFirePoint.transform.TransformDirection(Vector3.forward) * fireForce);
		GetComponent<AudioSource>().clip = fireSound;
		GetComponent<AudioSource>().Play();

		StartCoroutine(SmokeParticles());
		StartCoroutine(FireParticles());		
	}

	public void Rotate(float deg)
	{
		if (!gui.enabled || gui.CurrentState == CannonGUI.DisplayState.Feedback) return;
		
		Camera.main.transform.parent = transform;

		if (deg < 0 && (YawAngle < maxYaw || YawDirection == "right") ||
			deg > 0 && (YawAngle < maxYaw || YawDirection == "left"))
			transform.eulerAngles = new Vector3(transform.eulerAngles.x,
												transform.eulerAngles.y + deg,
												transform.eulerAngles.z);
	}

	public void Pitch(float deg)
	{
		if (!gui.enabled || gui.CurrentState == CannonGUI.DisplayState.Feedback) return;

		float currAngle = PitchAngle;
		float maxDeg = deg;
		
		if (deg < 0 && currAngle < maxPitch)
		{
			maxDeg = PitchAngle - maxPitch;
			maxDeg = Mathf.Max(maxDeg, deg);
		}
		else if (deg > 0 && currAngle > -maxPitch)
		{
			maxDeg = maxPitch + PitchAngle;
			maxDeg = Mathf.Min(maxDeg, deg);
		}
		
		if (deg < 0 && currAngle < maxPitch ||
			deg > 0 && currAngle > -maxPitch)
			barrel.eulerAngles = new Vector3(barrel.eulerAngles.x + maxDeg,
											 barrel.eulerAngles.y,
											 barrel.eulerAngles.z);
	}

	public int UncorrectedPitch
	{
		get
		{
			float corrected = barrel.eulerAngles.x;
			if (corrected > 300)
				corrected = 360 - corrected;
			else
				corrected *= -1;
			return (int)corrected;
		}
		private set
		{
		}
	}

	public int PitchAngle
	{
		get
		{
			return UncorrectedPitch+pitchCorrection;
		}
		private set
		{
		}
	}

	public string YawDirection
	{
		get
		{
			string direction = "";
			if (cannonStartYaw - transform.eulerAngles.y < -1)
			{
				direction = "right";
			}
			else if (cannonStartYaw - transform.eulerAngles.y > 1)
			{
				direction = "left";
			}
			return direction;
		}
		private set
		{
		}
	}
	
	public int YawAngle
	{
		get
		{
			return(int)Mathf.Abs(cannonStartYaw - transform.eulerAngles.y);
		}
		private set
		{
		}
	}

	public void PitchUp()
	{
		if (buttonUpAudio) clickSource.PlayOneShot(buttonUpAudio, clickVolume);
		Pitch(-pitchDegreesPerStep);
	}

	public void PitchDown()
	{
		if (buttonDownAudio) clickSource.PlayOneShot(buttonDownAudio, clickVolume);
		Pitch(pitchDegreesPerStep);
	}

	public void RotateLeft()
	{
		if (buttonLeftAudio) clickSource.PlayOneShot(buttonLeftAudio, clickVolume);
		Rotate(-yawDegreesPerStep);
	}

	public void RotateRight()
	{
		if (buttonRightAudio) clickSource.PlayOneShot(buttonRightAudio, clickVolume);
		Rotate(yawDegreesPerStep);
	}
}
