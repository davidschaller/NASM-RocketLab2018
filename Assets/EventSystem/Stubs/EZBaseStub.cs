using UnityEngine;
using System.Collections;

public class EZBaseStub : MonoBehaviour
{
	public MonoBehaviour scriptWithMethodToInvoke;
	public string methodToInvoke;
	public string Text
	{
		get; set;
	}
	
	public Camera RenderCamera
	{
		get; set;
	}
	
	public void SetCamera (Camera c)
	{
		
	}
}
