using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class TransformSaver : ScriptableObject
{
    private class TransformSave
    {
        public int instanceID;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 localScale;
        
        public TransformSave(int instanceID, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            this.instanceID = instanceID;
            this.position = position;
            this.rotation = rotation;
            this.localScale = localScale;
        }
    }
    
    private static ArrayList transformSaves = new ArrayList();
    
    [MenuItem ("Custom/Transform Saver/Record Selected Transforms")]
    static void DoRecord()
    {
        Transform[] selection = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        transformSaves = new ArrayList(selection.Length);
        
        foreach (Transform selected in selection)
        {
            TransformSave transformSave = new TransformSave(selected.GetInstanceID(), selected.position, selected.rotation, selected.localScale);
            transformSaves.Add(transformSave);
        }
        
        EditorUtility.DisplayDialog("Transform Saver Record", "Recorded " + transformSaves.Count + " Transforms.", "OK", "");
    }
 
    [MenuItem ("Custom/Transform Saver/Apply Saved Transforms")]
    static void DoApply()
    {
        Transform[] transforms = FindObjectsOfType(typeof(Transform)) as Transform[];
        int numberApplied = 0;
        
        foreach (Transform transform in transforms)
        {
            TransformSave found = null;
            
            for (int i=0; i < transformSaves.Count; i++)
            {
                if (((TransformSave) transformSaves[i]).instanceID == transform.GetInstanceID())
                {
                    found = (TransformSave) transformSaves[i];
                    break;
                }
            }
            
            if (found != null)
            {
                transform.position = found.position;
                transform.rotation = found.rotation;
                transform.localScale = found.localScale;
                numberApplied++;
            }
        }
        
        EditorUtility.DisplayDialog("Transform Saver Apply", "Applied " + numberApplied + " Transforms successfully out of " + transformSaves.Count + " possible.", "OK", "");
    }
}