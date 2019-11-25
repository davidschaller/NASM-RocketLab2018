using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CoveringsManager
{
    private static List<WallSection> coverings = new List<WallSection>();
    public static List<WallSection> Coverings
    {
        get
        {
            return coverings;
        }
        set
        {
            coverings = value;
        }
    }

    public static void LoadCoverings(Camera p_camera)
    {
        foreach (WallSection item in coverings)
        {

            bool ischanged = ChangeExteriorCoverings(item.Pos, Common.GetMaterialByName(item.InteriorMaterialName), p_camera);

            if (!ischanged)
            {
                foreach (Transform tr in BuildingManager.Picked.InteriorWalls)
                {
                    if (tr.position == item.Pos)
                    {
                        Renderer renderer = tr.GetChild(0).GetComponent<Renderer>();

                        Material[] materials = renderer.materials;

                        materials[0] = Common.GetMaterialByName(item.InteriorMaterialName);
                        materials[1] = Common.GetMaterialByName(item.ExteriorMaterialName);
                        renderer.materials = materials;
                    }
                }
            }
        }

        /*
        for (int i = 0; i < coverings.Count - 1; i++)
        {
            ChangeExteriorCoverings(coverings[i].Pos, Common.GetMaterialByName(coverings[i].InteriorMaterialName), p_camera);
        }
         */
    }

    public static bool ChangeExteriorCoverings(Vector3 p_wallPosition, Material p_material, Camera p_camera)
    {
        bool isChanged = false;

        foreach (Transform tr in BuildingManager.Picked.ExteriorWalls)
        {
            Vector2 v1 = Common.ToScreen2D(tr.Find("DoorMarker").position, p_camera);
            Vector2 v2 = Common.ToScreen2D(p_wallPosition, p_camera);

            if ((v1 - v2).magnitude < 11)
            {
                Renderer renderer = tr.GetComponent<Renderer>();
                Material[] materials = renderer.materials;

                if (CorruptedParser.IsCorrupted(tr))
                {
                    materials[0] = p_material;
                }
                else
                    materials[1] = p_material;

                renderer.materials = materials;

                isChanged = true;

                ChangeExteriorExtentionsCoverings(tr, p_material);

                if (!coverings.Exists(delegate(WallSection p) { return p.Pos == p_wallPosition && p.InteriorMaterialName.Equals(p_material.name); }))
                {
                    coverings.Add(new WallSection(p_wallPosition, p_material.name, p_material.name));
                }
            }
        }

        return isChanged;
    }

    private static void ChangeExteriorExtentionsCoverings(Transform p_wallSection, Material p_material)
    {
        foreach (Transform item in p_wallSection)
        {
            if (!item.name.Equals("DoorMarker"))
            {
                item.GetComponent<Renderer>().material = p_material;
            }
        }
    }

    public static bool ChangeInteriorCoverings(Vector3 p_wallPosition, bool p_isInterior, InteriorItemDefinition p_pickedCoverings)
    {
        bool isChanged = false;

        foreach (Transform tr in BuildingManager.Picked.InteriorWalls)
        {
            if (tr.position == p_wallPosition)
            {
                isChanged = true;

                if (p_pickedCoverings.material != null)
                {
                    Renderer renderer = tr.GetChild(0).GetComponent<Renderer>();

                    Material[] materials = renderer.materials;

                    if (renderer.materials.Length > 1)
                    {
                        if (p_isInterior)
                        {
                            materials[0] = p_pickedCoverings.material;
                        }
                        else
                            materials[1] = p_pickedCoverings.material;

                        renderer.materials = materials;
                    }

                    if (!coverings.Exists(delegate(WallSection p) { return p.Pos == tr.position &&
                            p.InteriorMaterialName.Equals(tr.GetChild(0).GetComponent<Renderer>().materials[0].name.Replace("(Instance)", "").Trim()) &&
                                p.InteriorMaterialName.Equals(tr.GetChild(0).GetComponent<Renderer>().materials[1].name.Replace("(Instance)", "").Trim());
                    }))
                    {
                        coverings.Add(new WallSection(tr.position, tr.GetChild(0).GetComponent<Renderer>().materials[0].name.Replace("(Instance)", "").Trim(), tr.GetChild(0).GetComponent<Renderer>().materials[1].name.Replace("(Instance)", "").Trim()));
                    }
                }
                else
                {
                    Debug.LogWarning(@"You are trying to apply empty covering. Make sure that material in InterItemDefinition is not null.");
                }
            }
        }

        return isChanged;
    }
}

