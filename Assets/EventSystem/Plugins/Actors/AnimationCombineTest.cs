using UnityEngine;
using System.Collections;

public class AnimationCombineTest : MonoBehaviour
{
	public AnimationClip aboveWaistAnimation;
	public AnimationClip sittingAnimation;
	
	public bool sittingUsingStandingAnimation = true;
	public bool male = true;
	
	void Fail( string msg )
	{
		Debug.LogWarning(msg, gameObject);
		gameObject.active = false;
	}
	
	void Awake ()
	{
		
		if (GetComponent<Animation>() == null)
		{
			if (Application.isEditor)
				Debug.LogWarning("There is no Animation on this object, can't combine animations.  Select this log entry to highlight in hierarchy", gameObject);
			enabled = false;
			return;
		}
		
		/*
		TODO: I got this to work successfully when applied to a model already sitting.
		Adding the AddMixingTransform(back) to the applied standing animation
		made it work OK.  I have not yet successfully applied it to a standing model
		with sitting and standing animations applied to it.
		*/
        if (GetComponent<Animation>())
        {
            if (sittingAnimation)
            {
                GetComponent<Animation>().AddClip(sittingAnimation, "sit");
            }
            if (aboveWaistAnimation)
            {
                GetComponent<Animation>().AddClip(aboveWaistAnimation, "upperBody");
            }
            if (aboveWaistAnimation)
            {
                GetComponent<Animation>().clip = aboveWaistAnimation;
            }
        }
		
		if (GetComponent<Animation>()["upperBody"] == null)
		{
			Fail("Failed to apply upperBody animation, disabling AnimationCombine");
			return;
		} 
		
		if (sittingUsingStandingAnimation)
		{
			Transform back = transform.Find("hips/spine_1/spine_2");
			if (back == null)
				back = transform.Find("Joint_group/hips/spine_1/spine_2");
			if (back == null)
			{
				Fail("Failed to find back transform (incompatible model hierarchy?), disabling AnimationCombine.  Attempted: hips/spine_1/spine_2 & Joint_group/hips/spine_1/spine_2");
				return;
			} 
			
			//Transform waist = transform.Find("hips/spine_1");
			//Transform hipsL = transform.Find("hips/leftUpLeg");
			//Transform hipsR = transform.Find("hips/rightUpLeg");

			//animation["upperBody"].AddMixingTransform(back, true);
			GetComponent<Animation>()["upperBody"].AddMixingTransform(back);

			//animation["sit"].AddMixingTransform(hipsL, true);
			//animation["sit"].AddMixingTransform(hipsR, true);
			//animation["sit"].AddMixingTransform(back);

			GetComponent<Animation>().CrossFade("upperBody");
			//animation.CrossFade("sit");
		}
		else
		{
			if (male)
			{
				//Transform back = transform.Find("hips/spine_1/spine_2");
				Transform waist = transform.Find("hips/spine_1");
				Transform hipsL = transform.Find("hips/leftUpLeg");
				Transform hipsR = transform.Find("hips/rightUpLeg");

				//animation["upperBody"].AddMixingTransform(back, true);
				//animation["upperBody"].AddMixingTransform(back);

				GetComponent<Animation>()["sit"].AddMixingTransform(hipsL, true);
				GetComponent<Animation>()["sit"].AddMixingTransform(hipsR, true);
				GetComponent<Animation>()["sit"].AddMixingTransform(waist);

				//animation.CrossFade("upperBody");
				GetComponent<Animation>().CrossFade("sit");
			}
			else
			{
				Transform back = transform.Find("Joint_group/hips/spine_1/spine_2");
				if (back == null)
				{
					Fail("Failed to find back transform (incompatible model hierarchy?), disabling AnimationCombine.  Attempted Joint_group/hips/spine_1/spine_2");
					return;
				}
				
				Transform waist = transform.Find("Joint_group/hips/spine_1");
				Transform hipsL = transform.Find("Joint_group/hips/leftUpLeg");
				Transform hipsR = transform.Find("Joint_group/hips/rightUpLeg");

				GetComponent<Animation>()["upperBody"].AddMixingTransform(back, true);
				//animation["upperBody"].AddMixingTransform(back);

				GetComponent<Animation>()["sit"].AddMixingTransform(hipsL, true);
				GetComponent<Animation>()["sit"].AddMixingTransform(hipsR, true);
				GetComponent<Animation>()["sit"].AddMixingTransform(waist);

				GetComponent<Animation>().CrossFade("upperBody");
				//animation.CrossFade("sit");
			}
		}
	}
}
