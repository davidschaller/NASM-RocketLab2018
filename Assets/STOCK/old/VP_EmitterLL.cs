using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VP_EmitterLL : MonoBehaviour
{
	static Material lineMaterial;

	static void CreateLineMaterial ()
	{
		if (!lineMaterial) {
			lineMaterial = new Material ("Shader \"Lines/Colored Blended\" {" +
		            "SubShader { Pass { " +
		            "    Blend SrcAlpha OneMinusSrcAlpha " +
		            "    ZWrite Off Cull Off Fog { Mode Off } " +
		            "    BindChannels {" +
		            "      Bind \"vertex\", vertex Bind \"color\", color }" +
		            "} } }");
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}
	
	public LinkedList<ParticleSystem.Particle> particles;
	static LinkedList<ParticleSystem.Particle> pool;
	
	public int initialPool = 1000;
	
	
	private Vector3 lastSpawnPosition = Vector3.zero;
	private float lastSpawnTime = 0f;
	private float cachedDeltaTime;
	private double renderTime = 0;
	private double debugTime = 0;

	void Start ()
	{
		CreateLineMaterial ();
		
		if (pool == null) {
			pool = new LinkedList<ParticleSystem.Particle> ();
			for (int i = 0; i < initialPool; i++)
				pool.AddLast (new ParticleSystem.Particle ());
		}
		particles = new LinkedList<ParticleSystem.Particle> ();
	}	
	
	void Update ()
	{
		// création des particules à la souris
		if (Input.GetMouseButton (0)) {
			Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 pos = r.origin + r.direction.normalized * Vector3.Distance (Vector3.zero, Camera.main.transform.position);
             
			int n = Random.Range (10, 100);
			int i;
			for (i=0; i<n; i++) {
				SpawnRow (pos, 0.05f, Random.Range (5f, 15f), 0.1f, new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0.5f, 1f)), Random.onUnitSphere * 0.1f);
			}
		}else{
			
			SpawnParticle(Random.insideUnitSphere * 50,Random.Range(5f,15f),0.1f,new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0.5f, 1f)),Random.onUnitSphere * 0.1f);
			
		}
		
		
		
		debugTime = Time.realtimeSinceStartup;
		UpdateParticles ();
		debugTime = Time.realtimeSinceStartup - debugTime;
	}
	
	void UpdateParticles ()
	{		
		
		if (particles.Count > 0) {
			ParticleSystem.Particle particle;
			cachedDeltaTime = Time.deltaTime;
			LinkedListNode<ParticleSystem.Particle> deleteLastNode = null;
			
			for (LinkedListNode<ParticleSystem.Particle> current = particles.First; current != null; current = current.Next) {
            
				if (deleteLastNode != null) {
					particles.Remove (deleteLastNode);
					pool.AddFirst (deleteLastNode);
					deleteLastNode = null;
				}
			
				particle = current.Value;
				particle.remainingLifetime += cachedDeltaTime;
				if (particle.remainingLifetime < particle.startLifetime) {
					particle.position += particle.velocity * cachedDeltaTime;
				} else {
					deleteLastNode = current;
				}
				current.Value = particle;
			
              
			}
		
			if (deleteLastNode != null) {
				particles.Remove (deleteLastNode);
				pool.AddFirst (deleteLastNode);
			}
		
		}
	}   
	
	       
	public ParticleSystem.Particle SpawnParticle (Vector3 position, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3), float age = 0f)
	{
		ParticleSystem.Particle particle;
		if (pool.Count == 0)
			particle = new ParticleSystem.Particle ();
		else {
			particle = pool.Last.Value;
			pool.RemoveLast ();
		}
		
		// initialisation des données de la nouvelle particule
		particle.startLifetime = startLifetime;
		particle.remainingLifetime = 0f;
		particle.size = size;
		particle.color = color;
		particle.velocity = velocity;
		if (age == 0) {
			particle.position = position;
		} else {
			particle.position = position + velocity * age;
		}
		particles.AddLast (particle);
		return particle;
	}
	
	public void SpawnRow (Vector3 endPosition, float stepDistance = 1f, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3))
	{
		SpawnRow (lastSpawnPosition, endPosition, stepDistance, startLifetime, size, color, velocity);   
	}
          
	public void SpawnRow (Vector3 startPosition, Vector3 endPosition, float stepDistance = 1f, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3))
	{
            
		float elapsed = (Time.time - lastSpawnTime);
		bool linkRow = elapsed < 0.1;
            
		if (!linkRow)
			startPosition = endPosition;
            
		Vector3 dir = endPosition - startPosition;
		int count = Mathf.FloorToInt (dir.magnitude / stepDistance);
            
		lastSpawnPosition = endPosition;
            
		if (count < 2)
			SpawnParticle (endPosition, startLifetime, size, color, velocity);
		else {
			int i;
			float stepDist = dir.magnitude / count;
			dir = dir.normalized * stepDist;
       
			float age = 0;
			float stepTime = elapsed / count;
            
			for (i=0; i<count; i++) {
                  
				startLifetime = Random.Range (5f, 15f);
				color = new Color (Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0f, 1f), Random.Range (0.5f, 1f));
				velocity = Random.onUnitSphere * 0.1f;
                  
				SpawnParticle (endPosition, startLifetime, size, color, velocity, age);
                  
				age += stepTime;
				endPosition -= dir;
			}
		}
            
		lastSpawnTime = Time.time;
            
	}
     
	
	void OnGUI ()
	{
        
		GUILayout.Space (20);
		if(particles.Count > 0){
			bool dbg = Input.GetKey (KeyCode.Space);
			if (dbg) {
				int i = 0;
				Vector2 pos;
				for (LinkedListNode<ParticleSystem.Particle> current = particles.First; current != null; current = current.Next) {
					GUI.color = current.Value.color;
					pos = Camera.main.WorldToScreenPoint (current.Value.position);
					GUI.Label (new Rect (pos.x, Screen.height - pos.y, 30, 20), i.ToString ());
					i++;
				}
			}
			GUILayout.Label (particles.Count.ToString ());
		}
		GUI.color = Color.white;
		GUILayout.Label ("updates " + (debugTime*1000).ToString ("f2") + "ms");
		
		GUI.color = Color.white;
		GUILayout.Label ("GL " + (renderTime*1000).ToString ("f2") + "ms");
	}
	
	void OnPostRender ()
	{
		renderTime = Time.realtimeSinceStartup;
		if(particles.Count>0){
			lineMaterial.SetPass (0);
			Vector3 pos;
			float size;
			Color color = Color.white;
			float ratio;
			
			float div = 1f / 255f;
			GL.Begin (GL.LINES);
			for (LinkedListNode<ParticleSystem.Particle> current = particles.First; current != null; current = current.Next) {
				if (current.Value.startLifetime > 0) {
					
					ratio = (1f - (current.Value.remainingLifetime / current.Value.startLifetime)) * div;
					color.r = current.Value.color.r * ratio;
					color.g = current.Value.color.g * ratio;
					color.b = current.Value.color.b * ratio;
					//color.a = 1f - current.Value.lifetime / current.Value.startLifetime;
					GL.Color (color);
					pos = current.Value.position;
					size = current.Value.size;
					GL.Vertex (pos);
					GL.Vertex (pos + Vector3.up * size);
					GL.Vertex (pos);
					GL.Vertex (pos + Vector3.right * size);
					GL.Vertex (pos);
					GL.Vertex (pos + Vector3.forward * size);
				}
			}
			
			GL.End ();
		}
		renderTime = Time.realtimeSinceStartup - renderTime;
	}
	
}
