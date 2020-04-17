using UnityEngine;
using System.Collections;

public abstract class StarfieldBase : MonoBehaviour
{
    public bool myUseScintillation = false; // Disabling scintillation rather than setting it to zero on the material removes the extra math operations it uses.

    protected ParticleSystem myStarfieldPS;
    protected ParticleSystemRenderer myStarfieldRenderer;

    protected void Awake()
    {
        GrabComponents();
    }

    protected void GrabComponents()
    {
        myStarfieldPS = GetComponent<ParticleSystem>();
        myStarfieldRenderer = GetComponent<ParticleSystemRenderer>();
    }

    protected void SetMaterialAndProperties(Material aMaterial, float aStarSize, float aMinDistance, float aMaxDistance)
    {
        if (aMaterial == null) // Can happen after script reload
            return;

        myStarfieldRenderer.sharedMaterial = aMaterial;
        myStarfieldRenderer.sharedMaterial.SetFloat("_ParticleSize", aStarSize);
        myStarfieldRenderer.sharedMaterial.SetFloat("_ClosestParticleDistance", aMinDistance);
        myStarfieldRenderer.sharedMaterial.SetFloat("_FarthestParticleDistance", aMaxDistance);

        RefreshScintillationKeyword();
    }

    public void SetScintillation(bool onOff)
    {
        myUseScintillation = onOff;
        RefreshScintillationKeyword();
    }

    private void RefreshScintillationKeyword()
    {
        if (myUseScintillation)
            myStarfieldRenderer.sharedMaterial.EnableKeyword("ENABLE_SCINTILLATION");
        else
            myStarfieldRenderer.sharedMaterial.DisableKeyword("ENABLE_SCINTILLATION");
    }
    
#if UNITY_EDITOR
    protected void Reset()
    {
        if (myStarfieldPS != null)
            MonoBehaviour.DestroyImmediate(myStarfieldPS);

        myStarfieldPS = gameObject.AddComponent<ParticleSystem>();
        myStarfieldRenderer = GetComponent<ParticleSystemRenderer>();

        // Setup the particle system component the first time it's added
        SetParticleSystemPropertiesInEditor();

        if (Application.isEditor && !Application.isPlaying)
            myStarfieldPS.hideFlags = HideFlags.NotEditable; // Restore with HideFlags.None;
    }

    private void SetParticleSystemPropertiesInEditor()
    {
        UnityEditor.SerializedObject serializedObject = new UnityEditor.SerializedObject(myStarfieldPS);
        serializedObject.FindProperty("lengthInSec").floatValue = Mathf.Infinity;
        serializedObject.ApplyModifiedProperties();

        ParticleSystem.MainModule mainModule = myStarfieldPS.main;
        mainModule.loop = false;
        mainModule.playOnAwake = false;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        ParticleSystem.EmissionModule em = myStarfieldPS.emission;
        em.enabled = false;
    }
#endif
}
