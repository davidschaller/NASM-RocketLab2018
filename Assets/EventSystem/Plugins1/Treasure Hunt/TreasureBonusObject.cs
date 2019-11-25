using UnityEngine;
using System.Collections;

public class TreasureBonusObject : MonoBehaviour
{
	public GameObject spherePrefab;
	public Texture2D itemImage;
	public string itemName;
	public string itemDescription;
	public string shortDescription;
	public string buttonText = "Save & Continue";

	public string bonusBoxStyle = "bonus object box";
	public string nameStyle = "Label";
	public string descriptionStyle = "Label";
	public string buttonStyle = "Button";
	public string imageStyle = "bonus object image";
	
	Transform sphere;

	Renderer texturePlane;

	Transform player;
	void Start ()
	{
		sphere = ((GameObject)Instantiate(spherePrefab, transform.position, transform.rotation)).transform;
		texturePlane = GetComponentInChildren<Renderer>();
		texturePlane.material.mainTexture = itemImage;
		player = GameObject.FindWithTag("Player").transform;
	}
	
	void LateUpdate ()
	{
		transform.LookAt(player);
	}


	public void CollectIfInPrefs ()
	{
		if (IsInPrefs)
		{
			Collect();
		}		
	}

	public void ClearPrefs ()
	{
		PlayerPrefs.DeleteKey("TBO " + itemName);
	}

	public bool IsInPrefs
	{
		get
		{
			return PlayerPrefs.HasKey("TBO " + itemName);
		}
		private set
		{
		}
	}
	
	void Collect ()
	{
		PlayerPrefs.SetInt("TBO " + itemName, 1);
		Destroy(sphere.gameObject);
		Destroy(GetComponent<Collider>());
		TreasureHuntCollectionGUI.FoundBonusObject(this);
		Destroy(texturePlane.gameObject);
	}
	
	void OnTriggerEnter(Collider coll)
	{
		Debug.Log("Triggered by " + coll);
		Collect();
		displayGUI = true;
	}

	bool collected = false;
	public void DisplayFromCollection()
	{
		enabled = true;
		displayGUI = true;
		collected = true;
	}
	
	bool displayGUI = false;
	void OnGUI ()
	{
		if (displayGUI)
		{
			GUI.depth = -110;
			GUI.skin = TreasureHuntController.GUISkin;

			GUIStyle boxStyle = GUI.skin.GetStyle(bonusBoxStyle);
			Rect boxRect = new Rect(Screen.width/2 - boxStyle.fixedWidth/2,
									Screen.height/2 - boxStyle.fixedHeight/2,
									boxStyle.fixedWidth,
									boxStyle.fixedHeight);
			GUI.Box(boxRect, "Bonus Object!", boxStyle);

			boxRect.y += boxStyle.padding.top + boxStyle.margin.top;
			boxRect.height -= boxStyle.padding.top + boxStyle.margin.top + boxStyle.padding.bottom + boxStyle.margin.bottom;
			
			GUILayout.BeginArea(boxRect);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUIStyle thumbStyle = new GUIStyle(GUI.skin.GetStyle(imageStyle));
		
			if (itemImage != null)
				thumbStyle.normal.background = itemImage;
			
			GUILayout.Label("", thumbStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Label(itemName, nameStyle);
			GUILayout.Label(itemDescription, descriptionStyle);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(buttonText, buttonStyle))
			{
				enabled = false;
				if (!collected)
				{
					TreasureHuntCollectionGUI.Main.enabled = true;
					TreasureHuntCollectionGUI.Main.DisplayEasterEggs();
				}
				GUIManager.PlayButtonSound();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea();
		}
	}
}
