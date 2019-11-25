using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Oleg: 
/// 
/// This class updates 'walk to' positions for all connected rivals.
/// </summary>
public class RivalSynchronizer : MonoBehaviour
{
#if Crypto

	// How often to get the latest positions from the server
    public int pullDelay = 5;
    // How often to push Player's positions to the server
    public int pushDelay = 5;

    private float pushTimer = 0, 
                  pullTimer = 0;

    private Vector3 tempTarget = Vector3.zero;

    private void Awake()
    {

    }

    private void Start()
    {
        pushTimer = 0;
        pullTimer = 0;
    }

    //private bool isJustLoaded = true;

    private void Update()
    {
        if (CryptoNet.GameId == -999 || string.IsNullOrEmpty(CryptoGUI.PlayerName) ||
                MovementController.ControlTarget == null || RivalManager.RivalList == null)
        {
            return;
        }

        if (pullTimer >= pullDelay)// && !isJustLoaded)
        {
            StartCoroutine(CryptoNet.GetPlayersPositions());
            pullTimer = 0;
        }

        if (/*!MovementController.Main.IsLocked && */(MovementController.ClickTarget != tempTarget || pushTimer >= pushDelay))
        {
            tempTarget = MovementController.ClickTarget;

            Vector3 saveTarget = (tempTarget == Vector3.zero) ? MovementController.ControlTarget.GetTransform().position : tempTarget;

            StartCoroutine(CryptoNet.UpdatePlayerPosition(CryptoNet.GameId, CryptoGUI.PlayerName, saveTarget));
            
            pushTimer = 0;
        }

        if (!CryptoNet.GettingPlayersPositions && !string.IsNullOrEmpty(CryptoNet.PlayersPositions))
        {
            //if (isJustLoaded)
            //{
            //    isJustLoaded = false;
            //}
            //else
                UpdateRivalsMoveTargets(CryptoNet.PlayersPositions);

            CryptoNet.PlayersPositions = string.Empty;
        }

        pushTimer += Time.deltaTime;
        pullTimer += Time.deltaTime;
    }

    /// <summary>
    /// Split an answer from the server ans set 'move to' positions for all rivals
    /// </summary>
    /// <param name="input">"player1-0.0;0.0;0.0|player2-0.0;0.0;0.0|player3-0.0;0.0;0.0|"</param>
    private void UpdateRivalsMoveTargets(string input)
    {
        string[] playersSplit = input.Split('|');

        foreach (string player in playersSplit)
        {
            if (string.IsNullOrEmpty(player))
                continue;

            string[] infoSplit = player.Split(':');

            if (!infoSplit[0].Equals(CryptoGUI.PlayerName))
            {
                NPCRival rival = RivalManager.RivalList.Find(delegate(NPCRival p) { return p.npcName.Equals(infoSplit[0]); });

                if (rival != null)
                {
                    if (!rival.ClickTargets.Contains(StringToVector3(infoSplit[1])))
                        rival.ClickTargets.Add(StringToVector3(infoSplit[1]));
                }
                else
                {
                    Debug.Log("adding rival = " + player);
                    StartCoroutine(CryptoNet.GetConfig(infoSplit[0],CryptoPlayerManager.KeyToJoin));
                    RivalManager.AddRival(infoSplit[0], StringToVector3(infoSplit[1]), playersSplit.Length);
                }
            }
        }
    }
#endif
    public static Vector3 StringToVector3(string input)
    {
        string[] inputSplit = input.Split(';');
        return new Vector3(float.Parse(inputSplit[0]), float.Parse(inputSplit[1]), float.Parse(inputSplit[2]));
    }
}

