using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Puffy_Emitter))]
public class Puffy_Emitter_Inspector : Editor {
	
	public override void OnInspectorGUI(){
		Puffy_Emitter myTarget = (Puffy_Emitter) target;
		
		myTarget.autoEmit = EditorGUILayout.Toggle("Auto emit",myTarget.autoEmit);
		myTarget.freezed = EditorGUILayout.Toggle("Freezed",myTarget.freezed);
		
		EditorGUILayout.Separator();
		
		
		myTarget.chunkSize = (int)EditorGUILayout.Slider("Chunk Size",myTarget.chunkSize,64,4096);
		myTarget.autoResize = EditorGUILayout.Toggle("Auto Resize",myTarget.autoResize);
		
		EditorGUILayout.Separator();
		
		myTarget.trailMode = EditorGUILayout.Toggle("Trail mode",myTarget.trailMode);
		
		if(myTarget.trailMode){
			myTarget.autoTrailStep = EditorGUILayout.Toggle("Trail auto step",myTarget.autoTrailStep);
			if(!myTarget.autoTrailStep){
				myTarget.trailStepDistance = EditorGUILayout.FloatField("Trail step distance",myTarget.trailStepDistance);
			}else{
				myTarget.autoTrailStepFactor = EditorGUILayout.FloatField("Trail step factor",myTarget.autoTrailStepFactor);
			}
		}
		
		EditorGUILayout.Separator();
		
		myTarget.startDirection = EditorGUILayout.Vector3Field("Direction",myTarget.startDirection);
		myTarget.startDirectionVariation = EditorGUILayout.Vector3Field("Direction Variation",myTarget.startDirectionVariation);
		
		EditorGUILayout.Separator();
		
		myTarget.lifeTime = Mathf.Max (0,EditorGUILayout.FloatField("Life time",myTarget.lifeTime));
		myTarget.lifeTimeVariation = Mathf.Min (myTarget.lifeTime , Mathf.Max (0,EditorGUILayout.FloatField("Life time variation (-/+)",myTarget.lifeTimeVariation)));
		
		myTarget.startSpeed = Mathf.Max (0,EditorGUILayout.FloatField("Start speed",myTarget.startSpeed));
		myTarget.startSpeedVariation = Mathf.Min (myTarget.startSpeed , Mathf.Max (0,EditorGUILayout.FloatField("Start speed variation (-/+)",myTarget.startSpeedVariation)));
		
		
		EditorGUILayout.Separator();
		
		myTarget.startSize = Mathf.Max (0,EditorGUILayout.FloatField("Start size",myTarget.startSize));
		myTarget.startSizeVariation = Mathf.Min (myTarget.startSize , Mathf.Max (0,EditorGUILayout.FloatField("Start size variation (-/+)",myTarget.startSizeVariation)));
		
		myTarget.endSize = Mathf.Max (0,EditorGUILayout.FloatField("End size",myTarget.endSize));
		myTarget.endSizeVariation = Mathf.Min (myTarget.endSize , Mathf.Max (0,EditorGUILayout.FloatField("End size variation (-/+)",myTarget.endSizeVariation)));
		
		EditorGUILayout.Separator();
		
		myTarget.startColor = EditorGUILayout.ColorField("Start color", myTarget.startColor);
		myTarget.startColorVariation = EditorGUILayout.ColorField("Start color variation (-/+)", myTarget.startColorVariation);
				
		myTarget.endColor = EditorGUILayout.ColorField("End color", myTarget.endColor);
		myTarget.endColorVariation = EditorGUILayout.ColorField("End color variation (-/+)", myTarget.endColorVariation);
	
		if(GUI.changed){
			EditorUtility.SetDirty(target);
		}
	}
}
