using UnityEngine;

public class TargetRegistry : MonoBehaviour
{
	public string multiplierGUIStyle = "Box";
	public Rect multiplierOffset = new Rect(0,0,0,0);
	
	public ParticleSystem dirtParticles;
	public AudioClip dirtSFX;
	public bool dirtSticksParticles = false;
	public string dirtGUIStyle = "Box";

	public ParticleSystem earthworksParticles;
	public AudioClip earthworksSFX;
	public bool earthworksSticksParticles = false;
	public string earthworksGUIStyle = "Box";
	
	public ParticleSystem buildingParticles;
	public AudioClip buildingSFX;
	public bool buildingSticksParticles = false;
	public string buildingGUIStyle = "Box";
	
	public ParticleSystem waterParticles;
	public AudioClip waterSFX;
	public bool waterSticksParticles = false;
	public string waterGUIStyle = "Box";
	
	public ParticleSystem shipDeckParticles;
	public AudioClip shipDeckSFX;
	public bool shipDeckSticksParticles = true;
	public bool fadeShipDeckDisappearance = false;
	public string shipdeckGUIStyle = "Box";
	
	public ParticleSystem emplacementParticles;
	public AudioClip emplacementSFX;
	public bool emplacementSticksParticles = false;
	public string emplacementGUIStyle = "Box";
	
	static TargetRegistry main;
	public static TargetRegistry Main
	{
		get
		{
			return main;
		}
		set
		{
			main = value;
		}
	}

	void Awake ()
	{
		Debug.Log("TargetRegistry", gameObject);
		
		currentMultiplier = 0;
		main = this;
	}

	public static bool FadeType(ProjectileTarget.TargetType targetType)
	{
		bool fade = false;
		switch(targetType)
		{
			case ProjectileTarget.TargetType.ShipDeck:
				fade = true;
				break;
		}

		return fade;
	}

	static int currentMultiplier = 0;
	public static int CurrentMultiplier
	{
		get
		{
			return currentMultiplier;
		}
		set
		{
			currentMultiplier = value;
		}
	}
	
	public static string GetGUIStyle(ProjectileTarget.TargetType targetType)
	{
		string style = "Box";
		
		switch(targetType)
		{
			case ProjectileTarget.TargetType.Dirt:
				style = Main.dirtGUIStyle;
				break;
			case ProjectileTarget.TargetType.EarthWorks:
				style = Main.earthworksGUIStyle;
				break;
			case ProjectileTarget.TargetType.Building:
				style = Main.buildingGUIStyle;
				break;
			case ProjectileTarget.TargetType.Water:
				style = Main.waterGUIStyle;
				break;
			case ProjectileTarget.TargetType.ShipDeck:
				style = Main.shipdeckGUIStyle;
				break;
			case ProjectileTarget.TargetType.Emplacement:
				style = Main.emplacementGUIStyle;
				break;
		}
		
		return style;
	}

	public static string GetMultiplierStyle ()
	{
		return Main.multiplierGUIStyle;
	}

	public static Rect GetMultiplierOffset ()
	{
		return Main.multiplierOffset;
	}
									 
	public static void DoParticles(ProjectileTarget.TargetType targetType, Vector3 pos, Transform targTransform)
	{
		ParticleSystem prefab = null;
		AudioClip sound = null;
		bool sticky = false;
		switch(targetType)
		{
			case ProjectileTarget.TargetType.Dirt:
				prefab = Main.dirtParticles;
				sound = Main.dirtSFX;
				sticky = Main.dirtSticksParticles;
				break;
			case ProjectileTarget.TargetType.EarthWorks:
				prefab = Main.earthworksParticles;
				sound = Main.earthworksSFX;
				sticky = Main.earthworksSticksParticles;
				break;
			case ProjectileTarget.TargetType.Building:
				prefab = Main.buildingParticles;
				sound = Main.buildingSFX;
				sticky = Main.buildingSticksParticles;
				break;
			case ProjectileTarget.TargetType.Water:
				prefab = Main.waterParticles;
				sound = Main.waterSFX;
				sticky = Main.waterSticksParticles;
				break;
			case ProjectileTarget.TargetType.ShipDeck:
				prefab = Main.shipDeckParticles;
				sound = Main.shipDeckSFX;
				sticky = Main.shipDeckSticksParticles;
				break;
			case ProjectileTarget.TargetType.Emplacement:
				prefab = Main.emplacementParticles;
				sound = Main.emplacementSFX;
				sticky = Main.emplacementSticksParticles;
				break;
		}

		ParticleSystem emitter = null;
		if (prefab != null)
		{
			Debug.Log("Instantiate particles for " + targetType);
			emitter = (ParticleSystem)Instantiate(prefab, pos, Quaternion.identity);
			ProjectileTarget pt = targTransform.GetComponent<ProjectileTarget>();
			if (pt)
				pt.ImpactParticles = emitter;
			if (sticky)
			{
				if (targetType == ProjectileTarget.TargetType.ShipDeck)
					emitter.transform.parent = targTransform.parent;
				else
					emitter.transform.parent = targTransform;
			}
		}
		else
		{
			Debug.Log("No particle type defined for " + targetType);
		}

		if (sound != null)
		{
            if (!emitter.gameObject.GetComponent<AudioSource>())
                Common.AddAudioTo(emitter.gameObject);

			emitter.gameObject.GetComponent<AudioSource>().clip = sound;
			emitter.gameObject.GetComponent<AudioSource>().PlayOneShot(sound);
		}
	}
}