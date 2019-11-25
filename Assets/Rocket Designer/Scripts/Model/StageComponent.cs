
using UnityEngine;
[System.Serializable]
public class StageComponent : RocketComponentBase, IImprovable
{
    public float reliability = 10,
                 liftoffWeight = 2;

    public string xmlAttrReliabilityHint = "reliability_hint",
                  xmlAttrLiftoffWeightHint = "liftoff_weight_hint",
                  xmlAttrReliabilityName = "reliability",
                  xmlAttrLiftoffWeightName = "liftoff_weight";

    float improvement = 0;

    public override string[] GetProperties()
    {
        string headerText = DialogXMLParser.GetText(xml.text, xmlAttrHeader, ActiveLanguage.English),
               description = DialogXMLParser.GetText(xml.text, xmlAttrComponentDescription, ActiveLanguage.English),
               costName = DialogXMLParser.GetText(xml.text, xmlAttrCostName, ActiveLanguage.English),
               reliabilityName = DialogXMLParser.GetText(xml.text, xmlAttrReliabilityName, ActiveLanguage.English),
               liftoffWeightName = DialogXMLParser.GetText(xml.text, xmlAttrLiftoffWeightName, ActiveLanguage.English),
               costHint = DialogXMLParser.GetText(xml.text, xmlAttrCostHint, ActiveLanguage.English),
               reliabilityHint = DialogXMLParser.GetText(xml.text, xmlAttrReliabilityHint, ActiveLanguage.English),
               liftoffWeightHint = DialogXMLParser.GetText(xml.text, xmlAttrLiftoffWeightHint, ActiveLanguage.English);

        string[] result = new string[12];

        result[0] = headerText;
        result[1] = description;
        result[2] = costName;
        result[3] = reliabilityName;
        result[4] = liftoffWeightName;
        result[5] = costHint;
        result[6] = reliabilityHint;
        result[7] = liftoffWeightHint;
        result[8] = cost.ToString();
        result[9] = reliability.ToString();
        result[10] = liftoffWeight.ToString();
        result[11] = improvement.ToString();

        return result;
    }

    public float GetReliability()
    {
        return reliability + improvement;
    }

    public bool RollTheDice()
    {
        int random = Random.Range(0, 100);

        Debug.Log("Stage random=" + random + ", " + " rebiability=" + GetReliability() * 10);

        //return random > GetReliability() * 10;
        return random <= GetReliability() * 10;
    }

    public void Improve(float improveValue)
    {
        improvement += improveValue;
    }

    // it might be temprorary parameter
    public float offsetOnStart = 10;

    public void SetLastImprovement(float value)
    {
        improvement = value;
    }

    public float GetImprovement()
    {
        return improvement;
    }
}
