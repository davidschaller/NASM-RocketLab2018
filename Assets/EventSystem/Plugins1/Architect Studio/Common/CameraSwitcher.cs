using UnityEngine;
using System.Collections.Generic;

public static class CameraSwitcher
{
    private static bool is3DView = false;
    public static bool Is3DView
    {
        get
        {
            return is3DView;
        }
        set 
        {
            
            if (value != is3DView)
            {
                if (value)
                {
                    if (lightList == null)
                    {
                        UpdateLightsList();
                    }
                    SwitchOffAmbient();
                    TourHouseGUI.Combine();
                }
                else
                {
                    SwitchOnAmbient();
                    TourHouseGUI.Decombine();
                }

            }
            is3DView = value;
        }
    }

    private static void SwitchOnAmbient()
    {
        //RenderSettings.ambientLight = new Color(1f, 1f, 1f);

        if (lightList != null)
        {
            foreach (Light light in lightList)
            {
                if (light && light.gameObject.layer != LayerMask.NameToLayer("BackPlane") && light.gameObject.layer != LayerMask.NameToLayer("AlwaysActive") && light.gameObject.layer != LayerMask.NameToLayer("PlanView"))
                {
                    light.enabled = false;
                }
                else if (light && light.gameObject.layer == LayerMask.NameToLayer("PlanView"))
                {
                    light.enabled = true;
                }
            }
        }
    }

    private static void SwitchOffAmbient()
    {
        //RenderSettings.ambientLight = new Color(0.5f, 0.5f, 0.5f); 

        if (lightList != null)
        {
            foreach (Light light in lightList)
            {
                if (light && light.gameObject.layer != LayerMask.NameToLayer("BackPlane") && light.gameObject.layer != LayerMask.NameToLayer("AlwaysActive") && light.gameObject.layer != LayerMask.NameToLayer("PlanView"))
                {
                    light.enabled = true;
                }
                else if (light && light.gameObject.layer == LayerMask.NameToLayer("PlanView"))
                {
                    light.enabled = false;
                }
            }
        }
    }
    

    private static List<Light> lightList;

    private static void UpdateLightsList()
    {
        Light[] lights = (Light[])GameObject.FindObjectsOfType(typeof(Light));
        if (lights != null)
        {
            lightList = new List<Light>();
            lightList.AddRange(lights);
        }
    }

    public static void UpdateLightsList(Light p_light, bool p_add)
    {
        if (lightList == null)
            UpdateLightsList();

        if (p_add)
            lightList.Add(p_light);
        else
        {
            if (lightList.Contains(p_light))
                lightList.Remove(p_light);
        }
    }

    private static float fieldOfView = 0;
    private static Rect rect = new Rect(0, 0, 0, 0);

    public static Camera ExpandView(Camera p_cam3D)
    {
        if (p_cam3D && fieldOfView == 0)
        {
            fieldOfView = p_cam3D.GetComponent<Camera>().fieldOfView;
            rect = p_cam3D.rect;

            p_cam3D.fieldOfView = 55;
            p_cam3D.rect = new Rect(0.15f, 0.23f, 0.84f, 0.73f);
        }
        return p_cam3D;
    }

    public static Camera RestoreView(Camera p_cam3D)
    {
        if (p_cam3D && fieldOfView != 0)
        {
            p_cam3D.rect = rect;
            p_cam3D.fieldOfView = fieldOfView;

            fieldOfView = 0;
        }
        return p_cam3D;
    }
}