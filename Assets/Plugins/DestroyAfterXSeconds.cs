using UnityEngine;
using System.Collections;

public class DestroyAfterXSeconds : MonoBehaviour
{
	public float lifetimeInSeconds = 10;
	
	void Start ()
	{
		Invoke("DestroyObject", lifetimeInSeconds);
	}
	
	void DestroyObject ()
	{
		Destroy(gameObject);
	}
}
