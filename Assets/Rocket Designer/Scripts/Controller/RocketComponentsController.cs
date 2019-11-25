using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RocketComponentsController : MonoBehaviour
{
    const float MAX_RELIABILITY = 10;

    List<RocketComponentBase> rocketComponents;

    public class RocketConfig
    {
        public RocketComponentBase.ComponentName pickedPropellant = RocketComponentBase.ComponentName.None,
                                                 pickedControl = RocketComponentBase.ComponentName.None,
                                                 pickedStage = RocketComponentBase.ComponentName.None,
                                                 pickedShape = RocketComponentBase.ComponentName.None;

        public bool IsComplete()
        {
            bool result = true;

            result &= pickedControl != RocketComponentBase.ComponentName.None;
            result &= pickedPropellant != RocketComponentBase.ComponentName.None;
            result &= pickedShape != RocketComponentBase.ComponentName.None;
            result &= pickedStage != RocketComponentBase.ComponentName.None;

            return result;
        }

        public string rocketName = string.Empty;
    }

    RocketConfig rocketConfig;
    public RocketConfig Rocket_Config
    {
        get { return rocketConfig; }
    }

    public RealRocket[] RealRockets;
    public TextAsset xml;

    void LoadRealRocket()
    {
        if (xml)
        {
            foreach (RealRocket rr in RealRockets)
            {
                rr.SetNameText(DialogXMLParser.GetText(xml.text, rr.xml_tag + "/name", ActiveLanguage.English));
                rr.SetDescriptionTitle(DialogXMLParser.GetText(xml.text, rr.xml_tag + "/description_title", ActiveLanguage.English));
                rr.SetDescription(DialogXMLParser.GetText(xml.text, rr.xml_tag + "/description", ActiveLanguage.English));
                rr.SetTooltip(DialogXMLParser.GetText(xml.text, rr.xml_tag + "/tooltip", ActiveLanguage.English));
            }
        }
    }

    public RealRocket FindRealRocket()
    {
        RealRocket result = null;
        int maxcmp = -1;
        foreach (RealRocket rr in RealRockets)
        {
            int cmp = CompareRocket(rocketConfig, rr, RocketComponentBase.ComponentType.None);
            if (cmp > maxcmp)
            {
                maxcmp = cmp;
                result = rr;
            }
        }
        return result;
    }

    public RealRocket FindRealRocket(RocketComponentBase.ComponentType cct)
    {
        RealRocket result = null;
        int maxcmp = -1;
        foreach (RealRocket rr in RealRockets)
        {
            int cmp = CompareRocket(rocketConfig, rr, cct);
            if (cmp > maxcmp)
            {
                maxcmp = cmp;
                result = rr;
            }
        }
        return result;
    }

    private int CompareRocket(RocketConfig rcfg, RealRocket rr, RocketComponentBase.ComponentType cct)
    {
        int result = 0;
        if (ComareType(rcfg.pickedControl, cct) && rcfg.pickedControl == rr.pickedControl)
            result++;
        if (ComareType(rcfg.pickedPropellant, cct) && rcfg.pickedPropellant == rr.pickedPropellant)
            result++;
        if (ComareType(rcfg.pickedShape, cct) && rcfg.pickedShape == rr.pickedShape)
            result++;
        if (ComareType(rcfg.pickedStage, cct) && rcfg.pickedStage == rr.pickedStage)
            result++;
        return result;
    }

    private bool ComareType(RocketComponentBase.ComponentName rcn, RocketComponentBase.ComponentType cct)
    {
        RocketComponentBase.ComponentType rct = RocketComponentBase.GetComponentTypeByName(rcn);
        bool res = rct == cct || cct == RocketComponentBase.ComponentType.None;
        return res;
    }

    public List<AllRockets> allRocketsList;

    void Awake()
    {
        rocketComponents = new List<RocketComponentBase>();

        foreach (Transform child in transform)
        {
            rocketComponents.Add(child.GetComponent<RocketComponentBase>());
        }

        rocketComponents.Sort(CompareComponentsNames);

        rocketConfig = new RocketConfig()
        {
            pickedControl = RocketComponentBase.ComponentName.None,
            pickedPropellant = RocketComponentBase.ComponentName.None,
            pickedShape = RocketComponentBase.ComponentName.None,
            pickedStage = RocketComponentBase.ComponentName.None
        };

        LoadRealRocket();
    }

    int CompareComponentsNames(RocketComponentBase a, RocketComponentBase b)
    {
        return string.Compare(a.name, b.name);
    }

    [System.Serializable]
    public class AllRockets
    {
        public RocketComponentBase.ComponentName shape = RocketComponentBase.ComponentName.None,
                                                 stages = RocketComponentBase.ComponentName.None; 

        public Transform baseRocketCone;

        public Transform fins,
                         gimbals,
                         vanes;

        public Transform hull,
                         liquid,
                         solid;

        public List<Transform> bodyList;

        public Transform removableOuterPanel;

        public void ShowBody(bool showBody, bool hideOuterPanel)
        {
            foreach (Transform body in bodyList)
            {
                body.gameObject.SetActive(showBody);
            }

            removableOuterPanel.gameObject.SetActive(showBody && !hideOuterPanel);
            baseRocketCone.gameObject.SetActive(showBody);
        }

        public void SetPickedControl(RocketComponentBase.ComponentName control)
        {
            switch (control)
            {
                case RocketComponentBase.ComponentName.MoveableFins:
                    fins.gameObject.SetActive(true);
                    gimbals.gameObject.SetActive(false);
                    vanes.gameObject.SetActive(false);
                    break;
                case RocketComponentBase.ComponentName.GimbaledEngines:
                    gimbals.gameObject.SetActive(true);
                    fins.gameObject.SetActive(false);
                    vanes.gameObject.SetActive(false);
                    break;
                case RocketComponentBase.ComponentName.ExhaustVanes:
                    vanes.gameObject.SetActive(true);
                    fins.gameObject.SetActive(false);
                    gimbals.gameObject.SetActive(false);
                    break;
                default:
                    vanes.gameObject.SetActive(false);
                    fins.gameObject.SetActive(false);
                    gimbals.gameObject.SetActive(false);
                    break;
            }
        }

        public void SetPickedPropellant(RocketComponentBase.ComponentName propellant)
        {
            switch (propellant)
            {
                case RocketComponentBase.ComponentName.Kerosene:
                    hull.gameObject.SetActive(true);
                    liquid.gameObject.SetActive(false);
                    solid.gameObject.SetActive(false);
                    break;
                case RocketComponentBase.ComponentName.LiquidHydrogen:
                    hull.gameObject.SetActive(false);
                    liquid.gameObject.SetActive(true);
                    solid.gameObject.SetActive(false);
                    break;
                case RocketComponentBase.ComponentName.SolidFuel:
                    hull.gameObject.SetActive(false);
                    liquid.gameObject.SetActive(false);
                    solid.gameObject.SetActive(true);
                    break;
                default:
                    if (hull == null)
                    {
                        Debug.LogError("NULL HULL" + baseRocketCone.name, baseRocketCone.transform);
                    }
                    hull.gameObject.SetActive(false);
                    liquid.gameObject.SetActive(false);
                    solid.gameObject.SetActive(false);
                    break;
            }
        }

        public void HideAll(bool hideOuterPanel)
        {
            ShowBody(false, hideOuterPanel);
            SetPickedControl(RocketComponentBase.ComponentName.None);
            SetPickedPropellant(RocketComponentBase.ComponentName.None);
        }
    }

    private void StageComponents(AllRockets newRocket, Transform tr)
    {
        foreach (Transform item in tr)
        {
            if (item.name.StartsWith("Base_RocketCone"))
            {
                newRocket.baseRocketCone = item;
            }
            else if (item.name.StartsWith("Control_Systems"))
            {
                foreach (Transform control in item)
                {
                    if (control.name.StartsWith("Fins"))
                    {
                        newRocket.fins = control;
                    }
                    else if (control.name.StartsWith("Gimbals"))
                    {
                        newRocket.gimbals = control;
                    }
                    else if (control.name.StartsWith("Vanes"))
                    {
                        newRocket.vanes = control;
                    }
                }
            }
            else if (item.name.StartsWith("Interior_Geo") || item.name.StartsWith("Internal_Geo"))
            {
                foreach (Transform interior in item)
                {
                    if (interior.name.StartsWith("Interior_Hull"))
                    {
                        newRocket.hull = interior;
                    }
                    else if (interior.name.StartsWith("LiquidFuel_System"))
                    {
                        newRocket.liquid = interior;
                    }
                    else if (interior.name.StartsWith("SolidFuel_System"))
                    {
                        newRocket.solid = interior;
                    }
                }
            }
            else if (item.name.StartsWith("Remove_Outerpanel") || item.name.StartsWith("Remove_OuterPanel"))
            {
                newRocket.removableOuterPanel = item;
            }
            else if (item.name.StartsWith("CRS_") || item.name.StartsWith("FRS_") || item.name.StartsWith("RRS_"))
            {
                if (newRocket.shape == RocketComponentBase.ComponentName.None)
                {
                    if (item.name.StartsWith("CRS_"))
                    {
                        newRocket.shape = RocketComponentBase.ComponentName.Cylindrical;
                    }
                    else if (item.name.StartsWith("FRS_"))
                    {
                        newRocket.shape = RocketComponentBase.ComponentName.Flared;
                    }
                    else if (item.name.StartsWith("RRS_"))
                    {
                        newRocket.shape = RocketComponentBase.ComponentName.Tapered;
                    }
                }

                if (item.name.Contains("_1"))
                {
                    newRocket.bodyList.Add(item);

                    if (newRocket.stages == RocketComponentBase.ComponentName.None)
                        newRocket.stages = RocketComponentBase.ComponentName.OneStage;
                }
                else if (item.name.Contains("_2"))
                {
                    newRocket.bodyList.Add(item);

                    if (newRocket.stages == RocketComponentBase.ComponentName.None)
                        newRocket.stages = RocketComponentBase.ComponentName.TwoStage;
                }
                else if (item.name.Contains("_3"))
                {
                    newRocket.bodyList.Add(item);

                    if (newRocket.stages == RocketComponentBase.ComponentName.None)
                        newRocket.stages = RocketComponentBase.ComponentName.ThreeStage;
                }
            }

            StageComponents(newRocket, item);
        }
    }

    public void CollectComponents(Transform trInstantiatedRocket)
    {
        allRocketsList = new List<AllRockets>();

        foreach (Transform tr in trInstantiatedRocket)
        {
            AllRockets newRocket = new AllRockets();
            newRocket.bodyList = new List<Transform>();
            
            StageComponents(newRocket, tr);

            allRocketsList.Add(newRocket);
        }

        ReconfigRocket();
    }

    public Dictionary<RocketComponentBase.ComponentType, string> GetTabs()
    {
        Dictionary<RocketComponentBase.ComponentType, string> result = new Dictionary<RocketComponentBase.ComponentType, string>();

        result.Add(RocketComponentBase.ComponentType.Propellant, rocketComponents.Find(p => p.componentType == RocketComponentBase.ComponentType.Propellant).GetComponentTypeText());
        result.Add(RocketComponentBase.ComponentType.Control, rocketComponents.Find(p => p.componentType == RocketComponentBase.ComponentType.Control).GetComponentTypeText());
        result.Add(RocketComponentBase.ComponentType.Stages, rocketComponents.Find(p => p.componentType == RocketComponentBase.ComponentType.Stages).GetComponentTypeText());
        result.Add(RocketComponentBase.ComponentType.Shape, rocketComponents.Find(p => p.componentType == RocketComponentBase.ComponentType.Shape).GetComponentTypeText());

        return result;
    }

    public Dictionary<RocketComponentBase.ComponentName, string> GetButtons(RocketComponentBase.ComponentType componentType)
    {
        Dictionary<RocketComponentBase.ComponentName, string> result = new Dictionary<RocketComponentBase.ComponentName, string>();

        for (int i = 0; i < rocketComponents.Count; i++)
        {
            if (rocketComponents[i].componentType == componentType)
            {
                result.Add(rocketComponents[i].componentName, rocketComponents[i].GetComponentName());
            }
        }

        return result;
    }

    public string[] GetScreenDetails(RocketComponentBase.ComponentName componentName)
    {
        string[] result = new string[12];

        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName == componentName)
            {
                result = item.GetProperties();

                break;
            }
        }

        return result;
    }

    public void UpdateRocket(RocketComponentBase.ComponentType componentType, RocketComponentBase.ComponentName componentName)
    {
        switch (componentType)
        {
            case RocketComponentBase.ComponentType.Control:
                if (rocketConfig.pickedControl != componentName)
                {
                    rocketConfig.pickedControl = componentName;
                }
                break;
            case RocketComponentBase.ComponentType.Propellant:
                if (rocketConfig.pickedPropellant != componentName)
                {
                    rocketConfig.pickedPropellant = componentName;
                }
                break;
            case RocketComponentBase.ComponentType.Shape:
                if (rocketConfig.pickedShape != componentName)
                {
                    rocketConfig.pickedShape = componentName;
                }
                break;
            case RocketComponentBase.ComponentType.Stages:
                if (rocketConfig.pickedStage != componentName)
                {
                    rocketConfig.pickedStage = componentName;
                }
                break;
        }

        ReconfigRocket();
    }

    public bool AreAllComponentsSet()
    {
        return rocketConfig.IsComplete();
    }

	void ReconfigRocket()
    {
        if (rocketConfig != null)
        {
            RocketComponentBase.ComponentName currentStage = RocketComponentBase.ComponentName.OneStage;

            if (rocketConfig.pickedStage != RocketComponentBase.ComponentName.None)
                currentStage = rocketConfig.pickedStage;

            RocketComponentBase.ComponentName currentShape = RocketComponentBase.ComponentName.Cylindrical;
            bool showBody = false;
            if (rocketConfig.pickedShape != RocketComponentBase.ComponentName.None)
            {
                currentShape = rocketConfig.pickedShape;
                showBody = true;
            }

            foreach (AllRockets item in allRocketsList)
            {
                if (item.stages == currentStage && item.shape == currentShape)
                {
                    item.ShowBody(showBody, hideOuterPanel);
                    item.SetPickedControl(rocketConfig.pickedControl);
                    item.SetPickedPropellant(rocketConfig.pickedPropellant);
                }
                else
                    item.HideAll(hideOuterPanel);
            }
        }
    }

    public float GetCost(RocketComponentBase.ComponentName componentName)
    {
        if (componentName == RocketComponentBase.ComponentName.None)
        {
            return 0;
        }
        else
        {
            foreach (RocketComponentBase item in rocketComponents)
            {
                if (item.componentName == componentName)
                {
                    return item.cost;
                }
            }

            Debug.LogError("Couldn't find component with name " + componentName);

            return 0;
        }
    }

    public float GetRefund(RocketComponentBase.ComponentType componentType)
    {
        float result = 0;

        switch (componentType)
        {
            case RocketComponentBase.ComponentType.Control:
                result = GetCost(rocketConfig.pickedControl);
                break;
            case RocketComponentBase.ComponentType.Propellant:
                result = GetCost(rocketConfig.pickedPropellant);
                break;
            case RocketComponentBase.ComponentType.Shape:
                result = GetCost(rocketConfig.pickedShape);
                break;
            case RocketComponentBase.ComponentType.Stages:
                result = GetCost(rocketConfig.pickedStage);
                break;
        }

        return result;
    }
	
    public GameObject GetPreview(RocketComponentBase.ComponentName componentName)
    {
        GameObject result = null;

		foreach (RocketComponentBase item in rocketComponents)
		{
			if (item.componentName == componentName)
			{
                result = item.previewPrefab;
				break;
			}
		}

        return result;
    }

    public float GetComponentValue(RocketComponentBase.ComponentType componentType)
    {
        switch (componentType)
        {
            case  RocketComponentBase.ComponentType.Propellant:
                if (rocketConfig.pickedPropellant != RocketComponentBase.ComponentName.None)
                {
                    foreach (RocketComponentBase item in rocketComponents)
                    {
                        if (item.componentName == rocketConfig.pickedPropellant)
                        {
                            if (item is PropellantComponent)
                            {
                                PropellantComponent propellantComponent = item as PropellantComponent;

                                return propellantComponent.thrust;
                            }
                            else
                                Debug.LogError("The component with name '" + item.componentName + "' is not a PropellantComponent.");
                        }
                    }
                }
                break;
            case RocketComponentBase.ComponentType.Control:
                if (rocketConfig.pickedControl != RocketComponentBase.ComponentName.None)
                {
                    foreach (RocketComponentBase item in rocketComponents)
                    {
                        if (item.componentName == rocketConfig.pickedControl)
                        {
                            if (item is ControlComponent)
                            {
                                ControlComponent controlComponent = item as ControlComponent;

                                return controlComponent.control;
                            }
                            else
                                Debug.LogError("The component with name '" + item.componentName + "' is not a ControlComponent.");
                        }
                    }
                }
                break;
            case RocketComponentBase.ComponentType.Shape:
                if (rocketConfig.pickedShape != RocketComponentBase.ComponentName.None)
                {
                    foreach (RocketComponentBase item in rocketComponents)
                    {
                        if (item.componentName == rocketConfig.pickedShape)
                        {
                            if (item is ShapeComponent)
                            {
                                ShapeComponent shapeComponent = item as ShapeComponent;

                                return shapeComponent.aerodynamics;
                            }
                            else
                                Debug.LogError("The component with name '" + item.componentName + "' is not a ShapeComponent.");
                        }
                    }
                }
                break;
            case RocketComponentBase.ComponentType.Stages:
                if (rocketConfig.pickedStage != RocketComponentBase.ComponentName.None)
                {
                    foreach (RocketComponentBase item in rocketComponents)
                    {
                        if (item.componentName == rocketConfig.pickedStage)
                        {
                            if (item is StageComponent)
                            {
                                StageComponent stageComponent = item as StageComponent;

                                return stageComponent.liftoffWeight;
                            }
                            else
                                Debug.LogError("The component with name '" + item.componentName + "' is not a StageComponent.");
                        }
                    }
                }
                break;
        }

        return 0;
    }

    public float GetRisk()
    {
        float maxRisk = 0;
        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName != RocketComponentBase.ComponentName.None)
            {
                if (item.componentName == rocketConfig.pickedControl || item.componentName == rocketConfig.pickedPropellant
                    || item.componentName == rocketConfig.pickedStage)
                {
                    IImprovable improvable = item as IImprovable;

                    float totalReliability = improvable.GetReliability();

                    totalReliability = Mathf.Clamp(totalReliability, 0, MAX_RELIABILITY);

                    //float risk = 1 - totalReliability / MAX_RELIABILITY;
                    float risk = totalReliability / MAX_RELIABILITY;

                    if (maxRisk < risk)
                    {
                        maxRisk = risk;
                    }
                }
            }
            else
                Debug.LogError(item.transform.name + " has None ComponentName");
        }

        return maxRisk;
    }

    public void SetName(string rocketName)
    {
        rocketConfig.rocketName = rocketName;
        if (TextToTexture.Main)
        {
            TextToTexture.Main.ApplyText(rocketName);
        }
    }

    public void GetThrustWeightRatio(out float thrust, out float weight, float aeroEffect, float thrustFactor, float weightFactor)
    {
        thrust = GetComponentValue(RocketComponentBase.ComponentType.Propellant);
        weight = GetComponentValue(RocketComponentBase.ComponentType.Stages);

        //thrust *= thrustFactor;
        //weight *= weightFactor;

        if (thrust != 0)
        {
            float aerodynamics = GetComponentValue(RocketComponentBase.ComponentType.Shape);
            if (aerodynamics != 0)
                thrust += thrust * aerodynamics * aeroEffect / 100;
        }
    }

    public RocketComponentBase[] GetFailureComponents()
    {
        List<RocketComponentBase> result = new List<RocketComponentBase>();

        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName == rocketConfig.pickedControl || item.componentName == rocketConfig.pickedPropellant
                || item.componentName == rocketConfig.pickedStage)
            {
                if (item is IImprovable)
                {
                    IImprovable improvable = item as IImprovable;
                    if (improvable.RollTheDice())
                    {
                        result.Add(item);
                    }
                }
            }
        }

        return result.ToArray();
    }

    public void ImproveComponents(RocketComponentBase[] toImprove, float improveValue)
    {
        foreach (RocketComponentBase item in toImprove)
        {
            IImprovable improvable = item as IImprovable;
            improvable.Improve(improveValue);
        }
    }

    public float GetRocketTransformOffsetOnStart()
    {
        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName == rocketConfig.pickedStage)
            {
                StageComponent stageComponent = item as StageComponent;

                return stageComponent.offsetOnStart;
            }
        }

        return 0;
    }

    public RocketComponentBase[] GetActiveComponents()
    {
        List<RocketComponentBase> result = new List<RocketComponentBase>();

        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName != RocketComponentBase.ComponentName.None)
            {
                if (item.componentName == rocketConfig.pickedControl || item.componentName == rocketConfig.pickedPropellant
                    || item.componentName == rocketConfig.pickedStage)
                {
                    result.Add(item);
                }
            }
        }

        return result.ToArray();
    }

    public float[] GetImprovementsAsArray()
    {
        float[] result = new float[9];

        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item is IImprovable)
            {
                IImprovable improvable = item as IImprovable;

                switch (item.componentName)
                {
                    case RocketComponentBase.ComponentName.SolidFuel:
                        result[0] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.Kerosene:
                        result[1] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.LiquidHydrogen:
                        result[2] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.MoveableFins:
                        result[3] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.ExhaustVanes:
                        result[4] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.GimbaledEngines:
                        result[5] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.OneStage:
                        result[6] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.TwoStage:
                        result[7] = improvable.GetImprovement();
                        break;
                    case RocketComponentBase.ComponentName.ThreeStage:
                        result[8] = improvable.GetImprovement();
                        break;
                }
            }
        }

        return result;
    }

    public void SetImprovementsFromArray(float[] improvements)
    {
        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item is IImprovable)
            {
                IImprovable improvable = item as IImprovable;

                switch (item.componentName)
                {
                    case RocketComponentBase.ComponentName.SolidFuel:
                        improvable.SetLastImprovement(improvements[0]);
                        break;
                    case RocketComponentBase.ComponentName.Kerosene:
                        improvable.SetLastImprovement(improvements[1]);
                        break;
                    case RocketComponentBase.ComponentName.LiquidHydrogen:
                        improvable.SetLastImprovement(improvements[2]);
                        break;
                    case RocketComponentBase.ComponentName.MoveableFins:
                        improvable.SetLastImprovement(improvements[3]);
                        break;
                    case RocketComponentBase.ComponentName.ExhaustVanes:
                        improvable.SetLastImprovement(improvements[4]);
                        break;
                    case RocketComponentBase.ComponentName.GimbaledEngines:
                        improvable.SetLastImprovement(improvements[5]);
                        break;
                    case RocketComponentBase.ComponentName.OneStage:
                        improvable.SetLastImprovement(improvements[6]);
                        break;
                    case RocketComponentBase.ComponentName.TwoStage:
                        improvable.SetLastImprovement(improvements[7]);
                        break;
                    case RocketComponentBase.ComponentName.ThreeStage:
                        improvable.SetLastImprovement(improvements[8]);
                        break;
                }
            }
        }
    }

    public float GetTotalCost()
    {
        float result = 0;

        foreach (RocketComponentBase item in rocketComponents)
        {
            if (item.componentName == rocketConfig.pickedPropellant || item.componentName == rocketConfig.pickedControl ||
                item.componentName == rocketConfig.pickedShape || item.componentName == rocketConfig.pickedStage)
            {
                result += item.cost;
            }
        }

        return result;
    }

    bool hideOuterPanel = false;
    public void ToggleOuterPanel(bool hide)
    {
        hideOuterPanel = hide;
        ReconfigRocket();
    }
}

[System.Serializable]
public class RealRocket : RocketComponentsController.RocketConfig
{
    private string nametext;
    private string descriptionTitle;
    private string description;
    private string tooltip;

    public Texture Img;
    public string xml_tag;
    public bool inMuseum;

    public string NameText { get { return nametext; } }
    public string DescriptionTitle { get { return descriptionTitle; } }
    public string Description { get { return description; } }
    public string Tooltip { get { return tooltip; } }

    public void SetNameText(string val)
    {
        nametext = val;
    }

    public void SetDescriptionTitle(string val)
    {
        descriptionTitle = val;
    }

    public void SetDescription(string val)
    {
        description = val;
    }

    public void SetTooltip(string val)
    {
        tooltip = val;
    }
}

