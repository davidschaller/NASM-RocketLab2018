using UnityEngine;
using UnityEditor;
using System.Collections;

public class TexasUtility : MonoBehaviour
{
	[MenuItem("GameObject/Select All NPCs in scene")]
	[MenuItem("Event System/Select items/Select All NPCs in scene")]
	static void SelectAllNPCs ()
	{
		NPCController[] npcs = (NPCController[])GameObject.FindObjectsOfType(typeof(NPCController));
		GameObject[] gos = new GameObject[npcs.Length];
		for(int i=0;i<npcs.Length;i++)
		{
			gos[i] = npcs[i].gameObject;
		}
		
		Selection.objects = gos;
	}
	
	[MenuItem("Edit/Dump Scene Render Settings")]
	static void DumpRenderSettings ()
	{
		MTRenderSettings savedRenderSettings = new MTRenderSettings();
		savedRenderSettings = new MTRenderSettings();
		savedRenderSettings.fog = RenderSettings.fog;
		savedRenderSettings.fogColor = RenderSettings.fogColor;
		savedRenderSettings.fogDensity = RenderSettings.fogDensity;
		savedRenderSettings.ambientLight = RenderSettings.ambientLight;
		savedRenderSettings.skybox = RenderSettings.skybox;
		savedRenderSettings.haloStrength = RenderSettings.haloStrength;
		savedRenderSettings.flareStrength = RenderSettings.flareStrength;
		//savedRenderSettings.haloTexture = RenderSettings.haloTexture;
		//savedRenderSettings.spotCookie = RenderSettings.spotCookie;


        System.IO.Directory.CreateDirectory(Application.dataPath + "/08 Resources/RenderSettings");
		string sceneName = EditorApplication.currentScene;
		sceneName = sceneName.Replace(".unity", "");
		string[] names = sceneName.Split('/');
		sceneName = names[names.Length-1];
		
		Debug.Log("Attempt to create " + sceneName);
		AssetDatabase.CreateAsset(savedRenderSettings, AssetDatabase.GenerateUniqueAssetPath("Assets/08 Resources/RenderSettings/" + sceneName + ".asset"));
	}
}
