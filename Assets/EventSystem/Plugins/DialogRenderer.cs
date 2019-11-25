using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void DialogFinished();

public class DialogRenderer : MonoBehaviour
{
	static DialogRenderer main;
	static NPCController dialogNPC;
	public static NPCController DialogNPC
	{
		get
		{
			return dialogNPC;
		}
		set
		{
			dialogNPC = value;
			if (dialogNPC != null)
			{
                // Oleg: NPC reconstruction
                //dialogNPC.IsTalking = true;
				//dialogNPC.canTalk = false;
				CameraController.RequestCameraLock("NPC Dialog");
				CapsuleCollider c = (CapsuleCollider)dialogNPC.GetComponent(typeof(CapsuleCollider));
				
				if (c != null)
					CameraController.WatchPoint = dialogNPC.transform.position + Vector3.up * c.height;
				else
					CameraController.WatchPoint = dialogNPC.transform.position + Vector3.up * 1.7f;
			}
			else
			{
				CameraController.ReleaseLock("NPC Dialog");
				CameraController.WatchPoint = Vector3.zero;
			}
		}
	}

	Vector3 npcLookAt = Vector3.zero;
	void LateUpdate ()
	{
		if (dialogNPC != null)
		{
			PCCamera pc = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
            if (pc != null)
            {
                pc.WatchTarget = dialogNPC.transform;
                // Oleg: NPC reconstruction
                /*
                if (dialogNPC.npcIsSeated == false)
                {
                    pc.WatchTargetHeight = 1.4f;
                    //if (npcLookAt == Vector3.zero)
                    //{
                    npcLookAt = Camera.main.transform.position;
                    npcLookAt.y = dialogNPC.transform.position.y;
                    //}

                    dialogNPC.transform.LookAt(npcLookAt);
                    //Camera.main.transform.LookAt(camLookAt);
                }
                else
                {
                    pc.WatchTargetHeight = 0.9f;
                }
                 */
            }
		}
	}
	
	DialogParser dialog;
	
	DialogDescriptor currentDescriptor;
	EventPlayerBase eventPlayerCaller;
	DialogGoal goal;
	public static void Init (DialogDescriptor desc, EventPlayerBase eventPlayer)
	{
		//Debug.Log("Dialog Renderer triggered by " + eventPlayer.gameObject.name);
		if (main != null)
		{
			Debug.Log("Main is not null, cleaning up first");
			Finish();
		}
		if (main == null)
		{
			//Debug.Log("Main is null, creating new dialog renderer paraphernalia");
			GameObject go = new GameObject();
			go.name = "Dialog Renderer";
			main = (DialogRenderer)go.AddComponent(typeof(DialogRenderer));
			main.currentDescriptor = desc;
			main.dialog = new DialogParser(desc.dialogXML.text);
			main.eventPlayerCaller = eventPlayer;
			main.goal = (DialogGoal)eventPlayer.GetComponent(typeof(DialogGoal));
			if (main.goal == null) Debug.Log("No dialog goal on init of " + eventPlayer.gameObject.name);
			else Debug.Log("Dialog goal: " + main.goal.goalTargetName + " on init of " + eventPlayer.gameObject.name);
			answered.Clear();
		}
		if (main.currentDescriptor.linearDialog)
			main.currentSequenceLevel = main.dialog.FirstSequence();
		else
			main.currentSequenceLevel = 0;
	}
	
	static Dictionary<EventPlayerBase, DialogFinished> callbackMap = new Dictionary<EventPlayerBase, DialogFinished>();
	public static void AddCallbackForEventPlayer( EventPlayerBase player, DialogFinished callback)
	{
		if (!callbackMap.ContainsKey(player))
			callbackMap.Add(player, callback);
		callbackMap[player] += callback;
	}
	
	Vector2 dialogWH = new Vector2(400, 200);
	string currentQuestion = "";

	int currentSequenceLevel = 0;
	static Dictionary<string, bool> answered = new Dictionary<string, bool>();
    GUIStyle dialogAnswered = null;    

	void OnGUI ()
	{
		if (main == null) return;
		GUISkin skin = GUIManager.Skin;
		
		GUIStyle footer = skin.GetStyle("footer");
		GUIStyle dialogChoice = skin.GetStyle("Dialog Choice");
		if ( dialogAnswered == null )
		{	
			dialogAnswered = new GUIStyle(skin.GetStyle("Dialog Choice"));
			dialogAnswered.normal = dialogAnswered.focused;
		}
		GUIStyle arrowStyle = skin.GetStyle("Arrow");
		GUIStyle noArrow = new GUIStyle(arrowStyle);
		noArrow.normal.background = null;

		int numQuestions = currentDescriptor.linearDialog ? dialog.CountQuestionsForSequenceLevel(currentSequenceLevel) : dialog.Questions.Length;
		int footerHeight = Mathf.Max(((int)dialogChoice.CalcSize(new GUIContent("1")).y) * (numQuestions+2) + footer.padding.bottom, (int)footer.fixedHeight);
		
		Rect footerRect = new Rect(0, Screen.height - footerHeight, Screen.width, footerHeight);
		GUI.Box(footerRect, "", footer);
		if (currentQuestion != "")
		{
			string answer = dialog.AnswerTo(currentQuestion);
			GUIManager.RenderAnswer(answer, Screen.height - footerHeight);
		}
        else
        {
            if (dialogNPC != null)
            {
                // Oleg: NPC reconstruction
                //GUIManager.RenderAnswer(dialogNPC.greetingText, Screen.height - footerHeight);
            }
        }
		

		GUILayout.BeginArea(footerRect);
		if (dialogNPC != null)
		{
			GUIStyle talkTo = new GUIStyle(dialogChoice);
			string tstring = "Talk to " + dialogNPC.npcName;
			talkTo.normal.textColor = Color.white;
			Vector2 talkToSize = talkTo.CalcSize(new GUIContent(tstring));
			GUILayout.BeginHorizontal();
			GUILayout.Space(Screen.width/2 - talkToSize.x/2);
			GUILayout.Label(tstring, talkTo);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.EndArea();
			footerRect.y += talkToSize.y;
			footerRect.height -= talkToSize.y;
			GUILayout.BeginArea(footerRect);
		}
		

		string noMoreQuestions = currentDescriptor.noMoreQuestions;
		GUILayout.BeginHorizontal();
		//string longestQuestion = (noMoreQuestions.Length > dialog.LongestQuestionLength()) ? noMoreQuestions : dialog.LongestQuestion();
		
		//int questionOffset = Screen.width/2 - (int)dialogChoice.CalcSize(new GUIContent(longestQuestion)).x/2;
		//GUILayout.Space(questionOffset);
		GUILayout.BeginVertical();
		Vector2 arrowWH = new Vector2(arrowStyle.normal.background.width, arrowStyle.normal.background.height);
		
		foreach (string question in dialog.Questions)
		{
			if (!currentDescriptor.linearDialog || (currentDescriptor.linearDialog && dialog.SequenceLevelFor(question) <= currentSequenceLevel))
			{
				GUILayout.BeginHorizontal();
				GUIStyle dialogChoiceStyle = dialogChoice;

                Vector2 talkToSize = dialogChoiceStyle.CalcSize(new GUIContent(question));

                GUILayout.Space(Screen.width / 2 - talkToSize.x / 2 - 20);


				if (DialogManager.QuestionWasAsked(currentDescriptor.name, question))
				{
					dialogChoiceStyle = dialogAnswered;
					GUILayout.Box("", noArrow, GUILayout.Width(arrowWH.x), GUILayout.Height(arrowWH.y));
				}
				else
				{
					dialogChoiceStyle = dialogChoice;
					GUILayout.Box("", arrowStyle, GUILayout.Width(arrowWH.x), GUILayout.Height(arrowWH.y));
				}

				if (GUILayout.Button(question, dialogChoiceStyle))
				{
					if (dialogNPC != null && currentDescriptor.animations != null && currentDescriptor.animations.Length > 0)
					{
						int animIndex = dialog.AnimationIndexFor(question);
						if (animIndex >= 0 && animIndex < currentDescriptor.animations.Length)
						{
							AnimationClip animClip = currentDescriptor.animations[animIndex];

							if (!dialog.AnimationShouldLoopFor(question))
							{
                                // Oleg: NPC reconstruction
								//dialogNPC.PlayAnimationOnce(animClip);
							}
							else
							{
                                // Oleg: NPC reconstruction
								//dialogNPC.PlayAnimationLooping(animClip);
							}
							
						}
						else
						{
							if (animIndex < 0)
								Debug.Log("[info] No animation specified for question: " + question);
							else if (animIndex >= currentDescriptor.animations.Length)
								Debug.LogWarning("An animation was specified for this question, but the index (" + animIndex +
												 ") was higher than the largest index for " + currentDescriptor.animations.Length +
												 " animations.  Reminder: in this case, the last animation index would be " + (currentDescriptor.animations.Length-1) +
												 ", while the first would be 0");
						}
					}
					currentQuestion = question;

					Debug.Log("Player asked question: " + question + ".  Answer is: " + dialog.AnswerTo(question));
					if (!answered.ContainsKey(question))
					{
						DialogManager.RegisterUsedDialog( currentDescriptor.name, question );
						currentSequenceLevel++;
						answered.Add(question, true);
					}
					else
						answered[question] = true;
					
					
					if (dialog.StartsFlashback(question))
						Finish();
					GUIManager.PlayButtonSound();
				}
                GUILayout.FlexibleSpace();

				GUILayout.EndHorizontal();
			}
		}

        Vector2 noMoreQuestionSize = dialogChoice.CalcSize(new GUIContent(noMoreQuestions));

		GUILayout.BeginHorizontal();
        GUILayout.Space(Screen.width / 2 - noMoreQuestionSize.x / 2 - arrowWH.x);
		GUILayout.Box("", noArrow, GUILayout.Width(arrowWH.x), GUILayout.Height(arrowWH.y));

		if (!currentDescriptor.mustFinishAllQuestions || ((dialog != null) && answered.Count == dialog.Questions.Length))
		{
			if (GUILayout.Button(noMoreQuestions, dialogChoice))
			{
				Finish();
				GUIManager.PlayButtonSound();
			}
		}
        GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}

	static bool AllQuestionsAnswered
	{
		get
		{
			foreach (string question in main.dialog.Questions)
			{
				if (!answered.ContainsKey(question))
					return false;
			}
			
			return true;
		}
		set
		{
		}
	}
	
	public static void Finish ()
	{
		if (main != null)
		{
			if (callbackMap.ContainsKey(main.eventPlayerCaller))
			{
				callbackMap[main.eventPlayerCaller]();
				callbackMap[main.eventPlayerCaller] = null;
			}

			if (dialogNPC)
			{
                dialogNPC.IsTalking = false;
                //dialogNPC.RestoreCameraRotation();

                
				if (main.currentDescriptor.dialogAvailableOnce && AllQuestionsAnswered)
					dialogNPC.canTalk = false;
				else
					dialogNPC.canTalk = true;
                
				dialogNPC.DialogFinished();
                 
				CameraController.ReleaseLock("NPC Dialog");
				dialogNPC = null;
			}
			else
			{
				Debug.Log("[info] Dialog NPC was null on dialog finish.  " +
						  "This probably means the player didn't click 'talk to'" +
						  " and there was no NPC forced by the event player.");
			}

			main.dialog = null;
			main.goal = null;
			PCCamera cam = (PCCamera)Camera.main.GetComponent(typeof(PCCamera));
			cam.enabled = true;
			cam.ThirdPersonView();
			DialogRenderer.DialogNPC = null;
			
			Destroy(main.gameObject);
			main = null;
			answered.Clear();
		}
	}

	public static bool IsFinished (DialogDescriptor desc)
	{
		if (main && main.currentDescriptor == desc)
			return false;
		
		return true;
	}
	
	public static bool IsDialoging ()
	{
		return main == null ? false : true;
	}
}
