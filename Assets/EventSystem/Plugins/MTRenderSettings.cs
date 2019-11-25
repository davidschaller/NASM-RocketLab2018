using UnityEngine;

public class MTRenderSettings : ScriptableObject
{
	public bool fog = false;
	public Color fogColor = Color.white;
	public float fogDensity = 0.002f;
	public Color ambientLight = Color.white;
	public Material skybox;
	public float haloStrength = 0.5f;
	public float flareStrength = 1;
	//public Texture2D haloTexture;
	//public Texture2D spotCookie;
}