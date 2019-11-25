using UnityEngine;
using System.Collections;

public class WaypointGroup : MonoBehaviour
{
	public bool closedCircuit = false;
	
	public Waypoint[] waypoints;

	public Transform cameraWatchTarget;
	
	public int WaypointCount ()
	{
		return waypoints != null ? waypoints.Length : 0;
	}
	
	public Waypoint FirstWaypoint ()
	{
		return waypoints != null ? waypoints[0] : null;
	}

	public Waypoint LastWaypoint
	{
		get
		{
			return waypoints != null ? waypoints[waypoints.Length-1] : null;
		}
		private set
		{
		}
	}

	public void GroundAll ()
	{
		foreach(Waypoint w in waypoints)
		{
			w.Ground();
		}
	}
	
	public void AddWaypoint( GameObject go )
	{
		Waypoint newWP = (Waypoint)go.GetComponent(typeof(Waypoint));
		if (newWP != null)
		{
			Waypoint[] newPoints = new Waypoint[waypoints.Length+1];
			for (int i=0;i<waypoints.Length;i++)
			{
				newPoints[i] = waypoints[i];
			}
			newPoints[waypoints.Length] = newWP;
			waypoints = newPoints;
		}
		else
		{
			throw new System.Exception("Attempting to add GO to waypoint list, but the GO is not a waypoint.");
		}
	}
	
	public void AddWaypointAfter( GameObject go, Waypoint after)
	{
		Waypoint newWP = (Waypoint)go.GetComponent(typeof(Waypoint));
		if (newWP != null)
		{
			Waypoint[] newPoints = new Waypoint[waypoints.Length+1];
			int currIndex = 0;
			for (int i=0;i<waypoints.Length;i++)
			{
				newPoints[currIndex] = waypoints[i];
				if (after == waypoints[i])
				{
					currIndex++;
					newPoints[currIndex] = newWP;
				}
				currIndex++;
			}
			waypoints = newPoints;
		}
		else
		{
			throw new System.Exception("Attempting to add GO to waypoint list, but the GO is not a waypoint.");
		}
	}
	
	public void DeleteWaypoint( Waypoint wpt )
	{
		Waypoint[] newList = new Waypoint[waypoints.Length-1];
		int j=0;
		bool deleteDone = false;
		for(int i=0;i<waypoints.Length;i++)
		{
			if (waypoints[i] != null && wpt != null && waypoints[i].transform != wpt.transform)
			{
				newList[j] = waypoints[i];
			}
			else
			{
				deleteDone = true;
				if (waypoints[i] != null)
					DestroyImmediate(waypoints[i].gameObject);
			}
		}
		if (deleteDone)
			waypoints = newList;
	}
	
	public void ConsolidateWaypoints (bool feedback)
	{
		int updatedCount = 0;
		foreach(Waypoint pt in waypoints)
		{
			if (pt != null)
				updatedCount++;
		}
		
		if (updatedCount != waypoints.Length)
		{
			Debug.Log("Consolidating waypoint group " + name);
			Waypoint[] newList = new Waypoint[updatedCount];
			int idx = 0;
			for(int i=0;i<waypoints.Length;i++)
			{
				Waypoint wpt = waypoints[i];
				if (wpt != null)
				{
					newList[idx] = wpt;
					idx++;
				}
			}
			waypoints = newList;
		}
		
		for(int i=0;i<waypoints.Length;i++)
		{
			waypoints[i].gameObject.name = "Waypoint " + (i+1).ToString("000");
		}
		
		if (feedback)
			Debug.Log("Waypoint group is OK.");
	}
	
	public Waypoint GetNextWaypoint ( Waypoint curr )
	{
		Waypoint next = null;
		for(int i=0;i<waypoints.Length;i++)
		{
			if (waypoints[i].transform && waypoints[i].transform == curr.transform && (i+1 < waypoints.Length))
			{
				next = waypoints[i+1];
			}
		}
		
		if (next == null && closedCircuit)
			next = waypoints[0];
		
		return next;
	}
	
	public Waypoint GetPreviousWaypoint ( Waypoint curr )
	{
		Waypoint prev = null;
		for(int i=0;i<waypoints.Length;i++)
		{
			if (waypoints[i].transform && waypoints[i].transform == curr.transform && (i > 0))
			{
				prev = waypoints[i-1];
			}
		}
		
		if (closedCircuit && prev == null)
			prev = waypoints[waypoints.Length-1];
		
		return prev;
	}
	
}
