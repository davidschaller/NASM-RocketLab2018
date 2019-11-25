using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JsonFx.Json;
using System.Text.RegularExpressions;

// NOTE: This class is too comlicated. Need to separate the logic.

public class AS3DLayoutGrid : AS3DLayoutGridBase
{
    public Texture2D[] arrows, lines;

    private List<GameObject> wallPrefabs;
    public List<GameObject> WallPrefabs
    {
        get
        {
            return wallPrefabs;
        }
        private set { }
    }
    
    // TODO: Remove this
    public void LoadWallPrefabs()
    {
        wallPrefabs = new List<GameObject>();
        GameObject wallSectionContainer = GameObject.Find("Wall section prefabs");
        if (wallSectionContainer)
        {
            foreach (Transform tr in wallSectionContainer.transform)
                wallPrefabs.Add(tr.gameObject);
        }
        else
            Debug.LogWarning("Warning! Coudn't load wall section prefabs. Couldn't find Interior 'Design Camera Game' object");        
    }

    private static List<WallSection> wallPoints = new List<WallSection>();

	protected override void OnPostRender ()
    {
        lineMaterial.SetPass(0);

        GL.Begin(GL.LINES);

        DrawGridLines();
        

        // Temp line to draw walls
        GL.Color(Color.black);

        DrawDarkOutline(BuildingManager.Picked.OutsideWallMarkers);
        DrawDarkOutline(BuildingManager.Picked.InsideWallMarkers);

        // Instantiated walls
        foreach (List<WallSection> wall in WallManager.Walls)
        {
            for (int i = 0; i < wall.Count - 1; i++)
            {
                if ((wall[i].Pos - wall[i + 1].Pos).magnitude == 1)
                {
                    if (WallManager.IsWallInsideBuilding(wall[i].Pos, wall[i + 1].Pos))
                    {
                        DrawLine(wall[i].Pos, wall[i + 1].Pos, true);
                    }
                }
            }
        }

        for (int i = 0; i < wallPoints.Count - 1; i++)
        {
            if ((wallPoints[i].Pos - wallPoints[i + 1].Pos).magnitude == 1 && 
                (InteriorDesignGUI.SelectedItem.subtab == InteriorDesignGUI.SubTabs.Draw ||
                    InteriorDesignGUI.SelectedItem.subtab == InteriorDesignGUI.SubTabs.Erase))
            {
                if (InteriorDesignGUI.SelectedItem.subtab == InteriorDesignGUI.SubTabs.Erase)
                {
                    GL.Color(Color.grey);
                    if (WallManager.IsInstantiated(wallPoints[i].Pos, wallPoints[i + 1].Pos))
                    {
                        DrawLine(wallPoints[i].Pos, wallPoints[i + 1].Pos, true);
                    }
                }
                else
                {
                    if (WallManager.IsWallInsideBuilding(wallPoints[i].Pos, wallPoints[i + 1].Pos))
                    {
                        DrawLine(wallPoints[i].Pos, wallPoints[i + 1].Pos, true);
                    }
                }

            }
        }

        GL.End();
    }

    private static bool draggingMouse = false;

    Vector3 current,
            last;

	void OnGUI ()
	{
        if (CameraSwitcher.Is3DView || !BuildingManager.Picked)
            return;

        if (wallPrefabs == null || wallPrefabs.Count < 4)
            LoadWallPrefabs();

        /* At this point user is just moveing his mouse
         * We need to show arrow near the cursor, when it's close to the wall
         * Arrow always looks at the nearest wall
         */
        if (InteriorDesignGUI.IsCoveringsAction)
        {
            // User has choosed coverings or erase button

            Vector3 mpos = Common.CursorTo3D(Event.current.mousePosition);

            Vector3 mWorld = GetComponent<Camera>().ScreenToWorldPoint(mpos);
            Vector3 anchorPos = GetNearestIntersection(mWorld);

            Vector3 temp = current;

            current = WallManager.GetClosestWallSectionPair(anchorPos, current);

            if (temp != current)
                last = temp;

            if ((anchorPos - current).magnitude > 2)
            {
                last = Vector3.zero;
                current = Vector3.zero;
            }
   
            Vector2 v1 = Common.ToScreen2D(current, GetComponent<Camera>());
            Vector2 v2 = Common.ToScreen2D(last, GetComponent<Camera>());

            GUI.Label(new Rect(v1.x, v1.y, 10, 10), arrows[0]);
            GUI.Label(new Rect(v2.x, v2.y, 10, 10), arrows[0]);


            
            Vector2 arrowPos = ArrowCoveringsHint.Update(anchorPos, v1, v2, GetComponent<Camera>());

            ArrowCoveringsHint.ShowArrow(arrowPos, draggingMouse, arrows, lines);
        }

        // User has finished dragging
        if (Event.current.type == EventType.MouseUp && draggingMouse)
        {
            draggingMouse = false;
            ProcessFinishedDraggingAction();

            wallPoints.Clear();
        }
        // User has started dragging
        else if (Event.current.type == EventType.MouseDown && !draggingMouse && InteriorDesignGUI.SelectedItem != null)
        {
            //totalWallLength += wallPointsLength;
            draggingMouse = true;

            ProcessStartedDraggingAction(Event.current.mousePosition);
        }
        // User is dragging something
        else if (draggingMouse)
        {
            ProcessDraggingAction(Event.current.mousePosition);
        }
	}

    private bool IsCursorInsideSection(Vector3 p_anchorPos)
    {
        Vector2 v = GetComponent<Camera>().WorldToScreenPoint(p_anchorPos);
        v.y = Screen.height - v.y - 17;
        Rect brushRect = new Rect(v.x, v.y, 17, 17);

        return brushRect.Contains(Event.current.mousePosition);
    }

    private void ProcessStartedDraggingAction(Vector2 p_cursorPosition)
    {
        Vector3 mpos = Common.CursorTo3D(p_cursorPosition);

        Vector3 mWorld = GetComponent<Camera>().ScreenToWorldPoint(mpos);
        Vector3 anchorPos = GetNearestIntersection(mWorld);

        if (InteriorDesignGUI.IsWallCoveringsDrawing)
        {
            if (ArrowCoveringsHint.IsInteriorCovering.HasValue)
            {
                ApplyCoverings(anchorPos, current, last, ArrowCoveringsHint.IsInteriorCovering);
            }
        }
        else if (InteriorDesignGUI.IsWallAction)
        {
            wallPoints.Add(new WallSection(anchorPos, "wall1", "wall1"));
        }
        else if (InteriorDesignGUI.IsFloorDrawing)
        {
            if (IsCursorInsideSection(anchorPos))
            {
                RaycastHit hit;

                Ray insideRay = new Ray(mWorld, Vector3.down);

                if (Physics.Raycast(insideRay, out hit, Mathf.Infinity))
                {
                    if (hit.transform.name.Contains("Floor"))
                    {
                        FloorManager.InstantiateFloor(hit.transform, InteriorDesignGUI.SelectedItem);
                    }
                }
            }
        }
    }

    private void ProcessDraggingAction(Vector2 p_cursor2DPos)
    {
        Vector3 cursor3DPos = Common.CursorTo3D(p_cursor2DPos);
        Vector3 mWorld = GetComponent<Camera>().ScreenToWorldPoint(cursor3DPos);
        Vector3 anchorPos = GetNearestIntersection(mWorld);

        if (InteriorDesignGUI.IsWallCoveringsDrawing)
        {
            ApplyCoverings(anchorPos, current, last, ArrowCoveringsHint.IsInteriorCovering);
        }
        else if (InteriorDesignGUI.IsWallAction)
        {
            if (wallPoints.Count == 0)
                return;

            float dist = (anchorPos - wallPoints[wallPoints.Count - 1].Pos).magnitude;
            if (dist == 1)
            {
                WallSection section = new WallSection(anchorPos, "wall1", "wall1");

                if (!wallPoints.Exists(delegate(WallSection p) { return p.Pos.Equals(anchorPos); }))
                {
                    if (WallManager.IsWallInsideBuilding(wallPoints[wallPoints.Count - 1].Pos, section.Pos))
                        wallPoints.Add(section);
                }
            }
            else if (dist > 1)
            {
                while (dist > 1)
                {
                    Vector3 delta = (wallPoints[wallPoints.Count - 1].Pos - anchorPos).normalized / 1.45f;

                    Vector3 closestToOld = wallPoints[wallPoints.Count - 1].Pos - delta;

                    Vector3 oldAnchorPos = GetNearestIntersection(closestToOld);

                    WallSection section = new WallSection(oldAnchorPos, "wall1", "wall1");

                    if (!wallPoints.Exists(delegate(WallSection p) { return p.Pos.Equals(oldAnchorPos); }))
                    {
                        if (WallManager.IsWallInsideBuilding(wallPoints[wallPoints.Count - 1].Pos, section.Pos))
                            wallPoints.Add(section);
                    }


                    dist = (oldAnchorPos - wallPoints[wallPoints.Count - 1].Pos).magnitude;
                }
            }
        }
        else if (InteriorDesignGUI.IsFloorDrawing)
        {
            RaycastHit hit;
            Vector3 rayToFloor = new Vector3(mWorld.x, BuildingManager.Picked.Floor.position.y + 1f, mWorld.z);

            Ray insideRay = new Ray(rayToFloor, Vector3.down);

            if (Physics.Raycast(insideRay, out hit, Mathf.Infinity))
            {
                if (hit.transform.name.Contains("Floor:"))
                {
                    FloorManager.InstantiateFloor(hit.transform, InteriorDesignGUI.SelectedItem);
                }
            }
        }
    }

    private void ProcessFinishedDraggingAction()
    {
        if (InteriorDesignGUI.IsWallDrawing)
        {
            if (wallPoints.Count > 1)
            {
                WallManager.AddWall(wallPoints);
                WallManager.InstantiateWallSections(wallPoints, wallPrefabs);
            }
        }
        else if (InteriorDesignGUI.IsWallErasing)
        {
            if (wallPoints.Count > 1)
                WallManager.RemoveWallSection(wallPoints);
        }
    }

    private void ApplyCoverings(Vector3 p_anchorPos, Vector3 current, Vector3 last, bool? isInterior)
    {
        if (!isInterior.HasValue)
            return;

        if ((last - current).magnitude < 1.2f && last != Vector3.zero && current != Vector3.zero)
        {
            bool rotate90 = true;
            Vector3 wallPosition = WallManager.GetWallPosition(current, last, out rotate90);

            CoveringsManager.ChangeInteriorCoverings(wallPosition, isInterior.Value, InteriorDesignGUI.SelectedItem);
        }
        else
        {
            Transform realMarker = null;

            float cursorToMarkerDistance = 99999;
            float minFitDistance = 15;

            BuildingManager.Picked.GetClosestExteriorDoorMarker(GetComponent<Camera>(), Event.current.mousePosition, ref cursorToMarkerDistance, ref realMarker);
            if (cursorToMarkerDistance < minFitDistance)
            {
                CoveringsManager.ChangeExteriorCoverings(realMarker.position, InteriorDesignGUI.SelectedItem.material, GetComponent<Camera>());
            }
        }

        InteriorDesignGUI.UpdateWalls();
    }

    //private static int totalWallLength = 0;
    private static int wallPointsLength = 0;
    public static int GetWallLength()
    {
        if ((InteriorDesignGUI.IsWallDrawing || InteriorDesignGUI.IsWallErasing))
        {
            if (draggingMouse)
            {
                wallPointsLength = RecalcWallLength(InteriorDesignGUI.IsWallDrawing);
            }
        }

        return (/*totalWallLength + */wallPointsLength);
    }

    private static int RecalcWallLength(bool p_isWallDrawing)
    {
        int result = 0;

        if (p_isWallDrawing)
        {
            for (int i = 0; i < wallPoints.Count - 1; i++)
            {
                if ((wallPoints[i].Pos - wallPoints[i + 1].Pos).magnitude == 1)
                {
                    if (WallManager.IsWallInsideBuilding(wallPoints[i].Pos, wallPoints[i + 1].Pos))
                    {
                        result += 3;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < wallPoints.Count - 1; i++)
            {
                if ((wallPoints[i].Pos - wallPoints[i + 1].Pos).magnitude == 1)
                {
                    if (WallManager.IsInstantiated(wallPoints[i].Pos, wallPoints[i + 1].Pos))
                    {
                        result -= 3;
                    }
                }
            }
        }

        return result;
    }

    //@script RequireComponent(Camera)
}