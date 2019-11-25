using UnityEngine;
using System.Collections;
using System.Collections.Generic;	

public class Puffy_Emitter : MonoBehaviour {
	
	public static bool globalFreeze = false;
	
	public Puffy_Renderer PuffyRenderer = null;
	
	public bool autoEmit = true;
	
	public bool freezed = false;
	
	public float lifeTime = 5f;
	
	public float lifeTimeVariation = 0f;
	
	public Vector3 startDirection = Vector3.up;
	
	public Vector3 startDirectionVariation = Vector3.zero;
	
	public float startSize = 0.5f;
	
	public float endSize = 2f;
	
	public float startSizeVariation = 0f;
	
	public float endSizeVariation = 0f;
	
	public float startSpeed = 1f;
	
	public float startSpeedVariation = 0f;
	
	public Color startColor = Color.white;
	
	public Color endColor = Color.white;
	
	public Color startColorVariation = Color.black;
	
	public Color endColorVariation = Color.black;
	
	public bool autoResize = true;
	
	public int chunkSize = 512;
	
	public bool useThread = true;
	
	public bool trailMode = true;
	public float trailStepDistance = 0.1f;
	
	public bool autoTrailStep = true;
	public float autoTrailStepFactor = 0.3f;
	
	[System.NonSerialized]
	public Puffy_ParticleData[] particles;
		
	[System.NonSerialized]
	public double debugTime = 0;
	
	[System.NonSerialized]
	public int particleTotal = 0;
	
	[System.NonSerialized]
	public int particleLive = 0;

	[System.NonSerialized]
	public int[] particlesPointers;
	
	private int liveParticleCount = 0;
	private int _particleCount = 512;

	private Vector3 lastSpawnPosition = Vector3.zero;
	private float _deltaTime;
	private int _coresCount = 1;
	private List<PointerGroup> deadGroups = new List<PointerGroup>();
	private Transform _transform;
	
	void Start(){
		_transform = transform;
		lastSpawnPosition = _transform.position;
		
		_coresCount = SystemInfo.processorCount;
		
		_particleCount = chunkSize;
		
		particleTotal = _particleCount;
		particles = new Puffy_ParticleData[_particleCount];
		particlesPointers = new int[_particleCount];
		
		Clear();
		autoResize = true;
	}
	
	public void Clear(bool reset = false){
		int i;
		if(reset) Resize (chunkSize*2);
		for(i=0; i<_particleCount; i++){
			particlesPointers[i] = i;
			if(particles[i] == null) particles[i] = new Puffy_ParticleData();
			particles[i].startLifetime = 0f;
			particles[i].lifetime = 0f;
		}
		
		for(int g = 0 ; g < deadGroups.Count ; g++){
			deadGroups[g].index = 0;
		}
		
		liveParticleCount = 0;
		particleLive = 0;
	}
	
	bool waitingForLastParticle = false;
	public void Kill(){
		waitingForLastParticle = true;
	}
	
	public void Resurrect(){
		waitingForLastParticle = false;
		enabled = true;
	}
	
	
	void FixedUpdate () {
		
		if(!freezed && !globalFreeze){
			if(!waitingForLastParticle){
				if(autoEmit){
					if(trailMode){
						SpawnRow(lastSpawnPosition , _transform.position, trailStepDistance , _transform.TransformDirection(startDirection), startSpeed, lifeTime, startSize, endSize, startColor, endColor);
					}else{
						SpawnParticle(transform.position, _transform.TransformDirection(startDirection), startSpeed, lifeTime, startSize, endSize, startColor, endColor);
					}
				}
			}else if(waitingForLastParticle && liveParticleCount == 0){
				// kill only when no particle is left
				Clear();
				enabled = false;

				return;
			}
			debugTime = Time.realtimeSinceStartup;
			UpdateParticles();
			debugTime = Time.realtimeSinceStartup - debugTime;
		}
	}

	
	// mise à jour des particules
	void UpdateParticles(){
		
		if(liveParticleCount > 0){
			
			while(deadGroups.Count<_coresCount){
				deadGroups.Add(new PointerGroup());					
			}
			
			_deltaTime = Time.fixedDeltaTime;
			
			int count;
			int i;
			int j;
			if(liveParticleCount < _coresCount*10 || _coresCount == 1 || !useThread){
				UpdateParticlesTask(0,0,liveParticleCount);
			}else{
				
				int stp = Mathf.CeilToInt((float)liveParticleCount / _coresCount);
				List<UnityThreading.Task> taskList = new List<UnityThreading.Task>();
				
				j = 0;
				for(i=0;i<liveParticleCount;i+=stp){
					var end = Mathf.Min (i+stp,liveParticleCount);
					var start = i;
					var grp = j;
					taskList.Add(UnityThreadHelper.TaskDistributor.Dispatch(() => UpdateParticlesTask(grp,start,end)));
					j++;
				}
				count = taskList.Count;
				for(i=0;i<count;i++){
					taskList[i].Wait();
					taskList[i].Dispose();
				}
		
				taskList.Clear();
			}
			
			
			int groupCount = deadGroups.Count;
			int g;
			count = 0;
			for(g = 0 ; g < groupCount ; g++){
				count += deadGroups[g].pointers.Count;
				deadGroups[g].index = 0;
			}
			
			
			if(count > 0){
				int index;
				int pointer;
								
				if(count == liveParticleCount){
					//liveParticleCount = 0;
					particleLive = liveParticleCount;

				}else{
					
					int groupIndex;
					int minValue;
					for(i = 0 ; i < count ; i++){
						
						
						groupIndex = -1;
						minValue = int.MaxValue;
						
						// boucle sur les groupes de tri -- TODO trouver une optimisation pour réduire le nombre d'itérations inutiles !
						for(g = 0; g < groupCount; g++){
							// détection du groupe ayant l'index de particule le plus petit
							if(deadGroups[g].index < deadGroups[g].count){
								if(deadGroups[g].pointers[ deadGroups[g].index ] < minValue){
									groupIndex = g;
									minValue = deadGroups[g].pointers[ deadGroups[g].index ];
								}
							}
						}
						
						if(groupIndex > -1){
							pointer = deadGroups[groupIndex].pointers[deadGroups[groupIndex].index];
							index = particlesPointers[pointer];
							
							deadGroups[groupIndex].index++;
							
							liveParticleCount--;
						
							if(liveParticleCount >= 0){
								while(liveParticleCount != pointer && particles[particlesPointers[liveParticleCount]].startLifetime == 0f){
									liveParticleCount--;
									count--;
								}
							
								particlesPointers[pointer] = particlesPointers[liveParticleCount];
								particlesPointers[liveParticleCount] = index;
							}
							if(liveParticleCount < 0) liveParticleCount=0;
						}
						
						particleLive = liveParticleCount;
					}
				}
								
				for(g=0;g<groupCount;g++){
					deadGroups[g].index = 0;
					deadGroups[g].count = 0;
				}
				
			}
		}
	}
	
	void UpdateParticlesTask(int groupIndex, int start, int end){
		
		if(end > start && end > 0){
			int index;
			int pointer;
			int total = deadGroups[groupIndex].pointers.Count;
			deadGroups[groupIndex].index = 0;
						
			for(pointer=start; pointer<end; pointer++){
				
				index = particlesPointers[pointer];
				
				// call particle update
				particles[index].Update(_deltaTime);
				
				if(particles[index].lifetime > particles[index].startLifetime){

					if(deadGroups[groupIndex].index >= total){
						deadGroups[groupIndex].pointers.Add(pointer);
						total++;
					}else{
						deadGroups[groupIndex].pointers[deadGroups[groupIndex].index] = pointer;
					}
					particles[index].startLifetime = 0f;
					particles[index].lifetime = 0f;
					deadGroups[groupIndex].index++;
				}
			}
			
			deadGroups[groupIndex].count = deadGroups[groupIndex].index;
			deadGroups[groupIndex].index = 0;
			
			//deadGroups[groupIndex].pointers.Sort(0,deadGroups[groupIndex].count,new PointerComparer());

		}
	}
	
	private class PointerComparer : IComparer<int>
	{
		public int Compare(int a, int b)
		{
			return (a > b)?1:-1;
		}
	}

	
	// create one particle
	public int SpawnParticle(Vector3 start_position, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color){	
		
		if(autoResize && liveParticleCount >= _particleCount){
			if(!Resize (_particleCount + chunkSize)) return -1;
		}
				
		if(liveParticleCount < _particleCount){
			int index = particlesPointers[liveParticleCount];

			particlesPointers[liveParticleCount] = index;
			
			liveParticleCount++;
			
			particleLive = liveParticleCount;
			
			if(particles[index] == null) particles[index] = new Puffy_ParticleData();
			
			if(lifeTimeVariation != 0) start_lifetime += Random.Range(-lifeTimeVariation , lifeTimeVariation);
			if(startSizeVariation != 0) start_size += Random.Range(-startSizeVariation , startSizeVariation);
			if(endSizeVariation != 0) end_size += Random.Range(-endSizeVariation , endSizeVariation);
			if(startColorVariation != Color.black){
				start_color.r += Random.Range(-startColorVariation.r,startColorVariation.r);
				start_color.g += Random.Range(-startColorVariation.g,startColorVariation.g);
				start_color.b += Random.Range(-startColorVariation.b,startColorVariation.b);
			}
			if(endColorVariation != Color.black){
				end_color.r += Random.Range(-endColorVariation.r,endColorVariation.r);
				end_color.g += Random.Range(-endColorVariation.g,endColorVariation.g);
				end_color.b += Random.Range(-endColorVariation.b,endColorVariation.b);
			}
			
			if(startSpeedVariation != 0) start_speed += Random.Range(-startSpeedVariation , startSpeedVariation);
			
			if(startDirectionVariation != Vector3.zero){
				start_direction.x += Random.Range(-startDirectionVariation.x,startDirectionVariation.x);
				start_direction.y += Random.Range(-startDirectionVariation.y,startDirectionVariation.y);
				start_direction.z += Random.Range(-startDirectionVariation.z,startDirectionVariation.z);
			}
			
			particles[index].Spawn(start_position,start_direction,start_speed,start_lifetime,start_size,end_size,start_color,end_color);
			
			return index;
		}
		
		return -1;
	}
	
	public void SpawnRow(Vector3 row_start_position, Vector3 row_end_position){
		SpawnRow(row_start_position, row_end_position, trailStepDistance , transform.TransformDirection(startDirection),startSpeed,lifeTime,startSize,endSize,startColor,endColor);
	}
	
	// create a row of particles
	/*
	public void SpawnRow(Vector3 row_end_position, float stepDistance, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color){
		SpawnRow(lastSpawnPosition, row_end_position, stepDistance, start_direction, start_speed, start_lifetime ,  start_size, end_size, start_color,end_color);	
	}
	*/
	public void SpawnRow(Vector3 row_start_position, Vector3 row_end_position, float stepDistance, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color){

		Vector3 dir = row_end_position - row_start_position;
		float distance = dir.magnitude;
		int count;
			
		if(autoTrailStep){
			count = Mathf.FloorToInt(distance / (start_size*autoTrailStepFactor));
		}else{
			count = Mathf.FloorToInt(distance / stepDistance);
		}
		
		lastSpawnPosition = row_end_position;
		
		if(count < 2){
			SpawnParticle(row_end_position,start_direction, start_speed, start_lifetime, start_size , end_size, start_color,end_color);
		}else{
			int i;
			float stepDist = distance / count;
			dir = dir.normalized * stepDist;
			int index;
			float age = 0;
			float stepTime = _deltaTime / count;
			
			for(i=0;i<count;i++){
				
				index = SpawnParticle(row_end_position,start_direction, start_speed, start_lifetime, start_size , end_size, start_color,end_color);
				if(index > -1){
					particles[index].lifetime = age;
					particles[index].Update(0);
				}
				age += stepTime;
				row_end_position -= dir;
			}
		}
	
	}

	
	
	// resize particles array
	public bool Resize(int newParticleCount){		
		
		if(_particleCount != newParticleCount){
			double tmp = Time.realtimeSinceStartup;
			_particleCount = newParticleCount;
			Puffy_ParticleData[] tmp_particles = new Puffy_ParticleData[_particleCount];
			int[] tmp_particlesPointers = new int[_particleCount];
	
			int i=0;
			int j=0;
		
			for(i=0;i<liveParticleCount;i++){
				j = particlesPointers[i];
				tmp_particles[i] = particles[j];				
				tmp_particlesPointers[i] = i;
			}
			
			for(i=liveParticleCount;i<_particleCount;i++){
				tmp_particlesPointers[i] = i;
			}
			
			particles = tmp_particles;
			particlesPointers = tmp_particlesPointers;
			
			particleTotal = _particleCount;
			particleLive = liveParticleCount;
			
			tmp = Time.realtimeSinceStartup - tmp;

			return true;
		}
		return false;
	}
		
	class PointerGroup{
		public List<int> pointers = new List<int>();
		public int index = 0;
		public int count = 0;
	}
	
	
}

public class Puffy_ParticleData{
	public Vector3 position = Vector3.zero;
	public Vector3 direction = Vector3.up;
	
	public uint randomSeed = 0;
	
	public float startLifetime = 0f;
	public float lifetime = 0f;
	
	public Color startColor = Color.white;
	public Color color = Color.white;
	public Color endColor = Color.white;
	
	public float startSize = 0f;
	public float size = 0f;
	public float endSize = 1;
		
	public float ageRatio = 0f;
	
	public float speed = 0f;
	
	public void Spawn(Vector3 start_position, Vector3 start_direction, float start_speed, float start_lifetime, float start_size, float end_size, Color start_color, Color end_color){
		
		speed = start_speed;
		
		position = start_position;
		direction = start_direction;
		
		startSize = start_size;
		endSize = end_size;
		
		startColor = start_color;
		endColor = end_color;
		
		lifetime = 0f;
		startLifetime = start_lifetime;
		
		randomSeed = (uint)Random.Range(0,100);
	}
	
	public void Update(float deltaTime){
		
		lifetime += deltaTime;
		ageRatio = lifetime / startLifetime;
		
		position += direction * speed * deltaTime;
		color = Color.Lerp(startColor,endColor,ageRatio);
		color.a = (1f - ageRatio) * 0.5f;
		size = Mathf.Lerp(startSize,endSize,ageRatio);
	}
}