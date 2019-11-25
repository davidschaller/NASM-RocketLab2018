using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class CorruptedParser
{
    private static List<string> corruptedSectionsList;
    public static void ClearCorruptedSectionsList()
    {
        corruptedSectionsList = null;
    }

    internal static bool IsCorrupted(Transform tr)
    {
        if (corruptedSectionsList == null)
        {
            corruptedSectionsList = ParseCorruptedSections(BuildingManager.Picked.corruptedSections);
        }

        return corruptedSectionsList.Exists(delegate(string sectionName)
        { return sectionName.Equals(tr.name); }
        );
    }

    private static List<string> ParseCorruptedSections(string p_input)
    {
        List<string> result = new List<string>();

        string firstSectionName = BuildingManager.Picked.ExteriorWalls[0].name,
               buildingName = firstSectionName.Split(':')[0]; 


        if (p_input.Contains("*"))
        {
            foreach (Transform section in BuildingManager.Picked.ExteriorWalls)
            {
                result.Add(section.name);
            }
        }
        else if (p_input.Contains(","))
        {
            string[] groups = p_input.Split(',');

            foreach (string group in groups)
            {
                if (group.Contains("-"))
                {
                    int start = int.Parse(group.Split('-')[0]),
                        end = int.Parse(group.Split('-')[1]);

                    for (int i = start; i <= end; i++)
                    {
                        result.Add(string.Format("{0}:Layer{1}", buildingName, i));
                    }
                }
                else
                {
                    result.Add(string.Format("{0}:Layer{1}", buildingName, group));
                }
            }

            
        }
        else if (!string.IsNullOrEmpty(p_input))
        {
            if (p_input.Contains("-"))
            {
                int start = int.Parse(p_input.Split('-')[0]),
                    end = int.Parse(p_input.Split('-')[1]);

                for (int i = start; i <= end; i++)
                {
                    result.Add(string.Format("{0}:Layer{1}", buildingName, i));
                }
            }
            else
            {
                result.Add(string.Format("{0}:Layer{1}", buildingName, p_input));
            }
        }

        return result;
    }
}

