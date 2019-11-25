using UnityEngine;
using System.Collections;
using UnityEditor;

public class CannonUtility
{
	[MenuItem("Component/Cannons/Add Projectile Target to selected GO")]
	public static void AddProjectileTarget()
	{
		GameObject sel = Selection.activeGameObject;
		if ( sel != null)
		{
			if (sel.GetComponent(typeof(ProjectileTarget)) == null)
				sel.AddComponent(typeof(ProjectileTarget));
			if (sel.GetComponent<Rigidbody>() == null)
				sel.AddComponent(typeof(Rigidbody));
		}
	}

	[MenuItem("Component/Cannons/Add Projectile to selected GO")]
	public static void AddProjectile()
	{
		GameObject sel = Selection.activeGameObject;
		if ( sel != null)
		{
			if (sel.GetComponent(typeof(Projectile)) == null)
				sel.AddComponent(typeof(Projectile));
			if (sel.GetComponent<Rigidbody>() == null)
				sel.AddComponent(typeof(Rigidbody));
		}
	}
	
	[MenuItem("Component/Cannons/Add Player Cannon to selected GO (only one per scene!)")]
	public static void AddPlayerCannon()
	{
		GameObject sel = Selection.activeGameObject;
		if ( sel != null)
		{
			if (sel.GetComponent(typeof(CannonController)) == null)
				sel.AddComponent(typeof(CannonController));
			if (sel.GetComponent(typeof(CannonGUI)) == null)
				sel.AddComponent(typeof(CannonGUI));
			if (sel.GetComponent(typeof(AudioSource)) == null)
				sel.AddComponent(typeof(AudioSource));
		}
	}

	[MenuItem("Component/Cannons/Add Target Registry to selected GO (only one per scene!)")]
	public static void AddTargetRegistry()
	{
		GameObject sel = Selection.activeGameObject;
		if ( sel != null)
		{
			if (sel.GetComponent(typeof(TargetRegistry)) == null)
				sel.AddComponent(typeof(TargetRegistry));
		}
	}

}
