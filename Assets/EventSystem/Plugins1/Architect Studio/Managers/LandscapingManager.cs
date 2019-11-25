using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LandscapingManager
{
    private static List<ItemDefinition> landscapingList;
    public static List<ItemDefinition> LandscapingList
    {
        get
        {
            return landscapingList;
        }
    }

    static GameObject landscapingContainer = null;

    public static void Init()
    {
        landscapingList = new List<ItemDefinition>();
        float time = 0;
        while (landscapingList.Count == 0 && time < 5)
        {
            if (landscapingContainer == null)
                landscapingContainer = GameObject.Find("Landscaping Items");

            ItemDefinition[] landscapingItems = landscapingContainer.GetComponentsInChildren<ItemDefinition>();
            if (landscapingItems != null)
            {
                landscapingList.AddRange(landscapingItems);
            }

            time += Time.deltaTime;
        }
        InstantiateLandscapings();

        landscapingList.Sort(delegate(ItemDefinition p1, ItemDefinition p2)
            { return p1.sortKey.CompareTo(p2.sortKey); });
    
        DeactivateLandscapings();
    }

    public static void InstantiateLandscapings()
    {
        foreach (ItemDefinition item in landscapingList)
        {
            if (item.model != null)
            {
                GameObject model = (GameObject)GameObject.Instantiate(item.model, item.transform.position, item.transform.rotation);

                model.name = "Model";
                model.transform.parent = item.transform;

                item.DeactivateRenderers();
            }
        }
    }

    public static void DeactivateLandscapings()
    {
        foreach (ItemDefinition c in landscapingList)
        {
            if (c != null)
                c.gameObject.SetActiveRecursively(false);
        }
    }

    public static void ActivateLandscapings()
    {
        foreach (ItemDefinition c in landscapingList)
        {
            if (c != null)
                c.gameObject.SetActiveRecursively(true);
        }
    }
}
