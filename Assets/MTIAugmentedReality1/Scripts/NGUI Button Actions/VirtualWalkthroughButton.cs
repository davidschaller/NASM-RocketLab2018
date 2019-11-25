using UnityEngine;
using System.Collections;

public class VirtualWalkthroughButton : MonoBehaviour
{
	void OnClick ()
	{
		PersistentData.Main.virtualWalkthrough = true;
		
		StartCoroutine(StartWalkthrough());
	}
	
	IEnumerator StartWalkthrough ()
	{
		yield return 0;
		
		transform.parent.Find("Go Back button").GetComponent<DeactivateUIRootButton>().OnClick();
		
		LoadMainSceneButton.main.DoLoad();		
	}
}
