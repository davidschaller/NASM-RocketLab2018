using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class Desrializator
{
    public static Items Deserialize(string xml)
    {
        using (StringReader stringReader = new StringReader(xml))
        {
            // Skip BOM
            stringReader.Read();
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlReader reader = XmlReader.Create(stringReader, settings);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Items));
            return xmlSerializer.Deserialize(reader) as Items;
        }        
    }
}

[XmlRoot(ElementName = "items")]
public class Items
{
    private readonly List<Item> itemList = new List<Item>();

    [XmlElement(typeof(Item), ElementName = "item")]
    public List<Item> ItemList
    {
        get { return itemList; }
    }

    public string GetText(string id, string lang)
    {
        if (itemList.Exists(p => p.ID.Equals(id)))
            return itemList.Find(p => p.ID.Equals(id)).GetByLang(lang);
        else
            return string.Empty;
    }
}

[XmlRoot(ElementName = "item")]
public class Item
{
    private readonly List<Lang> langList = new List<Lang>();

    [XmlElement(typeof(Lang), ElementName = "lang")]
    public List<Lang> LangList
    {
        get { return langList; }
    }

    [XmlAttribute("id")]
    public string ID { get; set; }

    public string GetByLang(string lang)
    {
        if (langList.Exists(p => p.Value.Equals(lang)))
            return langList.Find(p => p.Value.Equals(lang)).Text;
        else
            return string.Empty;
    }
}

[XmlRoot(ElementName = "lang")]
public class Lang
{
    [XmlAttribute("value")]
    public string Value { get; set; }

    [XmlText]
    public string text;

    [XmlIgnore]
    public string Text
    {
        get
        {
            return text.Trim();
        }
    }
}