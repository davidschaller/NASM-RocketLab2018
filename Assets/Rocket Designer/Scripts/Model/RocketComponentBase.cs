using UnityEngine;

[System.Serializable]
public class RocketComponentBase : MonoBehaviour
{
    static public ComponentType GetComponentTypeByName(ComponentName cn)
    {
        ComponentType res = ComponentType.None;
        switch (cn)
        {
            case ComponentName.SolidFuel:
            case ComponentName.Kerosene:
            case ComponentName.LiquidHydrogen:
                res = ComponentType.Propellant;
                break;
            case ComponentName.MoveableFins:
            case ComponentName.ExhaustVanes:
            case ComponentName.GimbaledEngines:
                res = ComponentType.Control;
                break;
            case ComponentName.OneStage:
            case ComponentName.TwoStage:
            case ComponentName.ThreeStage:
                res = ComponentType.Stages;
                break;
            case ComponentName.Tapered:
            case ComponentName.Cylindrical:
            case ComponentName.Flared:
                res = ComponentType.Shape;
                break;
            case ComponentName.None:
                res = ComponentType.None;
                break;
        }
        return res;
    }

    public enum ComponentType
    {
        Propellant,
        Control,
        Stages,
        Shape,
        None
    }

    public ComponentType componentType;

    public enum ComponentName
    {
        SolidFuel,
        Kerosene,
        LiquidHydrogen,
        MoveableFins,
        ExhaustVanes,
        GimbaledEngines,
        OneStage,
        TwoStage,
        ThreeStage,
        Tapered,
        Cylindrical,
        Flared,
        None
    }

    public ComponentName componentName;

    public GameObject previewPrefab;

    public float cost = 5;

    public TextAsset xml;
    public string xmlAttrType = "copponent_type",
                  xmlAttrName = "component_name",
                  xmlAttrHeader = "component_header",
                  xmlAttrComponentDescription = "component_description",
                  xmlAttrCostHint = "cost_hint",
                  xmlAttrCostName = "cost";

    public string[] meshPathArray;

    Mesh[] myMeshes;

    public string GetComponentTypeText()
    {
        return DialogXMLParser.GetText(xml.text, xmlAttrType, ActiveLanguage.English);
    }

    public string GetComponentName()
    {
        return DialogXMLParser.GetText(xml.text, xmlAttrName, ActiveLanguage.English);
    }

    public virtual string[] GetProperties()
    {
        string headerText = DialogXMLParser.GetText(xml.text, xmlAttrHeader, ActiveLanguage.English),
               description = DialogXMLParser.GetText(xml.text, xmlAttrComponentDescription, ActiveLanguage.English),
               costName = DialogXMLParser.GetText(xml.text, xmlAttrCostName, ActiveLanguage.English),
               costHint = DialogXMLParser.GetText(xml.text, xmlAttrCostHint, ActiveLanguage.English);

        string[] result = new string[12];

        result[0] = headerText;
        result[1] = description;
        result[2] = costName;
        result[3] = string.Empty;
        result[4] = string.Empty;
        result[5] = costHint;
        result[6] = string.Empty;
        result[7] = string.Empty;
        result[8] = cost.ToString();
        result[9] = string.Empty;
        result[10] = string.Empty;
        result[11] = string.Empty;

        return result;
    }
}
