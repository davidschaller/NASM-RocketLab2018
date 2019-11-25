
using UnityEngine;
[System.Serializable]
public class ControlComponent : RocketComponentBase, IImprovable
{
    public float reliability = 10,
                 control = 2;

    public string xmlAttrReliabilityHint = "reliability_hint",
                  xmlAttrControlHint = "control_hint",
                  xmlAttrReliabilityName = "reliability",
                  xmlAttrControlName = "control";

    float improvement = 0;

    public override string[] GetProperties()
    {
        string headerText = DialogXMLParser.GetText(xml.text, xmlAttrHeader, ActiveLanguage.English),
               description = DialogXMLParser.GetText(xml.text, xmlAttrComponentDescription, ActiveLanguage.English),
               costName = DialogXMLParser.GetText(xml.text, xmlAttrCostName, ActiveLanguage.English),
               reliabilityName = DialogXMLParser.GetText(xml.text, xmlAttrReliabilityName, ActiveLanguage.English),
               controlName = DialogXMLParser.GetText(xml.text, xmlAttrControlName, ActiveLanguage.English),
               costHint = DialogXMLParser.GetText(xml.text, xmlAttrCostHint, ActiveLanguage.English),
               reliabilityHint = DialogXMLParser.GetText(xml.text, xmlAttrReliabilityHint, ActiveLanguage.English),
               controlHint = DialogXMLParser.GetText(xml.text, xmlAttrControlHint, ActiveLanguage.English);

        string[] result = new string[12];

        result[0] = headerText;
        result[1] = description;
        result[2] = costName;
        result[3] = reliabilityName;
        result[4] = controlName;
        result[5] = costHint;
        result[6] = reliabilityHint;
        result[7] = controlHint;
        result[8] = cost.ToString();
        result[9] = reliability.ToString();
        result[10] = control.ToString();
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

        Debug.Log("Control random=" + random + ", " + " rebiability=" + GetReliability() * 10);

        //return random > GetReliability() * 10;
        return random <= GetReliability() * 10;
    }

    public void Improve(float improveValue)
    {
        improvement += improveValue;
    }

    public void SetLastImprovement(float value)
    {
        improvement = value;
    }

    public float GetImprovement()
    {
        return improvement;
    }
}
