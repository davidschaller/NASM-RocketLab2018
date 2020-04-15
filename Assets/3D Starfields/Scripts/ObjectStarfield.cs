using UnityEngine;
using System.Collections;

public class ObjectStarfield : StarfieldBase
{
    public GameObject myRootObject;
    public float myStarSize = 100;
    public Material myStarMaterial;
    
    public Color[] myColors;

    void OnValidate()
    {
        GrabComponents();

        AddParticles();
    }

	new void Awake()
	{
        base.Awake();

		AddParticles ();
	}

    void AddParticles()
    {
        if (myRootObject == null)
            return;

        float farthestObjectSqDistance = 0.0f;
        int childCount = myRootObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform tr = myRootObject.transform.GetChild(i);
            farthestObjectSqDistance = Mathf.Max(farthestObjectSqDistance, (myRootObject.transform.position - tr.position).sqrMagnitude);
        }

        SetMaterialAndProperties(myStarMaterial, myStarSize, 0, farthestObjectSqDistance);

        Random.State previousState = Random.state;
        Random.InitState(0);

        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.velocity = Vector3.zero;
        emitParams.startLifetime = Mathf.Infinity;

        myStarfieldPS.Clear();        
        for(int i = 0; i < childCount; i++)
        {
            Transform tr = myRootObject.transform.GetChild(i);

            Color color = Color.white;
            if (myColors.Length > 0)
                color = myColors[Random.Range(0, myColors.Length)];

            emitParams.startColor = color;
            emitParams.position = tr.position;
            emitParams.startSize = myStarSize;

            myStarfieldPS.Emit(emitParams, 1);
        }

        Random.state = previousState;
    }
}
