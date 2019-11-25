using UnityEngine;
using System.Collections;
using System;

public class VariableTypeConverter
{
    public static string[] humanTypes = new string[] 
	{
		"int", 
		"float", 
		"string", 
		"string[]",
		"GameObject",
		"Transform",
		"WaypointGroup", 
		"Vector2", 
		"Vector3",
		"Rect",
		"ActorBase", 
		"NPCController", 
        "NPCController[]",
		"Animation", 
		"AnimationClip",
		"AudioClip",
		"bool",
		"DialogDescriptor",
		"PlayerGoal",
		"UnityObject",
		"Texture2D",
		"CannonController",
		"TreasureHuntController",
		"EventPlayer",
		"EventPlayer[]",
        "HorseController",
        "Material",
        "Color",
        "NPCInformator",
        "Camera",
        "Waypoint",
		"TextAsset",
		"LayerMask",
        "Mesh",
        "GUISkin",
        "WrapMode"
	};
    public static string[] systemTypeNames = new string[] 
	{
		"System.Int32", 
		"System.Single", 
		"System.String", 
		"System.String[]",
		"UnityEngine.GameObject",
		"UnityEngine.Transform",
		"WaypointGroup", 
		"UnityEngine.Vector2", 
		"UnityEngine.Vector3",
		"UnityEngine.Rect",
		"ActorBase", 
		"NPCController", 
        "NPCController[]",
		"UnityEngine.Animation", 
		"UnityEngine.AnimationClip",
		"UnityEngine.AudioClip",
		"System.Boolean",
		"DialogDescriptor",
		"PlayerGoal",
		"UnityEngine.Object",
		"UnityEngine.Texture2D",
		"CannonController",
		"TreasureHuntController",
		"EventPlayer",
		"EventPlayer[]",
        "HorseController",
        "UnityEngine.Material",
        "UnityEngine.Color",
        "NPCInformator",
        "UnityEngine.Camera",
        "Waypoint",
		"UnityEngine.TextAsset",
		"UnityEngine.LayerMask",
        "UnityEngine.Mesh",
        "UnityEngine.GUISkin",
        "UnityEngine.WrapMode"
	};


    // these don't look up using Type.GetType() for some reason...
    public static System.Type ProblemMap(string nm)
    {
        System.Type ret = null;
        switch (nm)
        {
            case "Animation":
                ret = typeof(Animation);
                break;
            case "AnimationClip":
                ret = typeof(AnimationClip);
                break;
            case "AudioClip":
                ret = typeof(AudioClip);
                break;
            case "UnityObject":
                ret = typeof(UnityEngine.Object);
                break;
            case "GameObject":
                ret = typeof(GameObject);
                break;
            case "Transform":
                ret = typeof(UnityEngine.Transform);
                break;
            case "Camera":
                ret = typeof(UnityEngine.Camera);
                break;
            case "Texture2D":
                ret = typeof(UnityEngine.Texture2D);
                break;
            case "Vector2":
                ret = typeof(UnityEngine.Vector2);
                break;
            case "Vector3":
                ret = typeof(UnityEngine.Vector3);
                break;
            case "Rect":
                ret = typeof(UnityEngine.Rect);
                break;
            case "Material":
                ret = typeof(Material);
                break;
            case "Color":
                ret = typeof(Color);
                break;
            case "NPCController[]":
                ret = typeof(NPCController[]);
                break;
            case "System.String[]":
                ret = typeof(System.String[]);
                break;
            case "TextAsset":
                ret = typeof(TextAsset);
                break;
            case "LayerMask":
                ret = typeof(LayerMask);
                break;
            case "EventPlayer[]":
                ret = typeof(EventPlayer[]);
                break;
            case "Mesh":
                ret = typeof(UnityEngine.Mesh);
                break;
            case "GUISkin":
                ret = typeof(UnityEngine.GUISkin);
                break;
            case "WrapMode":
                ret = typeof(UnityEngine.WrapMode);
                break;
        }

        return ret;
    }

    public static System.Type HumanToSystemType(string nm)
    {
        System.Type ret = null;

        int savedIndex = 0;
        for (int i = 0; i < humanTypes.Length; i++)
        {
            if (humanTypes[i] == nm)
            {
                ret = Type.GetType(systemTypeNames[i]);
                savedIndex = i;
            }
        }

        if (ret == null)
        {
            ret = ProblemMap(nm);
        }

        return ret;
    }

    public static string SystemNameToHuman(string sn)
    {
        string ret = null;

        for (int i = 0; i < systemTypeNames.Length; i++)
        {
            if (systemTypeNames[i] == sn)
                ret = humanTypes[i];
        }

        return ret;
    }

    public static System.Type SystemNameToSubstitutionType(string typeString)
    {
        System.Type ret = null;

        switch (typeString)
        {
            case "UnityEngine.GameObject":
                ret = typeof(SV_GameObject);
                break;
            case "UnityEngine.Transform":
                ret = typeof(SV_Transform);
                break;
            case "UnityEngine.Camera":
                ret = typeof(SV_Camera);
                break;
            case "WaypointGroup":
                ret = typeof(SV_WaypointGroup);
                break;
            case "Waypoint":
                ret = typeof(SV_Waypoint);
                break;
            case "System.Single":
                ret = typeof(SV_Float);
                break;
            case "System.String":
                ret = typeof(SV_String);
                break;
            case "System.String[]":
                ret = typeof(SV_StringArray);
                break;
            case "System.Boolean":
                ret = typeof(SV_Bool);
                break;
            case "System.Int32":
                ret = typeof(SV_Int);
                break;
            case "UnityEngine.Vector2":
                ret = typeof(SV_Vector2);
                break;
            case "UnityEngine.Vector3":
                ret = typeof(SV_Vector3);
                break;
            case "UnityEngine.Rect":
                ret = typeof(SV_Rect);
                break;
            case "ActorBase":
                ret = typeof(SV_ActorBase);
                break;
            case "NPCController":
                ret = typeof(SV_NPCController);
                break;
            case "NPCController[]":
                ret = typeof(SV_NPCControllerArray);
                break;
            case "HorseController":
                ret = typeof(SV_HorseController);
                break;
            case "UnityEngine.Animation":
                ret = typeof(SV_Animation);
                break;
            case "UnityEngine.AnimationClip":
                ret = typeof(SV_AnimationClip);
                break;
            case "UnityEngine.AudioClip":
                ret = typeof(SV_AudioClip);
                break;
            case "DialogDescriptor":
                ret = typeof(SV_DialogDescriptor);
                break;
            case "PlayerGoal":
                ret = typeof(SV_PlayerGoal);
                break;
            case "UnityEngine.Object":
                ret = typeof(SV_UnityObject);
                break;
            case "UnityEngine.Texture2D":
                ret = typeof(SV_Texture2D);
                break;
            case "CannonController":
                ret = typeof(SV_CannonController);
                break;
            case "TreasureHuntController":
                ret = typeof(SV_TreasureHuntController);
                break;
            case "EventPlayer":
                ret = typeof(SV_EventPlayer);
                break;
            case "EventPlayer[]":
                ret = typeof(SV_EventPlayerArray);
                break;
            case "UnityEngine.Material":
                ret = typeof(SV_Material);
                break;
            case "UnityEngine.Color":
                ret = typeof(SV_Color);
                break;
            case "NPCInformator":
                ret = typeof(SV_NPCInformator);
                break;
            case "UnityEngine.TextAsset":
                ret = typeof(SV_TextAsset);
                break;
            case "UnityEngine.LayerMask":
                ret = typeof(SV_LayerMask);
                break;
            case "UnityEngine.Mesh":
                ret = typeof(SV_Mesh);
                break;
            case "UnityEngine.GUISkin":
                ret = typeof(SV_GUISkin);
                break;
            case "UnityEngine.WrapMode":
                ret = typeof(SV_WrapMode);
                break;
        }

        return ret;
    }
}
