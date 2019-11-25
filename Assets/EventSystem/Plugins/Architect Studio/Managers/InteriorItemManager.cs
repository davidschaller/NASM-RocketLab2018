using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public static class InteriorItemManager
{
    private static List<InteriorItemDefinition> interiorItemsList;
    public static List<InteriorItemDefinition> InteriorItemsList
    {
        get
        {
            if (interiorItemsList == null)
            {

            }

            return interiorItemsList;
        }
    }

    /// <summary>
    /// 
    /// Initialization
    /// </summary>
    public static void Init(GameObject goItemsContainer)
    {
        InteriorItemDefinition[] interiorItems = goItemsContainer.GetComponentsInChildren<InteriorItemDefinition>();

        if (interiorItems != null)
        {
            interiorItemsList = new List<InteriorItemDefinition>();
            interiorItemsList.AddRange(interiorItems);
        }
    }

    /// <summary>
    /// 
    /// Place all interior items to the right panel
    /// </summary>
    /// 
    public static void InstantiateInteriorItems()
    {
        foreach (InteriorItemDefinition item in interiorItemsList)
        {
            if (item.model != null)
            {
                GameObject model = GameObject.Instantiate(item.model, item.transform.position, item.transform.rotation) as GameObject;

                model.name = "Model";
                model.transform.parent = item.transform;

                item.DeactivateRenderers();
            }
        }
    }

    public static List<InteriorItemDefinition> GetItemsByTabs(InteriorDesignGUI.Tabs p_tab, InteriorDesignGUI.SubTabs? p_subtab, bool asNavigation)
    {
        return interiorItemsList.FindAll(delegate(InteriorItemDefinition p) { return p.tab == p_tab && (p.subtab == p_subtab || p_subtab == null) && p.asNavigation == asNavigation; });
    }
    
    public static void ActivateAllInteriorItems()
    {
        if (interiorItemsList == null)
            return;

        foreach (InteriorItemDefinition item in interiorItemsList)
        {
            if (item != null)
                item.gameObject.SetActiveRecursively(true);
        }
    }

    public static void DeactivateAllInteriorItems()
    {
        if (interiorItemsList == null)
            return;

        foreach (InteriorItemDefinition item in interiorItemsList)
        {
            if (item != null)
                item.gameObject.SetActiveRecursively(false);
        }
    }
     
}
