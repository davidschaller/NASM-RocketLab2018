using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCAutomaton : MonoBehaviour
{
	public GameObject[] waypoints;
	public bool loopWaypoints = true;
	public bool pauseAtWaypoints = true;
	public Vector2 pauseRandomRange = new Vector2(1, 5);
	
	public AnimationClip idleAnimation;
	public AnimationClip walkAnimation;
	public float walkAnimSpeedMultiplier=1;
	
	UnityEngine.AI.NavMeshAgent nav;
	Animation anim;
	int waypointIndex = 0;
	float warmupTime = 0.5f;
	void Start ()
	{
		//idle = true;
		nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
		
		if (waypoints.Length == 0)
		{
			enabled = false;
			return;
		}
		
		//nav.SetDestination(waypoints[waypointIndex].transform.position);
		SelectNextWaypoint(-1);
		nav.autoRepath = true;
		
		anim = GetComponentInChildren<Animation>();
		anim.AddClip(walkAnimation, "walk");
		anim.AddClip(idleAnimation, "idle");
		
		anim["walk"].wrapMode = WrapMode.Loop;
		anim["idle"].wrapMode = WrapMode.Loop;
	}
	
	bool ReachedNode (Vector3 loc)
	{
		if ((transform.position - loc).magnitude < nav.stoppingDistance)
		{
			Log(name + " reached destination node");
			
			return true;			
		}
		else
		{
			//if (name.Contains("Robinson")) Log(name + " dist: " + (transform.position - loc).magnitude);
		}
		return false;
	}
	
	List<int> groundedIndices = new List<int>();
	int SelectNextWaypoint (int curr)
	{
		int ret = curr+1;
		if (curr == waypoints.Length-1 && loopWaypoints)
			ret = 0;
		else if (curr == waypoints.Length-1 && !loopWaypoints)
			return -1;
		
		if (!groundedIndices.Contains(ret))
		{
			Log(name + " is grounding " + ret);
			UnityEngine.AI.NavMeshAgent a = waypoints[ret].GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (a != null)
				DestroyImmediate(a);
			
			
			RaycastHit hit;
			if (Physics.Raycast(waypoints[ret].transform.position + Vector3.up * 5, -Vector3.up, out hit, 1000, 1 << LayerMask.NameToLayer("Walkable")))
			{
				Log(waypoints[ret].name + " grounded on " + hit.transform.name);
				
				waypoints[ret].transform.position = hit.point;
			}
			else
			{
				Debug.LogWarning("NO GROUND FOR " + waypoints[ret].name);
				
			}
			
			
			groundedIndices.Add(ret);
		}
			
		return ret;
	}
	
	void SetDestination (Vector3 loc)
	{
		nav.SetDestination(loc);
	}
	
	void Log(string msg)
	{
		/*if (Application.isEditor)
			Debug.Log(msg);
			*/
	}
	
	void AdvanceWaypoint ()
	{
		waypointIndex = SelectNextWaypoint(waypointIndex);
		if (waypointIndex != -1)
			SetDestination(waypoints[waypointIndex].transform.position);
	}
	
	bool resumeWalking = true;
	bool finishedWalking = false;
	bool idle = false;
	bool initialPath = true;
	int attemptedRepaths=0;
	bool pausedAtNode = false;
	
	bool trying = false;
	void Update ()
	{
		if (waypointIndex != -1)
		{
			if (ReachedNode(waypoints[waypointIndex].transform.position) && pauseAtWaypoints && !pausedAtNode)
			{
				pausedAtNode = true;
				nav.Stop();
				warmupTime = Random.Range(pauseRandomRange.x, pauseRandomRange.y);
				
				//Log(name + " will pause at this waypoint for " + warmupTime + " seconds", gameObject);
			}
			else if (ReachedNode(waypoints[waypointIndex].transform.position) && !pauseAtWaypoints && !pausedAtNode)
			{
				Log(name + " is advancing to next waypoint");
				trying = false;
			
				AdvanceWaypoint();
			}
			else if (!nav.hasPath && !pausedAtNode && !trying)
			{
				if (name.Contains("Robinson") && !trying) Log(name + " is trying to find a path to " + waypoints[waypointIndex].name + ", index " + waypointIndex);
				trying = true;
				nav.Stop();
			
				nav.ResetPath();
				SetDestination(waypoints[waypointIndex].transform.position);
			}			
		}
		
		if (nav.velocity.magnitude > 0)
		{
			anim["walk"].speed = nav.speed * walkAnimSpeedMultiplier;
			anim.CrossFade("walk");
		}
		else
		{
			anim.CrossFade("idle");
		}
		
		if (pausedAtNode)
		{
			warmupTime -= Time.deltaTime;
			if (warmupTime <= 0)
			{
				pausedAtNode = false;
				
				AdvanceWaypoint();
			}
		}
	}
}
