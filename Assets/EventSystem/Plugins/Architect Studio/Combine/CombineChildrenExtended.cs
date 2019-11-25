using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/*
Attach this script as a parent to some game objects. The script will then combine the meshes at startup.
This is useful as a performance optimization since it is faster to render one big mesh than many small meshes. See the docs on graphics performance optimization for more info.

Different materials will cause multiple meshes to be created, thus it is useful to share as many textures/material as you can.
*/
//[ExecuteInEditMode()]
[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildrenExtended : MonoBehaviour 
{
	
	/// Usually rendering with triangle strips is faster.
	/// However when combining objects with very low triangle counts, it can be faster to use triangles.
	/// Best is to try out which value is faster in practice.
    
    //public int frameToWait = 0;
    public bool generateTriangleStrips = false,
        //combineOnStart = false, 
        //destroyAfterOptimized = false, 
                castShadow = true,
                receiveShadow = true,
                keepLayer = true,
                addMeshCollider = true;//,
                //disableAfterOptimized = true;

    public List<GameObject> disabledObjects,
                             combinedMeshes;
	
    private void Start()
    {
        /*
        if (combineOnStart && frameToWait == 0)
        {
            Combine();
        }
        else
        {
            StartCoroutine(CombineLate());
        }
         */
        disabledObjects = new List<GameObject>();
        combinedMeshes = new List<GameObject>();
    }

    /*
    private IEnumerator CombineLate()
    {
        for (int i = 0; i < frameToWait; i++ ) yield return 0;
        Combine();
    }
     */

    /*
    [ContextMenu("Combine Now on Childs")]
    public void CallCombineOnAllChilds()
    {
        CombineChildrenExtended[] c = gameObject.GetComponentsInChildren<CombineChildrenExtended>();
        int count = c.Length;
        for (int i = 0; i < count; i++)
        {
            if (c[i] != this) 
                c[i].Combine();
        }
        combineOnStart = enabled = false;
    }
     */

	/// This option has a far longer preprocessing time at startup but leads to better runtime performance.
    [ContextMenu ("Combine Now")]
	public void Combine() 
    {
		Component[] filters  = GetComponentsInChildren(typeof(MeshFilter));
		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Hashtable materialToMesh = new Hashtable();
		
		for (int i = 0; i < filters.Length; i++)
        {
            // Don't combine invisible objects and doors
            if (filters[i].gameObject.layer == LayerMask.NameToLayer("Invisible"))
                continue;

            Transform model = filters[i].transform.parent;
            if (model != null)
            {
                if (model.name.Equals("Roof (3D View)"))
                    continue;

                if (model.parent && model.parent.name.Equals("Roofs"))
                    continue;

                InteriorItemDefinition doorDefinition = model.GetComponent<InteriorItemDefinition>();

                if (doorDefinition == null)
                {
                    Transform door = model.parent;
                    if (door)
                        doorDefinition = door.GetComponent<InteriorItemDefinition>();
                }

                if (doorDefinition != null && (doorDefinition.tab == InteriorDesignGUI.Tabs.Openings &&
                    (doorDefinition.subtab == InteriorDesignGUI.SubTabs.Doors)))
                {
                    doorDefinition.ActivateRenderers();
                    continue;
                }


                if (doorDefinition == null)
                {
                    // We have some problems with tree's rendering after combine
                    ItemDefinition tree = model.GetComponent<ItemDefinition>();
                    if (tree == null && model.parent)
                        tree = model.parent.GetComponent<ItemDefinition>();

                    if (tree != null)
                    {
                        tree.ActivateRenderers();
                        continue;
                    }
                }
            }

            MeshFilter filter = filters[i] as MeshFilter;
			Renderer curRenderer  = filters[i].GetComponent<Renderer>();
			MeshCombineUtility.MeshInstance instance = new MeshCombineUtility.MeshInstance();
			instance.mesh = filter.sharedMesh;
			if (curRenderer != null /*&& curRenderer.enabled*/ && instance.mesh != null) 
            {
				instance.transform = myTransform * filter.transform.localToWorldMatrix;
				
				Material[] materials = curRenderer.sharedMaterials;

				for (int m = 0; m < materials.Length; m++)
                {
					instance.subMeshIndex = System.Math.Min(m, instance.mesh.subMeshCount - 1);
	
					ArrayList objects = (ArrayList)materialToMesh[materials[m]];
					if (objects != null) 
                    {
						objects.Add(instance);
					}
					else
					{
						objects = new ArrayList();
						objects.Add(instance);
						materialToMesh.Add(materials[m], objects);
					}
				}

                /*
                if (Application.isPlaying && destroyAfterOptimized && combineOnStart)
                {
                    Destroy(curRenderer.gameObject);
                }
                else if (destroyAfterOptimized)
                {
                    DestroyImmediate(curRenderer.gameObject);
                }
                else
                    curRenderer.enabled = false;
                 */
				
				/*
                if (disableAfterOptimized)
                {
                    disabledObjects.Add(curRenderer.gameObject);
                    curRenderer.gameObject.active = false;
                }
                */
			}
		}
	
		foreach (DictionaryEntry de  in materialToMesh) 
        {
			ArrayList elements = (ArrayList)de.Value;
			MeshCombineUtility.MeshInstance[] instances = (MeshCombineUtility.MeshInstance[])elements.ToArray(typeof(MeshCombineUtility.MeshInstance));

			// We have a maximum of one material, so just attach the mesh to our own game object
			if (materialToMesh.Count == 1)
			{
				// Make sure we have a mesh filter & renderer
                if (GetComponent(typeof(MeshFilter)) == null)
                {
                    gameObject.AddComponent(typeof(MeshFilter));
                }
                if (!GetComponent("MeshRenderer"))
                {
                    gameObject.AddComponent<MeshRenderer>();
                }
	
				MeshFilter filter = (MeshFilter)GetComponent(typeof(MeshFilter));

                if (Application.isPlaying)
                {
                    filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                }
                else
                    filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);

				GetComponent<Renderer>().material = (Material)de.Key;
				GetComponent<Renderer>().enabled = true;
                if (addMeshCollider)
                {
                    gameObject.AddComponent<MeshCollider>();
                }

                GetComponent<Renderer>().castShadows = castShadow;
                GetComponent<Renderer>().receiveShadows = receiveShadow;
			}
			// We have multiple materials to take care of, build one mesh / gameobject for each material
			// and parent it to this object
			else
			{
				GameObject go = new GameObject("Combined mesh");
                
                combinedMeshes.Add(go);

                if (keepLayer)
                {
                    go.layer = gameObject.layer;
                }

				go.transform.parent = transform;
				go.transform.localScale = Vector3.one;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localPosition = Vector3.zero;
				go.AddComponent(typeof(MeshFilter));
				go.AddComponent<MeshRenderer>();
				go.GetComponent<Renderer>().material = (Material)de.Key;
				MeshFilter filter = (MeshFilter)go.GetComponent(typeof(MeshFilter));

                if (Application.isPlaying)
                {
                    filter.mesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                }
                else
                {
                    filter.sharedMesh = MeshCombineUtility.Combine(instances, generateTriangleStrips);
                }

                go.GetComponent<Renderer>().castShadows = castShadow;
                go.GetComponent<Renderer>().receiveShadows = receiveShadow;
                if (addMeshCollider)
                {
                    go.AddComponent<MeshCollider>();
                }
            }
		}	
	}

    [ContextMenu("Decombine Now")]
    public void Decombine()
    {
        foreach (GameObject combined in combinedMeshes)
        {
            DestroyImmediate(combined);
        }

        combinedMeshes.Clear();

        foreach (GameObject disabled in disabledObjects)
        {
            if (disabled != null)
            {
                disabled.active = true;
            }
        }

        foreach (InteriorItemDefinition interiorItem in InteriorDesignGUI.GridItems)
        {
            interiorItem.DeactivateRenderers();
        }

        foreach (ItemDefinition item in LandscapingGUI.GridItems)
            item.DeactivateRenderers();
    }
}