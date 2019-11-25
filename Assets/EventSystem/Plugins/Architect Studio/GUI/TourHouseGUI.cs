using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class TourHouseGUI : MonoBehaviour
{
    #region Public vars

    public GUISkin buttonSkin,
                   hintSkin,
                   labelSkin;

    public Camera playerCamera,
                  cam3D;
    public GameObject fpsController;

    public float shareInterfaceWidth = 400,
                 shareInterfaceHeight = 300;

    public float  inviteInterfaceWidth = 600,
                  inviteInterfaceHeight = 400;

    public float  processingInterfaceWidth = 300,
                  processingInterfaceHeight = 200;

    public string errorHeaderText = "Error";
    public string errorSaveMessageText = "Couldn't save your design. Server return error";
    public string pleaseWaitSavingText = "Please wait while we save your design";
    public string pleaseWaitSendingText = "Please wait while we senging invitations";

    public string emptyFromNameText = "Please enter your name",
                   emptyFromEmailText = "Please enter your email",
                   invalidYourEmailText = "Your email is invalid",
                   invalidEmailText = "{0} is invalid email",
                   atLeastOneText = "Please enter at least one email to send";

    public string okText = "OK",
                  sumbitText = "SUBMIT",
                  backText = "GO BACK",
                  submitToGalleryText = "SUBMIT TO GALLERY",
                  sendMailText = "SEND EMAIL INVITATION",
                  shareText = "SHARE",
                  returnText = "RETURN TO DESIGN";

    public string inviteHeaderText = "Invite others to tour your house",
                  inviteDescriptionText = "Send an email message inviting your friends and family to your house. They will receive a link to a private page with your 3D house model.",
                  secureText = "We will not keep your name or email - both will be deleted immediately after the email is sent. (Having the message from come from you will help prevent spam filters from catching it.)",
                  submitHeaderText = "Submit your house to the Design Gallery",
                  submitDescriptionText = "Let the world tour your 3D house design. They can also rate how well it met the client needs and the particulars of the site!",
                  shareHeaderText = "Share your house design with others",
                  shareDescriptionText = "You can submit your design to Architect Studio 3D Design Gallery to share with the world, or you can send a private email invitation to your friends and family.",
                  whichWouldyouText = "Which whould you like to do?";

    public string boldHintAboutMoving = "Use your mouse and arrow keys to move around. You can walk through walls to enter your house. Shift-key moves you faster.";

    public string fromText = "from:",
                  emailText = "email:";

    public string typeNameHereText = "Type your name here",
                  typeYourEmailHereText = "Type your email adress here",
                  typeEmailHereText = "Type email adress here";

    public string byText = "by:",
                  ageText = "age:",
                  stateText = "state:";

    public GUIContent[] switcherRoof = new GUIContent[2] { new GUIContent("ROOF ON"), new GUIContent("ROOF OFF") };

    public int snapShotWidth = 210,
               snapShotHeight = 146;

    public float  saveWindowWidth = 500,
                  saveWindowHeight = 400;

    public string subjectEmailText = "Invitation from AD3D";
    public string emailBodyText = "Hello. You can watch my design <a href=\"http://architectstudio3d.org/designstudio/20100706_gallery.html\">here<\a>. Enjoy.";

    public Vector2 screenShotCameraOffset;
    public int printing3DViewWidth = 500,
               printing3DViewHeight = 400; 

    #endregion

    #region Private vars

    private SaveAS3DManager oSaveAS3DManager = null;
    private InviteManager oInviteManager = null;

    private enum TourStates
    {
        tour,
        share,
        submit,
        invite,
        saving,
        error,
        sending
    }
    private TourStates tourState = TourStates.tour;

    private Rect shareInterfaceRect,
                 saveRect,
                 view3DRect,
                 inviteInterfaceRect,
                 processingRect;

    private GUIStyle buttonStyle,
                     switcherLeft,
                     switcherLeftActive,
                     switcherRight,
                     switcherRightActive,
                     view3DStyle,
                     boldHintStyle,
                     textBoxStyle,
                     textBoxHintStyle,
                     headerStyle,
                     centeredHeaderStyle,
                     beginStyle,
                     labelStyle,
                     needsBoxStyle,
                     textAreaStyle,
                     redHintStyle;

    private static CombineChildrenExtended[] combiners;

    private string by = string.Empty,
                   fromName = string.Empty,
                   fromEmail = string.Empty,
                   age = "0",
                   state = string.Empty;

    private string[] toEmails = { string.Empty, string.Empty, string.Empty, string.Empty };

    private bool[] isToEmailsFocused = { false, false, false, false };

    private bool isFromNameFocused = false,
                 isFromEmailFocused = false;

    private Info UserInfo;

    private string errorText = string.Empty;
    private TourStates backStateAfterError = TourStates.tour;

    #endregion

    private void Awake()
    {
        ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.TourHouse, GetComponent<TourHouseGUI>());

        shareInterfaceRect = new Rect(Screen.width / 2 - shareInterfaceWidth / 2 + 100,
            Screen.height / 2 - shareInterfaceHeight / 2, shareInterfaceWidth, shareInterfaceHeight);

        inviteInterfaceRect = new Rect(Screen.width / 2 - inviteInterfaceWidth / 2 + 100,
            Screen.height / 2 - inviteInterfaceHeight / 2, inviteInterfaceWidth, inviteInterfaceHeight);

        processingRect = new Rect(Screen.width / 2 - processingInterfaceWidth / 2 + 100,
            Screen.height / 2 - processingInterfaceHeight / 2, processingInterfaceWidth, processingInterfaceHeight);

        oSaveAS3DManager = (SaveAS3DManager)GameObject.FindObjectOfType(typeof(SaveAS3DManager));
        oInviteManager = (InviteManager)GameObject.FindObjectOfType(typeof(InviteManager));

        SetGUIStyles();

        combiners = (CombineChildrenExtended[])GameObject.FindObjectsOfType(typeof(CombineChildrenExtended));

        saveRect = new Rect(Screen.width / 2 - saveWindowWidth / 2 + 100, Screen.height / 2 - saveWindowHeight / 2, saveWindowWidth, saveWindowHeight);
    }

    private void OnEnable()
    {
        if (BuildingManager.Picked == null || !playerCamera)
            return;
		
        tourState = TourStates.tour;
		if (cam3D)
			cam3D.gameObject.SetActiveRecursively(true);

        FPSControllerActovator(true);

        if (LoadAS3DManager.oGameData != null &&
                LoadAS3DManager.oGameData.oInfo != null)
        {
            by = LoadAS3DManager.oGameData.oInfo.name;
            state = LoadAS3DManager.oGameData.oInfo.state;
            age = LoadAS3DManager.oGameData.oInfo.age.ToString();
        }

        Combine();
    }

    private void OnDisable()
    {
        FPSControllerActovator(false);
		if (cam3D)
			cam3D.gameObject.SetActiveRecursively(false);
        Decombine();
    }

    private void SetGUIStyles()
    {
        buttonStyle = buttonSkin.GetStyle("button");
        labelStyle = buttonSkin.GetStyle("label");

        switcherLeft = hintSkin.GetStyle("switcherLeft");
        switcherLeftActive = new GUIStyle(switcherLeft);
        switcherLeftActive.normal = switcherLeft.active;

        switcherRight = hintSkin.GetStyle("switcherRight");
        switcherRightActive = new GUIStyle(switcherRight);
        switcherRightActive.normal = switcherRight.active;

        boldHintStyle = labelSkin.GetStyle("boldHint");

        textBoxStyle = buttonSkin.GetStyle("textfield");
        textBoxHintStyle = new GUIStyle(textBoxStyle);
        textBoxHintStyle.normal.textColor = Color.grey;
        textBoxHintStyle.focused = textBoxHintStyle.normal;
        textBoxHintStyle.active = textBoxHintStyle.normal;

        view3DStyle = hintSkin.GetStyle("view3d");

        headerStyle = hintSkin.GetStyle("label");

        beginStyle = hintSkin.GetStyle("beginText");

        needsBoxStyle = hintSkin.GetStyle("needsbox");

        centeredHeaderStyle = new GUIStyle(headerStyle);
        centeredHeaderStyle.alignment = TextAnchor.MiddleCenter;

        textAreaStyle = hintSkin.GetStyle("textarea");
        redHintStyle = labelSkin.GetStyle("redHint");
    }

    private void FPSControllerActovator(bool on)
    {
        if (!fpsController)
            return;

        if (on)
        {
            fpsController.active = true;
            foreach (Transform tr in fpsController.transform)
                tr.gameObject.active = true;

            if (cam3D)
                cam3D.gameObject.SetActiveRecursively(true);
        }
        else
        {
            foreach (Transform tr in fpsController.transform)
                tr.gameObject.active = false;

            fpsController.active = false;

            if (cam3D)
                cam3D.gameObject.SetActiveRecursively(false);
        }
    }

    public static void Combine()
    {
        if (BuildingManager.Picked != null)
            BuildingManager.Picked.Combiner.Combine();

        if (combiners != null)
        {
            foreach (CombineChildrenExtended combiner in combiners)
            {
                combiner.Combine();
            }
        }
    }

    public static void Decombine()
    {
        BuildingManager.Picked.Combiner.Decombine();
        if (combiners != null)
        {
            foreach (CombineChildrenExtended combiner in combiners)
            {
                combiner.Decombine();
            }
        }
    }

    private void OnGUI()
    {
        GUI.depth = 1;

        if (BuildingManager.Picked == null || !playerCamera)
            return;

        switch (tourState)
        {
            case TourStates.share:
                FPSControllerActovator(false);
                ShowShare();
                break;
            case TourStates.submit:
                FPSControllerActovator(false);
                ShowSubmit();
                break;
            case TourStates.invite:
                FPSControllerActovator(false);
                ShowInvite();
                break;
            case TourStates.error:
                FPSControllerActovator(false);
                ShowError(errorText);
                break;
            case TourStates.saving:
                FPSControllerActovator(false);
                ShowProcessing(pleaseWaitSavingText);
                break;
            case TourStates.sending:
                FPSControllerActovator(false);
                ShowProcessing(pleaseWaitSendingText);
                break;
            default:
                FPSControllerActovator(true);
                DrawRoofButtons();
                break;
        }
    }

    private void ShowError(string p_errorText)
    {
        Vector2 headerSize = centeredHeaderStyle.CalcSize(new GUIContent(errorHeaderText));
        Vector2 detailsSize = beginStyle.CalcSize(new GUIContent(p_errorText));
        Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(okText));

        saveRect.height = headerSize.y + detailsSize.y + buttonSize.y + 80;
        saveRect.y = Screen.height / 2 - saveRect.height / 2;

        GUI.Box(saveRect, GUIContent.none, needsBoxStyle);
        GUI.BeginGroup(saveRect);

        Rect errorHeaderRect = new Rect(0, 20, saveRect.width, headerSize.y);
        GUI.Label(errorHeaderRect, errorHeaderText, centeredHeaderStyle);

        Rect detailsRect = new Rect();
        detailsRect.width = saveRect.width;
        detailsRect.y = errorHeaderRect.y + errorHeaderRect.height + 10;
        detailsRect.height = detailsSize.y;

        GUI.Label(detailsRect, p_errorText, beginStyle);

        Rect buttonRect = new Rect(saveRect.width / 2 - 50, saveRect.height - buttonSize.y - 30, 100, buttonSize.y);
        if (GUI.Button(buttonRect, okText, buttonStyle))
        {
            tourState = backStateAfterError;
        }
        
        GUI.EndGroup();        
    }

    private void ShowProcessing(string p_text)
    {
        GUI.Box(processingRect, GUIContent.none, needsBoxStyle);

        Rect insideProcessingRect = new Rect(processingRect.x + processingRect.width / 4, processingRect.y + processingRect.height / 4, processingRect.width / 2, processingRect.height / 1.5f);

        GUILayout.BeginArea(insideProcessingRect);

        GUILayout.Label(p_text, centeredHeaderStyle);

        GUILayout.EndArea();

        if (oSaveAS3DManager.RequestFinished)
            tourState = TourStates.tour;
    }

    private void ShowInvite()
    {
        Vector2 emailSize = labelStyle.CalcSize(new GUIContent(emailText));

        GUI.Box(inviteInterfaceRect, GUIContent.none, GUIStyle.none);
        GUI.BeginGroup(inviteInterfaceRect);

        Rect headerRect = new Rect(0, 0, inviteInterfaceRect.width, centeredHeaderStyle.CalcSize(new GUIContent(inviteHeaderText)).y);

        GUI.Label(headerRect, inviteHeaderText, centeredHeaderStyle);

        Rect descriptionRect = new Rect(0, headerRect.height, inviteInterfaceRect.width,
            centeredHeaderStyle.CalcHeight(new GUIContent(inviteDescriptionText), inviteInterfaceRect.width));

        GUI.Label(descriptionRect, inviteDescriptionText, beginStyle);

        Rect secureRect =  new Rect(0, descriptionRect.y + descriptionRect.height, inviteInterfaceRect.width / 2, redHintStyle.CalcHeight(new GUIContent(secureText), inviteInterfaceRect.width / 4f));
        
        GUI.Label(secureRect, secureText, textAreaStyle);

        Rect fromRect = new Rect();
        fromRect.x = secureRect.width + 10;
        fromRect.y = secureRect.y + 10;
        Vector2 fromSize = labelStyle.CalcSize(new GUIContent(fromText));
        fromRect.width = fromSize.x;
        fromRect.height = fromSize.y;
        GUI.Label(fromRect, fromText, labelStyle);

        Rect fromNameTextBoxRect = new Rect();
        fromNameTextBoxRect.y = fromRect.y + 5;
        fromNameTextBoxRect.width = descriptionRect.width / 2 - fromRect.width - 20;
        fromNameTextBoxRect.x = fromRect.x + fromRect.width + 10;
        fromNameTextBoxRect.height = textBoxStyle.CalcSize(new GUIContent(typeNameHereText)).y;

        fromName = GUIHelpers.ShowTextBoxWithHint(fromNameTextBoxRect, fromName, typeNameHereText, Event.current, textBoxStyle, textBoxHintStyle, ref isFromNameFocused);

        if (isFromNameFocused)
        {
            isFromEmailFocused = false;

            for (int i = 0; i < isToEmailsFocused.Length; i++)
                isToEmailsFocused[i] = false;
        }
        
        if (fromName != typeNameHereText && !string.IsNullOrEmpty(fromName))
        {
            fromName = Common.NormalizeToName(fromName);
        }

        Rect fromEmailRect = new Rect();
        fromEmailRect.x = fromRect.x;
        fromEmailRect.y = fromRect.y + fromRect.height;
        fromEmailRect.width = emailSize.x;
        fromEmailRect.height = emailSize.y;
        GUI.Label(fromEmailRect, emailText, labelStyle);

        Rect fromEmailTextBoxRect = new Rect();
        fromEmailTextBoxRect.y = fromEmailRect.y + 5;
        fromEmailTextBoxRect.width = descriptionRect.width / 2 - fromEmailRect.width - 20;
        fromEmailTextBoxRect.x = fromEmailRect.x + fromEmailRect.width + 10;
        fromEmailTextBoxRect.height = textBoxStyle.CalcSize(new GUIContent(typeYourEmailHereText)).y;

        fromEmail = GUIHelpers.ShowTextBoxWithHint(fromEmailTextBoxRect, fromEmail, typeYourEmailHereText, Event.current, textBoxStyle, textBoxHintStyle, ref isFromEmailFocused);
        if (isFromEmailFocused)
        {
            for (int i = 0; i < isToEmailsFocused.Length; i++)
                isToEmailsFocused[i] = false;
        }
        
        float maxY = secureRect.y + secureRect.height;

        for (int i = 0; i < toEmails.Length; i++)
        {
            Rect toEmailRect = new Rect();
            toEmailRect.width = emailSize.x;
            toEmailRect.height = emailSize.y;

            toEmailRect.y = secureRect.y + secureRect.height + toEmailRect.height * i;
            toEmailRect.x = inviteInterfaceRect.width / 2 - emailSize.x / 2 - fromEmailTextBoxRect.width / 2;

            GUI.Label(toEmailRect, emailText, labelStyle);

            Rect toEmailTextBoxRect = new Rect();
            toEmailTextBoxRect.y = toEmailRect.y + 5;
            toEmailTextBoxRect.width = fromEmailTextBoxRect.width;
            toEmailTextBoxRect.x = toEmailRect.x + toEmailRect.width + 10;
            toEmailTextBoxRect.height = textBoxStyle.CalcSize(new GUIContent(typeEmailHereText)).y;

            toEmails[i] = GUIHelpers.ShowTextBoxWithHint(toEmailTextBoxRect, toEmails[i], typeEmailHereText, Event.current, textBoxStyle, textBoxHintStyle, ref isToEmailsFocused[i]);

            if (isToEmailsFocused[i])
            {
                isFromNameFocused = false;
                isFromEmailFocused = false;

                for (int j = 0; j < isToEmailsFocused.Length; j++)
                {
                    if (j != i)
                        isToEmailsFocused[j] = false;
                }
            }

            maxY = toEmailRect.y + toEmailRect.height;
        }

        Vector2 submitSize = buttonStyle.CalcSize(new GUIContent(sumbitText));
        Rect submitButton = new Rect();
        submitButton.y = maxY + 20;
        submitButton.x = inviteInterfaceRect.width / 2 - submitSize.x / 2 - 20;
        submitButton.width = submitSize.x;
        submitButton.height = submitSize.y;

        if (GUI.Button(submitButton, sumbitText, buttonStyle))
        {
            errorText = CheckInviteParameters();

            if (!string.IsNullOrEmpty(errorText))
            {
                tourState = TourStates.error;
                backStateAfterError = TourStates.invite;
            }
            else
            {
                oInviteManager.SendInvitations(fromName, fromEmail, toEmails, subjectEmailText, emailBodyText);
                tourState = TourStates.sending;
            }
        }

        Vector2 backSize = buttonStyle.CalcSize(new GUIContent(backText));
        Rect backButtonRect = new Rect();
        backButtonRect.x = submitButton.x + submitButton.width + 40;
        backButtonRect.y = submitButton.y;
        backButtonRect.width = backSize.x;
        backButtonRect.height = backSize.y;

        if (GUI.Button(backButtonRect, backText, buttonStyle))
        {
            tourState = TourStates.share;
        }

        GUI.EndGroup();
    }

    private string CheckInviteParameters()
    {
        string result = string.Empty;

        if (string.IsNullOrEmpty(fromName))
            result += emptyFromNameText + "\n";

        if (string.IsNullOrEmpty(fromEmail))
            result += emptyFromEmailText + "\n";
        else if (!Common.IsValidEmail(fromEmail))
            result += invalidYourEmailText + "\n";

        bool isAtLeastOneEntered = false;

        for (int i = 0; i < toEmails.Length; i++)
        {
            if (!string.IsNullOrEmpty(toEmails[i]))
            {
                if (!Common.IsValidEmail(toEmails[i]))
                    result += string.Format(invalidEmailText, toEmails[i]) + "\n";

                isAtLeastOneEntered = true;
            }
        }

        if (!isAtLeastOneEntered)
            result += atLeastOneText + "\n";

        return result;

    }

    private void ShowSubmit()
    {
        GUI.Box(shareInterfaceRect, GUIContent.none, GUIStyle.none);
        GUILayout.BeginArea(shareInterfaceRect);

        GUILayout.BeginVertical();
        GUILayout.Label(submitHeaderText, centeredHeaderStyle);
        GUILayout.Label(submitDescriptionText, beginStyle);
        GUILayout.FlexibleSpace();
        
        GUILayout.BeginHorizontal();

        GUILayout.Label(byText, labelStyle, GUILayout.Width(100));
        by = GUILayout.TextArea(by, textBoxStyle);
        by = Common.NormalizeToName(by);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(ageText, labelStyle, GUILayout.Width(100));
        age = GUILayout.TextArea(age, 2, textBoxStyle);
        age = Common.NormalizeToNumeric(age);   

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(stateText, labelStyle, GUILayout.Width(100));
        state = GUILayout.TextArea(state, 2, textBoxStyle);
        
        if (!string.IsNullOrEmpty(state))
            state = state.ToUpper();

        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(sumbitText, buttonStyle))
        {
            int intAge = 0;
            int.TryParse(age, out intAge);

            UserInfo = new Info(by, 0, intAge, DateTime.Now, string.Empty, state);

            if (LoadAS3DManager.oGameData == null || LoadAS3DManager.oGameData.GameId < 0)
            {
                // Register new game
                oSaveAS3DManager.RegisterNewGame(UserInfo);
            }

            tourState = TourStates.saving;
            StartCoroutine(WaitForRegisterAndSave());             
        }

        if (GUILayout.Button(backText, buttonStyle))
        {
            tourState = TourStates.share;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndArea();
    }

    private void ShowShare()
    {
        GUI.Box(shareInterfaceRect, GUIContent.none, GUIStyle.none);
        GUILayout.BeginArea(shareInterfaceRect);

        GUILayout.BeginVertical();
        
        GUILayout.Label(shareHeaderText, centeredHeaderStyle);
        

        GUILayout.Label(shareDescriptionText, beginStyle);
        GUILayout.Label(GUIContent.none, GUILayout.Width(shareInterfaceRect.width), GUILayout.Height(50));
        GUILayout.Label(whichWouldyouText, beginStyle, GUILayout.Width(shareInterfaceRect.width));


        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button(submitToGalleryText, buttonStyle))
        {
            tourState = TourStates.submit;
        }

        if (GUILayout.Button(sendMailText, buttonStyle))
        {
            tourState = TourStates.invite;
        }

        if (GUILayout.Button(backText, buttonStyle))
        {
            tourState = TourStates.tour;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.EndArea();
    }
    
    /// <summary>
    /// 
    /// Draw ROOF ON/ROOF OFF buttons
    /// </summary>
    private void DrawRoofButtons()
    {
        view3DRect = playerCamera.pixelRect;
        view3DRect.y = Screen.height - playerCamera.pixelRect.y - playerCamera.pixelRect.height;

        GUI.Box(view3DRect, GUIContent.none, view3DStyle);


        Vector2 returnSize = buttonStyle.CalcSize(new GUIContent(returnText)),
                shareSize = buttonStyle.CalcSize(new GUIContent(shareText));

        Rect switcherRoofRect = new Rect(view3DRect.x + 30, view3DRect.y + view3DRect.height + 15, 161, 22);

        InteriorDesignGUI.SwitcherRoofIndex = 
            Common.SelectionGrid(switcherRoofRect, InteriorDesignGUI.SwitcherRoofIndex, switcherRoof,
                                    switcherLeft, switcherLeftActive, switcherRight, switcherRightActive);

        Rect shareRect = new Rect(switcherRoofRect.x + playerCamera.pixelRect.width - switcherRoofRect.width - 20,
                                    switcherRoofRect.y, shareSize.x, shareSize.y);

        Rect boldHintRect = new Rect(view3DRect.x, view3DRect.y + view3DRect.height + 35, 700, 40);

        GUI.Label(boldHintRect, boldHintAboutMoving, boldHintStyle);

        if (GUI.Button(shareRect, shareText, buttonStyle))
        {
            StartCoroutine(MakeScreenshotAndShare());
        }

        Rect returnRect = new Rect(switcherRoofRect.x + switcherRoofRect.width + 20, switcherRoofRect.y, returnSize.x, returnSize.y);

        if (GUI.Button(returnRect, returnText, buttonStyle))
        {
            ArchitectStudioGUI.ToLastState();
        }

        if (BuildingManager.Picked.PickedRoof != null)
        {
            if (InteriorDesignGUI.SwitcherRoofIndex == 0)
            {
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");
            }
        }
    }

    private byte[] screenShotBytes;   // Screenshot bytes

    private IEnumerator MakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Vector3 lookAt = BuildingManager.Picked.transform.position +
            BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * BuildingManager.Picked.lookAtOffset.x +
                BuildingManager.Picked.transform.TransformDirection(Vector3.right) * BuildingManager.Picked.lookAtOffset.y;
        
        Vector3 camPosition = lookAt + Vector3.up * 30 + BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * 100;

        Texture2D screenShot = ScreenShotMaker.GetImageFromCamera(snapShotWidth, snapShotHeight, playerCamera, new Rect(0, 0, 1, 1), camPosition, lookAt);

        screenShotBytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        tourState = TourStates.share;
    }

    private IEnumerator WaitForRegisterAndSave()
    {
        while (LoadAS3DManager.oGameData == null || (LoadAS3DManager.oGameData.GameId < 0 && LoadAS3DManager.oGameData.GameId != -500))
        {
            yield return 0;
        }

        if (LoadAS3DManager.oGameData.GameId == -500)
        {
            errorText = errorSaveMessageText;
            // Change the error message here
            tourState = TourStates.error;
            backStateAfterError = TourStates.tour;
        }
        else
        {
            oSaveAS3DManager.SaveScreenShot(LoadAS3DManager.oGameData.GameId, screenShotBytes);

            if (ClientManager.MyClients != null)
                oSaveAS3DManager.SaveClients(LoadAS3DManager.oGameData.GameId, ClientManager.MyClients);

            if (LocationManager.PickedLocation != null)
                oSaveAS3DManager.SaveLocation(LoadAS3DManager.oGameData.GameId, LocationManager.PickedLocation.id);

            if (BuildingManager.Picked != null)
                oSaveAS3DManager.SaveExterior(LoadAS3DManager.oGameData.GameId, BuildingManager.Picked);

            if (WallManager.Walls != null && InteriorDesignGUI.GridItems != null)
                oSaveAS3DManager.SaveInterior(LoadAS3DManager.oGameData.GameId, InteriorDesignGUI.GridItems, BuildingManager.Picked.FloorTiles, WallManager.Walls);

            if (LandscapingGUI.GridItems != null)
                oSaveAS3DManager.SaveLandscape(LoadAS3DManager.oGameData.GameId, LandscapingGUI.GridItems);

            if (UserInfo != null)
                oSaveAS3DManager.SaveUserInfo(LoadAS3DManager.oGameData.GameId, UserInfo);
            
            oSaveAS3DManager.Share(LoadAS3DManager.oGameData.GameId);

            tourState = TourStates.saving;
        }
    }
}
