using UnityEngine;

public static class TerrainManager
{
	private static Terrain mainTerrain;

    public static TerrainData CachedData
    {
        get
        {
            return MainTerrain.terrainData;
        }
    }

    public static Transform CachedTransform
    {
        get
        {
            return MainTerrain.transform;
        }
    }
	
	public static Terrain MainTerrain
	{
		get
		{
			if (mainTerrain == null)
			{
				mainTerrain = Terrain.activeTerrain;
				if (mainTerrain == null)
				{
					GameObject tr = GameObject.Find("/Terrain");
					if (tr != null)
						mainTerrain = tr.GetComponent<Terrain>();
					if (mainTerrain == null && Application.isEditor)
						Debug.LogWarning("Just can't find main terrain object...");
					
				}
			}

			return mainTerrain;
		}
		set
		{
			mainTerrain = value;
		}
	}

    public static float GetInterpolatetHeight(Vector3 pos)
    {
        Vector3 terrainLocalPos = pos - CachedTransform.position;
        Vector2 normalizedPos = new Vector2(Mathf.InverseLerp(0, CachedData.size.x, terrainLocalPos.x), Mathf.InverseLerp(0, CachedData.size.z, terrainLocalPos.z));

        return CachedData.GetInterpolatedHeight(normalizedPos.x, normalizedPos.y);
    }
}
