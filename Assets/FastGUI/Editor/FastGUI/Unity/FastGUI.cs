using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

[ExecuteInEditMode]
public class FastGUI : EditorWindow
{
	[MenuItem("Monster Juice/Fast GUI")]
    static void ShowWindow () 
	{
		EditorWindow.GetWindow<FastGUI>();
    }
	private string applicationDataPath 	= Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
	public static string assetFolderToBeParseed;
	private string applicationFolderToBeParseed;
	private string lastFolderChecked;
	
	static public int targetWidht;
	static public int targetHeight;
	
	public static Object objectToBeLoaded;
	public static string sourceFolder ="";

	private bool haveXML;
	private bool haveImagesFolder;
	private bool haveRoot;
	private bool haveTargetPanel;
	private bool isOnSize;
	
	private Color defaultColor;
	
	private UIPanel targetRootPanel;
	private UIPanel lastCheckedtargetRootPanel;
	public static UIRoot actualRoot;
	private float itemActual = 1;
	private float totalItens = 1;
	
	private UIAtlas targetAtlas;
	
	
	private XMLNodeList xmlObjects;
	
	
	private string debugerText;	
	
	
	private string currentObjectName = "";
	
	private bool originalAutomaticRoot;
	
	public static FastGUIOutput actualFastGUIOutput = null;
	StreamReader reader;
	
	void OnSelectionChange () 
	{ 
		Repaint();
	}
	void OnEnable()
	{
		defaultColor = GUI.backgroundColor;
	}
    void OnGUI () 
	{
		GUILayout.Label("FastGUI", EditorStyles.boldLabel);
		FastGUIEditorTools.DrawSeparator();
		GUILayout.BeginHorizontal();
		GUILayout.Label("FastGUI Folder:");
		objectToBeLoaded 				= EditorGUILayout.ObjectField(objectToBeLoaded, typeof(Object), false);
		if(objectToBeLoaded != null)
		{
			if(AssetDatabase.GetAssetPath(objectToBeLoaded).IndexOf(".xml") > -1)
			{
				objectToBeLoaded = null;
			}
		}
		assetFolderToBeParseed 			= AssetDatabase.GetAssetPath(objectToBeLoaded);
		applicationFolderToBeParseed 	= applicationDataPath+assetFolderToBeParseed;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginVertical();
		if(lastFolderChecked != applicationFolderToBeParseed)
		{
			ResetProperties();
			HastOutputFolder();
			HasAtlas();
			lastFolderChecked 	= applicationFolderToBeParseed;
			haveXML 			= HasXML();
			haveImagesFolder 	= HasImagesFolder();
			
		}
		GUILayout.EndVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Parent Panel:");
		targetRootPanel 				= EditorGUILayout.ObjectField(targetRootPanel, typeof(UIPanel), true) as UIPanel;
		GUILayout.EndHorizontal();
		if(targetRootPanel!=null)
		{
			haveTargetPanel = true;
		}
		else
		{
			haveTargetPanel = false;
		}
		if(haveTargetPanel)
		{
			if(lastCheckedtargetRootPanel != targetRootPanel)
			{
				lastCheckedtargetRootPanel = targetRootPanel;
				actualRoot = GetUIRoot(targetRootPanel.gameObject);
				
				
				if(actualRoot == null)
				{
					haveRoot = false;
				}
				else
				{
					haveRoot = true;
				}
			}
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Target Atlas:");
		targetAtlas 				= EditorGUILayout.ObjectField(targetAtlas, typeof(UIAtlas), false) as UIAtlas;
		GUILayout.EndHorizontal();
		
		
		GUILayout.Space(10);
		if(actualRoot != null && objectToBeLoaded!=null)
		{
			if(actualRoot.manualHeight != targetHeight || actualRoot.minimumHeight < targetHeight || actualRoot.automatic)
			{
				isOnSize = false;
			}
			else
			{
				isOnSize = true;
			}
			if(!isOnSize)
			{
				GUI.backgroundColor = new Color(171f/255, 26f/255, 37f/255,1);
				if(GUILayout.Button("Update the UIRoot"))
				{
					UpdateUIRootSize();
				}
				GUI.backgroundColor = defaultColor;
			}
		}
		if(haveXML && haveRoot && haveImagesFolder && isOnSize && haveTargetPanel)
		{
			GUI.backgroundColor = new Color(17f/255, 146f/255, 156f/255,1);
			if(GUILayout.Button("FastGUI it!"))
			{
				ParseeTargetFolder();
			}
			GUI.backgroundColor = defaultColor;
			
		}
		else
		{
			UpdateDebugger();
		}
	}
	private void ResetFields()
	{
		objectToBeLoaded 	= null;
		targetRootPanel		= null;
		targetAtlas			= null;
		
	}
	private void ResetProperties()
	{
		reader						= null;
		FastGUI.actualFastGUIOutput = null;
		NGUISettings.atlas 			= null;
		NGUISettings.font 			= null;
		NGUISettings.fontData 		= null;
		NGUISettings.fontName 		= null;
		NGUISettings.fontTexture 	= null;
		targetAtlas					= null;
		FastGUI.sourceFolder		= "";
	}
	private void ParseeTargetFolder()
	{
		if(targetAtlas != null)
			NGUISettings.atlas = targetAtlas;
		
		if(xmlObjects == null)
			ReadXML();
		
		if(actualFastGUIOutput == null)
			CreateOutputPrefab();
		
		itemActual = 0;
		
		string		tType		= null;
		string		tLastPath	= null;
		Transform	tLastAnchor	= null;
		string		tLastAnchorPath = null;	
			
		foreach(XMLNode tNode in xmlObjects)
		{
			EditorUtility.DisplayProgressBar(
				"FastGUI Progress",
				("Object: "+currentObjectName+"("+itemActual+"/"+totalItens+")"),
				itemActual/totalItens);
			
			tType		= tNode["@type"].ToString();
			itemActual 	+= 1.0f;
			
			if(tNode["@type"].ToString() == "ANCHOR")
			{
				tLastPath		= tNode.GetNode ("path>0")["_text"].ToString();
				tLastAnchor 	= FastGUIPostProcessing.CreateAnchor( tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);;
				tLastAnchorPath =  tNode.GetNode ("path>0")["_text"].ToString()+"/"+tNode.GetNode ("name>0")["_text"].ToString();
			}
			else if(tNode["@type"].ToString() == "CLIPPING")
			{
				tLastPath		= tNode.GetNode ("path>0")["_text"].ToString();
				tLastAnchor 	= FastGUIPostProcessing.CreateClippingPanel( tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);;
				tLastAnchorPath =  tNode.GetNode ("path>0")["_text"].ToString()+"/"+tNode.GetNode ("name>0")["_text"].ToString();
			}
			else if(tNode["@type"].ToString() == "IMAGEBUTTON")
			{
				FastGUIPostProcessing.CreateImageButton(tNode, targetRootPanel, tLastAnchor, tLastPath);
			}
			else if(tNode["@type"].ToString() == "SPRITE")
			{
				FastGUIPostProcessing.CreateSprite( tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if(tNode["@type"].ToString() == "SLICED_SPRITE")
			{
				FastGUIPostProcessing.CreateSlicedSprite( tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if(tNode["@type"].ToString() == "SLICED_IMAGEBUTTON")
			{
				FastGUIPostProcessing.CreateSlicedImageButton( tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if( tType == "TEXT_LABEL")
			{
				FastGUIPostProcessing.CreateTextLabel(tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if( tType == "INPUT_TEXT")
			{
				FastGUIPostProcessing.CreateTextInput(tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if( tType == "CHECKBOX" )
			{
				FastGUIPostProcessing.CreateCheckBox(tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if( tType == "SLIDER" )
			{
				FastGUIPostProcessing.CreateSlider(tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			else if( tType == "PROGRESSBAR" )
			{
				FastGUIPostProcessing.CreateProgressBar(tNode, targetRootPanel, tLastAnchor, tLastAnchorPath);
			}
			
			currentObjectName = tNode.GetNode("name>0")["_text"].ToString();
			/*
			Debug.Log(tNode["@type"]);
			Debug.Log ("name: " + tNode.GetNode ("name>0")["_text"]);
			Debug.Log ("path: " + tNode.GetNode ("path>0")["_text"]);
			Debug.Log ("posX: " + tNode.GetNode ("posX>0")["_text"]);
			Debug.Log ("posY: " + tNode.GetNode ("posY>0")["_text"]);
			if(tNode.GetNode ("source>0") != null)
				Debug.Log ("source: " + tNode.GetNode ("source>0")["_text"]);
				*/
			
			
		}
		EditorUtility.ClearProgressBar();
		actualRoot.automatic 	= originalAutomaticRoot;
		ResetProperties();
		ResetFields();
	}
	private void UpdateUIRootSize()
	{
		originalAutomaticRoot		= actualRoot.automatic;
		actualRoot.automatic 		= false;
		actualRoot.minimumHeight 	= targetHeight;
		actualRoot.manualHeight 	= targetHeight;
		
		
		float size = 2f / targetHeight;
		Vector3 ls = actualRoot.transform.localScale;

		if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) ||
			!(Mathf.Abs(ls.y - size) <= float.Epsilon) ||
			!(Mathf.Abs(ls.z - size) <= float.Epsilon))
		{
			actualRoot.transform.localScale = new Vector3(size, size, size);
		}
		
	}
	private UIRoot GetUIRoot(GameObject tTarget)
	{
		if(tTarget.transform.parent!=null)
		{
			if(tTarget.transform.parent.gameObject.GetComponent<UIRoot>())
			{
				return tTarget.transform.parent.gameObject.GetComponent<UIRoot>();
			}
			else
			{
				return GetUIRoot(tTarget.transform.parent.gameObject);
			}
		}
		return null;
	}
	private void UpdateDebugger()
	{
		FastGUIEditorTools.DrawSeparator();

		GUILayout.Label("Output", EditorStyles.boldLabel);
		
		
		if(!haveXML)
			if(objectToBeLoaded!=null)
				GUILayout.Label("FastGUIData.xml can't be found inside the folder: "+objectToBeLoaded.name+"\n", EditorStyles.wordWrappedMiniLabel);
		
		if(!haveImagesFolder)
			if(objectToBeLoaded!=null)
				GUILayout.Label("Image Foulder can't be found inside the folder: "+objectToBeLoaded.name+"\n", EditorStyles.wordWrappedMiniLabel);
		
		if(!haveRoot)		
			if(targetRootPanel!=null)
				GUILayout.Label("Can't find the UIRoot of panel: "+targetRootPanel, EditorStyles.wordWrappedMiniLabel);
		
		if(!haveTargetPanel)		
			GUILayout.Label("You must select one target Panel", EditorStyles.wordWrappedMiniLabel);
		
		
		if(objectToBeLoaded == null)
			GUILayout.Label("You must select one valid FastGUI export folder", EditorStyles.wordWrappedMiniLabel);	
		
		
		if(targetAtlas == null)
			GUILayout.Label("A new Atlas will be created", EditorStyles.wordWrappedMiniLabel);	
		
		FastGUIEditorTools.DrawSeparator();
	}
	
	private void ReadXML()
	{	
		string fileData = "";
		reader = new StreamReader(assetFolderToBeParseed+"/FastGUIData.xml");
		fileData = reader.ReadToEnd();
		reader.Close();
		
		XMLParser parser 	= new XMLParser();
        
        XMLNode node 		= parser.Parse( fileData );

        xmlObjects   		= node.GetNodeList("psd>0>layer");
		
		targetWidht = int.Parse(node.GetNode("psd>0")["@width"].ToString());
		targetHeight = int.Parse(node.GetNode("psd>0")["@height"].ToString());
		
		totalItens = (float)xmlObjects.Count;   
	}
	private bool HasXML()
	{
		string[] files = Directory.GetFiles(applicationFolderToBeParseed, "FastGUIData.xml", SearchOption.TopDirectoryOnly);
		
		if(files.Length > 0)
		{
			ReadXML();
			return true;
		}
		return false;
	}
	private void CreateOutputPrefab()
	{
		if(!Directory.Exists(applicationFolderToBeParseed+"/Output/"))
			Directory.CreateDirectory(applicationFolderToBeParseed+"/Output/");
		AssetDatabase.Refresh();
		
		Object prefabOutput = PrefabUtility.CreateEmptyPrefab(FastGUI.assetFolderToBeParseed+"/Output/FastGUIOutput.prefab");
		
		
		// Create a new game object for the atlas
		GameObject go = new GameObject("FastGUIOutput");
		go.AddComponent<FastGUIOutput>();

		// Update the prefab
		PrefabUtility.ReplacePrefab(go, prefabOutput);
		
		DestroyImmediate(go);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		
		actualFastGUIOutput = AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Output/FastGUIOutput.prefab", typeof(FastGUIOutput)) as FastGUIOutput;
		actualFastGUIOutput.references = new Dictionary<int, int>();
	}
	private bool HastOutputFolder()
	{
		//actualFastGUIOutput
		if(!Directory.Exists(applicationFolderToBeParseed+"/Output/"))
			return false;
		
		string[] files = Directory.GetFiles(applicationFolderToBeParseed+"/Output/", "FastGUIOutput.prefab", SearchOption.TopDirectoryOnly);
		if(files.Length > 0)
		{
			actualFastGUIOutput = AssetDatabase.LoadAssetAtPath(FastGUI.assetFolderToBeParseed+"/Output/FastGUIOutput.prefab", typeof(FastGUIOutput)) as FastGUIOutput;
			return true;
		}
		return false;
	}
	private bool HasImagesFolder()
	{
		if(!Directory.Exists(applicationFolderToBeParseed+"/Images/"))
			return false;
		
		string[] files = Directory.GetFiles(applicationFolderToBeParseed+"/Images/", "*.png", SearchOption.TopDirectoryOnly);
		if(files.Length > 0)
		{
			return true;
		}
		return false;
		
		
	}
	private bool HasAtlas()
	{
		if(!Directory.Exists(applicationFolderToBeParseed+"/Source/"))
			return false;
		
		string[] files = Directory.GetFiles(applicationFolderToBeParseed+"/Source/", objectToBeLoaded.name+".prefab", SearchOption.TopDirectoryOnly);
		if(files.Length > 0)
		{
			targetAtlas = AssetDatabase.LoadAssetAtPath(assetFolderToBeParseed+"/Source/"+objectToBeLoaded.name+".prefab", typeof(UIAtlas)) as UIAtlas;
			return true;
		}
		return false;
	}
	
}

