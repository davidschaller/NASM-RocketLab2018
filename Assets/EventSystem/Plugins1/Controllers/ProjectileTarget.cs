using UnityEngine;
using System.Collections;

public class ProjectileTarget : MonoBehaviour
{
	string pointsBoxStyle = "Box";
	string multiplierBoxStyle = "Box";
	
	public enum TargetType
	{
		Dirt,
		EarthWorks,
		Building,
		Water,
		ShipDeck,
		Emplacement
	}

	ShipMover ship;
	void Awake ()
	{
		ShipMover[] ships = transform.root.GetComponentsInChildren<ShipMover>();
		foreach(ShipMover s in ships)
		{
			if (transform.IsChildOf(s.transform))
			{
				ship = s;
			}
		}
	}

	void OnCollisionEnter (Collision coll)
	{
	}

	public TargetType targetType;
	public int pointValue = 10;

	public Vector2 boxWH = new Vector2(100, 50);
	public float timeToDisappear = 0;
	
	float riseSpeed = 50;
	Rect multiplierOffset;
	public void RegisterHit (GameObject shooter)
	{
		if (GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = false;
		shooter.SendMessage("RegisterTargetType", targetType);
		shooter.SendMessage("IncrementScore", pointValue);
		
		if (pointValue != 0)
		{
			showHit = true;
			pointsBoxStyle = TargetRegistry.GetGUIStyle(targetType);
			multiplierBoxStyle = TargetRegistry.GetMultiplierStyle();
			multiplierOffset = TargetRegistry.GetMultiplierOffset();
			
			StartCoroutine(FloatScore());
			Invoke("Disappear", timeToDisappear);
		}
		if (ship != null)
		{
			ship.Hits++;
		}
	}

	ParticleSystem impactParticles;
	public ParticleSystem ImpactParticles
	{
		get
		{
			return impactParticles;
		}
		set
		{
			impactParticles = value;
		}
	}
	
	IEnumerator FadeDisappear ()
	{
		MeshRenderer r = GetComponent<MeshRenderer>();
		if (!r) r = GetComponentInChildren<MeshRenderer>();

		float ti=1;
		while(ti > 0 && r.material.color.a >= 0)
		{
			r.material.color = new Color(r.material.color.r, r.material.color.g, r.material.color.b, r.material.color.a - Time.deltaTime);
			ti -= Time.deltaTime;
			yield return 0;
		}
		if (impactParticles)
			impactParticles.Stop();
		Destroy(gameObject);
	}

	void Disappear ()
	{
		if (TargetRegistry.FadeType(targetType))
			StartCoroutine(FadeDisappear());
		else if (GetComponent<Renderer>())
			GetComponent<Renderer>().enabled = false;
	}
	
	IEnumerator FloatScore ()
	{
		while(yOffset < 100)
		{
			yOffset += riseSpeed * Time.deltaTime;
			yield return 0;
		}
		showHit = false;
		if (targetType != TargetType.Water && targetType != TargetType.Dirt && !TargetRegistry.FadeType(targetType))
			Destroy(gameObject);
	}
	
	bool showHit = false;
	float yOffset = 0;
	void OnGUI ()
	{
		if (showHit)
		{
			GUI.skin = CannonGUI.GUISkin;
			
			Vector2 screenPos = GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position));
			Rect rect = new Rect( screenPos.x - boxWH.x/2, Screen.height - screenPos.y - boxWH.y/2 - yOffset, boxWH.x, boxWH.y );
			GUI.Box(rect, "+" + pointValue + "!", pointsBoxStyle);
			if (TargetRegistry.CurrentMultiplier > 1)
			{
				Rect multRect = new Rect(rect);
				
				multRect.x = rect.x + multiplierOffset.x;
				multRect.y = rect.y + multiplierOffset.y;
				multRect.width = multiplierOffset.width == 0 ? rect.width : multiplierOffset.width;
				multRect.height = multiplierOffset.height == 0 ? rect.height : multiplierOffset.height;
				
				GUI.Box(multRect, "x" + TargetRegistry.CurrentMultiplier + "!", multiplierBoxStyle);
			}
		}
	}
}