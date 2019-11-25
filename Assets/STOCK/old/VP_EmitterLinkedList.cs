using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VP_EmitterLinkedList : MonoBehaviour {
	
	public LinkedList<ParticleSystem.Particle> particles = new LinkedList<ParticleSystem.Particle>();
	public bool autoResize = false;
	public int chunkSize = 512;
	
	[System.NonSerialized]
	public double debugTime = 0;
	
	[System.NonSerialized]
	public int particleTotal = 0;
	
	[System.NonSerialized]
	public int particleLive = 0;
	
	private int liveParticleCount = 0;
	private int _particleCount = 1024;
	
	private int[] particlesPointers;
	private float lastSpawnTime = 0f;
	private Vector3 lastSpawnPosition = Vector3.zero;
	
	private LinkedListNode<ParticleSystem.Particle> lastLiveNode;
	
	void Start(){
		_particleCount = 0;
		particleTotal = _particleCount;
		Clear();
	}
	
	public void Clear(bool reset = false){
		particles.Clear();
		liveParticleCount = 0;
		particleLive = 0;
	}
	
	// redimensionnement de la liste de particules
	public void Resize(int newParticleCount){		
		
		while(particles.Count < newParticleCount){
			particles.AddLast(new ParticleSystem.Particle());
		}
		_particleCount = newParticleCount;
		particleTotal = newParticleCount;

	}
	
	// création d'une particules
	private ParticleSystem.Particle tempSwap;
	public LinkedListNode<ParticleSystem.Particle> SpawnParticle(Vector3 position, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3), float age = 0f){
		
		if(liveParticleCount >= _particleCount) Resize (_particleCount + chunkSize);
		
		if(liveParticleCount < _particleCount){
			
			if(lastLiveNode == null){
				lastLiveNode = particles.First;
			}else{
				lastLiveNode = lastLiveNode.Next;	
			}
			liveParticleCount++;
			particleLive = liveParticleCount;
			
			tempSwap = lastLiveNode.Value;

			tempSwap.startLifetime = startLifetime;
			tempSwap.remainingLifetime = 0f;
			tempSwap.size = size;
			tempSwap.color = color;
			tempSwap.velocity = velocity;
			if(age == 0){
				tempSwap.position = position;
			}else{
				tempSwap.position = position + velocity*age;
			}
			
			lastLiveNode.Value = tempSwap;
			
			return lastLiveNode;
		}
		
		return null;
	}
	
	// trace une ligne de particules
	public void SpawnRow(Vector3 endPosition, float stepDistance = 1f, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3)){
		SpawnRow(lastSpawnPosition, endPosition, stepDistance, startLifetime ,  size, color, velocity);	
	}
	
	public void SpawnRow(Vector3 startPosition, Vector3 endPosition, float stepDistance = 1f, float startLifetime = 1f, float size = 1f, Color color = default(Color), Vector3 velocity = default(Vector3)){
		
		float elapsed = (Time.time - lastSpawnTime);
		bool linkRow = elapsed < 0.1;
		
		if(!linkRow) startPosition = endPosition;
		
		Vector3 dir = endPosition - startPosition;
		int count = Mathf.FloorToInt(dir.magnitude / stepDistance);
		
		lastSpawnPosition = endPosition;
		
		if(count < 2){
			SpawnParticle(endPosition, startLifetime, size , color, velocity);
		}else{

			int i;
			float stepDist = dir.magnitude / count;
			dir = dir.normalized * stepDist;

			float age = 0;
			float stepTime = elapsed / count;
		
			for(i=0;i<count;i++){
				
				startLifetime = Random.Range(5f,15f);
				color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0.5f,1f));
				velocity = Random.onUnitSphere * 0.1f;
				
				SpawnParticle(endPosition, startLifetime, size , color, velocity,age);
				
				age += stepTime;
				endPosition -= dir;
			}
		}
		
		lastSpawnTime = Time.time;
		
	}
	
	
	private void KillParticle(LinkedListNode<ParticleSystem.Particle> p){
		if(p == null) return;

		if(p.Value.startLifetime > 0){
			liveParticleCount--;
			particleLive = liveParticleCount;
			
			// la particule morte et la dernière vivante échangent leurs places
			tempSwap = lastLiveNode.Value;
			lastLiveNode.Value = p.Value;
			p.Value = tempSwap;
			
			tempSwap = lastLiveNode.Value;
			tempSwap.startLifetime = 0f;
			lastLiveNode.Value = tempSwap;
			
			lastLiveNode = lastLiveNode.Previous;
		}
	}
	
	void Update () {
		
		CodeProfiler.Begin("create particle");
		// création des particules à la souris
		if(Input.GetMouseButton(0)){
			Ray r = Camera.main.ScreenPointToRay (Input.mousePosition);
			Vector3 pos = r.origin + r.direction.normalized * Vector3.Distance(Vector3.zero,Camera.main.transform.position);
			
			int n = Random.Range (10,30);
			int i;
			for(i=1;i<n;i++){
				SpawnRow(pos,0.1f,Random.Range(5f,15f),0.1f,new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0.5f,1f)),Random.onUnitSphere * 0.1f);
			}

		}
		CodeProfiler.End("create particle");
		
		CodeProfiler.Begin("update particle");
		double now = Time.realtimeSinceStartup;
		UpdateParticles();
		debugTime = Time.realtimeSinceStartup - now;
		CodeProfiler.End("update particle");
		
		
	}
	
	// mise à jour des particules
	void UpdateParticles(){
		float t = Time.deltaTime;
		
		int i = 0;
		if(liveParticleCount > 0){
			for (LinkedListNode<ParticleSystem.Particle> current = particles.First; current != null; current = current.Next){

				tempSwap = current.Value;
				tempSwap.remainingLifetime += t;
				if(tempSwap.remainingLifetime < tempSwap.startLifetime){
					tempSwap.position += tempSwap.velocity * t;
					current.Value = tempSwap;
				}else{
					KillParticle(current);
					// le pointeur de la dernière particule vivante a été échangé avec celui de celle qui vient de mourrir
					// il faut donc rester sur ce meme index pour l'itération suivante afin que cette particule soit calculée
					current = current.Previous;
					i--;
				} 
				i++;
				if(i >= liveParticleCount || current == null) break;
			}
		}
		/*
		// s'il y a peu de particules utilisées, réduction du tableau général
		if(autoResize && liveParticleCount < _particleCount * 0.5f && _particleCount > chunkSize && !Input.GetMouseButton(0)){
			Resize ((int)(_particleCount * 0.5f + chunkSize));
		}
		*/
	}
	
	
	void OnGUI() {
		
		bool dbg = Input.GetKey(KeyCode.Space);
		if(dbg){
			
			int i = 0;
			Vector2 pos;
			for (LinkedListNode<ParticleSystem.Particle> current = particles.First; current != null; current = current.Next){
				
				if(current.Value.startLifetime > 0){
					GUI.color = current.Value.color;
					pos = Camera.main.WorldToScreenPoint(current.Value.position);
					GUI.Label(new Rect(pos.x,Screen.height-pos.y,30,20),i.ToString());
				}
				
				i++;
				if(i >= liveParticleCount || current == null) break;
			}
			
		}
		
		
	}
	
}
