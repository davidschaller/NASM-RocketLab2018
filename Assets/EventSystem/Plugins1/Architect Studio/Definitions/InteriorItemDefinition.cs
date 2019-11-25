using UnityEngine;
using System.Collections;

public class InteriorItemDefinition : ItemDefinition
{
    public bool isCorrupted = false;
    public Material material;
    public bool asNavigation = false; // use this to separate wall navigation buttons and wall covering items

    public InteriorDesignGUI.Tabs tab;
    public InteriorDesignGUI.SubTabs subtab = InteriorDesignGUI.SubTabs.Draw;

    public Vector3 lightOffSet;

    private float angle = 0;
    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
            if (angle >= 360)
                angle = 0;
        }
    }

    private GameObject doorHeadersContainer;
    private void InstantiateWallHeaders(Transform p_parent)
    {
        if (!doorHeadersContainer)
            doorHeadersContainer = GameObject.Find("Door headers");

        if (!doorHeadersContainer)
        {
            Debug.LogWarning("Coudn't find Door Headers Game Object");
            return;
        }

        foreach (Transform tr in doorHeadersContainer.transform)
        {
            GameObject wallHeader = (GameObject)GameObject.Instantiate(tr.gameObject, p_parent.position, p_parent.rotation);
            wallHeader.transform.parent = p_parent;
            wallHeader.name = tr.name;

            switch (BuildingManager.Picked.PickedMarkerType)
            {
                case RoofMarker.MarkerTypes.m10Foot:
                    if (wallHeader.name == "2FootDoorHeader")
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Interior");
                    else
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Invisible");
                    break;
                case RoofMarker.MarkerTypes.m12Foot:
                    if (wallHeader.name == "4FootDoorHeader")
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Interior");
                    else
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Invisible");
                    break;
                case RoofMarker.MarkerTypes.m15Foot:
                    if (wallHeader.name == "7FootDoorHeader")
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Interior");
                    else
                        wallHeader.gameObject.layer = LayerMask.NameToLayer("Invisible");
                    break;
                default:
                    wallHeader.gameObject.layer = LayerMask.NameToLayer("Invisible");
                    break;
            }

        }
    }

    public new InteriorItemDefinition CreateCopy(Transform p_parent, GUIStyle p_style)
    {
        InteriorItemDefinition newItem = (InteriorItemDefinition)GameObject.Instantiate(this);

        newItem.ItemRect = GetOriginalItemRect(this.ItemRect,
                                       p_style,
                                       newItem.image ? new GUIContent(newItem.image) : new GUIContent(newItem.text),
                                       labelOffSet);

        newItem.transform.localScale = newItem.scale;

        //Debug.Log("new item rect is " + newItem.ItemRect);
        if (newItem.tab == InteriorDesignGUI.Tabs.Furnishings && newItem.subtab == InteriorDesignGUI.SubTabs.Lamps)
        {
            AddLightToItem(newItem);
        }

        
        if (p_parent)
            newItem.transform.parent = p_parent;

        // Need to instantiate wall headers for doors
        if (this.tab == InteriorDesignGUI.Tabs.Openings && p_parent.name != "Preview Placeholder")
            InstantiateWallHeaders(newItem.transform);

        return newItem;
    }

    private void AddLightToItem(InteriorItemDefinition newItem)
    {
        if (newItem.Model)
        {
            GameObject lightGO = new GameObject("Point Light");
            lightGO.transform.position = newItem.Model.transform.position;

            lightGO.transform.parent = newItem.Model;
            lightGO.transform.localPosition += newItem.lightOffSet;
            Light light = lightGO.transform.gameObject.AddComponent<Light>();
            light.intensity = 0.7f;
            CameraSwitcher.UpdateLightsList(light, true);
            light.gameObject.active = false;
        }
    }

    internal InteriorItemDefinition CreateCopy(Transform p_parent, GUIStyle p_style, float angle, Vector3 newPosition, Rect newRect)
    {
        Quaternion itemRotation = new Quaternion();
        itemRotation.eulerAngles = new Vector3(0, (angle + 90), 0);

        InteriorItemDefinition newItem = (InteriorItemDefinition)GameObject.Instantiate(this, newPosition, itemRotation);

        //Debug.Log("Trying to instantiate item with name: " + this.name + ", " + this.gameObject.name);
        if (newItem.tab == InteriorDesignGUI.Tabs.Furnishings && newItem.subtab == InteriorDesignGUI.SubTabs.Lamps)
        {
            AddLightToItem(newItem);
        }

        newItem.transform.localScale = newItem.scale;

        newItem.ItemRect = GetOriginalItemRect(newRect,
                                       p_style,
                                       newItem.image ? new GUIContent(newItem.image) : new GUIContent(newItem.name),
                                       labelOffSet);

        newItem.ClosestMarker = newPosition;
        newItem.Angle = angle;

        if (p_parent)
        {
            newItem.transform.parent = p_parent;
        }

        // Need to instantiate wall headers for doors
        if (this.tab == InteriorDesignGUI.Tabs.Openings && p_parent.name != "Preview Placeholder")
            InstantiateWallHeaders(newItem.transform);

        return newItem;
    }
}
