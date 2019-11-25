using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public static class FloorManager
{
    public static void InstantiateFloor(Transform p_tile, InteriorItemDefinition item)
    {
        if (p_tile && item && p_tile.GetComponent<Renderer>() && item.material)
        {
            p_tile.GetComponent<Renderer>().material = item.material;

            if (BuildingManager.Picked.FloorTiles.ContainsKey(p_tile.name))
            {
                BuildingManager.Picked.FloorTiles[p_tile.name] = item.material.name;
            }
        }
    }
}

