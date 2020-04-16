using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class NPCRival : NPCController
{
#if Crypto
    //private string TO_CHALLENGE_TEXT = "Do you want to challenge ";

    //private string interviewButtonText = "Challenge ";

    private List<Vector3> clickTargets = new List<Vector3>();
    public List<Vector3> ClickTargets
    {
        get
        {
            return clickTargets;
        }
        set
        {
            clickTargets = value;
        }
    }

    private bool showChallengeMsg = false;
 
    protected override void Awake()
    {
    	pageName = "NPCRival";
    	createcharacter = false;
    	base.Awake();
    }
    
    protected void MoveTo(Vector3 target)
    {
        //Vector3 vecTo = target - Actor.transform.position;
        //float angle = Vector3.Angle(Actor.transform.TransformDirection(Vector3.forward), vecTo);

        //float rightAngle = Vector3.Angle(Actor.transform.TransformDirection(Vector3.right), vecTo);
        //float leftAngle = Vector3.Angle(Actor.transform.TransformDirection(-Vector3.right), vecTo);

        Actor.MoveForward(forwardMoveSpeed, false);

        Actor.GetTransform().LookAt(target);

        /*
        if (Mathf.Abs(angle) > 2)
        {
            if (rightAngle < leftAngle)
            {
                Actor.TurnRight(rightAngle, target);
            }
            else if (rightAngle > leftAngle)
            {
                Actor.TurnLeft(leftAngle, target);
            }
        }
         */
    }

    private void Update()
    {
        if (clickTargets.Count > 0)
        {
            if (clickTargets[0] != Vector3.zero && (transform.position - clickTargets[0]).magnitude > 1)
            {
                MoveTo(clickTargets[0]);
            }
            else
                clickTargets.RemoveAt(0);
        }
        else if (!IsLocked)
        {
            Actor.Idle();
        }
    }



    private void OnGUI()
    {
        if (IsLocked)
            return;

        if (MovementController.ControlTarget != null)
        {
            Vector2 screenPos =
                GUIUtility.ScreenToGUIPoint(Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2));

            float dist = (transform.position - MovementController.ControlTarget.GetTransform().position).magnitude;

            if (dist < maxTalkDistance && !TicTacNet.Main.TicTacId.HasValue && !TicTacNet.Main.IsSendingChallenge && TicTacManager.Main.enabled)
            {
            	string text = GetText("interviewButtonText")+" " + name.Split(':')[1];

                Vector2 buttonSize = GUI.skin.GetStyle(GUIManager.NPCNameButtonStyle).CalcSize(new GUIContent(text));
                Rect buttonRect = new Rect(screenPos.x - buttonSize.x / 2,
                        Screen.height - screenPos.y - buttonSize.y, buttonSize.x, buttonSize.y);

                if (MessageGUI.IsClosed)
                {
                    if (GUI.Button(buttonRect, text, GUIManager.NPCNameButtonStyle))
                    {
                        showChallengeMsg = true;
                        MovementController.Main.LockPCControl();
                    }
                }
            }
        }

        if (showChallengeMsg)
        {
            //string result = MessageGUI.ShowDialog(TO_CHALLENGE_TEXT + name.Split(':')[1], "Yes", "No");
            ModalController.Main.AddDialog(GetText("TO_CHALLENGE_TEXT") + name.Split(':')[1], GetText("Yes_Btn"), GetText("No_Btn"), ModalProperties.Alignment.MiddleCenter, false, true, DialogResult);
            /*if (result != null)
            {              
                if (result.Equals("Yes"))
                    StartCoroutine(TicTacNet.Main.SendChallenge(TicTacNet.GameId, TicTacNet.playerName, name.Split(':')[1]));

                MovementController.Main.UnlockPCControl(true);
                showChallengeMsg = false;
            }*/
        }
    }
    
    void DialogResult(string res, bool accepted)
    {
        if (accepted)
        {
        	if (res.Equals(GetText("Yes_Btn")))
            	StartCoroutine(TicTacNet.Main.SendChallenge(TicTacNet.GameId, TicTacNet.playerName, name.Split(':')[1]));
        }

        MovementController.Main.UnlockPCControl(true);
        showChallengeMsg = false;
    }
    
    public void CreateCharacterByName(string player)
    {
		StartCoroutine(CharacterByConfig(player));
    }
        
    public IEnumerator CharacterByConfig(string player)
    {
		while (CryptoNet.ConfigByName[player] == null)
            yield return 0;

        while (!CharacterGenerator.ReadyToUse)
            yield return 0;

        string config = CryptoNet.ConfigByName[player].ToString();
		Debug.Log("config "+player+" "+CryptoPlayerManager.KeyToJoin+" "+config);
		
    	if(character != null )
    	{
    		//GameObject.Destroy(character);
    		//GameObject.DestroyObject(character);
    		Destroy(character);
    	}
    	
		CharacterGenerator generator = CharacterGenerator.CreateWithConfig(config);
        // Wait for the assets to be downloaded
        while (!generator.ConfigReady)
            yield return 0;

        // Create the character.
        character = generator.Generate();
        
        character.transform.parent = transform;
        character.transform.localPosition = Vector3.zero;
        character.transform.localRotation = Quaternion.identity;
        CapsuleCollider boxCollider = gameObject.AddComponent<CapsuleCollider>();
        
        //boxCollider.size = colliderSize;
        boxCollider.center = colliderCenter;

        //lod = new LOD(gameObject);
        //InvokeRepeating("CheckLOD", 0, 0.15f);

        if (!gameObject.rigidbody)
            gameObject.AddComponent(typeof(Rigidbody));
        //rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.freezeRotation = true;
    }
#endif
}