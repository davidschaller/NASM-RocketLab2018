using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using JsonFx.Json;
using System.Text;

public class GalleryGUI : MonoBehaviour
{
    private LoadAS3DManager oLoadAS3DManager;

    private float itemHeight = 200;
    private float itemWidth = 230;
    private float marginLeft = 10,
                  marginHeight = 10;
    private int imageHeight = 145,
                imageWidth = 204;

    public GUISkin buttonSkin,
                   headerSkin,
                   selectionSkin,
                   footerSkin,
                   hintSkin,
                   labelSkin;

    private int page = 1;
    private bool showMoreAboutGallery = false;


    void Awake()
    {
        Debug.Log("Awake");
    }

    private GUIStyle headerButtonStyle,
                     selectedHeaderButtonStyle,
                     buttonStyle,
                     selectedButtonStyle,
                     needsTextStyle,
                     needsHeaderStyle,
                     headerStyle,
                     centeredHeaderStyle,
                     logoStyle,
                     lineStyle,
                     galleryItemStyle,
                     galleryPictureStyle,
                     galleryAreaStyle,
                     gallerySelectionStyle,
                     gallerySelectionSkipStyle,
                     smallestTextStyle,
                     startStyle,
                     redLinkStyle,
                     needBoxStyle,
                     labelStyle;

    private List<string> sortMenu;
    private List<bool> reverts;
    private string pickedMenu;

    private void SetGUIStyles()
    {
        headerButtonStyle = headerSkin.GetStyle("button");
        selectedHeaderButtonStyle = new GUIStyle(headerButtonStyle);
        selectedHeaderButtonStyle.normal = selectedHeaderButtonStyle.active;
        selectedHeaderButtonStyle.hover = selectedHeaderButtonStyle.active;

        buttonStyle = buttonSkin.GetStyle("button");
        selectedButtonStyle = new GUIStyle(buttonStyle);
        selectedButtonStyle.normal = selectedButtonStyle.active;

        needsTextStyle = hintSkin.GetStyle("needstext");
        needsHeaderStyle = hintSkin.GetStyle("needsheader");

        headerStyle = hintSkin.GetStyle("label");
        centeredHeaderStyle = new GUIStyle(headerStyle);
        centeredHeaderStyle.alignment = TextAnchor.MiddleCenter;

        logoStyle = headerSkin.GetStyle("gallerylogo");
        lineStyle = headerSkin.GetStyle("upperline");

        galleryItemStyle = buttonSkin.GetStyle("galleryitem");
        galleryPictureStyle = buttonSkin.GetStyle("galleryPicture");

        galleryAreaStyle = selectionSkin.GetStyle("gamesarea");

        gallerySelectionStyle = selectionSkin.GetStyle("galleryButton");
        gallerySelectionSkipStyle = selectionSkin.GetStyle("gallerySkipButton");

        GUIStyle roomLabelStyle = labelSkin.GetStyle("roomLabel");
        smallestTextStyle = new GUIStyle(roomLabelStyle);
        smallestTextStyle.alignment = TextAnchor.MiddleLeft;
        smallestTextStyle.clipping = TextClipping.Clip;

        startStyle = buttonSkin.GetStyle("smallStar");

        redLinkStyle = hintSkin.GetStyle("redLink");

        needBoxStyle = hintSkin.GetStyle("needsbox");
        labelStyle = buttonSkin.GetStyle("label");
    }

    private void Start()
    {
        oLoadAS3DManager = (LoadAS3DManager)GameObject.FindObjectOfType(typeof(LoadAS3DManager));

        // Load to oLoadAS3DManager.GameList
        StartCoroutine(oLoadAS3DManager.LoadGames());

        SetGUIStyles();

        FillUpSortMenu();
    }

    private void FillUpSortMenu()
    {
        sortMenu = new List<string>();

        sortMenu.Add("rating");
        sortMenu.Add("architect");
        sortMenu.Add("age");
        sortMenu.Add("state");
        sortMenu.Add("floorplan");
        sortMenu.Add("date submitted");

        reverts = new List<bool>();
        reverts.Add(false);
        reverts.Add(false);
        reverts.Add(false);
        reverts.Add(false);
        reverts.Add(false);
        reverts.Add(false);

        pickedMenu = sortMenu[0];
    }

    private void OnGUI()
    {
        Header();

        if (showMoreAboutGallery)
        {
            ShowMoreGalleryInfo();
            return;
        }

        if (!isLoading)
        {
            Rect gamesRect = new Rect(Screen.width / 2 - 380, 103, 760, 430);

            if (LoadAS3DManager.GameList != null && LoadAS3DManager.GameList.Count > 0)
            {
                int numOfPages = 1;

                int totalRecords = LoadAS3DManager.GameList.Count;

                numOfPages = totalRecords / 6 + 1;
                

                DrawSelectionButtons(gamesRect, numOfPages);

                GUI.BeginGroup(gamesRect, GUIContent.none, galleryAreaStyle);                

                int maxIndex = (page - 1) * 6 + 6;

                if (maxIndex > LoadAS3DManager.GameList.Count)
                    maxIndex = LoadAS3DManager.GameList.Count;

                int positionIndex = 0;
                for (int i = (page - 1) * 6; i < maxIndex; i++)
                {
                    DrawGameItem(LoadAS3DManager.GameList[i], positionIndex);
                    positionIndex++;
                }
                GUI.EndGroup();    
            }
        }
        else
        {
            ShowProcessing();
        }
    }

    private void ShowMoreGalleryInfo()
    {
        Rect infoRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 160, 400, 320);

        GUI.Box(infoRect, GUIContent.none, needBoxStyle);
        Rect insideInfoRect = infoRect;
        insideInfoRect.y += 20;
        insideInfoRect.height -= 40;
        insideInfoRect.x += 20;
        insideInfoRect.width -= 40;

        GUILayout.BeginArea(insideInfoRect);
        GUILayout.BeginVertical();
        GUILayout.Label(introTextAll, needsTextStyle);

        GUILayout.Label(string.Empty, GUILayout.Width(insideInfoRect.width));
        foreach (string item in cases)
        {
            GUILayout.Label(item, needsTextStyle);
        }

        GUILayout.Label(string.Empty, GUILayout.Width(insideInfoRect.width));

        GUILayout.BeginHorizontal();
        GUILayout.Space(insideInfoRect.width / 2 - 50);
        if (GUILayout.Button("Close", buttonStyle, GUILayout.Width(100)))
        {
            showMoreAboutGallery = false;
        }

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void DrawSelectionButtons(Rect p_gamesRect, int p_max)
    {
        LeftSelectionButtons(p_gamesRect);
        RightSelectionButtons(p_gamesRect, p_max);
    }

    private int DrawSelectionButton(int p_input, int? p_min, int? p_max, bool p_skip, Vector2 p_pos, GUIStyle p_style, float p_rotateAngle)
    {
        int result = p_input;

        float buttonWidth = p_style.normal.background.width;
        float buttonHeight = p_style.normal.background.height;

        Vector2 center = new Vector2(p_pos.x + buttonWidth / 2, p_pos.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(p_rotateAngle, center);

        Rect buttonRect = new Rect(p_pos.x, p_pos.y, buttonWidth, buttonHeight);

        if (GUI.Button(buttonRect, string.Empty, p_style))
        {
            //int oldPage = result;

            if (p_skip)
            {
                if (p_min.HasValue)
                {
                    result = p_min.Value;
                }
                else if (p_max.HasValue)
                {
                    result = p_max.Value;
                }
                else
                    Debug.LogWarning("Can't jump. No MIN and MAX values found");
            }
            else
            {
                if (p_min.HasValue && result - 1 >= p_min.Value)
                {
                    result--;
                }
                else if (p_max.HasValue && result + 1 <= p_max.Value)
                {
                    result++;
                }
            }

            /*
            if (oldPage != result)
            {
                // Load to oLoadAS3DManager.GameList
                StartCoroutine(oLoadAS3DManager.LoadGames(result));
            }
             */
        }

        GUI.matrix = Matrix4x4.identity;

        return result;
    }

    private void LeftSelectionButtons(Rect p_gamesRect)
    {
        float buttonWidth = gallerySelectionStyle.normal.background.width;
        float buttonHeight = gallerySelectionStyle.normal.background.height;

        Vector2 leftPos = new Vector2(p_gamesRect.x - buttonWidth - buttonWidth / 3, p_gamesRect.y + p_gamesRect.height / 2 - buttonHeight);
        page = DrawSelectionButton(page, 1, null, false, leftPos, gallerySelectionStyle, 180);


        Vector2 leftSkipPos = new Vector2(leftPos.x - buttonWidth - 2, leftPos.y);
        page = DrawSelectionButton(page, 1, null, true, leftSkipPos, gallerySelectionSkipStyle, 180);
    }

    private void RightSelectionButtons(Rect p_gamesRect, int p_max)
    {
        float buttonWidth = gallerySelectionStyle.normal.background.width;
        float buttonHeight = gallerySelectionStyle.normal.background.height;

        Vector2 rightPos = new Vector2(p_gamesRect.x + p_gamesRect.width + buttonWidth / 3, p_gamesRect.y + p_gamesRect.height / 2 - buttonHeight);
        page = DrawSelectionButton(page, null, p_max, false, rightPos, gallerySelectionStyle, 0);

        Vector2 rightSkipPos = new Vector2(rightPos.x + buttonWidth + 2, rightPos.y);
        page = DrawSelectionButton(page, null, p_max, true, rightSkipPos, gallerySelectionSkipStyle, 0);
    }

    private int rMult = 0, 
                vMult = 0;

    private bool isLoading = false;

    private void DrawGameItem(GameData item, int index)
    {
        if (item.oInfo == null)
        {
            Debug.LogWarning("Trying to load game with null info. Aborted");
            return;
        }

        Rect itemRect = new Rect(25 + (marginLeft + itemWidth) * (index % 3 == 0 ? rMult = 0 : ++rMult),
                         marginHeight + (marginHeight + itemHeight) * (index % 3 == 0 ? vMult = index / 3 : vMult),
                         itemWidth, itemHeight);

        GUILayout.BeginArea(itemRect, GUIContent.none, galleryItemStyle);

        GUI.Label(new Rect(11, 165, itemRect.width / 1.5f, 20), /*"Shmykov Oleg 23"*/string.Format("{0} {1}", item.oInfo.name, item.oInfo.age), needsHeaderStyle);

        GUI.Label(new Rect(11, 180, itemRect.width / 1.5f, 20), /*"From CA, Date 8/28/2010"*/ 
            string.Format("From {0}, Date {1}", item.oInfo.state, 
                item.oInfo.dateOfSubmition.ToString("MM-dd-yyyy")), smallestTextStyle);

        GUIContent ratingContent = new GUIContent(rating);
        Vector2 ratingSize = smallestTextStyle.CalcSize(ratingContent);
        Rect ratingHeaderRect = new Rect(itemRect.width - ratingSize.x - 11, 165, ratingSize.x, ratingSize.y);
        GUI.Label(ratingHeaderRect, ratingContent, smallestTextStyle);
        
        GUIContent screenShotContent = GUIContent.none;

        if (item.Image != null)
            screenShotContent = new GUIContent(item.Image);
        
        if (GUI.Button(new Rect(13, 10, imageWidth, imageHeight), screenShotContent, galleryPictureStyle))
        {
            oLoadAS3DManager.LoadGame(item.gameId);
            isLoading = true;
        }

        float startWidth = startStyle.normal.background.width;

        for (int i = 0; i < item.oInfo.rating; i++)
        {
            Rect ratingRect = new Rect(itemRect.width - 25 - (startWidth) * i , ratingHeaderRect.y + 17, 15, 15);
            GUI.Label(ratingRect, GUIContent.none, startStyle);
        }

        GUILayout.EndArea();
    }

    private int progress = 0;
    private void ShowProcessing()
    {
        // Waiting for game loading
        if (LoadAS3DManager.oGameData != null)
        {
            // We're going to show gallery
            LoadAS3DManager.oGameData.ShowGallery = true;
        }
        else
            return;        

        Rect processingRect = new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 100);

        GUI.Box(processingRect, GUIContent.none, needBoxStyle);
        Rect insideProcessingRect = processingRect;
        insideProcessingRect.y += 20;
        insideProcessingRect.height -= 20;

        GUILayout.BeginArea(insideProcessingRect);
        GUILayout.Label("Loading...", labelStyle);

        if (oLoadAS3DManager.RequestFinished)
        {
            if (LoadAS3DManager.oGameData != null)
            {
                if (LocationManager.PickedLocation == null)
                {
                    LocationManager.Init();

                    if (LoadAS3DManager.oGameData.Location >= 0)
                        LocationManager.PickedLocation = LocationManager.Locations[LoadAS3DManager.oGameData.Location];
                    else
                        LocationManager.PickedLocation = LocationManager.Locations[0];
                }

                if (LocationManager.StreamList[LoadAS3DManager.oGameData.Location] != null)
                {
                    LoadGame();
                }
                else
                {
                    if (!LocationManager.PickedLocation.IsDownloading() && !LocationManager.PickedLocation.IsDownloaded())
                    {
                        Debug.Log("Downloading started");
                        LocationManager.PickedLocation.Download();
                    }
                    else
                    {
                        progress = Mathf.RoundToInt(LocationManager.PickedLocation.GetDownloadProgress() * 100);

                        GUILayout.Label(string.Format("{0}%", progress), centeredHeaderStyle);

                        if (LocationManager.PickedLocation.IsDownloaded())
                        {
                            LocationManager.StreamList[LoadAS3DManager.oGameData.Location] = LocationManager.PickedLocation.Stream;
                            LoadGame();
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Loading failed");
            }
        }
        else
            GUILayout.Label("Requesting game info...", labelStyle);

        GUILayout.EndArea();
    }

    private void LoadGame()
    {
        if (Application.CanStreamedLevelBeLoaded("FloorPlans"))
        {
            Application.LoadLevel("FloorPlans");
        }
    }

    private string introText = "Every house design solves the challenges of people and place differently. Tour these houses designed by other visitors to Architect Studio 3D and think about how well the architect met those challenges...";
    private string introTextAll = "Every house design solves the challenges of people and place differently. Tour these houses designed by other visitors to Architect Studio 3D and think about how well the architect met those challenges. Think about how well the architect meets client needs and the particular qualities of the location. Then rate the designs. Be thoughtful about your rating, asking yourself";
    private string[] cases = { "• Did the architect choose a floor plan that works well with the natural characteristics of the site?",
                               "• Does the interior layout of rooms respond to the client's needs and preferences?",
                               "• Do you think the architect was attempting to design a tastefully-decorated environment or was the architect's goal to create a funky, fun space? Was the architect effective in that attempt?" };

    private string moreLinkText = "Read More";

    public string sortBy = "Sort by:";
    public string rating = "Rating:";

    public string readMoreLink = "Read More";

    private void Header()
    {
        Rect logoRect = new Rect(40, 40, 210, 24);
        GUI.Box(logoRect, GUIContent.none, logoStyle);

        Rect upperLineRect = new Rect(logoRect.x + 125, logoRect.y - 25, 465, 1);
        GUI.Box(upperLineRect, GUIContent.none, lineStyle);

        GUIContent sortByContent = new GUIContent(sortBy);
        Vector2 sortBySize = needsTextStyle.CalcSize(sortByContent);

        Rect introTextRect = new Rect(logoRect.x + logoRect.width + 20, logoRect.y - 5, 605, 30);
        GUI.Label(introTextRect, introText, needsTextStyle);

        Vector2 moreLinkSize = redLinkStyle.CalcSize(new GUIContent(moreLinkText));
        Rect moreLinkRect = introTextRect;
        moreLinkRect.width = moreLinkSize.x;
        moreLinkRect.height = redLinkStyle.fixedHeight;
        moreLinkRect.x += introTextRect.width - 40;
        moreLinkRect.y += introTextRect.height - moreLinkRect.height - 2;

        if (GUI.Button(moreLinkRect, moreLinkText, redLinkStyle))
        {
            showMoreAboutGallery = !showMoreAboutGallery;
        }

        Rect sortByRect = new Rect(logoRect.x + 80, logoRect.y + logoRect.height + 20, sortBySize.x, sortBySize.y);
        GUI.Label(sortByRect, sortByContent, needsTextStyle);

        pickedMenu = DrawMenu(new Vector2(sortByRect.x + sortByRect.width + 5, sortByRect.y), pickedMenu);

    }

    private string DrawMenu(Vector2 p_start, string p_picked)
    {
        float marginLeft = 0;

        for (int i = 0; i < sortMenu.Count; i++)
        {
            string buttonText = sortMenu[i].ToUpper();
            GUIContent buttonContent = new GUIContent(buttonText);
            Vector2 buttonSize = headerButtonStyle.CalcSize(buttonContent);
            buttonSize.x *= 2;

            Rect buttonRect = new Rect(p_start.x + marginLeft, p_start.y, buttonSize.x, buttonSize.y);

            if (GUI.Button(buttonRect, buttonContent, p_picked.Equals(sortMenu[i]) ? selectedHeaderButtonStyle : headerButtonStyle))
            {
                if (LoadAS3DManager.GameList != null)
                    SortGameList(sortMenu[i], i);

                for (int j = 0; j < reverts.Count; j++)
                {
                    if (j == i)
                    {
                        reverts[j] = !reverts[j];
                    }
                    else
                        reverts[j] = false;
                }

                return sortMenu[i];
            }

            marginLeft += buttonSize.x;
        }

        return p_picked;
    }

    private void SortGameList(string p_sort, int index)
    {
        switch (p_sort)
        {
            case "age":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return !reverts[index] ? p1.oInfo.age.CompareTo(p2.oInfo.age) : p2.oInfo.age.CompareTo(p1.oInfo.age); });
                break;
            case "architect":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return !reverts[index] ? p1.oInfo.name.CompareTo(p2.oInfo.name) : p2.oInfo.name.CompareTo(p1.oInfo.name); });
                break;
            case "date submitted":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return !reverts[index] ? p1.oInfo.dateOfSubmition.CompareTo(p2.oInfo.dateOfSubmition) : p2.oInfo.dateOfSubmition.CompareTo(p1.oInfo.dateOfSubmition); });
                break;
            case "floorplan":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return (p1.oExterior != null && p2.oExterior != null) ? !reverts[index] ? p1.oExterior.id.CompareTo(p2.oExterior.id) : p2.oExterior.id.CompareTo(p1.oExterior.id) : 0; });
                break;
            case "rating":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return !reverts[index] ? p1.oInfo.rating.CompareTo(p2.oInfo.rating) : p2.oInfo.rating.CompareTo(p1.oInfo.rating); });
                break;
            case "state":
                LoadAS3DManager.GameList.Sort(delegate(GameData p1, GameData p2) { return !reverts[index] ? p1.oInfo.state.CompareTo(p2.oInfo.state) : p2.oInfo.state.CompareTo(p1.oInfo.state); });
                break;
        }
    }

}

