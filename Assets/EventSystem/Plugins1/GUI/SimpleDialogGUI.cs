using UnityEngine;
using System.Collections;

public class SimpleDialogGUI : MonoBehaviour
{
	public bool debugTest = false;
	public enum Languages
	{
		English,
		Spanish
	}
	public Languages debugLanguage;
	
	public string dialogText = "We've just learned that extra electricity produced by our new system can be sold at the rate of $50 per unit.  This means that we can earn some money by selling electricity we're not using.";
	
	public string buttonText = "Continue";

	Rect dialogRect = new Rect(200, 400, 200, 100);
	Rect buttonRect = new Rect(300, 500, 100, 20);	
	public GUISkin skin;
	public string boxStyle = "Box";
	public string buttonStyle = "Button";

    void Awake()
    {
        if (debugTest)
            enabled = true;
        else
            enabled = false;

        SimpleDialogRunner.enableCallback = EnableGUI;
    }
	
	void Reposition (float x, float y)
	{
		dialogRect.x = x;
		dialogRect.y = y;
		buttonRect.x = x + dialogRect.width/2;
		buttonRect.y = y + dialogRect.height - 5;
	}
	
	SimpleDialogRunner r;
	Transform referenceObject;
	float heightAboveReference;
	Camera activeCamera;
	GUIStyle boxGUIStyle;

    void EnableGUI(SimpleDialogRunner r)
    {
        if (r.ezGUIdialogPrefab)
        {
            InstantiateEzGUI(r.camera ?? Camera.main, r.distanceToCamera, r.dimensions, r.ezGUIdialogPrefab, r.xmlFile, r.forceNPC.npcName);

            if (r.animationClip)
            {
                r.animationClip.wrapMode = r.wrapMode;
                r.forceNPC.Npc.AddClip(r.animationClip, r.animationClip.name);

                if (r.wrapMode == WrapMode.Once)
                {
                    r.forceNPC.Actor.PlayAnimationOnce(r.animationClip.name);
                }
                else
                {
                    if (!r.forceNPC.Npc[r.animationClip.name])
                        r.forceNPC.Npc.AddClip(r.animationClip, r.animationClip.name);
                    
                    r.forceNPC.Npc.CrossFade(r.animationClip.name, .5f);
                }
            }

            foreach (Transform tr in r.forceNPC.transform)
            {
                if (tr.GetComponent<LODController>())
                {
                    foreach (Transform child in tr)
                    {
                        if (child.GetComponent<Animation>())
                        {
                            Common.AddAudioTo(child.gameObject);

                            FaceFXController faceFX = child.gameObject.AddComponent<FaceFXController>();
                            faceFX.ImportXML(r.xmlLipSynch.text);
                            faceFX.audio_clip = r.audioClip;
							r.faceFX = faceFX;
                            StartCoroutine(PlayFaceAnimation(faceFX, r.lipSynchAnimationClipName));
                            break;
                        }
                    }
                    break;
                }
            }

            if (McHenryCharacterMotor.Main)
            {
                McHenryCharacterMotor.Main.Freeze("start simple dialog");

                PCCamera.Main.FocusOnTargetView(r.forceNPC.transform, 2);
            }

            this.r = r;

            StartCoroutine("DialogHider");
        }
        else
        {
            enabled = true;
            this.r = r;

            if (debugLanguage == SimpleDialogGUI.Languages.English)
            {
                dialogText = DialogXMLParser.GetText(r.xmlFile.text, "DialogText", ActiveLanguage.English);
                buttonText = DialogXMLParser.GetText(r.xmlFile.text, "ButtonText", ActiveLanguage.English);
            }
            else if (debugLanguage == SimpleDialogGUI.Languages.Spanish)
            {
                dialogText = DialogXMLParser.GetText(r.xmlFile.text, "DialogText", ActiveLanguage.Spanish);
                buttonText = DialogXMLParser.GetText(r.xmlFile.text, "ButtonText", ActiveLanguage.Spanish);
            }

            boxStyle = DialogXMLParser.GetBoxStyle(r.xmlFile.text);
            buttonStyle = DialogXMLParser.GetButtonStyle(r.xmlFile.text);
            dialogRect = r.dimensions;
            if (r.camera != null)
                activeCamera = r.camera;
            else
                activeCamera = Camera.main;

            referenceObject = r.optionalPositionReference;
            if (referenceObject != null)
            {
                Debug.Log("Repositioning by reference object");
                heightAboveReference = r.heightAboveReference;
                Vector2 sp = activeCamera.WorldToScreenPoint(referenceObject.position) + Vector3.up * heightAboveReference;
                sp.x -= dialogRect.width / 2;
                dialogRect.x = sp.x;
                dialogRect.y = sp.y;
            }
            Reposition(dialogRect.x, dialogRect.y);
        }
    }

    IEnumerator DialogHider()
    {
        while (McHenryGameManager.Main.enabled)
        {
            yield return 0;
        }

        DestroyObject(instantiatedDialog);
    }
	
	void OnGUI ()
	{
		GUI.depth = -1;
		if (skin != null)
			GUI.skin = skin;

		GUI.Box(dialogRect, dialogText, boxStyle);
		if (boxGUIStyle == null)
		{
			Vector2 buttonSize = GUI.skin.GetStyle(buttonStyle).CalcSize(new GUIContent(buttonText));
			boxGUIStyle = GUI.skin.GetStyle(boxStyle);
			buttonRect.x = dialogRect.x + boxGUIStyle.fixedWidth/2 - buttonSize.x/2;
			buttonRect.y = dialogRect.y + dialogRect.height - buttonSize.y/3;
			
			buttonRect.height = buttonSize.y;
		}
		if (GUI.Button(buttonRect, buttonText, buttonStyle))
		{
			enabled = false;
            r.OnExit();
		}
	}

    GameObject instantiatedDialog;

    void InstantiateEzGUI(Camera cam, float distanceToCamera, Rect dimensions, GameObject ezPrefab, TextAsset xmlFile, string name)
    {
        Vector3 pos = cam.transform.position + cam.transform.TransformDirection(Vector3.forward) * distanceToCamera + cam.transform.TransformDirection(Vector3.forward);
        pos.x += dimensions.x;
        pos.y += dimensions.y;

        instantiatedDialog = (GameObject)GameObject.Instantiate(ezPrefab, pos, cam.transform.rotation);

		#if EZ_GUI
        instantiatedDialog.GetComponent<UIButton>().Text =
            DialogXMLParser.GetText(xmlFile.text, "DialogText", ActiveLanguage.English);
		#endif

        Transform trName = instantiatedDialog.transform.Find("NPC name");
        if (trName)
        {
            UIButton uiName = trName.GetComponent<UIButton>();

            if (uiName)
            {
				#if EZ_GUI
                uiName.Text = name;
				#endif
            }
            else
                Debug.LogWarning("UIButton of 'NPC name' is NULL");
        }
        else
            Debug.Log("'NPC name' is missing");

        Transform trButton = instantiatedDialog.transform.Find("Button");
        if (trButton)
        {
            UIButton btnOK = trButton.GetComponent<UIButton>();

#if EZ_GUI
            if (btnOK != null)
            {
                btnOK.scriptWithMethodToInvoke = this;
                btnOK.methodToInvoke = "ButtonClick";

                SpriteText spriteText = btnOK.GetComponentInChildren<SpriteText>();

                if (spriteText)
                {
                    spriteText.Text = DialogXMLParser.GetText(xmlFile.text, "ButtonText", ActiveLanguage.English);
                }
                else
                    Debug.LogWarning("sprite text for this button is missing");
            }
            else
                Debug.LogWarning("OK button is NULL");
			#endif
        }
        else
            Debug.Log("'Button' is missing");
    }

    IEnumerator PlayFaceAnimation(FaceFXController faceFX, string animationClipName)
    {
        while (!faceFX.IsReady)
        {
            yield return 0;
        }

        if (!animationClipName.StartsWith("Default_"))
            animationClipName = "Default_" + animationClipName;

        AnimationState animState = faceFX.GetComponent<Animation>()[animationClipName];
        if (animState != null)
        {
            animState.layer = 1;
            animState.wrapMode = WrapMode.ClampForever;
            animState.blendMode = AnimationBlendMode.Blend;
        }
        else
        {
            Debug.LogError("animState is NULL for " + animationClipName);
        }

        StartCoroutine(faceFX.PlayAnimCoroutine(animationClipName, faceFX.audio_clip));
    }

    void ButtonClick()
    {
        r.faceFX.StopAnim();
        bool unlock = r.eBase.EventPlayer.playEventOnExit == null || 
            ((EventPlayer)r.eBase.EventPlayer.playEventOnExit).scenario != r.eBase.EventPlayer.scenario;

        if (unlock)
        {
            if (McHenryCharacterMotor.Main)
            {
                McHenryCharacterMotor.Main.Resume("stop simple dialog");
                PCCamera.Main.DefocusView();
                PCCamera.Main.WatchTarget = null;
            }
        }
        DestroyObject(instantiatedDialog);
        StartCoroutine(FinishEventPlayerCor());
    }

    IEnumerator FinishEventPlayerCor()
    {
        while (r.forceNPC.Npc.GetComponent<AudioSource>().isPlaying)
            yield return 0;

        r.OnExit();
    }
}
