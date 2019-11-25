
[System.Serializable]
public class ShapeComponent : RocketComponentBase
{
    public float aerodynamics = 10;

    public string xmlAttrAerodynamicsHint = "aerodynamics_hint",
                  xmlAttrAerodynamicsName = "aerodynamics";

    public override string[] GetProperties()
    {
        string headerText = DialogXMLParser.GetText(xml.text, xmlAttrHeader, ActiveLanguage.English),
               description = DialogXMLParser.GetText(xml.text, xmlAttrComponentDescription, ActiveLanguage.English),
               costName = DialogXMLParser.GetText(xml.text, xmlAttrCostName, ActiveLanguage.English),
               aerodynamicsName = DialogXMLParser.GetText(xml.text, xmlAttrAerodynamicsName, ActiveLanguage.English),
               costHint = DialogXMLParser.GetText(xml.text, xmlAttrCostHint, ActiveLanguage.English),
               aerodynamicsHint = DialogXMLParser.GetText(xml.text, xmlAttrAerodynamicsHint, ActiveLanguage.English);

        string[] result = new string[12];

        result[0] = headerText;
        result[1] = description;
        result[2] = costName;
        result[3] = aerodynamicsName;
        result[4] = string.Empty;
        result[5] = costHint;
        result[6] = aerodynamicsHint;
        result[7] = string.Empty;
        result[8] = cost.ToString();
        result[9] = aerodynamics.ToString();
        result[10] = string.Empty;
        result[11] = string.Empty;

        return result;
    }
}
