using UnityEngine;
using System.Collections;

public class AboutThisAppButton : MonoBehaviour
{
	public GameObject aboutThisAppRoot;
	
	public static AboutThisAppButton main;
	void Start ()
	{
		main = this;
		if (aboutThisAppRoot != null)
			aboutThisAppRoot.SetActiveRecursively(false);
	}

	void OnClick ()
	{
		if (aboutThisAppRoot != null)
			aboutThisAppRoot.SetActiveRecursively(true);
		aboutThisAppRoot.transform.Find("Camera/Anchor/Panel/Virtual Walkthrough button").gameObject.AddComponent<VirtualWalkthroughButton>();
	}
}
