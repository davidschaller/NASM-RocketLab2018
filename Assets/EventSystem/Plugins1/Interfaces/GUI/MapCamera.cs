using UnityEngine;

public class MapCamera : MonoBehaviour
{
	public static bool MapViewActive
	{
		get
		{
			return main != null ? main.GetComponent<Camera>().enabled : false;
		}
	}

	public GUITexture playerMarkerPrefab;
	GUITexture playerMarker;
	
	static MapCamera main;
	AudioListener listener;
	void Awake ()
	{
		main = this;

		if (playerMarkerPrefab)
		{
			playerMarker = (GUITexture)Instantiate(playerMarkerPrefab);
			playerMarker.name = "Player map marker";
			playerMarker.enabled = false;
		}
		
		GetComponent<Camera>().enabled = false;
		GetComponent<Camera>().depth = 1000;
		tag = "Untagged";
		storedMainCamera = Camera.main;
		listener = GetComponent<AudioListener>();
		if (!listener)
			listener = gameObject.AddComponent<AudioListener>();
		
		if (listener)
			listener.enabled = false;
	}

	Camera storedMainCamera;
	void ToggleMapView ()
	{
		if (GetComponent<Camera>().enabled)
		{
			tag = "Untagged";
			GetComponent<Camera>().enabled = false;
			storedMainCamera.tag = "MainCamera";
			storedMainCamera.enabled = true;
			AudioListener l = storedMainCamera.GetComponent<AudioListener>();
			if (l)
				l.enabled = true;
			if (listener)
				listener.enabled = false;
			MovementController.Main.UnlockPCControl(false);

			if (playerMarker)
				playerMarker.enabled = false;
		}
		else
		{
			MovementController.Main.LockPCControl();
			storedMainCamera = Camera.main;
			storedMainCamera.tag = "Untagged";
			storedMainCamera.enabled = false;
			tag = "MainCamera";
			GetComponent<Camera>().enabled = true;
			AudioListener l = storedMainCamera.GetComponent<AudioListener>();
			if (l)
				l.enabled = false;
			if (listener)
				listener.enabled = true;

			if (playerMarker)
				playerMarker.enabled = true;
		}
	}
	
	void Update ()
	{
        if (MovementController.Main && (MovementController.Main.IsInBanjoGame || MovementController.IsWritingLetter))
            return;

		if (Input.GetKeyDown("m"))
		{
			ToggleMapView();
		}
	}

	GameObject player;
	void LateUpdate ()
	{
		if (playerMarker)
		{
			if (!player)
				player = GameObject.FindWithTag("Player");
			
			if (GetComponent<Camera>().enabled && player != null)
			{
				Vector3 loc = GetComponent<Camera>().WorldToViewportPoint(player.transform.position);
			
				playerMarker.transform.position = loc;
			}
		}
	}
}
