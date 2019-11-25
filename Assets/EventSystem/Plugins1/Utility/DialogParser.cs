using UnityEngine;
using System.Collections;
using System.Xml;
using System.Xml.XPath;

public class DialogParser
{
	string xmlText;
	XmlDocument dialogXML;
	public DialogParser (string xml)
	{
		xmlText = xml;
	    XmlSetup();
	}
	
	void XmlSetup ()
	{
	    dialogXML = new XmlDocument();
	    dialogXML.LoadXml(xmlText);
	}
	
	public string Greeting
	{
		get
		{
			return ReplaceTokens(dialogXML.SelectNodes("//Conversation/NPC")[0].FirstChild.Value.TrimEnd());
		}
		private set {}
	}
	
	public string[] Questions
	{
		get
		{
			string[] questions = new string[CachedQuestions.Count];
		    for (int i =0;i<CachedQuestions.Count;i++ )
		    {
				if (CachedQuestions[i].FirstChild.Value != null)
				{
					questions[i] = ReplaceTokens(CachedQuestions[i].FirstChild.Value.TrimEnd());
				}
				else
				{
					Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
				}
		    }

			return questions;
		}
		private set {}
	}

	public int FirstSequence ()
	{
		int minSeq = 999;
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				int curr = int.Parse(CachedQuestions[i].Attributes.GetNamedItem("seq").InnerText);
				if (curr < minSeq)
					minSeq = curr;
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}

		return minSeq;
	}
	
	public int LongestQuestionLength ()
	{
		int longest = 0;
		foreach (string q in Questions)
		{
			if (q.Length > longest)
				longest = q.Length;
		}
		return longest;
	}

	public string LongestQuestion ()
	{
		int longest = 0;
		string lq = "";
		
		foreach (string q in Questions)
		{
			if (q.Length > longest)
			{
				lq = q;
				longest = q.Length;
			}
		}
		return lq;
	}

	
	System.Xml.XmlNodeList cachedQuestions;
	System.Xml.XmlNodeList CachedQuestions
	{
		get
		{
			if (cachedQuestions == null)
				cachedQuestions = dialogXML.SelectNodes("//Conversation/NPC/QuestionGroup/PCQuestion");
			return cachedQuestions;
		}
		set
		{
			cachedQuestions = value;
		}
	}
	
	
	public int CountQuestionsForSequenceLevel(int sequenceLevel)
	{
		int count = 0;
		
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				int seq = int.Parse(CachedQuestions[i].Attributes.GetNamedItem("seq").InnerText);
				if (seq <= sequenceLevel)
					count++;
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}
		return count;
	}
	
	public int SequenceLevelFor( string question )
	{
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				if (ReplaceTokens(CachedQuestions[i].FirstChild.Value.TrimEnd()) == question)
				{
					//Debug.Log("Return " + CachedQuestions[i].Attributes.GetNamedItem("seq").InnerText);
					return int.Parse(CachedQuestions[i].Attributes.GetNamedItem("seq").InnerText);
				}
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}
		return 0;
	}
	
	public int AnimationIndexFor( string question )
	{
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				if (ReplaceTokens(CachedQuestions[i].FirstChild.Value.TrimEnd()) == question)
				{
					if (CachedQuestions[i].Attributes["animation"] != null)
						return int.Parse(CachedQuestions[i].Attributes.GetNamedItem("animation").InnerText);
				}
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}
		return -1;
	}


	public bool AnimationShouldLoopFor( string question )
	{
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				if (ReplaceTokens(CachedQuestions[i].FirstChild.Value.TrimEnd()) == question)
				{
					if (CachedQuestions[i].Attributes["loop"] != null)
						return CachedQuestions[i].Attributes.GetNamedItem("loop").InnerText == "true";
				}
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}
		return false;
	}
	

	public bool StartsFlashback( string question )
	{
		for (int i =0;i<CachedQuestions.Count;i++ )
		{
			if (CachedQuestions[i].FirstChild.Value != null)
			{
				if (ReplaceTokens(CachedQuestions[i].FirstChild.Value.TrimEnd()) == question)
				{
					return CachedQuestions[i].Attributes.GetNamedItem("flashback") != null && CachedQuestions[i].Attributes.GetNamedItem("flashback").InnerText=="true";
				}
			}
			else
			{
				Debug.LogError("A PCQuestion exists, but has no content (it is blank)");
			}
		}
		return false;
	}
	
	PlayerController pc;
	public string ReplaceTokens (string message)
	{
		if (pc == null) pc = (PlayerController)GameObject.FindObjectOfType(typeof(PlayerController));
		if (pc == null)
			return message;
		
		string replaced = message.Replace("-PC-", pc.playerName);
		replaced = replaced.Replace("-PC_GENDER-", pc.Gender.ToString());
		replaced = replaced.Replace("-PC_HIS_HER-", pc.Gender == Gender.Male ? "his" : "her");
		
		return replaced;
	}
	
	public string AnswerTo(string question)
	{
	    for (int i =0;i<CachedQuestions.Count;i++ )
	    {
	        if (CachedQuestions[i].FirstChild.Value.TrimEnd() == question)
			{
				return ReplaceTokens(CachedQuestions[i].SelectNodes("NPCResponse")[0].FirstChild.Value.TrimEnd());
			}
	    }
	
		return "";
	}
}
