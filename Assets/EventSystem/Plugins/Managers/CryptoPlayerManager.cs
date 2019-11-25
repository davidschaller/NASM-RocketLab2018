#if Crypto 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CryptoPlayerManager
{
    public static string Username { get; set; }
    public static string Password { get; set; }

    public static string KeyToJoin { get; set;}
    public static bool IsTimedRounds { get; set; }
    public static int RoundLimit { get; set;}
    public static string Config { get; set; }

    public static bool IsBeginner { get; set; }
    public static RunMode RunMode { get; set; }
    public static bool IsJustFromArena { get; set; }

    private static List<string> finalists;
    public static List<string> Finalists
    {
        get
        {
            if (finalists == null)
            {
                if (CryptoGUI.Main != null)
                {
                    finalists = CryptoGUI.Main.GetFinalists();
                }
            }

            return finalists;
        }
        set
        {
            // for debug only
            finalists = value;
        }
    }

    public static string Renegade { get; set; }
    
    public static bool HasIdentifiedFinalists { get; set; }
    public static bool IsLairUnlocked { get; set; }

    public static bool IsReady 
    {
        get
        {
            return !string.IsNullOrEmpty(Username) && Config != null;
        }
    }

    public enum StartState
    {
        NewGame,
        JoinGame,
        RejoinGame
    }

    private static StartState state = StartState.NewGame;
    public static StartState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;
        }
    }

    public static bool CanContactToRenegade { get; set; }

    public static bool TooLate { get; set; }
}


#endif