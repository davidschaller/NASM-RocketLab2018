import System.Collections.Generic;

class QuickDecals_Base extends EditorWindow 
{
	//used to make sure can't accidentally place decals, especially if window is not open
	static var window : QuickDecals_GUI;
	
	var scrollPos : Vector2;
	
	var decalMat : Material;
	
	var useRandom : boolean = false;
	var randomRtn : boolean = false;
	var matNum : int = 0;
	var matArray : List.<Material> = new List.<Material>();
	
	var initialPos : Vector2 = new Vector2(0f, 0f);	// this is for checking mouse delta
	function OnSceneGUI(scnView : SceneView)
	{
		if(window != null)
		{
			// cache event	    
			var e : Event = Event.current;

			// if shift-right clicking
			if(e.type == EventType.MouseUp)
			{
				if(e.modifiers == EventModifiers.Control && e.button == 1)
				{
					AttemptDecalPlacement();			
				}
			}
		}
	}
	
	function AttemptDecalPlacement()
	{
		var worldRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		var hitInfo : RaycastHit;
		if(Physics.Raycast(worldRay, hitInfo, 1000))
		{
			if(hitInfo.collider.gameObject.GetComponent(typeof(MeshRenderer)))
			{
				PlaceNewDecal(hitInfo);
			}
		}
	}
	
	function PlaceNewDecal(hitInfo : RaycastHit)
	{
		var chosenMaterial : Material;
		if(useRandom)
		{
			if(matNum == 0)
			{
				Debug.LogWarning("You need to add some decal materials first!");
				return;
			}
			var matIndex : int = Random.Range(0,matNum);
			chosenMaterial = matArray[matIndex];
			if(chosenMaterial == null)
			{
				Debug.LogWarning("Decal #"+(matIndex+1)+" has no material chosen, please add a material there.");
				return;
			}
		}
		else
		{
			chosenMaterial = decalMat;
			if(chosenMaterial == null)
			{
				Debug.LogWarning("No material chosen for the Decal- please choose one first!");
				return;
			}
		}
		
		var decalPos : Vector3 = hitInfo.point;
		var decalRtn : Quaternion = Quaternion.LookRotation(hitInfo.normal);
		
		var newDecal : GameObject;
		newDecal = Instantiate((Resources.LoadAssetAtPath("Assets/6by7/QuickDecals/DecalMesh/DecalMeshObject.fbx", typeof(Object))), Vector3.zero, Quaternion.identity);

		newDecal.GetComponent(typeof(MeshRenderer)).sharedMaterial = chosenMaterial;
		newDecal.transform.position = decalPos;
		newDecal.transform.rotation = decalRtn;
		
		newDecal.transform.Rotate(90,0,0);
		if(randomRtn)
		{
			newDecal.transform.Rotate(0,(Random.Range(0,360)),0);
		}
		newDecal.transform.Translate(0,0.005,0);
		
		newDecal.GetComponent(typeof((MeshRenderer))).castShadows = false;
		
		newDecal.isStatic = false;
		var staticFlags = StaticEditorFlags.BatchingStatic | StaticEditorFlags.LightmapStatic | StaticEditorFlags.OccludeeStatic;
		GameObjectUtility.SetStaticEditorFlags(newDecal, staticFlags);
		
		Selection.activeObject = newDecal;
	}
}