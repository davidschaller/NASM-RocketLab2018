using System.Collections.Generic;


// TODO DialogManager framework to use when saving is integrated
public static class DialogManager
{
	static Dictionary<string, List<string>> dialogs = new Dictionary<string, List<string>>();

	public static void RegisterUsedDialog(string dialogName, string dialogQuestion)
	{
		if (!dialogs.ContainsKey(dialogName))
		{
			dialogs[dialogName] = new List<string>();
		}

		dialogs[dialogName].Add(dialogQuestion);
	}

	public static bool QuestionWasAsked(string dialogName, string dialogQuestion)
	{
		if (!dialogs.ContainsKey(dialogName))
			return false;

		if (dialogs[dialogName].Contains(dialogQuestion))
			return true;
		
		return false;
	}

	public static bool DialogIsNew(string dialogName)
	{
		if (!dialogs.ContainsKey(dialogName))
		{
			return true;
		}

		return false;
	}
}