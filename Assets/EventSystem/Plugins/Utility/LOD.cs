using UnityEngine;
using System.Collections;

public class LOD
{
	GameObject gameObject;
	Animation[] anims;
	
	static int visibleLODs = 0;
	static int hiddenLODs = 0;
	static int inactiveLODs = 0;
	static int totalLODs = 0;
	
	LODState currentLOD = LODState.Visible;
	public LODState CurrentLOD
	{
		get
		{
			return currentLOD;
		}
		private set {}
	}
	
	public LOD(GameObject go)
	{
		gameObject = go;
		Component[] animations = gameObject.GetComponentsInChildren(typeof(Animation));
		anims = new Animation[animations.Length];
		for(int i=0;i<animations.Length;i++)
		{
			anims[i] = (Animation)animations[i];
		}
		
		visibleLODs++;
		totalLODs++;
	}
	
	void LODHide ()
	{
		NPCController npc = gameObject.GetComponent<NPCController>();
        if (npc)
        {
            //npc.OnLODHide();
        }
		
		gameObject.active = behindCamera ? false : true;
		
		foreach(Animation anim in anims)
			anim.enabled = false;
			
		//debugLOD = true;
		Component[] mrs = (Component[])gameObject.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		foreach (Component mr in mrs)
		{
			((SkinnedMeshRenderer)mr).enabled = false;
		}

		mrs = (Component[])gameObject.GetComponentsInChildren(typeof(MeshRenderer));
		foreach (Component mr in mrs)
		{
			((MeshRenderer)mr).enabled = false;
		}
	}
	
	void LODShow ()
	{
		gameObject.active = true;
        
        foreach (Animation anim in anims)
        {
            if (anim && anim.clip)
            {
                anim.enabled = true;
            }
        }
         
		
		//debugLOD = false;
		Component[] mrs = (Component[])gameObject.GetComponentsInChildren(typeof(SkinnedMeshRenderer));
		foreach (Component mr in mrs)
		{
			((SkinnedMeshRenderer)mr).enabled = true;
		}

		mrs = (Component[])gameObject.GetComponentsInChildren(typeof(MeshRenderer));
		foreach (Component mr in mrs)
		{
			((MeshRenderer)mr).enabled = true;
		}
	}

	bool forceDeactivate = false;
	public void ForceLODDeactivate ()
	{
		forceDeactivate = true;
		LODDeactivate();
	}
	
	void LODDeactivate ()
	{
		LODHide();
		gameObject.active = false;
	}
	
	bool behindCamera = false;
	public void UpdateLOD(LODState st, bool behindCamera)
	{
		if (forceDeactivate)
			return;
		
		this.behindCamera = behindCamera;
		if (currentLOD != st)
		{
            switch (st)
            {
                case LODState.Visible:
                    visibleLODs++;
                    LODShow();
                    break;
                case LODState.Hidden:
                    hiddenLODs++;
                    LODHide();
                    break;
                case LODState.Deactivate:
                    inactiveLODs++;
                    LODDeactivate();
                    break;
            }

            switch (currentLOD)
            {
                case LODState.Visible:
                    visibleLODs--;
                    break;
                case LODState.Hidden:
                    hiddenLODs--;
                    break;
                case LODState.Deactivate:
                    inactiveLODs--;
                    break;
            }
			
			currentLOD = st;
			
			MTDebug.WatchVariable("Visible LODs:     {0}", visibleLODs);
			MTDebug.WatchVariable("Hidden LODs:      {0}", hiddenLODs);
			MTDebug.WatchVariable("Deactivated LODs: {0}", inactiveLODs);
			MTDebug.WatchVariable("Total LODs:       {0}", totalLODs);
		}
	}
}
