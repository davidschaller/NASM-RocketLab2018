using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
	Vector3 initialImpactPoint = Vector3.zero;
	bool didHitSomething = false;
	float maxLifetime = 0.01f;
	public float MaxLifetime
	{
		get
		{
			return maxLifetime;
		}
		set
		{
			maxLifetime = value;
		}
	}
	
	float lifeTime = 0;
	
	void OnCollisionEnter (Collision coll)
	{
		ProjectileTarget targ = (ProjectileTarget)coll.gameObject.GetComponent(typeof(ProjectileTarget));
		if (targ != null)
		{
			if (targ.pointValue != 0)
			{
				targ.RegisterHit(notifyOnImpact);
				targetsHit.Add(targ);
				didHitSomething = true;
			}
			
			TargetRegistry.DoParticles(targ.targetType, transform.position, targ.transform);
			if (targ.targetType == ProjectileTarget.TargetType.Water || targ.targetType == ProjectileTarget.TargetType.Dirt)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
			}
		}

		if (initialImpactPoint == Vector3.zero)
		{
			Debug.Log("=== setup");
			
			Invoke("DestroySelf", maxLifetime);
		}
		
		if (notifyOnImpact != null)
		{
			notifyOnImpact.SendMessage("ProjectileImpact", this);


			float dist = (transform.position - startPos).magnitude;
			if (initialImpactPoint == Vector3.zero && dist > 0.1f)
			{
				Debug.Log("==== Impact: " + coll.gameObject.name);
				
				initialImpactPoint = transform.position;
				//notifyOnImpact.SendMessage("FirstImpactAt", initialImpactPoint);
			}
		}

	}

	Vector3 startPos = Vector3.zero;
	void Awake ()
	{
		startPos = transform.position;
	}

	void DestroySelf ()
	{
		Debug.Log("==== Did hit : "+ didHitSomething);
		
		if (!didHitSomething)
			notifyOnImpact.SendMessage("FirstImpactAt", initialImpactPoint);
		else
			notifyOnImpact.SendMessage("FirstImpactAt", Vector3.zero);
		Destroy(gameObject);
	}

	GameObject notifyOnImpact;
	public void NotifyOnImpact ( GameObject notifyOnImpact )
	{
		this.notifyOnImpact = notifyOnImpact;
	}

	List<ProjectileTarget> targetsHit = new List<ProjectileTarget>();
	public List<ProjectileTarget> GetTargets ()
	{
		return targetsHit;
	}

	void Update ()
	{
		lifeTime += Time.deltaTime;
	}
}