using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class BuildingManager
{
    private static List<BuildingDefinition> buildingsList;
    public static List<BuildingDefinition> BuildingsList
    {
        get
        {
            return buildingsList;
        }
        private set { }
    }
    
    private static BuildingDefinition picked;
    public static BuildingDefinition Picked
    {
        get
        {
            return picked;
        }
        set
        {
            picked = value;
            picked.ActivateRenderers();

            picked.ExteriorWalls = LoadExteriorWalls(picked.Model);
            picked.RoofMarkers = LoadRoofMarkers(picked.Model);
            picked.OutsideWallMarkers = LoadWallMarkers(picked.Model, "WallMarker");
            picked.InsideWallMarkers = LoadWallMarkers(picked.Model, "WallMarkerInside");

            BuildingRestructurer.SetDefaultCoveringsToExtentions(picked.Model.transform);
            BuildingRestructurer.SetDefaultCoveringsToCorners(picked.Model.transform);

            // List of WallSections to Save exterior coverings (inside) later
            CoveringsManager.Coverings.Clear();
        }
    }

    private static List<RoofMarker> LoadRoofMarkers(Transform model)
    {
        List<RoofMarker> result = new List<RoofMarker>();

        foreach (Transform item in model.transform)
        {
            if (item.name.Contains("FootRoofMarker"))
            {
                RoofMarker oRoofMarker = new RoofMarker();

                // Add new Roof Marker to 'Roof Markers and Wall Extentions'

                if (item.name.Contains("8"))
                {
                    oRoofMarker.LabelText = "8' 2.4m";
                    oRoofMarker.MarkerType = RoofMarker.MarkerTypes.m8Foot;
                }
                else if (item.name.Contains("10"))
                {
                    oRoofMarker.LabelText = "10' 3m";
                    oRoofMarker.MarkerType = RoofMarker.MarkerTypes.m10Foot;
                }
                else if (item.name.Contains("12"))
                {
                    oRoofMarker.LabelText = "12' 3.7m";
                    oRoofMarker.MarkerType = RoofMarker.MarkerTypes.m12Foot;
                }
                else
                {
                    oRoofMarker.LabelText = "15' 4.6m";
                    oRoofMarker.MarkerType = RoofMarker.MarkerTypes.m15Foot;
                }
                oRoofMarker.MarkerPosition = item.localPosition;
                result.Add(oRoofMarker);
            }
        }
        return result;
    }

	public static void Init ()
	{
        buildingsList = new List<BuildingDefinition>();

        BuildingDefinition[] buildings = (BuildingDefinition[])GameObject.FindObjectsOfType(typeof(BuildingDefinition));
        if (buildings != null)
        {
            buildingsList.AddRange(buildings);
            buildingsList.Sort(delegate(BuildingDefinition p1, BuildingDefinition p2)
                { return p1.transform.name.CompareTo(p2.transform.name); });
        }
	}
	
	public static void DeactivateAllBuildings ()
	{
		foreach(BuildingDefinition b in buildingsList)
		{
			if (b != null)
				b.gameObject.SetActiveRecursively(false);
		}
	}

	public static void ActivateAllBuildings ()
	{
        foreach (BuildingDefinition b in buildingsList)
		{
			if (b != null)
				b.gameObject.SetActiveRecursively(true);
		}
	}

    public static void InstantiateBuildingPrefabs()
    {
        for (int i = 0; i < buildingsList.Count; i++ )
        {
            if (buildingsList[i] != null && buildingsList[i].buildingModel != null)
            {
                GameObject model = (GameObject)GameObject.Instantiate(buildingsList[i].buildingModel, buildingsList[i].transform.position, buildingsList[i].transform.rotation);
                model.name = "Model";

                buildingsList[i] = BuildingRestructurer.RestructureRoofs(buildingsList[i], model);

                buildingsList[i] = BuildingRestructurer.InitializeFloors(buildingsList[i], model.transform);
                
                BuildingRestructurer.ReconstructWallExtensions(model.transform);

                model.transform.parent = buildingsList[i].transform;

                buildingsList[i].DeactivateRenderers(false, false);
            }
        }
    }

    /// <summary>
    /// Need exterior walls to update visibility, position openings, etc.
    /// </summary>
    /// <param name="model">Model of the picked building</param>
    /// <returns></returns>
    private static List<Transform> LoadExteriorWalls(Transform model)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform item in model)
        {
            // DoorMarker is inside Building*:Layer* GameObject 
            if (Regex.IsMatch(item.name, "Building.*:Layer"))
            {
                if (item.childCount == 0)
                {
                    GameObject fakemarker = new GameObject();
                    fakemarker.transform.parent = item;
                }

                // Walls should be white by default
                Renderer renderer = item.GetComponent<Renderer>();

                Material[] materials = renderer.materials;

                materials[0] = Common.GetMaterialByName("wall1");
                materials[1] = Common.GetMaterialByName("wall1");

                renderer.materials = materials;

                result.Add(item);
            }
        }

        return result;
    }

    /// <summary>
    /// Each building has special wallMarkers for bold dark outline
    /// </summary>
    /// <param name="model">Model of the picked building</param>
    /// <returns></returns>
    private static List<Vector3> LoadWallMarkers(Transform p_model, string p_name)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (Transform item in p_model)
        {
            if (item.name.Equals(p_name, System.StringComparison.InvariantCultureIgnoreCase))
            {
                result.Add(item.position);
            }
        }
        return result;
    }

    public static void RemoveAllBuildingObjects()
    {
        foreach (BuildingDefinition b in buildingsList)
        {
            GameObject.Destroy(b.gameObject);
        }
    }

    /// <summary>
    /// Position 3D roof models in scrolling panel
    /// </summary>
    /// <param name="picked">Picked building</param>
    /// <param name="scrollCam">Scrolling Camera</param>
    public static Rect PositionRoofsInScroller(BuildingDefinition picked, Camera scrollCam, Vector3 startPos)
    {
		int currMultiplier = 0;
 
        Rect result = new Rect();
        
        foreach (RoofDefinition roof in picked.Roofs)
		{
            roof.transform.position = startPos + scrollCam.transform.TransformDirection(Vector3.right) * currMultiplier * 35;
			currMultiplier++;

            result = new Rect(roof.LabelPos.x - 50, Screen.height - roof.LabelPos.y - 55, 100, 67);
		}
         
        return result;
    }

    /// <summary>
    /// Remove roof models from scroller
    /// </summary>
    public static void RemoveRoofsFromScroller()
    {
        foreach (BuildingDefinition b in buildingsList)
        {
            foreach (RoofDefinition roof in b.Roofs)
            {
                roof.transform.position = Vector3.zero;
            }
        }
    }

    internal static RoofDefinition RoofClicked(Camera cam, Vector2 mp)
    {
        foreach (RoofDefinition roof in BuildingManager.Picked.Roofs)
        {
            roof.LabelPos = cam.WorldToScreenPoint(roof.transform.position);

            Rect roofRect = new Rect(roof.LabelPos.x - 50,
                Screen.height - roof.LabelPos.y - 55, 100, 67);

            if (roofRect.Contains(mp))
            {
                foreach (RoofDefinition pickedRoof in BuildingManager.Picked.Roofs)
                    pickedRoof.IsPicked = false;

                roof.IsPicked = true;
                return roof;
            }
        }

        return null;
    }

    public static BuildingDefinition GetBuildingParent(GameObject go)
    {
        BuildingDefinition bc = go.GetComponent<BuildingDefinition>();
        Transform curr = go.transform;
        while (bc == null)
        {
            curr = curr.transform.parent;
            if (curr != null && curr.name != null)
            {
                bc = curr.GetComponent<BuildingDefinition>();
            }
            else
                return null;
        }
        return bc;
    }

    private static RoofDefinition GetRoofParent(GameObject go)
    {
        RoofDefinition bc = go.GetComponent<RoofDefinition>();
        Transform curr = go.transform;
        while (bc == null)
        {
            curr = curr.transform.parent;
            if (curr != null && curr.name != null)
            {
                bc = curr.GetComponent<RoofDefinition>();
            }
            else
                return null;
        }
        return bc;        
    }
}
