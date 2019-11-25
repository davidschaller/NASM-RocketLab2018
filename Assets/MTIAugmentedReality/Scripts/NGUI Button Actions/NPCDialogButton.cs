using UnityEngine;
using System.Collections;

public class NPCDialogButton : MTIUIBase
{
	UILabel dialogText;
	UILabel dialogNPCName;
	
	UIToggleStub dialogBoxToggle;
	UIToggleStub dialogNPCNameplateToggle;
	
	public static NPCDialogButton main;
	void Start ()
	{
		main = this;
		dialogText = transform.parent.Find("Dialog text").GetComponent<UILabel>();
		dialogNPCName = transform.parent.Find("Dialog NPC Name").GetComponent<UILabel>();
		dialogBoxToggle = transform.parent.Find("Dialog box").GetComponent<UIToggleStub>();
		dialogNPCNameplateToggle = transform.parent.Find("Dialog NPC Nameplate").GetComponent<UIToggleStub>();
		
		HideDialog();
	}
	
	void HideDialog ()
	{
		DeactivateUI(false);
		dialogBoxToggle.DeactivateUI(false);
		dialogText.GetComponent<UIToggleStub>().DeactivateUI(false);
		dialogNPCName.GetComponent<UIToggleStub>().DeactivateUI(false);
		dialogNPCNameplateToggle.DeactivateUI(false);
	}
	
	NPCController npc;
	public void ShowDialog (NPCController npc)
	{
		this.npc = npc;
		NPCController.waitingForNPCDialogButton = true;
		DeactivateUI(true);
		dialogBoxToggle.DeactivateUI(true);
		dialogText.GetComponent<UIToggleStub>().DeactivateUI(true);
		dialogNPCName.GetComponent<UIToggleStub>().DeactivateUI(true);
		dialogNPCNameplateToggle.DeactivateUI(true);	
		
		dialogText.text = npc.greetingText;
		dialogNPCName.text = npc.npcName;
		
		StartCoroutine(NPCGreeting());
	}
	
	Quaternion startRotation;
	IEnumerator NPCGreeting ()
	{
		startRotation = npc.transform.rotation;
		bool facing = false;
		//while (!facing)
		{
			Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.position - npc.transform.position);
			rotation.x = 0.0f;
			rotation.z = 0.0f;
			npc.transform.rotation = rotation;

			yield return 0;
		}
		
		npc.AnimateWithSound(npc.greetingAnimation.name, npc.standardGreeting);
	}
	
	void OnClick ()
	{
		npc.transform.rotation = startRotation;
		NPCController.waitingForNPCDialogButton = true;
		HideDialog();
		if (GPSActor.main)
			GPSActor.main.positionLocked = false;
		MoveWithCompass.lockOrientation = false;
		MoveWithCompass.ignoreCompass = false;
	}
}
