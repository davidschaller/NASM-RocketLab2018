using System;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;
using System.IO;

public class JsonSerializer
{
    public static string SerializeClients(List<int> p_myClients)
    {
        string result = string.Empty;

        result = JsonWriter.Serialize(p_myClients);

        return result;
    }
    public static List<int> DeserializeClients(string data)
    {
        int[] clientsArray = JsonReader.Deserialize<int[]>(data);

        if (clientsArray != null)
        {
            List<int> result = new List<int>();

            result.AddRange(clientsArray);

            return result;
        }
        else
            return new List<int>();
    }

    public static string SerializeExterior(BuildingDefinition p_building)
    {
        string result = string.Empty;

        Exterior exterior = new Exterior();
        exterior.id = p_building.id;
        exterior.pickedMarkerType = p_building.PickedMarkerType;
        exterior.pickedRoof = p_building.PickedRoof != null ? p_building.PickedRoof.id.ToString() : "0";
        exterior.pickedMaterial = p_building.PickedExteriorMaterial.clientSortKey;

        result = JsonWriter.Serialize(exterior);

        return result;
    }
    public static Exterior DeserializeExterior(string data)
    {
        return JsonReader.Deserialize<Exterior>(data);
    }

    public static string SerializeInterior(List<InteriorItemDefinition> p_interiorItems, 
        Dictionary<string, string> p_floorTiles, List<List<WallSection>> p_walls, List<WallSection> p_exteriorCoverings)
    {
        string result = string.Empty;

        Interior interior = new Interior();

        interior.floorTiles = p_floorTiles;

        foreach (InteriorItemDefinition item in p_interiorItems)
        {
            Debug.Log("Saving interior " + item.transform.name + " " + item.transform.localPosition);
            interior.InteriorItems.Add(new InteriorItem(item.gameObject.name.Replace("(Clone)", ""),
                                                            Common.Vector3ToString(item.ClosestMarker),
                                                                item.Angle,
                                                                    Common.RectToString(item.ItemRect)));


        }
        foreach (List<WallSection> item in p_walls)
        {
            List<string> serializedWallSection = new List<string>();

            foreach (WallSection wallSection in item)
            {
                serializedWallSection.Add(wallSection.Serialize());
            }


            interior.walls.Add(serializedWallSection);
        }

        List<string> exteriorCoverings = new List<string>();

        foreach (WallSection item in p_exteriorCoverings)
        {
            exteriorCoverings.Add(item.Serialize());
        }

        interior.exteriorCoverings = exteriorCoverings;

        result = JsonWriter.Serialize(interior);


        return result;
    }
    public static Interior DeserializeInterior(string data)
    {
        return JsonReader.Deserialize<Interior>(data);
    }

    public static string SerializeLandscape(List<ItemDefinition> p_landscapeItems)
    {
        string result = string.Empty;

        Landscape landscaping = new Landscape();

        foreach (ItemDefinition item in p_landscapeItems)
            landscaping.landscapeItems.Add(
                new SimpleItem(item.gameObject.name.Replace("(Clone)", ""),
                                    Common.Vector3ToString(item.ClosestMarker),
                                        Common.RectToString(item.ItemRect)));

        result = JsonWriter.Serialize(landscaping);

        return result;
    }
    public static Landscape DeserializeLandscape(string data)
    {
        return JsonReader.Deserialize<Landscape>(data);
    }

    public static Info DeserializeUserInfo(string data)
    {
        return JsonReader.Deserialize<Info>(data);
    }

    public static string SerializeGameList(List<GameData> p_gameList)
    {
        string result = string.Empty;

        result = JsonWriter.Serialize(p_gameList);

        Debug.Log(result);

        return result;
    }

    internal static string SerializeInfo(Info p_info)
    {
        string result = string.Empty;

        Info info = new Info(p_info.name, p_info.rating, p_info.age, p_info.dateOfSubmition, p_info.image, p_info.state);

        result = JsonWriter.Serialize(info);

        return result;
    }
}