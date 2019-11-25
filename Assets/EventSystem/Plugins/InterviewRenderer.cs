using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void InterviewFinished();

public class InterviewRenderer : MonoBehaviour
{
    private const string INTERVIEW_RENDERER = "Interview Renderer";

    private static InterviewRenderer main;
    
    private static Dictionary<EventPlayerBase, InterviewFinished> callbackMap =
        new Dictionary<EventPlayerBase, InterviewFinished>();

    public static void AddCallbackForEventPlayer(EventPlayerBase p_player, InterviewFinished p_callback)
    {
        if (!callbackMap.ContainsKey(p_player))
            callbackMap.Add(p_player, p_callback);

        callbackMap[p_player] += p_callback;
    }

    private static NPCInformator forcedNPC;
    public static NPCInformator ForcedNPC
    {
        get
        {
            return forcedNPC;
        }
        set
        {
            forcedNPC = value;

            SetWatchTarget(forcedNPC);
            
        }
    }

    private string currentLocation;

    private CryptoGUI cryptoGUI;

    public static void Init(string p_location, EventPlayerBase p_eventPlayer)
    {
        Debug.Log("Interview Renderer triggered by " + p_eventPlayer.gameObject.name);

        if (main != null)
        {
            Debug.Log("Main is not null, cleaning up first");
            Finish();
        }

        if (main == null)
        {
            Debug.Log("Main is null, creating new Interview Renderer");
            GameObject go = new GameObject(INTERVIEW_RENDERER);
            main = go.AddComponent(typeof(InterviewRenderer)) as InterviewRenderer;
            main.currentLocation = p_location;
        }

        main.cryptoGUI = GameObject.FindObjectOfType(typeof(CryptoGUI)) as CryptoGUI;

        if (main.cryptoGUI == null)
        {
            Debug.LogError("Couldn't find CryptoGUI");
        }
        else
        {
            main.cryptoGUI.ResetInterviewCounter();
            main.cryptoGUI.SelectLocation(main.currentLocation);
        }
    }

    private bool hasAnswered = false;
    private void Update()
    {
        if (main.cryptoGUI != null)
        {
            if (main.cryptoGUI.State == CryptoGUI.GUIState.InterviewAnswer && !hasAnswered)
            {
                forcedNPC.PlayAnswerAnimation();
                hasAnswered = true;
            }
            else if (main.cryptoGUI.State == CryptoGUI.GUIState.Interview)
            {
                hasAnswered = false;
            }
        }
    }

    public static void Finish()
    {
        if (main != null)
        {
            if (ForcedNPC != null)
            {
                ForcedNPC = null;
            }
            else
            {
                Debug.Log("[info] Forced NPC was null on interview finish.  " +
                          "This probably means the player didn't click 'talk to'" +
                          " and there was no NPC forced by the event player");
            }

            Destroy(main.gameObject);
            main = null;
        }
    }

    private static void SetWatchTarget(NPCInformator p_npc)
    {
        PCCamera pcCamera = Camera.main.GetComponent(typeof(PCCamera)) as PCCamera;
        if (pcCamera != null)
        {
            if (p_npc != null)
            {
                pcCamera.WatchTarget = p_npc.transform;
            }
            else
                pcCamera.WatchTarget = null;
            
        }
        else
            Debug.LogWarning("Can't set WatchTarget. PCCamera is NULL");
    }

    public static bool IsFinished()
    {
        if (main == null || main.cryptoGUI == null)
            return false;
        else
            return main.cryptoGUI.IsInterviewFinished;
    }
}