// Camera Path
// Available on the Unity3D Asset Store
// Copyright (c) 2013 Jasper Stocker http://camerapath.jasperstocker.com
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.

using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(CameraPathBezierAnimator))]
public class CameraPathBezierAnimatorEditor : Editor {
	
	private CameraPathBezierAnimator animator;
	private CameraPathBezier bezier;
	
	private RenderTexture pointPreviewTexture = null;
	private float aspect = 1.7777f;
	private Camera sceneCamera = null;
	private Skybox sceneCameraSkybox = null;
	
	void OnEnable()
	{
		animator = (CameraPathBezierAnimator)target;
		bezier = animator.GetComponent<CameraPathBezier>();
	}
	
	public void OnSceneGUI()
	{
		
	}
	
	public override void OnInspectorGUI()
	{
		Camera[] cams = Camera.allCameras;
		if(cams.Length == 0)
			return;
		if(Camera.main){
			sceneCamera = Camera.main;
			//aspect = sceneCamera.aspect;
		}else{
			sceneCamera = cams[0];
			//aspect = sceneCamera.aspect;
		}
		
		if(sceneCamera!=null)
			if(sceneCameraSkybox==null)
				sceneCameraSkybox = sceneCamera.GetComponent<Skybox>();
		
		if(pointPreviewTexture==null)
			pointPreviewTexture = new RenderTexture(400, Mathf.RoundToInt(400/aspect), 24);
		
		if(animator.animationTarget==null){
			EditorGUILayout.HelpBox("No animation target has been specified so there is nothing to animate. Select an animation target in the Camera Path Bezier Animator Component in the parent clip",MessageType.Warning);
		}else{
			if((EditorGUIUtility.isProSkin) && bezier.numberOfCurves > 0 && pointPreviewTexture!=null){
				if(!Application.isPlaying){
					float usePercentage = animator.normalised? animator.RecalculatePercentage(animator.editorTime): animator.editorTime;
					GameObject cam = new GameObject("Point Preview");
					cam.AddComponent<Camera>();
					if(sceneCamera!=null){
						cam.GetComponent<Camera>().backgroundColor = sceneCamera.backgroundColor;
						if(sceneCameraSkybox!=null)
							cam.AddComponent<Skybox>().material = sceneCameraSkybox.material;
						else
							if(RenderSettings.skybox!=null)
								cam.AddComponent<Skybox>().material = RenderSettings.skybox;
					}
					cam.transform.position = bezier.GetPathPosition(usePercentage);
					cam.GetComponent<Camera>().fieldOfView = bezier.GetPathFOV(animator.editorTime);
					
					Vector3 plusPoint, minusPoint;
					switch(bezier.mode)
					{
					case CameraPathBezier.viewmodes.usercontrolled:
						cam.transform.rotation = bezier.GetPathRotation(animator.editorTime);
						break;
						
					case CameraPathBezier.viewmodes.target:
						
						if(bezier.target != null){
							cam.transform.LookAt(bezier.target.transform.position);
						}else{
							EditorGUILayout.HelpBox("No target has been specified in the bezier path",MessageType.Warning);
							cam.transform.rotation = Quaternion.identity;
						}
						break;
						
					case CameraPathBezier.viewmodes.followpath:
						
						minusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						plusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						
						cam.transform.LookAt(cam.transform.position+(plusPoint-minusPoint));
						break;
						
					case CameraPathBezier.viewmodes.reverseFollowpath:
						
						minusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						plusPoint = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						
						cam.transform.LookAt(cam.transform.position+(minusPoint-plusPoint));
						break;
						
					case CameraPathBezier.viewmodes.mouselook:
						
						Vector3 minusPointb = bezier.GetPathPosition(Mathf.Clamp01(usePercentage-0.05f));
						Vector3 plusPointb = bezier.GetPathPosition(Mathf.Clamp01(usePercentage+0.05f));
						
						cam.transform.LookAt(cam.transform.position+(plusPointb-minusPointb));
						break;
					}
					
					cam.GetComponent<Camera>().targetTexture = pointPreviewTexture;
			        cam.GetComponent<Camera>().Render();
			        cam.GetComponent<Camera>().targetTexture = null;
					
					GUILayout.Space(7);
					GUILayout.Label("Animation Preview");
					Rect previewRect = new Rect(0,0, Screen.width, Screen.width/aspect);
					
					Rect layoutRect = EditorGUILayout.BeginVertical();
					previewRect.x = layoutRect.x;
					previewRect.y = layoutRect.y+5;
					EditorGUI.DrawPreviewTexture(previewRect, pointPreviewTexture);
					GUILayout.Space(previewRect.height+10);
					
					EditorGUILayout.BeginHorizontal();
					float time = EditorGUILayout.Slider(animator.editorTime*animator.pathTime,0,animator.pathTime);
					animator.editorTime = time/animator.pathTime;
					EditorGUILayout.LabelField("sec",GUILayout.Width(25));
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.EndVertical();
					DestroyImmediate(cam);
				}
			}
		}
		
		animator.playOnStart = EditorGUILayout.Toggle("Play on start", animator.playOnStart);
		
		EditorGUILayout.BeginHorizontal();
		animator.pathTime = EditorGUILayout.FloatField("Animation Time",animator.pathTime);
		EditorGUILayout.LabelField("sec",GUILayout.Width(25));
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		animator.pathSpeed = EditorGUILayout.FloatField("Animation Speed",animator.pathSpeed);
		EditorGUILayout.LabelField("m/sec",GUILayout.Width(25));
		EditorGUILayout.EndHorizontal();
		
		animator.pathTime = Mathf.Max(animator.pathTime,0.001f);//ensure it's a real number
		
		animator.animationTarget = (Transform)EditorGUILayout.ObjectField("Animate Object",animator.animationTarget, typeof(Transform),true);
		EditorGUILayout.HelpBox("This toggle can be used to specify what kind of object you are animating. If it isn't a camera, we recommend you uncheck this box",MessageType.Info);
		animator.isCamera = EditorGUILayout.Toggle("Is Camera", animator.isCamera);
		
		animator.mode = (CameraPathBezierAnimator.modes)EditorGUILayout.EnumPopup("Animation Mode",animator.mode);
		
		animator.normalised = EditorGUILayout.Toggle("Normalised Path",animator.normalised);
		
		EditorGUILayout.HelpBox("Set this if you want to start another camera path animation once this has completed",MessageType.Info);
		animator.nextAnimation = (CameraPathBezierAnimator)EditorGUILayout.ObjectField("Next Camera Path",animator.nextAnimation, typeof(CameraPathBezierAnimator),true);
		
		if(bezier.mode == CameraPathBezier.viewmodes.mouselook)
		{
			EditorGUILayout.HelpBox("Alter the mouse sensitivity here",MessageType.Info);
			animator.sensitivity = EditorGUILayout.Slider("Mouse Sensitivity",animator.sensitivity,0.1f, 2.0f);
			EditorGUILayout.HelpBox("Restrict the vertical viewable area here.",MessageType.Info);
			EditorGUILayout.LabelField("Mouse Y Restriction");
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(((int)animator.minX).ToString(),GUILayout.Width(30));
			EditorGUILayout.MinMaxSlider(ref animator.minX, ref animator.maxX, -180, 180);
			EditorGUILayout.LabelField(((int)animator.maxX).ToString(),GUILayout.Width(30));
			EditorGUILayout.EndHorizontal();
		}
		
        if (GUI.changed)
            EditorUtility.SetDirty (animator);
	}	
}
