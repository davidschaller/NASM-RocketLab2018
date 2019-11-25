using UnityEngine;

public class EventPlayerTrigger : MonoBehaviour 
{
    void OnTriggerEnter (Collider coll) {

        EventPlayerBase ep = (EventPlayerBase)transform.GetComponent(typeof(EventPlayerBase));
        if (ep)
        {
            print("Got EventPlayer trigger");
            ep.PlayerTriggered();
        }
	}

}



