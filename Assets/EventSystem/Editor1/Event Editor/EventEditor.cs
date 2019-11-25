using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Collections;

public class EventEditor : EditorWindow
{
	static EventEditor main;
	static int windowCount = 0;
	
	[MenuItem("Event System/Event Editor %&e")]
	[MenuItem("Assets/Event Editor %&e")]
	public static void NewEventEditor()
	{
		//if (main == null)
		{
			main = EditorWindow.GetWindow<EventEditor>();
			main.position = new Rect(Screen.width/2 - 650/2, 100, 650, 400);
			windowRect.x = main.position.width-210;
			main.name = "Event Editor " + windowCount;
			main.Show();
			windowCount++;
		}
		/*else
		{
			main.Show();
		}*/
	}
	
	[MenuItem("Assets/Create/Event Scenario")]
	static void CreateNewEventScenario ()
	{
		EventScenario newScenario = new EventScenario();
        System.IO.Directory.CreateDirectory(Application.dataPath + "/08 Resources/");
        AssetDatabase.CreateAsset(newScenario, AssetDatabase.GenerateUniqueAssetPath("Assets/08 Resources/Event Scenario.asset"));
		Selection.activeObject = newScenario;
		EventEditor.NewEventEditor();
	}
	
	[MenuItem("Event System/Add new Event Player")]
	static void AddNewEventPlayer ()
	{
		GameObject go = new GameObject();
		go.name = "Event Player";
		EventPlayer ep = (EventPlayer)go.AddComponent(typeof(EventPlayer));
		
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventScenario es = (EventScenario)Selection.activeObject;
			ep.scenario = es;
			go.name = "Event Player " + es.name;
			BlessEventPlayer(ep);
			Selection.activeGameObject = go;
		}
	}
	
	[MenuItem("Event System/Add new Gender Branch Event Player")]
	static void AddNewEventPlayerGenderBranch ()
	{
		GameObject go = new GameObject();
		go.name = "Event Player -- Gender Branch";
		EventPlayerGenderBranch ep = (EventPlayerGenderBranch)go.AddComponent(typeof(EventPlayerGenderBranch));
		Selection.activeGameObject = go;
	}


	[MenuItem("Event System/Re-Bless Event Player %&b")]
	static void BlessSelectedEventPlayers ()
	{
		UnityEngine.Object[] objs = (UnityEngine.Object[])Selection.GetFiltered(typeof(EventPlayer), SelectionMode.Editable);
		foreach (UnityEngine.Object ep in objs)
		{
			BlessEventPlayer((EventPlayer)ep);
		}
	}
	
	[MenuItem("Event System/Verify Event Player")]
	static void VerifySelectedEventPlayers ()
	{
		UnityEngine.Object[] objs = (UnityEngine.Object[])Selection.GetFiltered(typeof(EventPlayer), SelectionMode.Editable);
		foreach (UnityEngine.Object ep in objs)
		{
			VerifyEventPlayer((EventPlayer)ep);
		}
	}
	
	[MenuItem("Event System/Remove all SV from selected")]
	static void RemoveAllSVsFromSelected ()
	{
		Component[] svs = Selection.activeGameObject.GetComponents(typeof(SubstitutionBase));
		foreach (Component sv in svs)
		{
			DestroyImmediate(sv);
		}
	}
	
	static void BlessError(EventPlayer ep, string field)
	{
		Debug.Log(String.Format("Error blessing: {0} is probably missing a variable type implementation ({1}).", ep.name, field));
	}

	static bool SubstitutionExists( KeyValuePair<string, string> subVar, System.Type tp, GameObject go )
	{
		bool substitutionFound = false;
		
		Component[] components = go.GetComponents(tp);
		foreach(Component c in components)
		{
			FieldInfo myFieldInfo1 = tp.GetField("substitutionName", 
												 BindingFlags.Public | BindingFlags.Instance);

			if (myFieldInfo1 != null)
			{
				if ((string)myFieldInfo1.GetValue(c) == subVar.Key)
				{	
					//Debug.Log("Existing sub var found, won't bless it in: " + myFieldInfo1.GetValue(c) + ", " + subVar.Key);
					substitutionFound = true;
					break;
				}
			}
			else
				Debug.Log("No object to check for type: " + tp + ", component: " + c);
		}
		
		return substitutionFound;
	}
	
	static void BlessEventPlayer ( EventPlayer ep )
	{
		Debug.Log("Blessing event player: " + ep.name);
		
		Dictionary<string, string> subs = ep.scenario.GetSubstitutionVariables();
		
		foreach (KeyValuePair<string, string> subVar in subs)
		{
			System.Type tp = VariableTypeConverter.SystemNameToSubstitutionType (subVar.Value);//MapSubstitutionType(subVar.Value);
			
			if (tp == null)
			{
				BlessError(ep, subVar.Value);
			}
			else
			{
				if (!SubstitutionExists(subVar, tp, ep.gameObject))
				{
					Component newComponent = ep.gameObject.AddComponent(tp);
					tp.GetField("substitutionName").SetValue(newComponent, subVar.Key);
					
					if (newComponent == null) BlessError(ep, subVar.Value);
				}
				else
				{
					Debug.Log("Won't replace existing substitution: " + subVar.Key + ", " + subVar.Value);
				}
			}
		}
	}
	
	static void VerifyError(EventPlayer ep, string field)
	{
		Debug.Log(String.Format("{0} either needs to be blessed or is missing a variable type implementation ({1}).", ep.name, field));
	}
	
	static void VerifyEventPlayer ( EventPlayer ep )
	{
		Debug.Log("Verifying event player: " + ep.name);
		Dictionary<string, string> subs = ep.scenario.GetSubstitutionVariables();
		
		bool found = false;
		foreach (KeyValuePair<string, string> subVar in subs)
		{
			found = false;
			System.Type tp = VariableTypeConverter.SystemNameToSubstitutionType (subVar.Value); //MapSubstitutionType(subVar.Value);
			
			if (tp == null)
			{
				VerifyError(ep, subVar.Value);
			}
			else
			{
				Component[] gos = (Component[])ep.GetComponents(tp);
				foreach (Component svgo in gos)
				{
					if (((string)tp.GetField("substitutionName").GetValue(svgo)) == subVar.Key)
						found = true;
				}
				if (!found) VerifyError(ep, subVar.Value);
			}
		}
	}
	
	static void DrawLine(Vector2 start, Vector2 end)
	{
		Handles.DrawLine(start, end);
	}
	
	public void OnSelectionChange()
	{ 
		this.Repaint(); 
	}
	
	void DeleteAllEvents ()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventScenario evs = (EventScenario)Selection.activeObject;
			evs.DeleteAllEvents();
			this.Repaint();
		}
	}
	
	void DeleteSelectedEvent ()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventScenario evs = (EventScenario)Selection.activeObject;
			evs.DeleteEvent(EventBase.selected);
			this.Repaint();
		}
	}
	
	void ClearSelectedExits ()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventBase.selected.DeleteExitEvents();
			this.Repaint();
		}
	}
	
	bool addSubVar = false;
	void AddSubstitutionVariable ()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			Debug.Log("Add sub var at " + Time.time);
			addSubVar = true;
		}
	}

	void DeleteAllSubstitionVariables ()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventScenario evs = (EventScenario)Selection.activeObject;
			evs.DeleteAllSubstitutionVariables();
			EditorUtility.SetDirty(evs);
		}
	}

	EventScenario GetSelectedScenario()
	{
		if (Selection.activeObject.GetType() == typeof(EventScenario))
		{
			EventScenario evs = (EventScenario)Selection.activeObject;
			return evs;
		}
		
		return null;
	}


	bool showSubstitutionVars = false;
	
	int addEventType = 0;

	static Rect windowRect = new Rect(450, 30, 200, 40);
	Rect addVarRect = new Rect(450, 40, 200, 40);
	Rect showVarsRect = new Rect(450, 40, 200, 100);
	Texture2D wbackground;
	GUIStyle wh;
	
	static Color backgroundColor = Color.grey;
	void OnGUI ()
	{
		//GUI.skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
		
		if (wbackground == null)
		{
			wbackground = new Texture2D(1,1);
			wbackground.SetPixel(0,0, backgroundColor);
			wbackground.Apply();
			
			wh = new GUIStyle(GUI.skin.GetStyle("Box"));
			wh.normal.background = wbackground;
		}
		
		//GUI.DrawTexture( new Rect(0,0, Screen.width, Screen.height), wbackground);
		GUI.Box( new Rect(0,0, Screen.width, Screen.height), "", wh);
		
		backgroundColor = EditorGUI.ColorField( new Rect(0, Screen.height - 40, 200, 20), "BG Color", backgroundColor);
		if (GUI.Button(new Rect(200, Screen.height - 40, 140, 20), "Reset background"))
		{
			wbackground = null;
			wh = null;
		}
		
		GUILayout.BeginHorizontal();	
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		//GUILayout.Space(-200);
		addEventType = EditorGUILayout.Popup("", addEventType, EventUtility.eventTypes, GUILayout.Width(340));
		
		if (GUILayout.Button("Add Event") && Selection.activeObject.GetType() == typeof(EventScenario))
			EventUtility.AddEvent(addEventType, (EventScenario)Selection.activeObject);
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("Add Substitution Var"))
		{
			EventBase.selected = null;
			AddSubstitutionVariable();
		}
		if (GUILayout.Button("Show Substitution Vars"))
		{
			EventBase.selected = null;
			showSubstitutionVars = true;
		}
			
		GUILayout.Space(5);
		if (GUILayout.Button("Remove All Events"))
			DeleteAllEvents();
		if (GUILayout.Button("Remove Selected Event"))
			DeleteSelectedEvent();
		if (GUILayout.Button("Clear Selected Event Exits"))
			ClearSelectedExits();
		if (GUILayout.Button("Reset Substitution Vars"))
			DeleteAllSubstitionVariables();
		
		
		
		//GUI.FocusWindow(0);
		if (Selection.activeObject != null)
		{
			GUILayout.Label("Active selection: " + Selection.activeObject.name + ", type: " + Selection.activeObject.GetType());
			if (Selection.activeObject.GetType() == typeof(EventScenario))
			{
				if (addSubVar) 
				{
					BeginWindows();
					addVarRect = GUILayout.Window(1, addVarRect, AddSubstitutionVarWindow, "Add Substitution Variable");
					EndWindows();
				}
				if (showSubstitutionVars)
				{
					BeginWindows();
					showVarsRect = GUILayout.Window(2, showVarsRect, ShowSubstitutionVarsWindow, "Substitution Variables");
					EndWindows();
				}
				
				EventScenario evs = (EventScenario)Selection.activeObject;
				GUILayout.Label("Event size: " + evs.EventCount());
				EventBase[] events = evs.GetEvents();
				if (events != null && events.Length > 0)
				{
					for (int i=0;i<events.Length;i++)
					{
						if (EventBase.dropConnection != Vector2.zero)
						{
							for (int j=0;j<events.Length;j++)
							{
								if (events[j].editorPosition.Contains(EventBase.dropConnection))
								{
									Debug.Log("Found a match: " + events[j].GetNickname());
									EventBase.dropConnection = Vector2.zero;
									EventBase.dropOriginator.AddExitEvent(events[j]);
									EditorUtility.SetDirty(events[j]);
									EditorUtility.SetDirty(events[i]);
								}
							}
						}
						events[i].EditorGUI(i);
						if (events[i].exitEvents != null)
						{
							if (events[i].draggingConnector)
							{
								DrawLine(new Vector2(events[i].connectorRect.x+5, events[i].connectorRect.y+5), Event.current.mousePosition);
								this.Repaint();
							}

							foreach (EventBase exit in events[i].exitEvents)
							{
								Vector2 s = new Vector2(events[i].connectorRect.x+5, events[i].connectorRect.y+5);
								Vector2 d = new Vector2(exit.editorPosition.x, exit.editorPosition.y + exit.editorPosition.height/2);
								
								if (s.y > d.y)
									d.y = exit.editorPosition.y + exit.editorPosition.height;
								else if (s.y < d.y - exit.editorPosition.height/2)
									d.y = exit.editorPosition.y;
								DrawLine(s, d);
							}
						}

						if (events[i].isDirty)
						{
							EditorUtility.SetDirty(events[i]);
							events[i].isDirty = false;
						}
					}
				}
				
			}
		}
			
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		
		GUILayout.BeginVertical();
		if (Selection.activeObject != null)
		{
			EditorGUIUtility.LookLikeInspector();
			GUILayout.Box("Event Inspector");
			
			GUILayout.Label("Active selection: " + Selection.activeObject.name + ", type: " + Selection.activeObject.GetType());
			if (Selection.activeObject.GetType() == typeof(EventScenario) && EventBase.selected)
			{
				BeginWindows();
				windowRect = GUILayout.Window(0, windowRect, EventInspectorWindow, "Event Inspector");
				EndWindows();
			}
		}
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		
	}
	
	public static void AddNewVar(string nm, System.Type tp)
	{
		EventScenario evs = (EventScenario)Selection.activeObject;
		evs.AddSubstitutionVariable(nm, tp);
		EditorUtility.SetDirty(evs);
	}
	
	void WindowHeader()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(14);
		GUILayout.EndVertical();
	}
	
	string newSubName = "var";
	int newSubType = 0;
	
	
	void AddSubstitutionVarWindow(int id)
    {
        WindowHeader();
        GUILayout.Label("Note that all substitution variables *must* have unique names.");

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Name: ", GUILayout.Width(200));
            newSubName = EditorGUILayout.TextField(newSubName, GUILayout.Width(150));

        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Type: ", GUILayout.Width(200));
            newSubType = EditorGUILayout.Popup(newSubType, VariableTypeConverter.humanTypes, GUILayout.Width(150));
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Cancel"))
        {
            addSubVar = false;
        }
        else if (GUILayout.Button("Save Var"))
        {
            System.Type subType = VariableTypeConverter.HumanToSystemType(VariableTypeConverter.humanTypes[newSubType]);
            Debug.Log("Looked up: '" + subType + "'");

            AddNewVar(newSubName, subType);
            addSubVar = false;
        }
        GUILayout.EndHorizontal();

        GUI.DragWindow(new Rect(0, 0, 10000, 20));
    }
	
	void ShowSubstitutionVarsWindow (int id )
	{
		WindowHeader();
		
		EventScenario evs = GetSelectedScenario();
		
		GUILayout.Label("Substitution vars: " + evs.subNames.Length + "(" + evs.subTypes.Length + ")");
		foreach(string nm in evs.subNames)
		{
			GUILayout.BeginHorizontal();
			System.Type subType = evs.GetSubstitutionVariableType(nm);

            if (subType == null)
            {
                //Debug.Log("nm=" + nm + " is NULL");
                subType = VariableTypeConverter.HumanToSystemType(nm);
            }

			//System.Type subType = VariableTypeConverter.HumanToSystemType(nm);
			if (subType == null)
			{
				GUILayout.Label(String.Format("{0}: {1}", nm, "None (check EventScenario.ConvertToType())"));
			}
			else
			{
				GUILayout.Label(String.Format("{0}: {1}", nm, VariableTypeConverter.SystemNameToHuman(subType.ToString())));
			}
			if(GUILayout.Button("x", GUILayout.Width(40)))
				evs.DeleteSubstitutionVariable(nm);
			GUILayout.EndHorizontal();
		}
		
		GUILayout.Space(10);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel"))
		{
			showSubstitutionVars = false;
		}
		else if (GUILayout.Button("OK"))
		{
			showSubstitutionVars = false;
		}
		GUILayout.EndHorizontal();
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
	}
	
	void SubstituteFieldForVariable(string eventVarName, string substitutionVarName, EventBase ev)
	{
        Debug.Log("SubstituteFieldForVariable " + eventVarName + " " + substitutionVarName + " " + ev.name);

		EventScenario evs = (EventScenario)Selection.activeObject;
		evs.MapSubstitution(ev, eventVarName, substitutionVarName);
		EditorUtility.SetDirty(evs);
	}
	
	int ChoiceIndex(string[] choices, string match)
	{
		int retIndex = 0;
		
		for (int i=0;i<choices.Length;i++)
		{
			if (choices[i] == match)
				retIndex = i;
		}
		
		return retIndex;
	}
	
	int GetSubstitutionMenuIndex(EventBase theEvent, string key, EventScenario scenario, System.Type fieldType, string fieldName, string[] choices)
	{
		int retIndex = 0;
		//selectionMap.Clear();
		if (!selectionMap.ContainsKey(key) && !FieldHasSubstitution(theEvent, key, scenario, fieldType, choices))
		{
			//Debug.Log("Adding key for selectionmap: " + key);
			selectionMap[key] = 0;
			int currIdx = 0;
			currIdx = 0;
			foreach (SubstitutionMap map in scenario.GetMapsForEvent(theEvent))
			{
				string potentialKey = String.Format("{0}{1}", map.theEvent.name, map.eventVarName);
				if (key == potentialKey && scenario.GetSubstitutionVariableType(map.substitutionName) == fieldType)
				{
					selectionMap[key] = ChoiceIndex(choices, map.substitutionName);
					retIndex = (int)selectionMap[key];
					//Debug.Log("Made a match for key: "+ key + ", subname: " + map.substitutionName + ", idx: " + retIndex);
					break;
				}
				else if (key == potentialKey)
				{
					//Debug.Log("Rejecting match key: " + key + ", subname: " + map.substitutionName + ", idx: " + retIndex + " (potential: " + potentialKey + ") - scenario's sub type: " + scenario.GetSubstitutionVariableType(map.substitutionName) + ", fieldType: " + fieldType);
				}
			
				if(scenario.GetSubstitutionVariableType(map.substitutionName) == fieldType)
				{
					//Debug.Log("Incrementing on " + fieldType + "/" + map.substitutionName);
					currIdx++;
				}
				else
				{
					//Debug.Log("Not incrementing for type " + fieldType);
				}
			}
		}
		else if (!selectionMap.ContainsKey(key) && FieldHasSubstitution(theEvent, key, scenario, fieldType, choices))
		{
			// this is the first setting of menu selections, so we need to set the menu index to the choice that this
			// substitution var is currently set to
			selectionMap[key] = 0;
			retIndex = 0;
			
			for(int currIdx=0;currIdx<choices.Length;currIdx++)
			{
				foreach (SubstitutionMap map in scenario.GetMapsForEvent(theEvent))
				{
					//Debug.Log("Checking " + map.eventVarName);
					string potentialKey = String.Format("{0}{1}", map.theEvent.name, map.eventVarName);
					if (key == potentialKey && scenario.GetSubstitutionVariableType(map.substitutionName) == fieldType && map.substitutionName == choices[currIdx])
					{
						selectionMap[key] = ChoiceIndex(choices, map.substitutionName);
						//retIndex = (int)selectionMap[key];
						retIndex = currIdx;
						
						//Debug.Log("Made a substituted match for key: "+ key + ", subname: " + map.substitutionName + ", idx: " + retIndex + " potential key: " + potentialKey + ", evVarName: " + map.eventVarName + " evn: " + map.theEvent.name);
					
					}
					else
					{
						// Debug.Log("Rejecting substitute key: " + key +  " (potential: " + potentialKey + ") " + ", subname: " + map.substitutionName + ", idx: " + retIndex +", scenario's sub type: " + scenario.GetSubstitutionVariableType(map.substitutionName) + ", fieldType: " + fieldType);
 						//if (map.substitutionName != choices[currIdx])
						//Debug.Log("Rejected because: map.substitutionName (" + map.substitutionName + ") != choices (" + choices[currIdx] +") idx: " + currIdx);
 						//else
						//Debug.Log("Rejected for unknown reason");
					}
			
					if(scenario.GetSubstitutionVariableType(map.substitutionName) == fieldType)
					{
						//Debug.Log("Incrementing on " + fieldType + "/" + map.substitutionName);
						//currIdx++;
					}
					else
					{
						//Debug.Log("Not incrementing for type " + fieldType);
					}
				}
			}
			
		}
		else if (selectionMap.ContainsKey(key))
		{
			retIndex = (int)selectionMap[key];	
		}
		return retIndex;
	}
	
	bool FieldHasSubstitution (EventBase theEvent, string key, EventScenario scenario, System.Type fieldType, string[] choices)
	{
		foreach (SubstitutionMap map in scenario.GetMapsForEvent(theEvent))
		{
			string potentialKey = String.Format("{0}{1}", map.theEvent.name, map.eventVarName);
			if (key == potentialKey && scenario.GetSubstitutionVariableType(map.substitutionName) == fieldType)
			{
				return true;
			}
		}
		
		return false;
	}
	
	EventBase lastSelectedEvent;
	UnityEngine.Object obj;
	//int[] selections = new int[50];
	int fieldIndex = 0;
	Hashtable selectionMap = new Hashtable();
	
	static Dictionary<string, int> fieldWidths;
	void EventInspectorWindow(int id)
	{
		if (fieldWidths == null)
		{
			fieldWidths = new Dictionary<string, int>();
			fieldWidths.Add("name", 150);
			fieldWidths.Add("sub", 38);
			fieldWidths.Add("pop", 200);
		}
		WindowHeader();
		EventBase sel = EventBase.selected;
	
		
		if (GUI.Button(new Rect(0,0, 20, 20), "X"))
		{
			EventBase.selected = null;
			return;
		}
		
		string name = "No selection";
		if (EventBase.repaint)
			this.Repaint();
		if (sel == null)
		{
			name = "No selection";
			GUILayout.Label("Event: " + name);
			
		}
		else
		{
			bool isDirty = false;
			
			
			GUILayout.BeginHorizontal();
			name = sel.GetNickname();
			GUILayout.Label("Event: '" + name + "'", GUILayout.Width(160));
			
			// TODO figure out why this doesn't always update when you change a name and select a different event?
			name = EditorGUILayout.TextField(name, GUILayout.Width(300));
			if (GUI.changed)
			{
				name = name.Replace("\n","");
				sel.SetNickname(name);
				isDirty = true;
			}

			if (GUILayout.Button("Reset name", GUILayout.Width(100)))
			{
				name = "event";
				sel.SetNickname(name);
				isDirty = true;
				this.Repaint();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			

			GUILayout.BeginHorizontal();
			GUILayout.Label("Notify on exit: " + (sel.exitEvents == null ? 0 : sel.exitEvents.Length) + " events", GUILayout.Width(160));
			if (GUILayout.Button("Reset exit notifies"))
				sel.DeleteExitEvents();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			Type type = sel.GetType();
			GUILayout.Label("Type: " + type);

			GUILayout.Space(20);
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("Event Var", GUILayout.Width(225));
			GUILayout.Label("Substitution (red = substituted)", GUILayout.Width(200));
			GUILayout.EndHorizontal();

			
			EventScenario evs = (EventScenario)Selection.activeObject;
			
			FieldInfo[] myFieldInfo = sel.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance
			                                      | BindingFlags.Public);
			
			fieldIndex = 0;

			// TODO This is where all types should be mapped for the inspector window
			foreach (FieldInfo f in myFieldInfo)
			{
				string key = String.Format("{0}{1}", sel.name, f.Name);
				
				GUILayout.BeginVertical();
				if (FieldHasSubstitution(sel, key, evs, f.FieldType, evs.SubstitutionNamesByType(f.FieldType.ToString())))
				{
					GUI.color = Color.red;
				}
				else
				{
					GUI.color = Color.grey;
				}

                GUILayout.BeginHorizontal(GUILayout.Height(30));

                GUILayout.Label(f.Name + ": ", GUILayout.Width(fieldWidths["name"]));

				
				if (f.FieldType.ToString() == "System.Single")
				{
					type.GetField(f.Name).SetValue(sel, EditorGUILayout.FloatField((float)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-192);
				}
				else if (f.FieldType.ToString() == "System.Int32")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.IntField((int)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-192);
				}
				else if (f.FieldType.ToString() == "System.Boolean")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.Toggle((bool)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-192);
				}
				else if (f.FieldType.ToString() == "System.String")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.TextField((string)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-190);
				}
				else if (f.FieldType.ToString() == "UnityEngine.Vector2")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.Vector2Field(f.Name, (Vector2)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-106);

				}
				else if (f.FieldType.ToString() == "UnityEngine.Vector3")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.Vector3Field(f.Name, (Vector3)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-190);
				}
				else if (f.FieldType.ToString() == "UnityEngine.Rect")
				{
                    type.GetField(f.Name).SetValue(sel, EditorGUILayout.RectField(f.Name, (Rect)type.GetField(f.Name).GetValue(sel), GUILayout.Width(200)));
					//GUILayout.Space(-190);
				}
				else  // this is the "catch-all" for all other types (UnityEngine.GameObject, WaypointGroup, etc)
				{
                    GUILayout.Space(12);
                    GUILayout.Label(f.Name, GUILayout.Width(fieldWidths["name"]), GUILayout.Width(184));
					//GUILayout.Space(-115);
				}

				// this should check to see that the selected index is of a valid type and return it if so.  If not, return the index of the first valid?
				int idx = GetSubstitutionMenuIndex(sel, key, evs, f.FieldType, f.Name, evs.SubstitutionNamesByType(f.FieldType.ToString()));
				selectionMap[key] = EditorGUILayout.Popup("", idx, evs.SubstitutionNamesByType(f.FieldType.ToString()), GUILayout.Width(fieldWidths["pop"]));
				if ((int)selectionMap[key] != idx)
				{
					//selectionMap.Clear();
					//Debug.Log("Attempt to change selection to " + (int)selectionMap[key] + " for " + f.Name);
				}
					
				
				if (GUILayout.Button("Sub", GUILayout.Width(fieldWidths["sub"])))
				{
					//SubstituteFieldForVariable(f.Name, evs.SubstitutionNamesByType(f.FieldType.ToString())[selections[fieldIndex]], sel);
					SubstituteFieldForVariable(f.Name, evs.SubstitutionNamesByType(f.FieldType.ToString())[(int)selectionMap[key]], sel);
				}
				
				GUILayout.Space(20);
				
				string ftype = f.FieldType.ToString();
				if (ftype.Contains("."))
					ftype = ftype.Substring( ftype.LastIndexOf(".")+1 );
				GUILayout.Label(ftype);

				// if (GUILayout.Button("Tell", GUILayout.Width(fieldWidths["sub"])))
				// {
				// Debug.Log(selectionMap[key] + " index (" + idx + ") for " + key);
				// foreach(string k in selectionMap.Keys)
				// {
				// Debug.Log("Key: " + k);
				// }
				// }
					
				GUILayout.Space(50);
				
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
				fieldIndex++;
			}
			GUI.color = Color.white;

			if (GUILayout.Button("Clear Cache"))
				selectionMap.Clear();

			if (isDirty || GUI.changed)
				EditorUtility.SetDirty(sel);
			
			if (lastSelectedEvent != sel)
			{
				this.Repaint();
				selectionMap.Clear();
			}
			
			lastSelectedEvent = sel;
		}
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
	}
	
	void OnCloseWindow ()
	{
		windowCount--;
	}
}
