using UnityEngine;
using System.Collections;

public class DeactivateUIRootButton : MonoBehaviour
{
	public void OnClick ()
	{
		transform.root.gameObject.SetActiveRecursively(false);
	}
}
