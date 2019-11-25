using UnityEngine;
using System.Collections;

public class RoofDefinition : MonoBehaviour
{
    private bool isPicked = false;
    public bool IsPicked
    {
        get
        {
            return isPicked;
        }
        set
        {
            isPicked = value;
        }
    }

    public string typeName;         // Hip, Gable, Clerestory...
    public string materialName;        // Asphalt, Wood Shingles, Slate...
    public Material material;
    public int id = 0;
    public int roofMaterialIndex = 0;

    // The place where typeName/materialName will be placed in scrolling
    private Vector2 labelPos;
    public Vector2 LabelPos
    {
        get
        {
            return labelPos;
        }
        set
        {
            labelPos = value;
        }
    }


    public Vector3 offSet;


}
