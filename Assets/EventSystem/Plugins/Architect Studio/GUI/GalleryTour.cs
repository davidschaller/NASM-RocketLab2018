using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GalleryTour : MonoBehaviour
{
    public GUISkin buttonSkin, hintSkin, LabelSkin;
    public Camera playerCamera;
    public GameObject fpsController;
    public Texture2D star,
                     starHovered;

    public Camera cam3D;

    private SaveAS3DManager oSaveAS3DManager = null;

    private GUIStyle view3DStyle;
    private CombineChildrenExtended[] combiners;

    void Awake()
    {
        ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.GalleryTour, GetComponent<GalleryTour>());
        oSaveAS3DManager = (SaveAS3DManager)GameObject.FindObjectOfType(typeof(SaveAS3DManager));

        starContent = new GUIContent(star);
        pickedStarContent = new GUIContent(starHovered);
	    
		if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.oInfo != null)
		{
			pickedRating = LoadAS3DManager.oGameData.oInfo.rating - 1;
		}

        view3DStyle = hintSkin.GetStyle("view3d");

        combiners = (CombineChildrenExtended[])GameObject.FindObjectsOfType(typeof(CombineChildrenExtended));
    }

    void OnEnable()
    {
        if (fpsController)
        {
            fpsController.active = true;
            foreach (Transform tr in fpsController.transform)
                tr.gameObject.active = true;
        }

        if (cam3D)
            cam3D.gameObject.SetActiveRecursively(true);

        if (playerCamera)
        {
            playerCamera.rect = new Rect(0.15f, 0.23f, 0.84f, 0.73f);
        }

        StartCoroutine(Combine());   
    }

    void OnDisable()
    {
        if (fpsController)
        {
            foreach (Transform tr in fpsController.transform)
            {
                tr.gameObject.active = false;
            }
            fpsController.active = false;
        }

        if (cam3D)
            cam3D.gameObject.SetActiveRecursively(false);

        Decombine();
    }

    private IEnumerator Combine()
    {
        yield return new WaitForSeconds(3);

        BuildingManager.Picked.Combiner.Combine();
        
        if (combiners != null)
        {
            foreach (CombineChildrenExtended combiner in combiners)
            {
                combiner.Combine();
            }
        }
    }

    private void Decombine()
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

    void OnGUI()
    {
        if (BuildingManager.Picked == null || !playerCamera)
            return;

        Rect view3DRect = playerCamera.pixelRect;
        view3DRect.y = Screen.height - playerCamera.pixelRect.y - playerCamera.pixelRect.height;

        GUI.Box(view3DRect, GUIContent.none, view3DStyle);

        DrawRating();

        if (isInfoSyned)
        {
            oSaveAS3DManager.SaveUserInfo(LoadAS3DManager.oGameData.GameId, new Info(LoadAS3DManager.oGameData.oInfo, pickedRating));

            isInfoSyned = false;
        }
    }

    private bool isInfoSyned = false;

    GUIContent starContent, pickedStarContent;

    int pickedRating = 0;

    private void DrawRating()
    {
        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.oInfo != null)
        {
            GUI.skin = buttonSkin;

            Rect infoRect = new Rect(playerCamera.pixelRect.x,
                  playerCamera.pixelRect.y + playerCamera.pixelRect.height - 100, playerCamera.pixelRect.width, 120);

            GUI.Box(infoRect, GUIContent.none);

            GUILayout.BeginArea(infoRect);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            GUILayout.Label("RATING", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label("ARCHITECT", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label("AGE", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label("STATE", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label("FLOORPLAN", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label("DATE");

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            for (int i = 0; i < 5; i++)
            {
                GUIContent content;
                
                if (pickedRating < i)
                    content = starContent;
                else
                    content = pickedStarContent;

                if (GUILayout.Button(content, GUIStyle.none, 
                        GUILayout.Width(playerCamera.pixelRect.width / 30)))
                {
                    isInfoSyned = true;
                    pickedRating = i;
                }
            }

            GUILayout.Label(LoadAS3DManager.oGameData.oInfo.name ?? "anonymous", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label(LoadAS3DManager.oGameData.oInfo.age.ToString(), GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label(LoadAS3DManager.oGameData.oInfo.state ?? "none", GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label(BuildingManager.Picked.selection, GUILayout.Width(playerCamera.pixelRect.width / 6));
            GUILayout.Label(LoadAS3DManager.oGameData.oInfo.dateOfSubmition.ToString("MM dd yyyy"), 
                GUILayout.Width(playerCamera.pixelRect.width / 6));

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();            
        }
    }

}
