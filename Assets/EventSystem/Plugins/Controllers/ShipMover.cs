using UnityEngine;
using System.Collections;

public class ShipMover : MonoBehaviour
{
	public float forwardMoveDistance = 5;
	public float moveSpeed = 1;

	
	Transform forwardTarget;
	Transform startTarget;
	void Awake ()
	{
		GameObject s = new GameObject();
		s.name = "Ship rear target";
		s.transform.position = transform.position;
		startTarget = s.transform;
		
		GameObject f = new GameObject();
		f.name = "Ship forward target";
		f.transform.position = transform.position + transform.TransformDirection(Vector3.forward) * forwardMoveDistance;
		forwardTarget = f.transform;
	}

	enum Stage
	{
		Forward,
		Back
	}

	public int hitsToList = 3;
	public int hitsToSink = 5;
	public int hitsToKill = 7;
	public float sinkDistancePerHit = 2;
	int hits = 0;
	public int Hits
	{
		get
		{
			return hits;
		}
		set
		{
			hits = value;
			Debug.Log("Total hits: " + hits);
			if (hits == hitsToList)
				List();
			else if (hits >= hitsToSink)
				Sink();
		}
	}

	void StopFires ()
	{
		ParticleSystem[] emitters = GetComponentsInChildren<ParticleSystem>();
		foreach(ParticleSystem e in emitters)
		{
			e.Stop();
		}
	}
	
	void Sink ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y - sinkDistancePerHit, transform.position.z);
		forwardTarget.position = new Vector3(forwardTarget.position.x, forwardTarget.position.y - sinkDistancePerHit, forwardTarget.position.z);
		startTarget.position = new Vector3(startTarget.position.x, startTarget.position.y - sinkDistancePerHit, startTarget.position.z);
		
		if (hitsToKill == hits)
		{
			move = false;
			Invoke("StopFires", 0.5f);
		}
	}
	
	public float listAngle = 10;
	float listTarget = 0;
	void List ()
	{
		listTarget = listAngle;
	}
	
	Stage stage = Stage.Forward;
	float counter = 0;
	bool move = true;
	void LateUpdate ()
	{
		if (!move) return;
		
		switch(stage)
		{
			case Stage.Forward:
				transform.position = Vector3.Lerp(startTarget.position, forwardTarget.position, counter);
				//Debug.Log((transform.position - forwardTarget.position).magnitude + " stage: " + stage);
				
				if ((transform.position - forwardTarget.position).magnitude < 0.1f)
				{
					counter = 0;
					stage = Stage.Back;
				}
				break;
			case Stage.Back:
				transform.position = Vector3.Lerp(forwardTarget.position, startTarget.position, counter);
				//Debug.Log((transform.position - startTarget.position).magnitude + " stage: " + stage);

				if ((transform.position - startTarget.position).magnitude < 0.1f)
				{
					counter =0;
					stage = Stage.Forward;
				}
				break;
		}
		if (transform.eulerAngles.z != listTarget)
			transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, listTarget), 0.5f * Time.deltaTime);
		counter += moveSpeed * Time.deltaTime;
	}
}
