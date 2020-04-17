using UnityEngine;
using System.Collections;

public class RandomStarfield : StarfieldBase
{
    public float myMaxDistance = 2000;
    public float myMinDistance = 100;
    public int myStarCount = 2000;
    public int myRandomSeed = 0;
    public float myStarSize = 100;

    [Tooltip("Generate stars in the world around (0,0,0); otherwise generate around the transform position")]
    public bool myCenterAroundZero = true;

    public Material myStarMaterial;

    public float myColorAlphaMult = 1;
    public Color[] myColors;

    void OnValidate()
    {
        Refresh();
    }

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        myStarfieldPS.Clear();

        SetMaterialAndProperties(myStarMaterial, myStarSize, myMinDistance, myMaxDistance);

        ParticleSystem.MainModule mainModule = myStarfieldPS.main;
        mainModule.maxParticles = myStarCount;

        Random.State previousState = Random.state;
        Random.InitState(myRandomSeed);

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = Vector3.zero;
        emitParams.startLifetime = Mathf.Infinity;

        Vector3 starfieldCenter = myCenterAroundZero ? Vector3.zero : transform.position;
        
        for (int i = 0; i < myStarCount; ++i)
        {
            Color color = Color.white;
            if (myColors.Length > 0)
                color = myColors[Random.Range(0, myColors.Length)];

            color.a *= myColorAlphaMult;

            Vector3 randomInsideUnit = Random.insideUnitSphere;
            Vector3 pos = starfieldCenter + randomInsideUnit.normalized * myMinDistance + randomInsideUnit * (myMaxDistance - myMinDistance);

            emitParams.startColor = color;
            emitParams.position = pos;
            emitParams.startSize = myStarSize;

            myStarfieldPS.Emit(emitParams, 1);
        }

        Random.state = previousState;
    }

    public void Refresh()
    {
        GrabComponents();
        Generate();
    }

    public void Regenerate()
    {
        myRandomSeed = Mathf.FloorToInt(Time.time * 1000);
        Generate();
    }
}
