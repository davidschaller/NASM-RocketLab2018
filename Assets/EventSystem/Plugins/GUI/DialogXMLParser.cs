using UnityEngine;
using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public enum ActiveLanguage
{
	English,
	Spanish
}

public static class DialogXMLParser
{
	static XPathNavigator GetNavigator(string xmlString)
	{
		XmlDocument xml = new XmlDocument();
		xml.LoadXml(xmlString);
		return xml.CreateNavigator();
	}

	public static string GetText(string xmlString, string section, ActiveLanguage lang)
	{
		string enText = "";
		string selString = "/root/" + section + "/" + lang;
			
		XPathNodeIterator iterator = GetNavigator(xmlString).Select(selString);
		iterator.MoveNext(); 
		if (iterator != null && iterator.Current != null)
			enText = iterator.Current.Value;
		//else
			//Debug.LogWarning("Couldn't find XML node for " + section + ".  That might be OK.");
		
		return enText;
	}
	
	public static string[] GetTextArray(string xmlString, string section, ActiveLanguage lang)
	{
		List<string> ret = new List<string>();
			
		XPathNodeIterator iterator = GetNavigator(xmlString).Select("/root/" + section + "/" + lang);
		while (iterator.MoveNext())
		{
			ret.Add(iterator.Current.Value);
		}
		
		string[] r = new string[ret.Count];
		
		for(int i=0;i<ret.Count;i++)
			r[i] = ret[i];
		
		return r;
	}
			
	public static string GetBoxStyle(string xmlString)
	{
		string text = "BBox";
		
		XPathNodeIterator iterator = GetNavigator(xmlString).CreateNavigator().Select("/root/DialogStyle/@box");
		iterator.MoveNext(); text = iterator.Current.Value;
		
		return text;		
	}
	
	public static string GetButtonStyle(string xmlString)
	{
		string text = "BBox";
		XPathNodeIterator iterator = GetNavigator(xmlString).CreateNavigator().Select("/root/DialogStyle/@button");
		iterator.MoveNext(); text = iterator.Current.Value;
		
		return text;		
	}
	
	public static string GetTextStyle(string xmlString)
	{
		string text = "LLabel";
		XPathNodeIterator iterator = GetNavigator(xmlString).CreateNavigator().Select("/root/DialogStyle/@label");
		iterator.MoveNext(); text = iterator.Current.Value;
		
		return text;		
	}

    public static string GetStyle(string xmlString, string stylePath)
    {
        string result = string.Empty;

        XPathNodeIterator iterator = GetNavigator(xmlString).CreateNavigator().Select(stylePath);
        iterator.MoveNext();
        result = iterator.Current.Value;

        return result;
    }

}
