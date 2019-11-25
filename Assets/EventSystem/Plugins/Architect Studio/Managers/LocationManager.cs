using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class LocationManager
{
    private static List<LocationDefinition> locations = new List<LocationDefinition>();
    public static List<LocationDefinition> Locations
    {
        get
        {
            return locations;
        }
        private set { }
    }

    private static LocationDefinition pickedLocation;
    public static LocationDefinition PickedLocation
    {
        get
        {
            return pickedLocation;
        }
        set
        {
            RenderSettings.skybox = null;

            GameObject goImportedScene = GameObject.Find("Imported Scene");
            if (goImportedScene)
                GameObject.DestroyImmediate(goImportedScene);

            pickedLocation = value;

            if (pickedLocation.IsDownloaded())
            {
                pickedLocation.Instantiate();
            }
            else if (!LocationManager.PickedLocation.IsDownloaded() && !LocationManager.PickedLocation.IsDownloading())
            {
                pickedLocation.Download();
            }
        }
    }

    public static void Init()
    {
        LocationDefinition[] locationsArray = (LocationDefinition[])GameObject.FindObjectsOfType(typeof(LocationDefinition));

        if (locationsArray != null)
        {
            locations = new List<LocationDefinition>();
           
            locations.AddRange(locationsArray);
            locations.Sort(delegate(LocationDefinition p1, LocationDefinition p2)
            { return p1.id.CompareTo(p2.id); });

            if (streamList == null)
            {
                streamList = new List<WWW>();

                for (int i = 0; i < locationsArray.Length; i++)
                    streamList.Add(null);

            }
        }

        DeactivateAllLocations();
    }

    public static void DeactivateAllLocations()
    {
        foreach (LocationDefinition c in locations)
        {
            if (c != null)
                c.gameObject.SetActiveRecursively(false);
        }
    }

    public static void ActivateAllClients()
    {
        foreach (LocationDefinition c in locations)
        {
            if (c != null)
                c.gameObject.SetActiveRecursively(true);
        }
    }

    private static List<WWW> streamList;
    public static List<WWW> StreamList
    {
        get
        {
            return streamList;
        }
        set
        {
            streamList = value;
        }
    }
}
