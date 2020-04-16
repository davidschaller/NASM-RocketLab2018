using UnityEngine;
using System.Collections;

public class ObjectFieldGenerator : MonoBehaviour
{
    public float myMaxDistance = 500;
    public float myMinDistance = 0;
    public int myObjectCount = 50;
    public int myRandomSeed = 0;
    public GameObject myGameObjectPrefab;
    
    public void Regenerate()
    {
        DeleteAllChildren();

        Random.State previousState = Random.state;
        Random.InitState(myRandomSeed);

        for (int i = 0; i < myObjectCount; ++i)
        {
            Vector3 randomInsideUnit = Random.insideUnitSphere;
            Vector3 pos = randomInsideUnit.normalized * myMinDistance + randomInsideUnit * (myMaxDistance - myMinDistance);

            GameObject obj = GameObject.Instantiate(myGameObjectPrefab, pos, Quaternion.identity) as GameObject;
            obj.transform.SetParent(transform, true);
        }

        Random.state = previousState;
    }
    
    public void DeleteAllChildren()
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; ++i)
        {
            if (children[i] == transform)
                continue;

            GameObject.DestroyImmediate(children[i].gameObject);
        }
    }
}
