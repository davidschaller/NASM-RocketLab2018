using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class FastGUIAtlasManager: EditorWindow 
{
	public static void CheckSourceFolder()
	{
		if(FastGUI.sourceFolder == "")
		{
			AssetDatabase.CreateFolder(FastGUI.assetFolderToBeParseed,"Source");
			FastGUI.sourceFolder = FastGUI.assetFolderToBeParseed+"/Source/";
		}
	}
	public static UIAtlas CreateNewAtlas()
	{
		CheckSourceFolder();
		string prefabPath = "";
		string matPath = "";
		
		// If we have an atlas to work with, see if we can figure out the path for it and its material
		if (NGUISettings.atlas != null && NGUISettings.atlas.name == NGUISettings.atlasName)
		{
			prefabPath = AssetDatabase.GetAssetPath(NGUISettings.atlas.gameObject.GetInstanceID());
			if (NGUISettings.atlas.spriteMaterial != null) matPath = AssetDatabase.GetAssetPath(NGUISettings.atlas.spriteMaterial.GetInstanceID());
		}
		NGUISettings.atlasName 	= FastGUI.objectToBeLoaded.name;
		prefabPath 				= FastGUI.sourceFolder + NGUISettings.atlasName + ".prefab";
		matPath 				= FastGUI.sourceFolder + NGUISettings.atlasName + ".mat";

		// Try to load the prefab
		GameObject go = null;

		
		// Try to load the material
		Material mate = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;

		// If the material doesn't exist, create it
		if (mate == null)
		{
			Shader shader = Shader.Find("Unlit/Transparent Colored");
			mate = new Material(shader);

			// Save the material
			AssetDatabase.CreateAsset(mate, matPath);
			AssetDatabase.Refresh();

			// Load the material so it's usable
			mate = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
		}

		if (NGUISettings.atlas == null || NGUISettings.atlas.name != NGUISettings.atlasName)
		{
			Object prefab = (go != null) ? go : PrefabUtility.CreateEmptyPrefab(prefabPath);

			// Create a new game object for the atlas
			go = new GameObject(NGUISettings.atlasName);
			go.AddComponent<UIAtlas>().spriteMaterial = mate;

			// Update the prefab
			PrefabUtility.ReplacePrefab(go, prefab);

			DestroyImmediate(go);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			// Select the atlas
			go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
		}
		return go.GetComponent<UIAtlas>();
	}
	
	public static UIFont CreateNewFont(string pTagetFontName)
	{	
		string[] directorys 			= Directory.GetDirectories(Application.dataPath+"/", "Typeface", SearchOption.AllDirectories);	
		string targetFolder		 		= "";
		string prefabPath 				= "";
		
		if(directorys.Length == 0)
		{
			Debug.LogWarning("Can't find the Typeface directory for the font creation, this name is Case Sensetive and must be 'TypeFace' ");
		}
		else
		{
			string[] tFiles = Directory.GetFiles(directorys[0], "*", SearchOption.AllDirectories );
			foreach(string tFile in tFiles)
			{
				if(tFile.IndexOf(".png") >= 0 && tFile.IndexOf(".meta") < 0)
				{
					NGUISettings.fontTexture 	= AssetDatabase.LoadAssetAtPath(FastGUIUtils.GetProjectRelativePath(tFile), typeof(Texture2D)) as Texture2D;
				}
				else if(tFile.IndexOf(".txt") >= 0 && tFile.IndexOf(".meta") < 0)
				{
					NGUISettings.fontData 		= AssetDatabase.LoadAssetAtPath(FastGUIUtils.GetProjectRelativePath(tFile), typeof(TextAsset)) as TextAsset;
					string tProjectRelative 	= FastGUIUtils.GetProjectRelativePath(tFile);
					targetFolder 				= FastGUIUtils.GetParentFolderPath(tProjectRelative);
				}
			}
	
			// Assume default values if needed
			
			
			NGUISettings.fontName 	= pTagetFontName;
			prefabPath 				= targetFolder + NGUISettings.fontName + ".prefab";
			
	
			// Draw the atlas selection only if we have the font data and texture specified, just to make it easier
			if (NGUISettings.fontData == null && NGUISettings.fontTexture == null)
			{
				Debug.LogError("NGUISettings.fontData is "+NGUISettings.fontData+ " and NGUISettings.fontTexture is" +NGUISettings.fontTexture);
				return null;
			}
			GameObject go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

			bool create = true;
			if (create)
			{
				UIAtlasMaker.AddOrUpdate(NGUISettings.atlas, NGUISettings.fontTexture);

				if (go == null || go.GetComponent<UIFont>() == null)
				{
					// Create a new prefab for the atlas
#if UNITY_3_4
					Object prefab = EditorUtility.CreateEmptyPrefab(prefabPath);
#else
					Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
#endif
					// Create a new game object for the font
					go = new GameObject(NGUISettings.fontName);
					NGUISettings.font 				= go.AddComponent<UIFont>();
					NGUISettings.font.atlas 		= NGUISettings.atlas;
					NGUISettings.font.spriteName 	= NGUISettings.fontTexture.name;
					BMFontReader.Load(NGUISettings.font.bmFont, NGUITools.GetHierarchy(NGUISettings.font.gameObject), NGUISettings.fontData.bytes);

					// Update the prefab
#if UNITY_3_4
					EditorUtility.ReplacePrefab(go, prefab);
#else
					PrefabUtility.ReplacePrefab(go, prefab);
#endif
					DestroyImmediate(go);
					AssetDatabase.Refresh();

					// Select the atlas
					go = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;
				}

				NGUISettings.font = go.GetComponent<UIFont>();
				NGUISettings.font.spriteName = NGUISettings.fontTexture.name;
				NGUISettings.font.atlas = NGUISettings.atlas;
				
				return NGUISettings.font;
			}
		}
		return null;
	}
}
