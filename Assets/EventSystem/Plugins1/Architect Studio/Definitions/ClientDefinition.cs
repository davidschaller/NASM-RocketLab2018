using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientDefinition : MonoBehaviour
{
	public string job;
	public List<string> needsAndInterests;
	public TextAsset description;
    public Texture2D image;

	public int clientSortKey = 0;


	public void Deactivate ()
	{
		gameObject.SetActiveRecursively(false);
	}

	public void Activate ()
	{
		gameObject.SetActiveRecursively(true);
	}
}
