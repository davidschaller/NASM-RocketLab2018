using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LayoutGUI : MonoBehaviour
{
    public GUISkin selectionSkin,
                   hintSkin,
                   labelSkin;

    private GUIStyle selectionButtonStyle,
                     headerStyle,
                     textStyle;

    void Awake()
    {
        ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.Layout, GetComponent<LayoutGUI>());

        LocationManager.Init();

        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.Location >= 0)
        {
            locIndex = LoadAS3DManager.oGameData.Location;
            LocationManager.PickedLocation = LocationManager.Locations[LoadAS3DManager.oGameData.Location];

            LocationManager.PickedLocation.Stream = LocationManager.StreamList[locIndex];
        }
        else
            LocationManager.PickedLocation = LocationManager.Locations[0];
        

        selectionButtonStyle = selectionSkin.GetStyle("button");
        headerStyle = hintSkin.GetStyle("label");
        textStyle = labelSkin.GetStyle("label");

        enabled = false;
    }

    void OnEnable()
    {
        if (!ClientManager.IsMyClientsListComplete)
        {
            ClientManager.CompleteList();
        }
    }

    void OnDisable()
    {
    }

    int locIndex = 0;

    void OnGUI()
    {
        GUI.depth = 1;

        Rect imageRect = new Rect(Screen.width / 2 - LocationManager.Locations[locIndex].picture.width * 0.75f / 2 + 50,
            30, LocationManager.Locations[locIndex].picture.width * 0.75f, LocationManager.Locations[locIndex].picture.height * 0.75f);

        GUI.Label(imageRect, LocationManager.Locations[locIndex].picture);

        Vector2 nameSize = headerStyle.CalcSize(new GUIContent(LocationManager.Locations[locIndex].name));
        Rect nameRect = new Rect(imageRect.x + imageRect.width / 2 - nameSize.x / 2, imageRect.y + imageRect.height,
								 nameSize.x, nameSize.y);

        GUI.Label(nameRect, LocationManager.Locations[locIndex].name, headerStyle);

		// calc these early for feature list location
        float buttonWidth = selectionButtonStyle.normal.background.width;
        float buttonHeight = selectionButtonStyle.normal.background.height;
        Vector2 leftArrowCenter = new Vector2(imageRect.x - buttonWidth / 2, imageRect.y + buttonHeight / 2);

        Rect features = new Rect(leftArrowCenter.x, nameRect.y + nameRect.height + 10, Screen.width - leftArrowCenter.x,
                                 imageRect.height);

        GUILayout.BeginArea(features);
        GUILayout.BeginVertical();

        foreach (string item in LocationManager.Locations[locIndex].features)
        {
            GUILayout.Label("* " + item, textStyle, GUILayout.Width(imageRect.width));
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();

        if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
        {
            GUIUtility.RotateAroundPivot(180, leftArrowCenter);

            if (GUI.Button(new Rect(imageRect.x - buttonWidth + 10,
                                    imageRect.y - imageRect.height / 4 * 3 + buttonHeight / 2,
                                    buttonWidth,
                                    buttonHeight), string.Empty, selectionButtonStyle))
            {
                locIndex--;
                if (locIndex < 0)
                {
                    locIndex = LocationManager.Locations.Count - 1;
                }

                LocationManager.PickedLocation = LocationManager.Locations[locIndex];
            }

            GUI.matrix = Matrix4x4.identity;

            if (GUI.Button(new Rect(imageRect.x + imageRect.width + 10,
                                    imageRect.y + imageRect.height / 4 * 3 - buttonHeight / 2,
                                    buttonWidth,
                                    buttonHeight), string.Empty, selectionButtonStyle))
            {
                locIndex++;
                if (locIndex > LocationManager.Locations.Count - 1)
                {
                    locIndex = 0;
                }

                LocationManager.PickedLocation = LocationManager.Locations[locIndex];
            }

        }

		/*
		  Don't need button here...
		  since top navbar buttons will not be the primary
		  navigation later, selection of location is implicit
		  (whatever's selected is the selected location..main GUI
		  has 'choose this location' for user feedback)
		*/

        
         
    }
}
