#if Crypto
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Oleg: 
/// This class manages all connected rivals.
/// When we get an array of connected Players from CryptoNet we should instantiate them on the scene.
/// </summary>
public static class RivalManager
{
    #region private constants
    private const string RESPAWN_PLACEHOLDER_NAME = "Respawn";

    #endregion

    private static List<NPCRival> rivalList;
    public static List<NPCRival> RivalList
    {
        get
        {
            return rivalList;
        }
    }

    public static bool HasInstantiatedRivals
    {
        get
        {
            return rivalList != null && rivalList.Count > 0;
        }
    }

    // Where to place rivals by default
    private static GameObject respawnPlaceholder = null;

    private static void Init()
    {
        rivalList = new List<NPCRival>();

        if (respawnPlaceholder == null)
            respawnPlaceholder = GameObject.Find(RESPAWN_PLACEHOLDER_NAME) as GameObject;
    }


    static List<Vector3> usedPositions = new List<Vector3>();
    /// <summary>
    /// Instantiate NPCRival Game Objects
    /// </summary>
    /// <param name="connectedPlayers">An array of all connected players from CryptoNet</param>
    public static void InstantiateConnectedRivals(List<string> connectedPlayers, string playersPositions)
    {
        Init();

        if (connectedPlayers == null)
        {
            Debug.LogWarning("Can't instantiate rivals. No connected players were found");
            return;
        }

        if (respawnPlaceholder == null)
        {
            Debug.LogWarning("Can't instantiate rivals. No respawn placeholder was found");
            return;
        }

        List<string> positions = null;

        if (!string.IsNullOrEmpty(playersPositions))
        {
            positions = new List<string>();
            positions.AddRange(playersPositions.Split('|'));
        }

        foreach (string player in connectedPlayers)
        {
            if (string.IsNullOrEmpty(player.Trim()) || player.Trim().Equals(CryptoGUI.PlayerName))
                continue;

            GameObject rivalGO = new GameObject(string.Format("NPCRival:{0}", player));

            string set = string.Empty;

            if (positions != null)
               set = positions.Find(delegate(string s) { return s.Split(':')[0].Equals(player); });
                
            if (!string.IsNullOrEmpty(set))
            {
                Vector3 pos = RivalSynchronizer.StringToVector3(set.Split(':')[1]);
                
                float height = TerrainManager.GetInterpolatetHeight(pos);

                if (height <= pos.y && pos != Vector3.zero)
                {
                    rivalGO.transform.position = pos;
                }
                else
                {  
                    Vector3 rivalPosition = GetNextPosition(usedPositions, connectedPlayers.Count);

                    rivalGO.transform.position = rivalPosition;
                    usedPositions.Add(rivalPosition);
                }
            }
            else
            {
                Vector3 rivalPosition = GetNextPosition(usedPositions, connectedPlayers.Count);

                rivalGO.transform.position = rivalPosition;
                usedPositions.Add(rivalPosition);
            }
            

            
            NPCRival rival = rivalGO.AddComponent<NPCRival>();
            
            rival.CreateCharacterByName(player);
            
            rival.npcName = player;
            rival.forwardMoveSpeed = 2;

            rivalList.Add(rival);
        }
    }

    public static void AddRival(string newPlayer, Vector3 pos, int totalPlayers)
    {
        GameObject rivalGO = new GameObject(string.Format("NPCRival:{0}", newPlayer));

        float height = TerrainManager.GetInterpolatetHeight(pos);

        if (height <= pos.y && pos != Vector3.zero)
        {
            rivalGO.transform.position = pos;
        }
        else
        {
            Vector3 rivalPosition = GetNextPosition(usedPositions, totalPlayers);

            rivalGO.transform.position = rivalPosition;
            usedPositions.Add(rivalPosition);
        }

        NPCRival rival = rivalGO.AddComponent<NPCRival>();
        rival.CreateCharacterByName(newPlayer);
        rival.npcName = newPlayer;
        rival.forwardMoveSpeed = 2;

        rivalList.Add(rival);
    }

    private static Vector3 GetNextPosition(List<Vector3> usedPositions, int numberOfConnectedPlayers)
    {
        Vector3 result = Vector3.zero;
        
        do
        {
            result = respawnPlaceholder.transform.position + Vector3.up + Vector3.right * Random.Range(0, numberOfConnectedPlayers) +
                                    Vector3.forward * Random.Range(0, numberOfConnectedPlayers);
        }
        while (usedPositions.Contains(result));

        return result;
    }

    internal static NPCRival GetRivalByName(string name)
    {
        return rivalList.Find(p => p.name.Split(':')[1].Equals(name));
    }

    public static void Reset()
    {
        rivalList = null;

    }
}
#endif