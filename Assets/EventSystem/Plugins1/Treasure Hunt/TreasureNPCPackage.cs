using UnityEngine;
using System.Collections;

[System.Serializable]
public class TreasureNPCPackage
{
	public NPCController npcController;
	public string npcLocationName;
	public EventPlayer npcTreasureDialogEvent;
	public Color minimapColor = Color.red;
	public Texture2D scrollDot;
}