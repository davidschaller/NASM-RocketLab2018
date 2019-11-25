using UnityEngine;
using System.Collections;

public class GoalIndicator : MonoBehaviour
{
	public bool bounces = false;
	public float bounceSpeed = 0.15f;
	public float bounceHeight = 0.1f;
	
	float ypos = 0;
	GUITexture tex;
	void Awake ()
	{
		ypos = transform.position.y;
		tex = GetComponentInChildren<GUITexture>();
		if (tex != null)
			tex.transform.parent = null;
	}
	
	void Update ()
	{
		if (bounces)
		{
			Vector3 pos = transform.position;
			pos.y = ypos + Mathf.PingPong(Time.time*bounceSpeed, bounceHeight);
			transform.position = pos;
		}
	}

	void LateUpdate ()
	{
		if (tex != null && Camera.main != null)
		{
			tex.transform.position = Camera.main.WorldToViewportPoint(transform.position);

			if (tex.transform.position.z > 0)
			{
				if (!tex.enabled)
					tex.enabled = true;
			}
			else if (tex.enabled)
			{
				tex.enabled = false;
			}
		}
	}

	public void Destruct ()
	{
		if (tex)
			Destroy(tex.gameObject);
		Destroy(gameObject);
	}
}
