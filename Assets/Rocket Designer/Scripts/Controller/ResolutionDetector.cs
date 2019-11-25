using UnityEngine;
using System.Collections.Generic;

public class ResolutionDetector : MonoBehaviour
{
    public static ResolutionDetector Main { get; private set; }

    public enum Resolutions
    {
        Ipad,
        IpadRetina,
        Web,
        Standalone
    }

    public Resolutions detectedResolution { get; private set;}

    public Transform Gui2D { get; private set; }
    public Transform Gui3D { get; private set; }

    public Transform ipadDefault2D,
                     ipadDefault3D,
                     ipadRetina2D,
                     ipadRetina3D,
                     web2D,
                     web3D,
                   	 standalone2D,
                     standalone3D;

    void Awake()
    {
        if (!Main)
            Main = this;

#if UNITY_IOS
		switch(iPhoneSettings.generation)
		{
			case iPhoneGeneration.iPad3Gen:
			case iPhoneGeneration.iPad4Gen:
			case iPhoneGeneration.iPadUnknown:
				Gui2D = ipadRetina2D;
                Gui3D = ipadRetina3D;
                detectedResolution = Resolutions.IpadRetina;
				break;
			default:
				Gui2D = ipadDefault2D;
                Gui3D = ipadDefault3D;   
                detectedResolution = Resolutions.Ipad;
				break;
		}
#elif UNITY_WEBGL
        Gui2D = web2D;
        Gui3D = web3D;   
        detectedResolution = Resolutions.Web;
#else
	Gui2D = standalone2D;
	Gui3D = standalone3D;   
	detectedResolution = Resolutions.Standalone;
	break;
#endif

        if (Gui2D)
            Gui2D.gameObject.SetActive(true);

        if (Gui3D)
            Gui3D.gameObject.SetActive(true);
    }
}
