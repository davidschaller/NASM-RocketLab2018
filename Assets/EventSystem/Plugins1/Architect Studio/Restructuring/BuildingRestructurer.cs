using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Text.RegularExpressions;

public static class BuildingRestructurer
{
    public static BuildingDefinition RestructureRoofs(BuildingDefinition p_buildingDefinition, GameObject p_model)
    {
        BuildingDefinition result = p_buildingDefinition;

        Renderer[] childModels = p_model.GetComponentsInChildren<Renderer>();

        Transform goRoofs = result.transform.Find("Roofs");
        if (goRoofs)
        {
            RoofDefinition[] roofDefinitions = goRoofs.GetComponentsInChildren<RoofDefinition>();

            foreach (Renderer item in childModels)
            {
                if (Regex.IsMatch(item.name, "Crenulated|Crentulated|Flat|Funky|Hip|Gable|Rollerball|Gambrel|HipCrenulated", RegexOptions.IgnoreCase))
                {
                    foreach (RoofDefinition roof in roofDefinitions)
                    {
                        if (item.name.Equals(roof.name))
                        {
                            Renderer itemCopy = (Renderer)GameObject.Instantiate(item, Vector3.zero, item.transform.rotation);

                            itemCopy.transform.parent = roof.transform;
                            itemCopy.transform.localPosition = Vector3.zero;
                            itemCopy.gameObject.layer = LayerMask.NameToLayer("RoofScroll");
                            itemCopy.enabled = true;

                            Material[] materials = itemCopy.materials;

                            if (materials.Length > 1)
                            {
                                materials[roof.roofMaterialIndex] = roof.material;
                            }
                            else
                                materials[0] = roof.material;

                            itemCopy.materials = materials;


                            roof.transform.Rotate(new Vector3(0, -65, 0));

                            if (!result.Roofs.Contains(roof))
                                result.Roofs.Add(roof);
                        }
                    }
                    GameObject.DestroyImmediate(item);
                }
            }


        }
        else
            Debug.LogWarning("Roofs reconstructing failed. Roofs GameObject doesn't exists");

        result.Roofs.Sort(delegate(RoofDefinition p1, RoofDefinition p2) { return p1.id.CompareTo(p2.id); });

        return result;
    }

    public static BuildingDefinition InitializeFloors(BuildingDefinition p_buildingDefinition, Transform p_model)
    {
        BuildingDefinition result = p_buildingDefinition;

        Transform goFloor = p_model.Find("Floor");
        if (goFloor)
        {
            result.FloorTiles = new Dictionary<string, string>();
            foreach (Transform tile in goFloor)
            {
                tile.gameObject.AddComponent<MeshCollider>();
                tile.gameObject.layer = LayerMask.NameToLayer("PlanView");
                result.FloorTiles.Add(tile.name, "Wood");
            }
        }
        else
            Debug.LogWarning("Floor reconstructing failed. Floor GameObject doesn't exists");

        return result;
    }

    public static void ReconstructWallExtensions(Transform p_model)
    {
        List<Transform> walls = GetExteriorWalls(p_model);

        Dictionary<int, Transform> wallExtensions = new Dictionary<int,Transform>();
        wallExtensions.Add(4, p_model.transform.Find("4FootWallExtension"));
        wallExtensions.Add(7, p_model.transform.Find("7FootWallExtension"));
        
        foreach (Transform wall in walls)
        {
            List<string> extensionTemplates = GetExtensionTemplates(wall);

            foreach (string extensionTemplateName in extensionTemplates)
            {
                foreach (KeyValuePair<int, Transform> wallExtension in wallExtensions)
                {
                    Transform interiorWallSections = wallExtension.Value.Find("InteriorWallSections");

                    if (interiorWallSections)
                    {
                        string sectionNameToMove = string.Format("{0}{1}", wallExtension.Key, extensionTemplateName);

                        Transform sectionToMove = interiorWallSections.Find(sectionNameToMove);

                        if (sectionToMove)
                        {
                            sectionToMove.parent = wall;
                        }
                        else
                            Debug.Log("Can't move section with name " + sectionNameToMove + " " + wall.name);
                    }
                    else
                        Debug.Log("InteriorWallSections is null. Can't reconstruct wall extensions");
                }
            }
        }
    }

    public static void SetDefaultCoveringsToCorners(Transform p_model)
    {
        Transform corners = p_model.Find("Corners");
        if (corners.childCount > 0)
        {
            Transform interiorWallSections = corners.Find("InteriorWallSections");
            foreach (Transform corner in interiorWallSections)
            {
                corner.GetComponent<Renderer>().material = Common.GetMaterialByName("wall1");
            }
        }
    }

    public static void SetDefaultCoveringsToExtentions(Transform p_model)
    {
        List<Transform> walls = GetExteriorWalls(p_model);

        foreach (Transform wall in walls)
        {
            foreach (Transform tr in wall)
            {
                if (tr.name.Contains(":Layer"))
                    tr.GetComponent<Renderer>().material = Common.GetMaterialByName("wall1");
            }
        }
    }

    private static List<Transform> GetExteriorWalls(Transform p_model)
    {
        List<Transform> result = new List<Transform>();

        foreach (Transform tr in p_model)
        {
            if (Regex.IsMatch(tr.name, "Building.*:Layer"))
                result.Add(tr);
        }

        return result;
    }

    private static List<string> GetExtensionTemplates(Transform p_wall)
    {
        List<string> result = new List<string>();

        foreach (Transform tr in p_wall)
        {
            if (tr.name.Contains("FootWallExtension:Layer"))
            {
                result.Add(tr.name.Substring(1));
            }
        }

        return result;
    }
}

