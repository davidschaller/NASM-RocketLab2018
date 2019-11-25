using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ArchitectStudioGUI : MonoBehaviour
{
    public enum modes
    {
        Design,
        Gallery,
        Print
    }

    private static modes mode = modes.Design;
    public static modes Mode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
        }
    }

    public GUISkin headerSkin,
                   buttonSkin,
                   hintSkin;

	public Texture2D navTitle;
	public Texture2D navEnd;
    public Texture2D flwSmall;
	
	public enum States
	{
		ChooseClient,
		Layout,
		DesignExterior,
		Interior,
		Landscaping,
		TourHouse,
        GalleryTour
	}

    private GUIStyle headerButtonStyle,
                     selectedHeaderButtonStyle,
                     buttonStyle,
                     selectedButtonStyle,
                     labelStyle;


	private static States currentState;
    private static States lastState;

    public static States CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private static ArchitectStudioGUI main = null;
    public static bool IsActive
    {
        get
        {
            return main != null;
        }
    }
    
	private static Dictionary<States, MonoBehaviour> stateScripts = new Dictionary<States, MonoBehaviour>();
	public static void RegisterStateScript(States state, MonoBehaviour script)
	{
		if (!stateScripts.ContainsKey(state))
			stateScripts.Add(state, script);
		else
			stateScripts[state] = script;
	}

    public static void SetState(States p_newState)
    {
        if (currentState != p_newState && stateScripts.ContainsKey(currentState))
        {
            stateScripts[currentState].enabled = false;
        }

        lastState = currentState;
        currentState = p_newState;

        if (stateScripts.ContainsKey(currentState))
        {
            if (stateScripts[currentState] != null)
            {
                stateScripts[currentState].enabled = true;
            }
        }

        oPopupHintGUI.HideHints();

        if (!string.IsNullOrEmpty((oPopupHintGUI.GetHintNameByState(currentState))) && !oPopupHintGUI.IsShownAlready(currentState))
        {
            oPopupHintGUI.ShowHint(currentState);
        }

        SoundManager.SoundSwitcher(currentState);
    }

    private static PopupHintGUI oPopupHintGUI;
    private Rect imageRect = new Rect(10, 50, 124, 76);

	private void Awake ()
	{
        if (LoadAS3DManager.oGameData != null)
            Debug.Log("Saved game started, game id is " + LoadAS3DManager.oGameData.GameId);
        else
            Debug.Log("GameData is empty. New game will be created on Save");

        oPopupHintGUI = (PopupHintGUI)GameObject.FindObjectOfType(typeof(PopupHintGUI));

        SetGUIStyles();

        if (main == null)
            main = this;
	}

	private void Start ()
	{
        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.ShowGallery)
        {
            mode = modes.Gallery;
            SetState(States.GalleryTour);
        }
        else
        {
            mode = modes.Design;
            SetState(States.ChooseClient);
        }
	}

    private void SetGUIStyles()
    {
        headerButtonStyle = headerSkin.GetStyle("button");
        selectedHeaderButtonStyle = new GUIStyle(headerButtonStyle);
        selectedHeaderButtonStyle.normal = selectedHeaderButtonStyle.active;
        selectedHeaderButtonStyle.hover = selectedHeaderButtonStyle.active;

        buttonStyle = buttonSkin.GetStyle("button");
        selectedButtonStyle = new GUIStyle(buttonStyle);
        selectedButtonStyle.normal = selectedButtonStyle.active;
       
        labelStyle = buttonSkin.GetStyle("label");
    }
	
	private void Header()
	{
        Common.DrawTextureAt(Vector2.zero, navTitle);
		
		GUILayout.BeginArea(new Rect(navTitle.width, 0, Screen.width-navTitle.width-navEnd.width, navTitle.height));
		GUILayout.BeginHorizontal();
        if (GUILayout.Button("CHOOSE CLIENT", currentState == States.ChooseClient ? selectedHeaderButtonStyle : headerButtonStyle))
		{            
			SetState(States.ChooseClient);
		}
        else if (GUILayout.Button("LOCATION", currentState == States.Layout ? selectedHeaderButtonStyle : headerButtonStyle))
		{
            if (ClientManager.IsValid)
			    SetState(States.Layout);
		}
        else if (GUILayout.Button("DESIGN EXTERIOR", currentState == States.DesignExterior ? selectedHeaderButtonStyle : headerButtonStyle))
		{
            if (ClientManager.IsValid)
			    SetState(States.DesignExterior);
		}
        else if (GUILayout.Button("INTERIOR", currentState == States.Interior ? selectedHeaderButtonStyle : headerButtonStyle))
		{
            if (CanShowNextButton(States.DesignExterior))
			    SetState(States.Interior);
		}
        else if (GUILayout.Button("LANDSCAPING", currentState == States.Landscaping ? selectedHeaderButtonStyle : headerButtonStyle))
		{
            if (CanShowNextButton(States.DesignExterior))
			    SetState(States.Landscaping);
		}
        else if (GUILayout.Button("TOUR HOUSE", currentState == States.TourHouse ? selectedHeaderButtonStyle : headerButtonStyle))
		{
            if (CanShowNextButton(States.DesignExterior))
			    SetState(States.TourHouse);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
        Common.DrawTextureAt(new Vector2(Screen.width - navEnd.width, 0), navEnd);
	}

    #region Reviews Region

    private string REVIEW = "REVIEW",
                   YOUR_CLIENTS = "Your Clients",
                   YOUR_LOCATION = "Your Location",
                   BACK_TO_DESIGN_GALLERY = "BACK TO\nDESIGN GALLERY",
                   WRIGHTS_HINTS = "WRIGHT'S HINTS:\n{0}",
                   BACK_TO_GALLERY_TOUR = "BACK TO\nGALLERY TOUR";

    /// <summary>
    /// 
    /// Show reviews
    /// </summary>
    /// 
    private void ShowReviews(Rect p_imageRect)
    {
        float marginTop = 0;

        if (mode != modes.Print)
        {
            if (currentState > States.ChooseClient)
            {
                marginTop = ShowClientsReview(p_imageRect, marginTop);
            }

            if (currentState > States.Layout && LocationManager.PickedLocation != null)
            {
                marginTop = ShowLocationReview(p_imageRect, marginTop);
            }
        }

        if (mode == modes.Gallery)
        {
            marginTop = ShowBackToGallery(p_imageRect, marginTop);

            if (currentState < States.GalleryTour)
            {
                ShowBackToGalleryTour(p_imageRect, marginTop);
            }
        }
        else if (mode == modes.Design)
        {
            Vector2 flwSmallPos = new Vector2(10, Screen.height - 190);

            Common.DrawTextureAt(flwSmallPos, flwSmall);
            

            Rect hintsButton = new Rect(10, Screen.height - 100, 120, 35);

            string about = oPopupHintGUI.GetHintNameByState(currentState);

            if (!string.IsNullOrEmpty(about))
            {
                if (GUI.Button(hintsButton, string.Format(WRIGHTS_HINTS, about), buttonStyle))
                {
                    if (!oPopupHintGUI.Active)
                        oPopupHintGUI.ShowHint(currentState);
                    else
                        oPopupHintGUI.HideHints();
                }
            }
        }
    }

    /// <summary>
    /// Load "Gallery" level if "Back" button is pressed.
    /// </summary>
    /// <param name="p_imageRect"></param>
    /// <param name="p_marginTop"></param>
    private float ShowBackToGallery(Rect p_imageRect, float p_marginTop)
    {
        float newMarginTop = p_marginTop;

        Vector2 buttonSize = labelStyle.CalcSize(new GUIContent(BACK_TO_DESIGN_GALLERY)) * 0.8f;

        Rect backToDesignRect = new Rect(p_imageRect.x + p_imageRect.width / 2 - buttonSize.x / 2, p_marginTop + 40, buttonSize.x, buttonSize.y);

        if (GUI.Button(backToDesignRect, BACK_TO_DESIGN_GALLERY, buttonStyle))
        {
            Application.LoadLevel("Gallery");
        }

        newMarginTop += backToDesignRect.height + 10;

        return newMarginTop;
    }

    private void ShowBackToGalleryTour(Rect p_imageRect, float p_marginTop)
    {
        Vector2 buttonSize = labelStyle.CalcSize(new GUIContent(BACK_TO_GALLERY_TOUR)) * 0.8f;

        Rect backToGalleryTourRect =
            new Rect(p_imageRect.x + p_imageRect.width / 2 - buttonSize.x / 2, p_marginTop + 40, buttonSize.x, buttonSize.y);

        if (GUI.Button(backToGalleryTourRect, BACK_TO_GALLERY_TOUR, buttonStyle))
        {
            SetState(States.GalleryTour);
        }
    }

    private float ShowLocationReview(Rect p_imageRect, float p_marginTop)
    {
        float newMarginTop = ShowLocationPicture(p_imageRect, p_marginTop);

        newMarginTop = ShowReviewText(p_imageRect, newMarginTop, YOUR_LOCATION, REVIEW, States.Layout);

        return newMarginTop;
    }

    private float ShowClientsReview(Rect p_imageRect, float p_marginTop)
    {
        float newMarginTop = ShowYourClientsPisture(p_imageRect);

        newMarginTop = ShowReviewText(p_imageRect, newMarginTop, YOUR_CLIENTS, REVIEW, States.ChooseClient);

        return newMarginTop;
    }

    private float ShowReviewText(Rect p_imageRect, float p_marginTop, string p_text, string p_buttonText, States p_setTo)
    {
        Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(p_buttonText));
        Vector2 textSize = labelStyle.CalcSize(new GUIContent(p_text));

        Rect textRect = new Rect(p_imageRect.width / 2 + p_imageRect.x - textSize.x / 2, p_marginTop, textSize.x, textSize.y);
        GUI.Label(textRect, p_text, labelStyle);

        Rect buttonRect = new Rect(imageRect.width / 2 - buttonSize.x / 2 + imageRect.x, p_marginTop + textSize.y, buttonSize.x, buttonSize.y);
        if (GUI.Button(buttonRect, REVIEW, buttonStyle))
        {
            SetState(p_setTo);
        }

        float result = p_marginTop + buttonSize.y + textSize.y;

        return result;
    }

    /// <summary>
    /// Adjust review clients picture
    /// </summary>
    /// <param name="p_imageRect"></param>
    private float ShowYourClientsPisture(Rect p_imageRect)
    {
        int numSpaces = 0;

        // Get the number of spaces for offset
        for (int i = 3; i >= 0; i--)
        {
            int cientIndex = 0;

            if (i < ClientManager.MyClients.Count)
                cientIndex = ClientManager.MyClients[i];

            if (cientIndex == 0)
                numSpaces++;
        }

        GUILayout.BeginArea(p_imageRect);
        GUILayout.BeginHorizontal();
        GUILayout.Space(numSpaces * 17);
        for (int i = 3; i >= 0; i--)
        {
            int cientIndex = 0;

            if (i < ClientManager.MyClients.Count)
                cientIndex = ClientManager.MyClients[i];

            // Draw only picked clients
            if (cientIndex != 0)
                GUILayout.Label(ClientManager.Clients[cientIndex].image, GUILayout.Width(ClientManager.Clients[cientIndex].image.width * 0.18f));
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        return p_imageRect.y + p_imageRect.height;
    }

    private float ShowLocationPicture(Rect p_imageRect, float p_marginTop)
    {
        Vector2 imagePos = new Vector2(p_imageRect.x + p_imageRect.width / 2 -
            LocationManager.PickedLocation.thumbnailPicture.width / 2, p_marginTop) + new Vector2(0, 10);

        Common.DrawTextureAt(imagePos, LocationManager.PickedLocation.thumbnailPicture);

        return p_imageRect.height + imagePos.y;
    }

    #endregion

    /// <summary>
    /// The bottom button name is different due to the current state.
    /// </summary>
    /// <param name="p_currentState">Current state</param>
    /// <returns>Button name</returns>
    private string GetNextButtonText(States p_currentState)
    {
        switch (p_currentState)
        {
            case States.ChooseClient:
                return "CLOSE CLIENT REVIEW";
            case States.Layout:
                return "CHOOSE THIS LOCATION";
            case States.DesignExterior:
                return "CONTINUE:\nDESIGN INTERIOR";
            case States.Interior:
                return "CONTINUE:\nLANDSCAPING";
            case States.Landscaping:
                return "CONTINUE:\nTOUR HOUSE";
            default:
                return string.Empty;
        }        
    }

    /// <summary>
    /// This funcion is responsible for menu navigation.
    /// User can't start Tour, Interior design etc. until the building and clients are not picked.
    /// </summary>
    /// <param name="p_currentState">Current state</param>
    /// <returns>Can use open this state or not</returns>
    private bool CanShowNextButton(States p_currentState)
    {
        switch (p_currentState)
        {
            case States.ChooseClient:
                return ClientManager.IsValid && ChooseClientGUI.IsFinished();
            case States.DesignExterior:
                return ClientManager.IsValid && ExteriorDesignGUI.IsFinished();
            case States.Interior:
                return ClientManager.IsValid && ExteriorDesignGUI.IsFinished() && InteriorDesignGUI.IsFinished();
            case States.Landscaping:
                return ClientManager.IsValid && ExteriorDesignGUI.IsFinished();
            case States.Layout:
                return ClientManager.IsValid;
            default:
                return false;
        }
    }    

    private void OnGUI()
    {
        GUI.depth = 0;

        ShowReviews(imageRect);

        if (currentState > States.DesignExterior && LocationManager.PickedLocation != null)
        {
            if (LocationManager.PickedLocation.IsDownloaded() && !LocationManager.PickedLocation.IsInstantiated())
            {
                LocationManager.PickedLocation.Instantiate();
            }
        }

        // For design mode only
        if (mode == modes.Design)
        {
            Header();

            GUI.skin = hintSkin;

            if (CanShowNextButton(currentState))
            {
                string buttonText = GetNextButtonText(currentState);

                Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(buttonText));

                if (GUI.Button(new Rect(Screen.width / 2 - buttonSize.x / 2, Screen.height - 55 - buttonSize.y / 2, buttonSize.x, buttonSize.y), buttonText))
                {
                    if (currentState == States.Layout)
                    {
                        if (!string.IsNullOrEmpty(LocationManager.PickedLocation.url) && !LocationManager.PickedLocation.IsDownloaded())
                        {
                            if (!LocationManager.PickedLocation.IsDownloaded() && !LocationManager.PickedLocation.IsDownloading())
                                LocationManager.PickedLocation.Download();
                        }
                    }

                    SetState(currentState + 1);
                }
            }
        }
    }

    internal static void ToLastState()
    {
        SetState(lastState);
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Quit");

        if (LocationManager.PickedLocation && LocationManager.PickedLocation.Stream != null &&
                LocationManager.PickedLocation.Stream.assetBundle != null)
        {
            LocationManager.PickedLocation.Stream.assetBundle.Unload(false);
        }
    } 
}
