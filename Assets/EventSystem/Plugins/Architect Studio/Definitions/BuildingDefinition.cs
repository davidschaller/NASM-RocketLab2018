using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;


public class BuildingDefinition : MonoBehaviour
{
    private CombineChildrenExtended combiner;
    public CombineChildrenExtended Combiner
    {
        get
        {
            return combiner;
        }
        set
        {
            combiner = value;
        }
    }

    public GameObject buildingModel;
    public int id;

    public Vector3 offSet;

    public Texture2D plan,
                     selection;

    public Texture2D[] fronts,
                       sides;

    public Vector2 lookAtOffset;

    public string corruptedSections; //36-63,33,67 or *

    // Front and Side images for Exterior Tab
    public Texture2D FrontImageByPickedHeight
    {
        get
        {
            return fronts[(int)pickedMarkerType];
        }
        private set { }
    }
    public Texture2D SideImageByPickedHeight
    {
        get
        {
            return sides[(int)pickedMarkerType];
        }
        private set { }
    }

    private List<RoofMarker> roofMarkers = new List<RoofMarker>();
    public List<RoofMarker> RoofMarkers
    {
        get
        {
            return roofMarkers;
        }
        set 
        {
            roofMarkers = value;
        }
    }

    // Need this markers to draw bold dark outline
    private List<Vector3> outsideWallMarkers = new List<Vector3>();
    public List<Vector3> OutsideWallMarkers
    {
        get
        {
            return outsideWallMarkers;
        }
        set
        {
            outsideWallMarkers = value;
        }
    }

    // Need this markers to draw bold dark outline
    private List<Vector3> insideWallMarkers = new List<Vector3>();
    public List<Vector3> InsideWallMarkers
    {
        get
        {
            return insideWallMarkers;
        }
        set
        {
            insideWallMarkers = value;
        }
    }

    public List<RoofDefinition> Roofs;

    private Dictionary<string, string> floorTiles;
    public Dictionary<string, string> FloorTiles
    {
        get
        {
            return floorTiles;
        }
        set
        {
            if (value != null)
            {
                if (FloorTiles != null)
                {
                    floorTiles = value;

                    UpdateTiles(floorTiles);
                }
                else
                {
                    floorTiles = new Dictionary<string, string>(value);
                }
            }
            
        }

    }

    private void UpdateTiles(Dictionary<string, string> p_floorTiles)
    {
        foreach (KeyValuePair<string, string> item in p_floorTiles)
        {
            Transform tile = Floor.Find(item.Key);
            if (tile)
            {
                tile.GetComponent<Renderer>().material = Common.GetMaterialByName(item.Value);
            }
        }
    }

    // Roof marker type. Need this to update wall heights and roof position.
    private RoofMarker.MarkerTypes pickedMarkerType = RoofMarker.MarkerTypes.m8Foot; // by default is 8' 2.4m
    public RoofMarker.MarkerTypes PickedMarkerType
    {
        get
        {
            return pickedMarkerType;
        }
        set
        {
            pickedMarkerType = value;
            UpdateExtensionsHeight(pickedMarkerType);
        }
    }

    private RoofDefinition pickedRoof;
    public RoofDefinition PickedRoof
    {
        get
        {
            return pickedRoof;
        }
        set
        {
            pickedRoof = value;
            UpdateRoofMaterial(pickedExteriorMaterial);
        }
    }

    private ExMatDefinition pickedExteriorMaterial;
    public ExMatDefinition PickedExteriorMaterial
    {
        get
        {
            return pickedExteriorMaterial;
        }
        set
        {
            pickedExteriorMaterial = value;
            UpdateExteriorWallMaterial(pickedExteriorMaterial);
            UpdateExtentionsMaterial(pickedExteriorMaterial);
            UpdateCornersMaterial(pickedExteriorMaterial);
            if (pickedRoof)
            {
                UpdateRoofMaterial(pickedExteriorMaterial);
            }
        }
    }

    public RoofMarker PickedRoofMarker(RoofMarker.MarkerTypes markerType)
    {
        return RoofMarkers.Find(delegate(RoofMarker p) { return p.MarkerType == markerType; } );
    }

    public Vector2 GetClosestExteriorDoorMarker(Camera cam, Vector2 pos, ref float dist, ref Transform out_realMarker)
    {
        Vector2 r_closestmarker = Vector2.zero;
        dist = 999999f;

        // DoorMarkers from the external walls
        foreach (Transform marker in exteriorWalls)
        {
            if (marker.Find("DoorMarker"))
            {
                Vector2 v = Common.ToScreen2D(marker.Find("DoorMarker").position, cam);

                // Use this for debug only
                //GUI.Box(new Rect(v.x, v.y, 2, 2), GUIContent.none, outlineRectStyle);

                float distance = (v - pos).magnitude;
                if (distance < dist)
                {
                    dist = distance;
                    r_closestmarker = v;
                    out_realMarker = marker.Find("DoorMarker");
                }
            }
        }

        return r_closestmarker;
    }

    private Vector2 GetClosestInteriorDoorMarker(Camera cam, Vector3 p_anchorPos, ref Vector3 out_realMarker)
    {
        Vector2 r_closestmarker = Vector2.zero;

        WallSection wallSection = WallManager.GetClosestWallSection(p_anchorPos);

        if (wallSection != null)
        {
            out_realMarker = new Vector3(wallSection.Pos.x, 
                 BuildingManager.Picked.Floor.position.y, wallSection.Pos.z);

            r_closestmarker = cam.WorldToScreenPoint(p_anchorPos);
            r_closestmarker.y = Screen.height - r_closestmarker.y;
        }

        return r_closestmarker;
    }


    /// <summary>
    /// 
    /// Get closest door marker from the external/internal wall to the cursor position.
    /// Use this function to position doors and windows to appropriate places
    /// </summary>
    /// <param name="cam">Current camera</param>
    /// <param name="pos">Cursor position</param>
    /// <param name="dist">Distance to the closest marker</param>
    /// 
    public Vector2 GetClosestDoorMarker(Camera cam, Vector2 pos, Vector3 p_anchorPos, out float dist, out Vector3 out_realMarker)
    {
        Vector2 r_closestmarker = Vector2.zero,
                r_interiorMarker = Vector3.zero,
                r_exteriorMarker = Vector3.zero;

        out_realMarker = Vector3.zero;
        dist = 999999f;

        Transform realMarkerTransform = null;

        r_exteriorMarker = GetClosestExteriorDoorMarker(cam, pos, ref dist, ref realMarkerTransform);

        if (realMarkerTransform)
        {
            out_realMarker = realMarkerTransform.position;
            r_closestmarker = r_exteriorMarker;
        }

        r_interiorMarker = GetClosestInteriorDoorMarker(cam, p_anchorPos, ref out_realMarker);

        if (r_interiorMarker != Vector2.zero)
        {
            dist = 0;
            r_closestmarker = r_interiorMarker;
        }

        return r_closestmarker;
    }

    // DoorMarkers from the exterior walls
    private List<Transform> exteriorWalls = new List<Transform>();
    public List<Transform> ExteriorWalls
    {
        get
        {
            return exteriorWalls;
        }
        set 
        {
            exteriorWalls = value;
        }
    }

    /// <summary>
    /// Show or Hide wall segment by roof marker type. 
    /// If marker is not null then show appropriate wall model 
    /// (for 8, 10, 12 or 15), else hide all segment.
    /// </summary>
    /// <param name="wall">Wall segment to modify</param>
    /// <param name="marker">Roof marker type</param>
    public void ShowWallByRoofMarker(Transform wall, RoofMarker.MarkerTypes? marker)
    {
        switch (marker)
        {
            case RoofMarker.MarkerTypes.m8Foot:
                if (wall.name.Contains("8"))
                {
                    foreach (Transform tr in wall)
                    {
                        tr.gameObject.layer = LayerMask.NameToLayer("Interior");
                    }
                }
                else
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                break;
            case RoofMarker.MarkerTypes.m10Foot:
                if (wall.name.Contains("10"))
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Interior");
                }
                else
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                break;
            case RoofMarker.MarkerTypes.m12Foot:
                if (wall.name.Contains("12"))
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Interior");
                }
                else
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                break;
            case RoofMarker.MarkerTypes.m15Foot:
                if (wall.name.Contains("15"))
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Interior");
                }
                else
                {
                    foreach (Transform tr in wall)
                        tr.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                break;
            default:
                foreach (Transform tr in wall)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                break;
        }


    }

    private Transform model = null;
    public Transform Model
    {
        get
        {
            if (!model)
            {
                model = transform.Find("Model");
            }

            return model;
        }
    }

    private Transform interiorWalls = null;
    public Transform InteriorWalls
    {
        get
        {
            if (!interiorWalls)
            {
                interiorWalls = transform.Find("Interior Walls");
            }
            return interiorWalls;
        }
    }

    private Transform interiorFloor = null;
    public Transform InteriorFloor
    {
        get
        {
            if (!interiorFloor)
            {
                interiorFloor = transform.Find("Interior Floors");
            }

            return interiorFloor;
        }
    }

    private Transform floor;
    public Transform Floor
    {
        get
        {
            if (!floor)
                floor = Model.Find("Floor");

            return floor;
        }
    }

    private Transform ceiling = null;
    public Transform Ceiling
    {
        get
        {
            if (!ceiling)
                ceiling = Model.Find("Ceiling");

            return ceiling;
        }
    }

    internal void UpdateLayer(string p_layer)
    {
        foreach (Transform tr in Model)
        {
            if (!LayerMask.LayerToName(tr.gameObject.layer).Equals("Invisible"))
                tr.gameObject.layer = LayerMask.NameToLayer(p_layer);
        }
        Model.gameObject.layer = LayerMask.NameToLayer(p_layer);
    }

    /// <summary>
    /// One material for all exterior wall sections.
    /// By default material with 0 index is exterior.
    /// </summary>
    /// <param name="p_pickedExteriorMaterial"></param>
    private void UpdateExteriorWallMaterial(ExMatDefinition p_pickedExteriorMaterial)
    {
        foreach (Transform wall in exteriorWalls)
        {
            Renderer renderer = wall.GetComponent<Renderer>();
            Material[] materials = renderer.materials;

            if (CorruptedParser.IsCorrupted(wall))
            {
                materials[1] = p_pickedExteriorMaterial.material;
            }
            else
                materials[0] = p_pickedExteriorMaterial.material;

            renderer.materials = materials;
        }
    }

    private void UpdateExtentionsMaterial(ExMatDefinition p_pickedExteriorMaterial)
    {
        Renderer[] childModels =
            BuildingManager.Picked.Model.GetComponentsInChildren<Renderer>();

        foreach (Renderer item in childModels)
        {
            if (item.name.Contains("FootWallExtensionExterior"))
                item.material = p_pickedExteriorMaterial.material;
        }
    }

    private void UpdateCornersMaterial(ExMatDefinition p_picked)
    {
        Transform corners = Model.Find("Corners");
        Transform exCorners = corners.Find("CornersExterior");

        if (exCorners)
        {
            exCorners.GetComponent<Renderer>().material = p_picked.material;
        }
        else
            corners.GetComponent<Renderer>().material = p_picked.material;
    }

    private void UpdateRoofMaterial(ExMatDefinition p_picked)
    {
        Material[] materials = pickedRoof.transform.GetChild(0).GetComponent<Renderer>().materials;

        if (materials.Length > 1)
        {
            materials[pickedRoof.roofMaterialIndex == 0 ? 1 : 0] = p_picked.material;

            pickedRoof.transform.GetChild(0).GetComponent<Renderer>().materials = materials;        
        }
    }

    private void UpdateExtensionsHeight(RoofMarker.MarkerTypes markerType)
    {
        foreach (Transform wall in BuildingManager.Picked.ExteriorWalls)
        {
            foreach (Transform item in wall)
            {
                if (item.name.Contains(RoofMarker.GetWallExtension(markerType)))
                {
                    item.gameObject.layer = LayerMask.NameToLayer("Default");
                }
                else if (item.name.Contains("FootWallExtension"))
                {
                    item.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
            }
        }

        List<Transform> exteriorExtensions = GetExteriorExtensions();

        foreach (Transform item in exteriorExtensions)
        {
            if (item.name.Contains(RoofMarker.GetWallExtension(markerType)))
            {
                item.gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                item.gameObject.layer = LayerMask.NameToLayer("Invisible");
            }
        }
    }

    private List<Transform> GetExteriorExtensions()
    {
        List<Transform> result = new List<Transform>();

        result.Add(BuildingManager.Picked.Model.Find("2FootWallExtension").Find("2FootWallExtensionExterior"));
        result.Add(BuildingManager.Picked.Model.Find("4FootWallExtension").Find("4FootWallExtensionExterior"));
        result.Add(BuildingManager.Picked.Model.Find("7FootWallExtension").Find("7FootWallExtensionExterior"));

        return result;
    }

    public void ActivateRenderers()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = true;
    }

    public void DeactivateRenderers(bool keepRoofs, bool keepFloors)
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if ((renderer.name.Equals("Deck") || renderer.name.Contains("Floor")))
            {
                if (keepFloors)
                    continue;
            }

            if (renderer && renderer.transform.parent && renderer.transform.parent.name.Equals("Roof (3D View)"))
            {
                if (keepRoofs)
                {
                    continue;
                }
            }

            renderer.enabled = false;
        }
    }
}
