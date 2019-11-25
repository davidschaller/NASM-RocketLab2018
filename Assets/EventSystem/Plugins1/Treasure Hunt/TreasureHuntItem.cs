using UnityEngine;
using System.Collections;

public class TreasureHuntItem : MonoBehaviour
{
	public string itemName;
	public string summaryText;
	public string moreInfoText;
	public string readMore = "Read More";
	public string saveContinue = "Save & Continue";
	public Texture2D thumbnailImage;
	
	public string summaryBoxStyle = "Box";
	public string moreInfoBoxStyle = "Box";
	public string titleStyle = "Label";
	public string summaryStyle = "Label";
	public string moreInfoStyle = "Label";
	public string buttonStyle = "Button";
	public string summaryThumbStyle = "mystery item thumbnail";
	public string moreInfoThumbStyle = "mystery item thumbnail";

	public TreasureNPCPackage[] npcs = new TreasureNPCPackage[3];

	public bool forceCentered = false;

	public Vector2 summaryPosition;
	public Vector2 moreInfoPosition;
	
	Rect summaryDimensions;
	Rect moreInfoDimensions;


	void SaveAndContinue ()
	{
		TreasureHuntController.CollectionGUI.enabled = true;
		displayStatus = DisplayStatus.NotReady;
		GameObject.Destroy(highlight);
		GameObject.Destroy(minimapMarker);
		enabled = false;
	}

	GameObject player;
	void Awake ()
	{
		player = GameObject.FindWithTag("Player");
		enabled = false;
	}

	public void CollectIfInPrefs ()
	{
		if (IsInPrefs)
		{
			TreasureHuntController.main.ShuffleToFront(this);
			Collect();
			//TreasureHuntController.StartNextMysterySilent();
			TreasureHuntController.main.MysteryIndex++;
		}
	}

	public void ClearPrefs ()
	{
		PlayerPrefs.DeleteKey("THI " + itemName);
	}

	public bool IsInPrefs
	{
		get
		{
			Debug.Log("'" + itemName + "': " +  PlayerPrefs.HasKey("THI " + itemName));
			return PlayerPrefs.HasKey("THI " + itemName);
		}
		private set
		{
		}
	}

	public enum DisplayStatus
	{
		NotReady,
		Collectible,
		Basic,
		MoreInfo,
	}
	
	public GameObject highlightPrefab;
	public GameObject minimapMarkerPrefab;
	GameObject highlight;
	GameObject minimapMarker;
	public void Reveal ()
	{
		if (highlightPrefab == null) Debug.LogWarning("Highlight prefab is null for this hunt item", gameObject);
		highlight = (GameObject)GameObject.Instantiate(highlightPrefab, transform.position, transform.rotation);
		highlight.transform.parent = transform.parent;

		if (minimapMarkerPrefab == null) Debug.LogWarning("Minimap marker prefab is null for this hunt item", gameObject);
		minimapMarker = (GameObject)Instantiate(minimapMarkerPrefab, transform.position + Vector3.up * 50, transform.rotation);
		minimapMarker.transform.parent = transform.parent;
		minimapMarker.layer = LayerMask.NameToLayer("Minimap Camera Only");

		displayStatus = DisplayStatus.Collectible;
		player = GameObject.FindWithTag("Player");

		enabled = true;
	}

	public void DisplayBasic ()
	{
		enabled = true;
		displayStatus = DisplayStatus.Basic;
	}

	void Collect ()
	{
		Debug.Log("Collecting the '" + itemName + "' mystery object");
		PlayerPrefs.SetInt("THI " + itemName, 1);
		displayStatus = DisplayStatus.Basic;
		TreasureMysteryScrollGUI.CollectedCount++;
	}
	
	DisplayStatus displayStatus = DisplayStatus.NotReady;

	Rect collectButton = new Rect(Screen.width/2, 100, 200, 80);
	public Vector2 collectButtonWH = new Vector2(200, 80);
	public float collectButtonDistance = 10;
	
	void OnGUI ()
	{
		GUI.skin = TreasureHuntController.GUISkin;
		GUI.depth = -110;

		if (displayStatus == DisplayStatus.Collectible &&
			(transform.position - player.transform.position).magnitude < collectButtonDistance)
		{
			collectButton.width = collectButtonWH.x;
			collectButton.height = collectButtonWH.y;
			collectButton.x = Screen.width/2 - collectButton.width/2;
			collectButton.y = Screen.height/2 - collectButton.height/2;
			if (!behindCamera && GUI.Button(collectButton, "Collect the " + name, TreasureHuntController.CollectButtonStyle))
			{
				Collect();
				TreasureHuntController.StartNextMystery();
				GUIManager.PlayButtonSound();
			}
		}
		else if (displayStatus == DisplayStatus.Basic)
		{
			GUIStyle sumBox = GUI.skin.GetStyle(summaryBoxStyle);
			
			summaryDimensions.width = sumBox.fixedWidth;
			summaryDimensions.height = sumBox.fixedHeight;

			if (forceCentered)
			{
				summaryDimensions.x = Screen.width/2 - summaryDimensions.width/2;
				summaryDimensions.y = Screen.height/2 - summaryDimensions.height/2;
			}
			else
			{
				summaryDimensions.x = summaryPosition.x;
				summaryDimensions.y = summaryPosition.y;
			}
			
			GUI.Box( summaryDimensions, "", sumBox );
			GUILayout.BeginArea( sumBox.margin.Remove(summaryDimensions) );
			GUILayout.Label("Mystery Object: " + name, titleStyle);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUIStyle thumb = new GUIStyle(GUI.skin.GetStyle(summaryThumbStyle));
			thumb.normal.background = thumbnailImage;
			GUILayout.Box("", thumb);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Label(summaryText, summaryStyle);
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button(readMore, buttonStyle))
			{
				displayStatus = DisplayStatus.MoreInfo;
				GUIManager.PlayButtonSound();
			}
						  
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(saveContinue, buttonStyle))
			{
				SaveAndContinue();
				GUIManager.PlayButtonSound();
			}
			
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea();
		}
		else if (displayStatus == DisplayStatus.MoreInfo)
		{
			GUIStyle sumBox = GUI.skin.GetStyle(moreInfoBoxStyle);
			
			moreInfoDimensions.width = sumBox.fixedWidth;
			moreInfoDimensions.height = sumBox.fixedHeight;

			if (forceCentered)
			{
				moreInfoDimensions.x = Screen.width/2 - moreInfoDimensions.width/2;
				moreInfoDimensions.y = Screen.height/2 - moreInfoDimensions.height/2;
			}
			else
			{
				moreInfoDimensions.x = moreInfoPosition.x;
				moreInfoDimensions.y = moreInfoPosition.y;
			}
			
			GUI.Box( moreInfoDimensions, "", sumBox );
			GUILayout.BeginArea( sumBox.margin.Remove(moreInfoDimensions) );
			GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
			
			GUILayout.Label("Mystery Object: " + name, titleStyle);

			GUILayout.Label(summaryText, summaryStyle);

			GUILayout.Label(moreInfoText, moreInfoStyle);

			GUILayout.EndVertical();


			

			GUILayout.BeginVertical();
			
			GUIStyle thumb = new GUIStyle(GUI.skin.GetStyle(moreInfoThumbStyle));
			thumb.normal.background = thumbnailImage;
			GUILayout.Box("", thumb);			

			

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(saveContinue, buttonStyle))
			{
				SaveAndContinue();
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.EndVertical();

			GUILayout.EndHorizontal();

			
			GUILayout.EndArea();
		}
	}

	bool behindCamera = true;
	void Update ()
	{
		if (displayStatus == DisplayStatus.Collectible)
		{
			behindCamera = Vector3.Dot(Camera.main.transform.TransformDirection(Vector3.forward), transform.position - Camera.main.transform.position) < 0.40f;
		}
	}

	public Texture2D NPCDotByName (string nm)
	{
		foreach (TreasureNPCPackage npc in npcs)
		{
			if (npc.npcController.npcName == nm)
				return npc.scrollDot;
		}
		
		return null;
	}
}
