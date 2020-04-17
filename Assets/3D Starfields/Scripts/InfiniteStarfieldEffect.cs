using UnityEngine;
using System.Collections;

public class InfiniteStarfieldEffect : StarfieldBase
{
    public float myMaxDistance = 5000;
    public int myStarCount = 2000;
    public float myMinCorridorRadius = 0;
    public float myMaxCorridorRadius = 2000;
    public float myStarSize = 200;
    public float mySpeed = 1;
    public int myNewParticlesPerFrame = 10;

    public Material myStarMaterial;

    public Color[] myColors;
    
    private Camera myCamera;
    private Transform myCameraTr;

    private ParticleSystem.EmitParams myEmitParams;
    
    new void Awake()
    {
        base.Awake();
        
        myCamera = Camera.main;
        myCameraTr = myCamera.transform;

        myEmitParams = new ParticleSystem.EmitParams();
    }

    void Start()
    {
        SetMaterialAndProperties(myStarMaterial, myStarSize, 0, myMaxDistance);
    }

    void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(myCamera);
        
        int particlesThisFrame = 0;
        while(myStarfieldPS.particleCount < myStarCount && particlesThisFrame < myNewParticlesPerFrame)
        {
            particlesThisFrame++;

            Color color = Color.white;
            if (myColors.Length > 0)
                color = myColors[Random.Range(0, myColors.Length)];

            Vector2 randomInCircle = Random.insideUnitCircle;
            Vector3 pos = myCameraTr.position + myCameraTr.forward * myMaxDistance + myCameraTr.rotation * (randomInCircle.normalized * myMinCorridorRadius + randomInCircle * (myMaxCorridorRadius - myMinCorridorRadius));
            Vector3 velocity = -myCameraTr.forward * mySpeed;

            float distance = myMaxDistance;
            for (int i = 0; i < planes.Length; ++i)
            {
                float distanceToPlane = myMaxDistance;
                planes[i].Raycast(new Ray(pos, velocity), out distanceToPlane);

                distance = Mathf.Min(Mathf.Abs(distanceToPlane), distance);
            }

            float particleDuration = distance / mySpeed + 0.5f;

            myEmitParams.startColor = color;
            myEmitParams.position = pos;
            myEmitParams.startSize = myStarSize;
            myEmitParams.velocity = velocity;
            myEmitParams.startLifetime = particleDuration;

            myStarfieldPS.Emit(myEmitParams, 1);
        }
    }
}
