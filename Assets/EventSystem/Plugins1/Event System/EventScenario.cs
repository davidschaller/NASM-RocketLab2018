using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

[System.Serializable]
public class SubstitutionMap
{
    public EventBase theEvent;
    public string eventVarName;
    public string substitutionName;

    public SubstitutionMap(EventBase ev, string eventVarName, string substitutionName)
    {
        this.theEvent = ev;
        this.eventVarName = eventVarName;
        this.substitutionName = substitutionName;
    }
}

public class EventScenario : ScriptableObject
{
    // vars can have a one-to-many relation with events, but are defined once in a scenario
    // substition vars are only necessary for events that need references to in-game resources
    // (since those are not known before the scene runs)
    // Although primarily useful for runtime types (eg: Transform), it should be useful for "static"
    // types like floats, ints, etc for scenario re-usability
    // types:
    // UnityEngine.Vector3
    // UnityEngine.Transform
    // System.Single (float)
    // TestSerialize (my own script)
    // System.Int32 (int)

    // =============== CAN I USE DELEGATES FOR ANY OF THE BELOW?? ===============

    // for the scenario:
    // need to name the substitution variable (array of strings)
    [HideInInspector]
    public string[] subNames = new string[0];

    // need to give it a type (Transform, Vector3, Script type, etc) -- array of System.Type?
    [HideInInspector]
    public string[] subTypes = new string[0];

    // need to store the event & event variable the substitution should be done on

    // for the event player
    // need to read the scenario definition and present settings for each substitution variable in the scene
    // can I add a variable to a class at runtime?
    // or should I make runtime substitution components that are added to the eventplayer by some IDE utility?
    // or should I auto-generate the event-player script code from the EventEditor code?


    [HideInInspector]
    public EventBase[] events;

    public EventBase[] GetEvents()
    {
        return events;
    }

    public bool SubstitutionExists(string name)
    {
        foreach (string nm in subNames)
        {
            if (name == nm)
                return true;
        }

        return false;
    }

    public string[] SubstitutionNamesByType(string type)
    {
        List<string> matches = new List<string>();
        int i = 0;
        for (i = 0; i < subTypes.Length; i++)
        {
            if (subTypes[i] == type)
            {
                matches.Add(subNames[i]);
            }
        }

        string[] ret = new string[matches.Count];
        i = 0;
        foreach (string match in matches)
        {
            ret[i] = match;
            i++;
        }
        return ret;
    }

    string FormatMessage(string msg)
    {
        return String.Format("{0}: {1}", name, msg);
    }

    void ScenarioException(string msg)
    {
        throw new System.Exception(FormatMessage(msg));
    }

    public void MapSubstitutions(EventBase ev, EventPlayer player)
    {
        foreach (SubstitutionMap map in substitutions)
        {
            if (map.theEvent == ev)
            {
                System.Type type = ev.GetType();
                FieldInfo[] myFieldInfo = ev.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance
                                                      | BindingFlags.Public);

                //string savedSubName = "";

                // TODO finish type mappings for all variables
                foreach (FieldInfo f in myFieldInfo)
                {
                    if (map.eventVarName == f.Name)
                    {
                        object value = null;
                        switch (f.FieldType.ToString())
                        {
                            case "WaypointGroup":
                                Component[] groupCandidates = (Component[])player.GetComponents(typeof(SV_WaypointGroup));
                                foreach (Component candidate in groupCandidates)
                                {
                                    SV_WaypointGroup go = (SV_WaypointGroup)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionGroup;
                                    }
                                }
                                break;
                            case "Waypoint":
                                Component[] waypointCandidates = (Component[])player.GetComponents(typeof(SV_Waypoint));
                                foreach (Component candidate in waypointCandidates)
                                {
                                    SV_Waypoint go = (SV_Waypoint)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionWaypoint;
                                    }
                                }
                                break;
                            case "UnityEngine.Rect":
                                Component[] rectCandidates = (Component[])player.GetComponents(typeof(SV_Rect));
                                foreach (Component candidate in rectCandidates)
                                {
                                    SV_Rect go = (SV_Rect)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionRect;
                                    }
                                }
                                break;
                            case "System.Boolean":
                                Component[] boolCandidates = (Component[])player.GetComponents(typeof(SV_Bool));
                                foreach (Component candidate in boolCandidates)
                                {
                                    SV_Bool go = (SV_Bool)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionBool;
                                    }
                                }
                                break;
                            case "System.Single":
                                Component[] singleCandidates = (Component[])player.GetComponents(typeof(SV_Float));
                                foreach (Component candidate in singleCandidates)
                                {
                                    SV_Float go = (SV_Float)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionFloat;
                                    }
                                }
                                break;
                            case "System.Int32":
                                Component[] int32Candidates = (Component[])player.GetComponents(typeof(SV_Int));
                                foreach (Component candidate in int32Candidates)
                                {
                                    SV_Int go = (SV_Int)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionInt;
                                    }
                                }
                                break;
                            case "ActorBase":
                                Component[] actorCandidates = (Component[])player.GetComponents(typeof(SV_ActorBase));
                                foreach (Component candidate in actorCandidates)
                                {
                                    SV_ActorBase go = (SV_ActorBase)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionActor;
                                    }
                                }
                                break;
                            case "CannonController":
                                Component[] cannonCandidates = (Component[])player.GetComponents(typeof(SV_CannonController));
                                foreach (Component candidate in cannonCandidates)
                                {
                                    SV_CannonController go = (SV_CannonController)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionCannon;
                                    }
                                }
                                break;
                            case "TreasureHuntController":
                                Component[] treasureCandidates = (Component[])player.GetComponents(typeof(SV_TreasureHuntController));
                                foreach (Component candidate in treasureCandidates)
                                {
                                    SV_TreasureHuntController go = (SV_TreasureHuntController)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionTreasure;
                                    }
                                }
                                break;

                            case "EventPlayer":
                                Component[] playerCandidates = (Component[])player.GetComponents(typeof(SV_EventPlayer));
                                foreach (Component candidate in playerCandidates)
                                {
                                    SV_EventPlayer go = (SV_EventPlayer)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionPlayer;
                                    }
                                }
                                break;
                            case "EventPlayer[]":
                                Component[] epArrayCandidates = (Component[])player.GetComponents(typeof(SV_EventPlayerArray));
                                foreach (Component candidate in epArrayCandidates)
                                {
                                    SV_EventPlayerArray go = (SV_EventPlayerArray)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionEventPlayerArray;
                                    }
                                }
                                break;
                            case "NPCController":
                                Component[] npcCandidates = (Component[])player.GetComponents(typeof(SV_NPCController));
                                foreach (Component candidate in npcCandidates)
                                {
                                    SV_NPCController go = (SV_NPCController)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionNPC;
                                    }
                                }
                                break;
                            case "NPCController[]":
                                Component[] npcArrayCandidates = (Component[])player.GetComponents(typeof(SV_NPCControllerArray));
                                foreach (Component candidate in npcArrayCandidates)
                                {
                                    SV_NPCControllerArray go = (SV_NPCControllerArray)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionNPCarray;
                                    }
                                }
                                break;
                            case "HorseController":
                                Component[] horseCandidates = (Component[])player.GetComponents(typeof(SV_HorseController));
                                foreach (Component candidate in horseCandidates)
                                {
                                    SV_HorseController go = (SV_HorseController)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionHorse;
                                    }
                                }
                                break;
                            case "NPCInformator":
                                Component[] npcInformatorCandidates = (Component[])player.GetComponents(typeof(SV_NPCInformator));
                                foreach (Component candidate in npcInformatorCandidates)
                                {
                                    SV_NPCInformator go = (SV_NPCInformator)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionInformatorNPC;
                                    }
                                }
                                break;
                            case "UnityEngine.Animation":
                                Component[] animationCandidates = (Component[])player.GetComponents(typeof(SV_Animation));
                                foreach (Component candidate in animationCandidates)
                                {
                                    SV_Animation go = (SV_Animation)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionAnimation;
                                    }
                                }
                                break;
                            case "UnityEngine.AnimationClip":
                                Component[] clipCandidates = (Component[])player.GetComponents(typeof(SV_AnimationClip));
                                foreach (Component candidate in clipCandidates)
                                {
                                    SV_AnimationClip go = (SV_AnimationClip)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionClip;
                                    }
                                }
                                break;
                            case "UnityEngine.AudioClip":
                                Component[] audioCandidates = (Component[])player.GetComponents(typeof(SV_AudioClip));
                                foreach (Component candidate in audioCandidates)
                                {
                                    SV_AudioClip go = (SV_AudioClip)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionClip;
                                    }
                                }
                                break;
                            case "UnityEngine.GameObject":
                                Component[] candidates = (Component[])player.GetComponents(typeof(SV_GameObject));
                                foreach (Component candidate in candidates)
                                {
                                    SV_GameObject go = (SV_GameObject)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionGO;
                                    }
                                }
                                break;
                            case "UnityEngine.Transform":
                                Component[] transformCandidates = (Component[])player.GetComponents(typeof(SV_Transform));
                                foreach (Component candidate in transformCandidates)
                                {
                                    SV_Transform go = (SV_Transform)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionTransform;
                                    }
                                }
                                break;
                            case "UnityEngine.Camera":
                                Component[] cameraCandidates = (Component[])player.GetComponents(typeof(SV_Camera));
                                foreach (Component candidate in cameraCandidates)
                                {
                                    SV_Camera go = (SV_Camera)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionCamera;
                                    }
                                }
                                break;
                            case "UnityEngine.Object":
                                Component[] objCandidates = (Component[])player.GetComponents(typeof(SV_UnityObject));
                                foreach (Component candidate in objCandidates)
                                {
                                    SV_UnityObject go = (SV_UnityObject)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionObject;
                                        if (go.substitutionObject == null) Debug.Log("Attempting to insert a null sub object");
                                    }
                                    //savedSubName = go.substitutionName;
                                }
                                break;
                            case "DialogDescriptor":
                                Component[] descCandidates = (Component[])player.GetComponents(typeof(SV_DialogDescriptor));
                                foreach (Component candidate in descCandidates)
                                {
                                    SV_DialogDescriptor go = (SV_DialogDescriptor)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionDescriptor;
                                    }
                                }
                                break;
                            case "PlayerGoal":
                                Component[] goalCandidates = (Component[])player.GetComponents(typeof(SV_PlayerGoal));
                                foreach (Component candidate in goalCandidates)
                                {
                                    SV_PlayerGoal go = (SV_PlayerGoal)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionGoal;
                                    }
                                }
                                break;
                            case "UnityEngine.Texture2D":
                                Component[] textureCandidates = (Component[])player.GetComponents(typeof(SV_Texture2D));
                                foreach (Component candidate in textureCandidates)
                                {
                                    SV_Texture2D go = (SV_Texture2D)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionTexture;
                                    }
                                }
                                break;
                            case "UnityEngine.Vector2":
                                Component[] v2Candidates = (Component[])player.GetComponents(typeof(SV_Vector2));
                                foreach (Component candidate in v2Candidates)
                                {
                                    SV_Vector2 go = (SV_Vector2)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionVector;
                                    }
                                }
                                break;
                            case "UnityEngine.Vector3":
                                Component[] v3Candidates = (Component[])player.GetComponents(typeof(SV_Vector3));
                                foreach (Component candidate in v3Candidates)
                                {
                                    SV_Vector3 go = (SV_Vector3)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionVector;
                                    }
                                }
                                break;
                            case "System.String":
                                Component[] stringCandidates = (Component[])player.GetComponents(typeof(SV_String));
                                foreach (Component candidate in stringCandidates)
                                {
                                    SV_String go = (SV_String)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionString;
                                    }
                                }
                                break;
                            case "System.String[]":
                                Component[] stringArrayCandidates = (Component[])player.GetComponents(typeof(SV_StringArray));
                                foreach (Component candidate in stringArrayCandidates)
                                {
                                    SV_StringArray go = (SV_StringArray)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionStringArray;
                                    }
                                }
                                break;
                            case "UnityEngine.Material":
                                Component[] materialCandidates = (Component[])player.GetComponents(typeof(SV_Material));
                                foreach (Component candidate in materialCandidates)
                                {
                                    SV_Material go = (SV_Material)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionMaterial;
                                    }
                                }
                                break;
                            case "UnityEngine.Color":
                                Component[] colorCandidates = (Component[])player.GetComponents(typeof(SV_Color));
                                foreach (Component candidate in colorCandidates)
                                {
                                    SV_Color go = (SV_Color)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionColor;
                                    }
                                }
                                break;
                            case "UnityEngine.TextAsset":
                                Component[] textAssetCandidates = (Component[])player.GetComponents(typeof(SV_TextAsset));
                                foreach (Component candidate in textAssetCandidates)
                                {
                                    SV_TextAsset go = (SV_TextAsset)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionTextAsset;
                                    }
                                }
                                break;
                            case "UnityEngine.LayerMask":
                                Component[] layerMaskCandidates = (Component[])player.GetComponents(typeof(SV_LayerMask));
                                foreach (Component candidate in layerMaskCandidates)
                                {
                                    SV_LayerMask go = (SV_LayerMask)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionLayerMask;
                                    }
                                }
                                break;
                            case "UnityEngine.Mesh":
                                Component[] meshCandidates = (Component[])player.GetComponents(typeof(SV_Mesh));
                                foreach (Component candidate in meshCandidates)
                                {
                                    SV_Mesh go = (SV_Mesh)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionMesh;
                                    }
                                }
                                break;
                            case "UnityEngine.GUISkin":
                                Component[] skinCandidates = (Component[])player.GetComponents(typeof(SV_GUISkin));
                                foreach (Component candidate in skinCandidates)
                                {
                                    SV_GUISkin go = (SV_GUISkin)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionSkin;
                                    }
                                }
                                break;
                            case "UnityEngine.WrapMode":
                                Component[] wrapModeCandidates = (Component[])player.GetComponents(typeof(SV_WrapMode));
                                foreach (Component candidate in wrapModeCandidates)
                                {
                                    SV_WrapMode go = (SV_WrapMode)candidate;

                                    if (f.Name == map.eventVarName && go.substitutionName == map.substitutionName)
                                    {
                                        value = go.substitutionWrapMode;
                                    }
                                }
                                break;
                            default:
                                ScenarioException("Attempting to map type (" + f.FieldType.ToString() + ") but no mapping logic exists for it!");
                                break;
                        }
                        if (value == null)
                        {
                            /*
                            string msg = String.Format("Failed to map substition variable: {0} for event scenario {5}.  " +
                                                       "If this was intentional and you intentionally didn't supply a value for this substituted variable, ignore this warning.  " +
                                                       "BTW, this could also happen because new vars were added to the event but you haven't re-blessed the player yet.  " +
                                                       "[Saved subname {4}, field name {1}, field type {3}, event var name {2}.]",
                                                       map.substitutionName,
                                                       f.Name,
                                                       map.eventVarName,
                                                       f.FieldType,
                                                       savedSubName,
                                                       name);
                            */
                            //throw new System.Exception(msg);
                            if (Application.isEditor)
                            {
                                //Debug.Log(msg, player.gameObject);
                            }
                        }
                        else
                        {
                            type.GetField(f.Name).SetValue(ev, value);
                        }
                    }
                }
            }
        }
    }

    //TODO [HideInInspector]
    public SubstitutionMap[] substitutions = new SubstitutionMap[0];
    public void MapSubstitution(EventBase ev, string eventVarName, string substitutionVarName)
    {
        bool replaceExisting = false;
        //int replaceIndex = 0;
        for (int i = 0; i < substitutions.Length; i++)
        {
            SubstitutionMap smap = substitutions[i];
            if (smap.theEvent == ev && smap.eventVarName == eventVarName)
            {
                Debug.Log("Replacing existing var Event: " + smap.theEvent.name + ", ev var: " + smap.eventVarName + ", sub: " + substitutionVarName);
                replaceExisting = true;
                smap.substitutionName = substitutionVarName;
                substitutions[i] = smap;
            }
        }

        if (!replaceExisting)
        {
            int newLength = substitutions.Length + 1;
            SubstitutionMap[] newSubs = new SubstitutionMap[newLength];
            for (int i = 0; i < substitutions.Length; i++)
            {
                newSubs[i] = substitutions[i];
            }
            newSubs[substitutions.Length] = new SubstitutionMap(ev, eventVarName, substitutionVarName);

            substitutions = newSubs;
        }
    }

    public List<SubstitutionMap> GetMapsForEvent(EventBase ev)
    {
        List<SubstitutionMap> ret = new List<SubstitutionMap>();
        foreach (SubstitutionMap map in substitutions)
        {
            if (map.theEvent == ev)
            {
                ret.Add(map);
            }
        }
        return ret;
    }

    public void AddSubstitutionVariable(string name, System.Type type)
    {
        if (SubstitutionExists(name))
        {
            Debug.Log("Variable already exists, not adding");
        }
        else
        {
            if (type == null) throw new System.Exception("New substitution variable's type is NULL! (This probably needs to be added to the \"Problem Map\")");

            string[] newNames = new string[subNames.Length + 1];
            for (int i = 0; i < subNames.Length; i++)
            {
                newNames[i] = subNames[i];
            }
            newNames[subNames.Length] = name;

            string[] newTypes = new string[subTypes.Length + 1];
            for (int i = 0; i < subTypes.Length; i++)
            {
                newTypes[i] = subTypes[i];
            }
            newTypes[subTypes.Length] = type.ToString();

            subNames = newNames;
            subTypes = newTypes;
        }
        Debug.Log(String.Format("New substitution var {0} of type {1}.  Now {2} names and {3} types", name, type, subNames.Length, subTypes.Length));
    }

    public void DeleteSubstitutionVariable(string nm)
    {
        bool found = false;
        int delIdx = 0;
        string[] newNames = new string[subNames.Length - 1];
        int currIdx = 0;
        for (int i = 0; i < subNames.Length; i++)
        {
            if (subNames[i] != nm)
            {
                newNames[currIdx] = subNames[i];
                currIdx++;
            }
            else
            {
                found = true;
                delIdx = i;
            }
        }

        if (found)
        {
            string[] newTypes = new string[subTypes.Length - 1];

            currIdx = 0;
            for (int i = 0; i < subTypes.Length; i++)
            {
                if (i != delIdx)
                {
                    newTypes[currIdx] = subTypes[i];
                    currIdx++;
                }
            }

            subNames = newNames;
            subTypes = newTypes;
        }

    }

    int GetSubstitutionVariableIndex(string nm)
    {
        for (int i = 0; i < subNames.Length; i++)
        {
            if (subNames[i] == nm)
            {
                return i;
            }
        }

        return -1;
    }

    public Dictionary<string, string> GetSubstitutionVariables()
    {
        Dictionary<string, string> subs = new Dictionary<string, string>();
        for (int i = 0; i < subNames.Length; i++)
        {
            subs.Add(subNames[i], subTypes[i]);
        }

        return subs;
    }

    System.Type ConvertToType(string tp)
    {
        System.Type ret = null;
        switch (tp)
        {
            case "UnityEngine.GameObject":
                ret = typeof(UnityEngine.GameObject);
                break;
            case "UnityEngine.Transform":
                ret = typeof(UnityEngine.Transform);
                break;
            case "UnityEngine.Animation":
                ret = typeof(UnityEngine.Animation);
                break;
            case "UnityEngine.AnimationClip":
                ret = typeof(UnityEngine.AnimationClip);
                break;
            case "UnityEngine.AudioClip":
                ret = typeof(UnityEngine.AudioClip);
                break;
            case "UnityEngine.Rect":
                ret = typeof(UnityEngine.Rect);
                break;
            case "UnityEngine.Vector2":
                ret = typeof(UnityEngine.Vector2);
                break;
            case "UnityEngine.Vector3":
                ret = typeof(UnityEngine.Vector3);
                break;
            case "UnityEngine.Texture2D":
                ret = typeof(UnityEngine.Texture2D);
                break;
            case "UnityEngine.Camera":
                ret = typeof(UnityEngine.Camera);
                break;
            case "UnityEngine.TextAsset":
                ret = typeof(UnityEngine.TextAsset);
                break;
            case "UnityEngine.LayerMask":
                ret = typeof(UnityEngine.LayerMask);
                break;
            case "UnityEngine.Color":
                ret = typeof(UnityEngine.Color);
                break;
            case "UnityEngine.Material":
                ret = typeof(UnityEngine.Material);
                break;
            case "UnityEngine.Mesh":
                ret = typeof(UnityEngine.Mesh);
                break;
            case "UnityEngine.GUISkin":
                ret = typeof(UnityEngine.GUISkin);
                break;
            case "UnityEngine.WrapMode":
                ret = typeof(UnityEngine.WrapMode);
                break;
            default:
                ret = Type.GetType(tp);
                break;
        }

        return ret;
    }

    public System.Type GetSubstitutionVariableType(string nm)
    {
        int subIndex = GetSubstitutionVariableIndex(nm);

        if (subIndex < 0 || subIndex >= subTypes.Length)
        {
            return null;
        }

        return ConvertToType(subTypes[subIndex]);
    }

    public void DeleteAllSubstitutionVariables()
    {
        subNames = new string[0];
        subTypes = new string[0];
    }

    public int EventCount()
    {
        return events == null ? 0 : events.Length;
    }

    public void AddEvent(EventBase newEvent)
    {
        if (events == null)
            events = new EventBase[0];
        EventBase[] ne = new EventBase[events.Length + 1];
        for (int i = 0; i < events.Length; i++)
        {
            ne[i] = events[i];
        }
        ne[events.Length] = newEvent;

        events = ne;
    }

    public void DeleteAllEvents()
    {
        events = null;
        events = new EventBase[0];
    }

    public void DeleteEvent(EventBase delEvent)
    {
        EventBase[] ne = new EventBase[events.Length - 1];
        bool found = false;
        int currIdx = 0;
        for (int i = 0; i < events.Length; i++)
        {
            if (events[i] != delEvent)
            {
                ne[currIdx] = events[i];
                currIdx++;
            }
            else
            {
                found = true;
            }
        }

        if (found)
            events = ne;
    }
}
