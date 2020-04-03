using UnityEngine;
using System.Collections;

public class Rocket_Launch: MonoBehaviour {

public GameObject rocketLaunchFX;
public GameObject rocketLaunchAnim;
public AudioSource rocketLaunchAudio;

    void Start (){

        rocketLaunchFX.SetActive(false);


    }  
  
  
void Update (){
 
    if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - launch rocket
    {

         StartCoroutine("LaunchRocket");


    }
            
}


IEnumerator LaunchRocket(){

 
        rocketLaunchAudio.Play();

        yield return new WaitForSeconds(7.5f);
        rocketLaunchAnim.GetComponent<Animation>().Play();

        rocketLaunchFX.SetActive(false);
        rocketLaunchFX.SetActive(true);
        

    }


}