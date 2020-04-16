using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CloudMover : MonoBehaviour {

	public float movX, movY, movZ;
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(transform.up * Time.deltaTime * movY);
		transform.Rotate(transform.right * Time.deltaTime * movZ);
		transform.Rotate(transform.forward * Time.deltaTime * movX);
	}
}
