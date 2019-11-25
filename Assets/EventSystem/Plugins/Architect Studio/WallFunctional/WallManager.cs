using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class WallManager
{
    private static List<List<WallSection>> walls = new List<List<WallSection>>();
    public static List<List<WallSection>> Walls
    {
        get
        {
            return walls;
        }
    }

    public static void RemoveAllWalls()
    {
        walls.Clear();
    }

    /// <summary>
    ///  Erase functionality. 
    /// </summary>
    /// <param name="p_wall"></param>
    public static void RemoveWallSection(List<WallSection> p_wall)
    {
        Vector3 wallPosition = Vector3.zero;

        for (int i = 0; i < p_wall.Count - 1; i++)
        {
            bool rotate90 = false;
            wallPosition = WallManager.GetWallPosition(p_wall[i].Pos, p_wall[i + 1].Pos, out rotate90);

            if (wallPosition != Vector3.zero)
            {
                ReconstructWall(p_wall[i].Pos, p_wall[i + 1].Pos);

                foreach (Transform tr in BuildingManager.Picked.InteriorWalls)
                {
                    if (tr.position == wallPosition)
                    {
                        GameObject.Destroy(tr);
                    }
                }
            }

            wallPosition = Vector3.zero;
        }
    }

    private static void ReconstructWall(Vector3 p_wallPoint1, Vector3 p_wallPoint2)
    {
        List<WallSection> modifiedWall = walls.Find(delegate(List<WallSection> wall)
        {
            return wall.Exists(delegate(WallSection section) {return section.Pos == p_wallPoint1;}) 
                    && 
                   wall.Exists(delegate(WallSection section) {return section.Pos == p_wallPoint2;});
        });

        if (modifiedWall != null)
        {
            int wallPoint1Index = modifiedWall.FindIndex(delegate(WallSection p)
            {
                return p.Pos == p_wallPoint1;
            });

            int wallPoint2Index = modifiedWall.FindIndex(delegate(WallSection p)
            {
                return p.Pos == p_wallPoint2;
            });

            if (wallPoint1Index > wallPoint2Index)
            {
                int temp = wallPoint1Index;
                wallPoint1Index = wallPoint2Index;
                wallPoint2Index = temp;
            }

            List<WallSection> wallPart1 = new List<WallSection>(),
                              wallPart2 = new List<WallSection>();

            for (int i = 0; i <= wallPoint1Index; i++)
                wallPart1.Add(modifiedWall[i]);

            for (int i = wallPoint2Index; i < modifiedWall.Count; i++)
                wallPart2.Add(modifiedWall[i]);

            RemoveWall(modifiedWall);

            if (wallPart1.Count > 1)
                AddWall(wallPart1);

            if (wallPart2.Count > 1)
                AddWall(wallPart2);
        }
        else
            Debug.LogWarning("WARNING: Wall section was removed but reconstruction failed. Can't find the wall");
    }

    /// <summary>
    /// Instantiate wall models
    /// </summary>
    /// <param name="p_wall">Wall position</param>
    public static void InstantiateWallSections(List<WallSection> p_wall, List<GameObject> p_wallPrefabs)
    {
        for (int i = 0; i < p_wall.Count - 1; i++)
        {
            if ((p_wall[i].Pos - p_wall[i + 1].Pos).magnitude == 1)
            {
                // Check if wall is inside the building
                if (IsWallInsideBuilding(p_wall[i].Pos, p_wall[i + 1].Pos))
                {
                    // Get wall position. It is between the wall points
                    bool rotate90 = false;
                    Vector3 wallPosition = GetWallPosition(p_wall[i].Pos, p_wall[i + 1].Pos, out rotate90);
                    p_wall[i + 1].Rotate90 = rotate90;


                    if (!IsInstantiated(wallPosition))
                    {
                        InstantiateWallSection(wallPosition, p_wallPrefabs, rotate90, p_wall[i].InteriorMaterialName, p_wall[i].ExteriorMaterialName);
                    }
                }
            }
        }
    }

    private static float accuracy = 0.2f;  // Sometimes can't cast Ray to floors - it's a little bit offside of our start point
                                           // So let's cast a small 'Rect' around the original Ray size of Step

    public static float Accuracy
    {
        get
        {
            return accuracy;
        }
    }

    /// <summary>
    /// Returns true if both wall points are inside the building.
    /// </summary>
    /// <param name="p_wallPoint1">Wall section first point</param>
    /// <param name="p_wallPoint2">Wall section second point</param>
    /// <returns></returns>
    public static bool IsWallInsideBuilding(Vector3 p_wallPoint1, Vector3 p_wallPoint2)
    {
        // Create several rays around original direction

        List<Ray> listRay1 = new List<Ray>();
        listRay1.Add(new Ray(p_wallPoint1, Vector3.down)); // original Ray
        listRay1.Add(new Ray(p_wallPoint1 + new Vector3(1, 0, 0) * accuracy, Vector3.down));
        listRay1.Add(new Ray(p_wallPoint1 + new Vector3(-1, 0, 0) * accuracy, Vector3.down));
        listRay1.Add(new Ray(p_wallPoint1 + new Vector3(0, 0, 1) * accuracy, Vector3.down));
        listRay1.Add(new Ray(p_wallPoint1 + new Vector3(0, 0, -1) * accuracy, Vector3.down));

        // The second wall point should be inside the building too
        List<Ray> listRay2 = new List<Ray>();
        listRay2.Add(new Ray(p_wallPoint2, Vector3.down));
        listRay2.Add(new Ray(p_wallPoint2 + new Vector3(1, 0, 0) * accuracy, Vector3.down));
        listRay2.Add(new Ray(p_wallPoint2 + new Vector3(-1, 0, 0) * accuracy, Vector3.down));
        listRay2.Add(new Ray(p_wallPoint2 + new Vector3(0, 0, 1) * accuracy, Vector3.down));
        listRay2.Add(new Ray(p_wallPoint2 + new Vector3(0, 0, -1) * accuracy, Vector3.down));

        bool isAbsolute1 = true;
        bool isAbsolute2 = true;

        bool isInside1 = Raycast(listRay1, "Floor|Door", out isAbsolute1);
        bool isInside2 = Raycast(listRay2, "Floor|Door", out isAbsolute2);

        return (isInside1 && isInside2) && !(!isAbsolute1 && !isAbsolute2);
    }

    private static bool Raycast(List<Ray> p_listRay, string p_hitPartName, out bool o_isAbsolute)
    {
        bool result = false;
        o_isAbsolute = true;

        RaycastHit hit;

        foreach (Ray ray in p_listRay)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (Regex.IsMatch(hit.transform.name, p_hitPartName))
                {
                    result = true;
                }
                else
                    o_isAbsolute = false;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns position where wall should be instantiated
    /// </summary>
    /// <param name="start">Start wall point</param>
    /// <param name="end">End wall point</param>
    /// <param name="rotate90">If the wall line is vertical the model should be rotated</param>
    /// <returns></returns>
    public static Vector3 GetWallPosition(Vector3 p_wallPoint1, Vector3 p_wallPoint2, out bool rotate90)
    {
        Vector3 result = Vector3.zero;
        rotate90 = false;

        // Always! Magnitude between each point pair is 1
        if ((p_wallPoint1 - p_wallPoint2).magnitude != 1)
            return result;

        // Vertical line
        if (p_wallPoint1.x == p_wallPoint2.x)
        {
            rotate90 = true;
            if (p_wallPoint1.z > p_wallPoint2.z)
            {
                result = new Vector3(p_wallPoint1.x, BuildingManager.Picked.Floor.position.y, p_wallPoint1.z - 0.5f);
            }
            else
            {
                result = new Vector3(p_wallPoint1.x, BuildingManager.Picked.Floor.position.y, p_wallPoint1.z + 0.5f);
            }
        }
        // Horizontal line
        else if (p_wallPoint1.z == p_wallPoint2.z)
        {
            if (p_wallPoint1.x > p_wallPoint2.x)
            {
                result = new Vector3(p_wallPoint1.x - 0.5f, BuildingManager.Picked.Floor.position.y, p_wallPoint2.z);
            }
            else
            {
                result = new Vector3(p_wallPoint1.x + 0.5f, BuildingManager.Picked.Floor.position.y, p_wallPoint2.z);
            }
        }

        // The result position is between start and end points
        return result;
    }

    /// <summary>
    /// Make sure if there is no wall are instantiated already
    /// </summary>
    /// <param name="p_wallPosition"></param>
    /// <returns></returns>
    private static bool IsInstantiated(Vector3 p_wallPosition)
    {
        foreach (Transform tr in BuildingManager.Picked.InteriorWalls)
        {
            if (tr.position == p_wallPosition)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check if wall model prefab is instantiated between wall points pair
    /// </summary>
    /// <param name="p_wallPoint1">Start wall point</param>
    /// <param name="p_wallPoint2">End wall point</param>
    /// <returns></returns>
    public static bool IsInstantiated(Vector3 p_wallPoint1, Vector3 p_wallPoint2)
    {
        bool rotate = false;
        Vector3 wallPosition = GetWallPosition(p_wallPoint1, p_wallPoint2, out rotate);

        return IsInstantiated(wallPosition);
    }

    /// <summary>
    /// Instantiate all kind of walls to this position (8, 10, 12 and 15)
    /// </summary>
    /// <param name="wallPosition"></param>
    /// <param name="p_wallPrefabs"></param>
    /// <param name="p_rotate90"></param>
    /// <param name="interiorCoveringsName"></param>
    /// <param name="exteriorCoveringsName"></param>
    private static void InstantiateWallSection(Vector3 p_wallPosition, List<GameObject> p_wallPrefabs,
        bool p_rotate90, string p_interiorCoveringsName, string p_exteriorCoveringsName)
    {
        foreach (GameObject item in p_wallPrefabs)
        {
            GameObject interiorWallModel = (GameObject)GameObject.Instantiate(item, p_wallPosition, item.transform.rotation);
            
            

            Renderer renderer = interiorWallModel.transform.GetChild(0).GetComponent<Renderer>();

            Material[] materials = renderer.materials;

            if (renderer.materials.Length > 1)
            {
                materials[0] = Common.GetMaterialByName(p_interiorCoveringsName);
                materials[1] = Common.GetMaterialByName(p_exteriorCoveringsName);

                renderer.materials = materials;
            }

            interiorWallModel.name = item.name;

            if (p_rotate90)
                interiorWallModel.transform.eulerAngles = new Vector3(0, 90, 0);

            interiorWallModel.transform.parent = BuildingManager.Picked.InteriorWalls;

            // But show only walls with current height
            BuildingManager.Picked.ShowWallByRoofMarker(interiorWallModel.transform, BuildingManager.Picked.PickedMarkerType);
        }
    }

    public static void AddWall(List<WallSection> p_wallPoints)
    {
        List<WallSection> newWall = new List<WallSection>();

        foreach (WallSection item in p_wallPoints)
            newWall.Add(item);

        walls.Add(newWall);
    }

    private static void RemoveWall(List<WallSection> p_modifiedWall)
    {
        walls.Remove(p_modifiedWall);
    }

    public static WallSection GetClosestWallSection(Vector3 p_anchorPos)
    {
        foreach (List<WallSection> wall in WallManager.walls)
        {
            WallSection wallSection = wall.Find(delegate(WallSection p) { return p.Pos == p_anchorPos; });

            if (wallSection != null)
            {
                return wallSection;
            }
        }

        return null;
    }

    public static Vector3 GetClosestWallSectionPair(Vector3 p_anchorPos, Vector3 current)
    {
        foreach (List<WallSection> wall in WallManager.walls)
        {
            WallSection section = wall.Find(delegate(WallSection p) { return p.Pos == p_anchorPos; });

            if (section != null && current != section.Pos)
            {
                current = section.Pos;
            }
        }
        return current;
    }

    public static void MakeWallsVisible()
    {
        MakeInteriorWallsVisible();
        MakeExteriorWallsVisible();
    }

    private static void MakeInteriorWallsVisible()
    {
        Transform interiorWallsContainer = BuildingManager.Picked.InteriorWalls;
        foreach (Transform tr in interiorWallsContainer)
            BuildingManager.Picked.ShowWallByRoofMarker(tr, BuildingManager.Picked.PickedMarkerType);
    }

    private static void MakeExteriorWallsVisible()
    {
        foreach (Transform tr in BuildingManager.Picked.ExteriorWalls)
        {
            tr.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }
}

