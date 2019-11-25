using UnityEngine;
using System.Collections;

public class ActorUtility
{
	static int tries=4;
	static ISceneActor player = null;
	static Biped biped = null;
    static Quadruped quadruped = null;

    public static bool PlayerFound
    {
        get
        {
            if (player != null)
                return true;
            else
                return false;
        }
    }

	public static IEnumerator WaitForPlayer ()
	{
		Debug.Log ("Waiting for player");
		GameObject go = (GameObject)GameObject.FindWithTag ("Player");

        if (go)
        {
			biped = (Biped)go.GetComponent (typeof(Biped));
			quadruped = go.GetComponent<Quadruped> ();

		    while (biped == null && quadruped == null && tries > 0)
		    {
				biped = (Biped)go.GetComponent (typeof(Biped));
				quadruped = go.GetComponent<Quadruped> ();
				yield return new WaitForSeconds (0);
				tries--;
				Debug.Log ("Tries: " + tries);
			}
		}
		else
			Debug.Log("No player go");
		if (tries <= 0)
		{
			Debug.Log("Couldn't find player...");
			throw new System.Exception("Couldn't find PC controlled object to give control!");
		}
		//Debug.Log("Found player: " + biped);
		if (biped != null)
			player = (ISceneActor)biped;
		else if (quadruped != null)
			player = (ISceneActor)quadruped;
		tries=4;
	}
	
	public static IEnumerator WaitForCamera ()
	{
		while (Camera.main == null)
		{
			yield return 0;
		}
	}
	
	static ActorUtility main;
	public static IEnumerator WaitForPlayerAndSetControl ()
	{
		Debug.Log("Trying to wait for player");
		yield return MovementController.Main.StartCoroutine(ActorUtility.WaitForPlayer());
		
		if (player != null && !FlashbackManager.FlashbackActive)
		{
			Debug.Log("Finished waiting for player: " + player);

			Debug.Log("Wait for camera");
			yield return MovementController.Main.StartCoroutine(ActorUtility.WaitForCamera());
			Debug.Log("...got camera");
			
			PCCamera pc = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
			pc.Target = player.GetTransform();
			pc.WatchTarget = null;
			pc.enabled = true;
			MovementController.ControlTarget = player;
			MovementController.Main.ClearWalkTarget();
		}
	}
}
