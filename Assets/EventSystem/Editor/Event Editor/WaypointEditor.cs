using UnityEngine;
using UnityEditor;
using System.Collections;

public class WaypointEditor : EditorWindow
{
	[MenuItem("GameObject/Create Other/Waypoint at end of group %&p")]
	[MenuItem("Waypoints/Add Waypoint to end of group %&p")]
	static void CreateNewWaypoint ()
	{
		
		if (Selection.activeGameObject)
		{
			WaypointGroup group = (WaypointGroup)Selection.activeGameObject.GetComponent(typeof(WaypointGroup));
			Vector3 createPosition = Vector3.zero;
			Quaternion createRotation = Quaternion.identity;
			
			if (group == null && Selection.activeGameObject.transform.parent != null)
			{
				group = (WaypointGroup)Selection.activeGameObject.transform.parent.GetComponent(typeof(WaypointGroup));
				createPosition = Selection.activeGameObject.transform.position;
				createRotation = Selection.activeGameObject.transform.rotation;
			}
			else
			{
				createPosition = group.transform.position;
				createRotation = group.transform.rotation;
			}

			if (group)
			{
				GameObject go = new GameObject();
				go.AddComponent(typeof(Waypoint));
				go.name = "Waypoint";
				
				go.transform.parent = group.transform;
				group.AddWaypoint(go);
				
				go.transform.position = createPosition;
				go.transform.rotation = createRotation;

				Selection.activeGameObject = go;
				
				group.ConsolidateWaypoints(false);
			}
		}
		
	}

	[MenuItem("Waypoints/Add Waypoint after current selection")]
	static void AddWaypointAfterSelection ()
	{
		
		if (Selection.activeGameObject)
		{
			WaypointGroup group = null;
			Waypoint after = null;
			if (Selection.activeGameObject.transform.parent != null)
			{
				group = (WaypointGroup)Selection.activeGameObject.transform.parent.GetComponent(typeof(WaypointGroup));
				after = (Waypoint)Selection.activeGameObject.GetComponent(typeof(Waypoint));
				
			}

			if (group && after)
			{
				GameObject go = new GameObject();
				go.AddComponent(typeof(Waypoint));
				go.name = "Waypoint";
				
				go.transform.parent = group.transform;
				group.AddWaypointAfter(go, after);
				
				go.transform.position = Selection.activeGameObject.transform.position;
				go.transform.rotation = Selection.activeGameObject.transform.rotation;

				Selection.activeGameObject = go;
				
				group.ConsolidateWaypoints(false);
			}
		}
		
	}
	
	
	
	
	[MenuItem("Waypoints/Consolidate Waypoint Group")]
	static void ConsolidateWaypointGroup ()
	{
		// primarily in case some waypoints were deleted in the IDE through other than "Delete Selected Waypoints"
		WaypointGroup group = (WaypointGroup)Selection.activeGameObject.GetComponent(typeof(WaypointGroup));
		if (group != null)
		{
			group.ConsolidateWaypoints(true);
		}
	}
	
	[MenuItem("GameObject/Create Other/Waypoint Group %&g")]
	static void CreateNewWPGroup ()
	{
		GameObject go = new GameObject();
		WaypointGroup wpg = go.AddComponent<WaypointGroup>();
		go.name = "Waypoint Group";
		
		GameObject wp = new GameObject("Waypoint");
		wp.transform.parent = go.transform;
		wp.AddComponent<Waypoint>();
		wpg.AddWaypoint(wp);
		
		Selection.activeGameObject = go;
	}
	
	[MenuItem("Waypoints/Select Next &%#n")]
	static void SelectNextWaypoint ()
	{
		Waypoint sel = (Waypoint)Selection.activeGameObject.GetComponent(typeof(Waypoint));
		WaypointGroup group = sel.GetWaypointGroup();
		if (group != null)
		{
			Waypoint next = group.GetNextWaypoint(sel);
			if (next != null)
				Selection.activeGameObject = next.gameObject;
			else
				throw new System.Exception("No next waypoints");
		}
	}
	
	[MenuItem("Waypoints/Select Previous &%#p")]
	static void SelectPreviousWaypoint ()
	{
		Waypoint sel = (Waypoint)Selection.activeGameObject.GetComponent(typeof(Waypoint));
		WaypointGroup group = sel.GetWaypointGroup();
		if (group != null)
		{
			Waypoint previous = group.GetPreviousWaypoint(sel);
			if (previous != null)
			{
				Selection.activeGameObject = previous.gameObject;
			}
			else
			{
				throw new System.Exception("No previous waypoints");
			}
		}
	}
	
	static float unselectedRadius = 0.3f;
	static float selectedRadius = 0.5f;
	static float orientationRadius = 0.1f;
	static WaypointGroup editorSelectedGroup;
	[DrawGizmo (GizmoType.NotInSelectionHierarchy | GizmoType.Pickable | GizmoType.InSelectionHierarchy)]
	static void RenderWaypointGizmo (Waypoint wpt, GizmoType gizmoType)
	{
	    Vector3 position = wpt.transform.position;

		if ((gizmoType & GizmoType.InSelectionHierarchy ) != 0)
		{
			editorSelectedGroup = wpt.GetWaypointGroup();
		    if ((gizmoType & GizmoType.Active) != 0)
		        Gizmos.color = Color.red * 0.5f;
		    else
		        Gizmos.color = Color.red * 0.3F;
		    Gizmos.DrawSphere (position, selectedRadius);
		}
		
		if (editorSelectedGroup != null && editorSelectedGroup == wpt.GetWaypointGroup())
		{
			Gizmos.color = Color.yellow * 0.5F;
	    	Gizmos.DrawSphere (position, unselectedRadius);
			
			Gizmos.color = Color.red;
	    	Gizmos.DrawSphere (position + wpt.transform.TransformDirection(Vector3.right)*unselectedRadius, orientationRadius);
			
			Gizmos.color = Color.blue;
	    	Gizmos.DrawSphere (position + wpt.transform.TransformDirection(Vector3.forward)*unselectedRadius, orientationRadius);
	
		}
		
		Waypoint next = wpt.GetWaypointGroup().GetNextWaypoint(wpt);
		
		if (next != null && next.GetWaypointGroup() == editorSelectedGroup)
		{
			Debug.DrawLine(wpt.transform.position, next.transform.position, Color.green * 0.5f);
		}
	
	}
	
}
