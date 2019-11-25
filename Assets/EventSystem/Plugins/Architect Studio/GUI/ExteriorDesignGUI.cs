using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ExteriorDesignGUI : MonoBehaviour
{
	public Camera scrollCam;
	public Camera cam3D;
	public GUISkin tabSkin;
    public GUISkin selectionSkin,
                   buttonSkin,
                   labelSkin,
                   hintSkin;

    Texture2D borderTex; // border for picked item

    private GUIStyle scrollingHintStyle,
                     btnStyle,
                     activeBtnStyle,
                     roofLabelStyle,
                     arrowButtonStyle;

	private void Awake ()
	{
		ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.DesignExterior, GetComponent<ExteriorDesignGUI>());

        borderTex = LoadSelectedButtonStyle();

        BuildingManager.Init();
        BuildingManager.InstantiateBuildingPrefabs();

        StartCoroutine(TryLoadSavedExterior());

        SetGUIStyles();

        this.enabled = false;

        BuildingManager.DeactivateAllBuildings();
	}

    private void SetGUIStyles()
    {
        scrollingHintStyle = labelSkin.GetStyle("scrollingHint");
        roofLabelStyle = labelSkin.GetStyle("roofLabel");

        btnStyle = new GUIStyle();
        btnStyle.imagePosition = ImagePosition.ImageOnly;
        btnStyle.wordWrap = true;
        btnStyle.alignment = TextAnchor.MiddleCenter;

        activeBtnStyle = new GUIStyle(btnStyle);
        activeBtnStyle.normal.background = borderTex;
        arrowButtonStyle = selectionSkin.GetStyle("button");        
    }

    private Texture2D LoadSelectedButtonStyle()
    {
        Texture2D result = new Texture2D(64, 64);

        for (int i = 0; i <= result.height; i++)
            for (int j = 0; j <= result.width; j++)
                {
                    if (i < 2 || j < 2 || i >= result.height - 2 || j >= result.width - 2)
                        result.SetPixel(i, j, new Color(0.7f, 0.4f, 0.2f, 1));
                    else
                        result.SetPixel(i, j, new Color(0.7f, 0.4f, 0.2f, 0));
                }

        result.Apply();

        return result;
    }

	private void OnEnable ()
	{
        BuildingManager.ActivateAllBuildings();

        foreach (ItemDefinition interiorItem in InteriorDesignGUI.GridItems)
            interiorItem.ActivateRenderers();

        foreach (ItemDefinition item in LandscapingGUI.GridItems)
            item.ActivateRenderers();

        scrollCam.gameObject.SetActiveRecursively(true);
        cam3D.gameObject.SetActiveRecursively(true);

        if (BuildingManager.Picked != null && BuildingManager.Picked.PickedRoof != null)
        {
            BuildingManager.Picked.ActivateRenderers();
            

            if (InteriorDesignGUI.SwitcherRoofIndex == 0)
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
            else
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");
        }
	}

	private void OnDisable ()
	{
        scrollCam.gameObject.SetActiveRecursively(false);
        cam3D.gameObject.SetActiveRecursively(false);

        BuildingManager.DeactivateAllBuildings();
        if (BuildingManager.Picked != null)
        {
            BuildingManager.Picked.DeactivateRenderers(true, true);

            foreach (ItemDefinition interiorItem in InteriorDesignGUI.GridItems)
                interiorItem.DeactivateRenderers();

            foreach (ItemDefinition item in LandscapingGUI.GridItems)
                item.DeactivateRenderers();
        }
	}

    private bool isBuildingScrollCamModified = false;

	private void Update ()
	{
        // Need to change this only once (it breaks after switch to the Scene View)
        if (!isBuildingScrollCamModified)
        {
            scrollCam.rect = new Rect(selectionBox.x / Screen.width,
                                              (Screen.height - selectionBox.y) / Screen.height - scrollCam.rect.height,
                                              selectionBox.width / Screen.width,
                                              scrollCam.rect.height);
            isBuildingScrollCamModified = true;
        }
	}

	Rect selectionBox = new Rect(200, 430, 680, 87);
	public enum TabStages
	{
		FloorPlan,
		HouseHeight,
		ExteriorMaterials,
		Roof
	}
	private static TabStages tabStage = TabStages.FloorPlan;
    public static TabStages TabStage
    {
        get
        {
            return tabStage;
        }
        private set { }
    }

    Rect contentPanel;
    float planScroller,
          materialScroller,
          roofScroller;
    

	void TabScrollerView ()
	{
		GUI.skin = tabSkin;
		GUIStyle tabButtonStyle = tabSkin.GetStyle("Button");
		GUIStyle selectedButtonStyle = new GUIStyle(tabSkin.GetStyle("Button"));
		selectedButtonStyle.normal = selectedButtonStyle.active;

        Rect tabBox = new Rect(selectionBox.x, selectionBox.y - tabButtonStyle.normal.background.height, selectionBox.width, 25);
        contentPanel = tabBox;
        contentPanel.height = selectionBox.height;
        contentPanel.y += tabBox.height - 8;
        contentPanel.width += 42;
        contentPanel.x -= 21;
        GUI.Box(contentPanel, GUIContent.none);

		GUILayout.BeginArea(tabBox);
		GUILayout.BeginHorizontal();

        if (GUILayout.Button("FLOOR PLAN", 
                tabStage == TabStages.FloorPlan ? selectedButtonStyle : tabButtonStyle,
                GUILayout.Width(tabButtonStyle.CalcSize(new GUIContent("FLOOR PLAN")).x + 20)))
        {
            tabStage = TabStages.FloorPlan;
        }
        if (GUILayout.Button("HOUSE HEIGHT", 
                tabStage == TabStages.HouseHeight ? selectedButtonStyle : tabButtonStyle,
                GUILayout.Width(tabButtonStyle.CalcSize(new GUIContent("HOUSE HEIGHT")).x + 20)))
        {
            tabStage = TabStages.HouseHeight;
        }
        if (GUILayout.Button("EXTERIOR MATERIALS", 
                tabStage == TabStages.ExteriorMaterials ? selectedButtonStyle : tabButtonStyle,
                GUILayout.Width(tabButtonStyle.CalcSize(new GUIContent("EXTERIOR MATERIALS")).x + 20)))
        {
            tabStage = TabStages.ExteriorMaterials;
        }
        if (GUILayout.Button("ROOF", 
                tabStage == TabStages.Roof ? selectedButtonStyle : tabButtonStyle,
                GUILayout.Width(tabButtonStyle.CalcSize(new GUIContent("ROOF")).x + 20)))
        {
            tabStage = TabStages.Roof;
        }
		GUILayout.EndHorizontal();
		GUILayout.EndArea();

        string hintText = string.Empty;

        switch (tabStage)
        {
            case TabStages.HouseHeight:
                HouseHeightTab();
                hintText = "Click to choose a house height";
                break;
            case TabStages.ExteriorMaterials:
                ExteriorMaterialsTab();
                hintText = "Click to choose exterior materials";
                break;
            case TabStages.Roof:
                RoofTab();
                hintText = "Click to choose a roof";
                break;
            default:
                FloorPlanTab();
                hintText = "Click to choose a floor plan";
                break;
        }

        // Hint under the scrolling panel
        GUI.Label(new Rect(contentPanel.x + 20, contentPanel.y + contentPanel.height, 300, 30),
                    hintText, scrollingHintStyle);


	}

    private Rect PositionBuildingsInScroller(Rect scroller, float offset)
    {
        Rect btnRect = new Rect();

        GUI.BeginGroup(scroller);
        int mult = 0;

        foreach (BuildingDefinition building in BuildingManager.BuildingsList)
        {
            btnRect = new Rect(10 + (67 + 23) * mult + offset, 10, 67, 67);

            if (GUI.Button(btnRect, building.selection,
                 (BuildingManager.Picked != null && BuildingManager.Picked.plan == building.plan) ?
                    activeBtnStyle : btnStyle))
            {
                PickBuilding(building);
            }
            mult++;
        }
        GUI.EndGroup();

        return btnRect; // return Rect of the last building
    }

    private Rect PositionExMaterialsInScroller(Rect scroller, float offset)
    {
        Rect btnRect = new Rect();

        GUI.BeginGroup(scroller);
        int mult = 0;

        foreach (ExMatDefinition exMaterial in ExMaterialManager.ExMaterialList)
        {
            btnRect = new Rect(10 + (67 + 23) * mult + offset, 10, 67, 67);

            if (GUI.Button(btnRect, exMaterial.image,
                 (BuildingManager.Picked.PickedExteriorMaterial == exMaterial) ?
                    activeBtnStyle : btnStyle))
            {
                PickExteriorMaterial(exMaterial);
            }
            mult++;
        }
        GUI.EndGroup();

        return btnRect; // return Rect of the last material
    }

    private void PickExteriorMaterial(ExMatDefinition p_exMaterial)
    {
        BuildingManager.Picked.PickedExteriorMaterial = p_exMaterial;
        InteriorDesignGUI.UpdateWalls();
    }

    private Vector3 startRoofPos;

    private void PickBuilding(BuildingDefinition picked)
    {
        WallManager.RemoveAllWalls();
        CorruptedParser.ClearCorruptedSectionsList();

        if (BuildingManager.Picked != null)
        {
            BuildingManager.RemoveRoofsFromScroller();
            Destroy(BuildingManager.Picked.gameObject);

            InteriorDesignGUI.ResetGridItems();
        }

        GameObject placement = GameObject.Find("Building Placement");
        BuildingManager.Picked = (BuildingDefinition)GameObject.Instantiate(picked,
                                                              placement.transform.position + picked.offSet,
                                                              placement.transform.rotation);

        BuildingManager.Picked.gameObject.name = "Building (3D View)";

        BuildingManager.Picked.Combiner = BuildingManager.Picked.gameObject.AddComponent<CombineChildrenExtended>();

        SetupCeilings(picked.PickedMarkerType);

        GameObject goInteriorWalls = new GameObject("Interior Walls");
        goInteriorWalls.transform.parent = BuildingManager.Picked.transform;

        GameObject goInteriorFloors = new GameObject("Interior Floors");
        goInteriorFloors.transform.parent = BuildingManager.Picked.transform;

        BuildingManager.Picked.PickedMarkerType = picked.PickedMarkerType;

        startRoofPos = scrollCam.transform.position + scrollCam.transform.TransformDirection(Vector3.forward) +
            scrollCam.transform.TransformDirection(Vector3.up) * 3.5f + (scrollCam.transform.TransformDirection(Vector3.left) * 80f);

        BuildingManager.Picked.fronts = picked.fronts;
        BuildingManager.Picked.plan = picked.plan;
        BuildingManager.Picked.sides = picked.sides;
        BuildingManager.Picked.FloorTiles = picked.FloorTiles;

        // Initialize exterior materials
        ExMaterialManager.Init();

        BuildingManager.Picked.PickedExteriorMaterial = ExMaterialManager.ExMaterialList[0];
    }

    private void SetupCeilings(RoofMarker.MarkerTypes p_markerTypes)
    {
        BuildingManager.Picked.Ceiling.localPosition =
            BuildingManager.Picked.PickedRoofMarker(BuildingManager.Picked.PickedMarkerType).MarkerPosition;
    }

    Rect lastBuildingRect,
         lastExMaterialRect,
         lastRoofRect;

    private void FloorPlanTab()
    {
        scrollCam.cullingMask = Common.SetCullingMask("Invisible");

        Rect insidePanel = contentPanel;
        insidePanel.x += 25;
        insidePanel.width -= 50;

        lastBuildingRect = PositionBuildingsInScroller(insidePanel, planScroller);
        
        float buttonWidth = arrowButtonStyle.normal.background.width * 0.85f;
        float buttonHeight = arrowButtonStyle.normal.background.height * 0.85f;

        Vector2 leftArrowCenter = new Vector2(selectionBox.x - buttonWidth / 2, selectionBox.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(180, leftArrowCenter);
        if (GUI.Button(new Rect(selectionBox.x - buttonWidth + 10,
                                 selectionBox.y - buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) && planScroller < 0)
        {
            planScroller += lastBuildingRect.width / 2;
        }

        GUI.matrix = Matrix4x4.identity;

        // scrolling view is drawn with orthographic camera
        if (GUI.Button(new Rect(selectionBox.x + selectionBox.width + 10,
                                 selectionBox.y + buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) &&
            LastItemInView(contentPanel, lastBuildingRect))
        {
            planScroller -= lastBuildingRect.width / 2;
        }
    }

    private void ExteriorMaterialsTab()
    {
        if (!BuildingManager.Picked)
            return;

        scrollCam.cullingMask = Common.SetCullingMask("Invisible");

        Rect insidePanel = contentPanel;
        insidePanel.x += 25;
        insidePanel.width -= 50;

        lastExMaterialRect = PositionExMaterialsInScroller(insidePanel, materialScroller);

        float buttonWidth = arrowButtonStyle.normal.background.width * 0.85f;
        float buttonHeight = arrowButtonStyle.normal.background.height * 0.85f;

        Vector2 leftArrowCenter = new Vector2(selectionBox.x - buttonWidth / 2, selectionBox.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(180, leftArrowCenter);
        if (GUI.Button(new Rect(selectionBox.x - buttonWidth + 10,
                                 selectionBox.y - buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) && planScroller < 0)
        {
            materialScroller += lastExMaterialRect.width / 2;
        }

        GUI.matrix = Matrix4x4.identity;

        if (GUI.Button(new Rect(selectionBox.x + selectionBox.width + 10,
                                 selectionBox.y + buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) &&
            LastItemInView(contentPanel, lastExMaterialRect))
        {
            materialScroller -= lastExMaterialRect.width / 2;
        }
    }

    private IEnumerator TryLoadSavedExterior()
    {
        while (InteriorItemManager.InteriorItemsList == null)
            yield return 0;

        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.oExterior != null)
        {
            Debug.Log("BuildingManager.BuildingsList.Count = " + BuildingManager.BuildingsList.Count + ", id = " + LoadAS3DManager.oGameData.oExterior.id);

            BuildingDefinition picked = BuildingManager.BuildingsList.Find(
                delegate(BuildingDefinition p)
                {
                    return p.id.Equals(LoadAS3DManager.oGameData.oExterior.id);
                });

            if (picked != null)
            {
                PickBuilding(picked);
                BuildingManager.Picked.PickedExteriorMaterial = ExMaterialManager.GetMaterialById(LoadAS3DManager.oGameData.oExterior.pickedMaterial);

                BuildingManager.Picked.PickedMarkerType = LoadAS3DManager.oGameData.oExterior.pickedMarkerType;
                UpdateInteriorWallsHeight(BuildingManager.Picked.PickedMarkerType);
                UpdateDoorsHeaders(BuildingManager.Picked.PickedMarkerType);

                if (!string.IsNullOrEmpty(LoadAS3DManager.oGameData.oExterior.pickedRoof))
                {
                    RoofDefinition pickedRoof = BuildingManager.Picked.Roofs.Find(delegate(RoofDefinition p) 
                        {
                            int pickedRoofID = 0;
                            
                            int.TryParse(LoadAS3DManager.oGameData.oExterior.pickedRoof, out pickedRoofID);

                            return p.id == pickedRoofID;
                        });

                    if (pickedRoof != null)
                    {
                        PickRoof(pickedRoof);
                    }
                    else
                        Debug.LogWarning("Saved roof was founded but was not instantiated");
                }
                else
                    Debug.Log("Coudn't find saved roof");
            }
            else
                Debug.LogWarning("Couldn't find saved building");
        }
    }

    private bool LastItemInView(Rect scrollingRect, Rect itemRect)
    {
        return (scrollingRect.x + itemRect.x + itemRect.width * 2) >= (scrollingRect.x + scrollingRect.width);
    }

    private Texture2D hItemTexture;         // Use this in Height Tab for buttons

    private float hItemWidth = 53f;         // Width if buttons in Height Tab
    private float hTabPaddingLeft = 44f;
    private float hTabPaddingBottom = 15f;
    private float hItemSpace = 24f;
    private float borderItemWidth = 70f;    // height is the same
    private float borderMarginTop = 10f;

    private void HouseHeightTab()   
    {
        scrollCam.cullingMask = Common.SetCullingMask("Invisible");

        GUIStyle buttonStyle = new GUIStyle(),
                 labelStyle = new GUIStyle();

        if (hItemTexture == null)
        {
            hItemTexture = new Texture2D(64, 64);
            for (int i = 0; i <= hItemTexture.height; i++)
                for (int j = 0; j <= hItemTexture.width; j++)
                {
                    if (i == 1 || j == 1 || i == hItemTexture.height - 1 || j == hItemTexture.width - 1)
                        hItemTexture.SetPixel(i, j, Color.black);
                    else
                        hItemTexture.SetPixel(i, j, new Color(0.7f, 0.7f, 0.7f, 1));
                }

            hItemTexture.Apply();
        }

        buttonStyle.normal.background = hItemTexture;
        labelStyle.normal.textColor = Color.black;

        GUIStyle borderStyle = new GUIStyle();
        borderStyle.normal.background = borderTex;

        if (BuildingManager.Picked != null && BuildingManager.Picked.RoofMarkers != null)
        {
            int multiplier = 0;
            foreach (RoofMarker marker in BuildingManager.Picked.RoofMarkers)
            {
                Rect buttonRect = new Rect(hTabPaddingLeft + contentPanel.x + (hItemWidth + hItemSpace) * multiplier,
                    contentPanel.y + contentPanel.height - RoofMarker.GetLabelHeight(marker.MarkerType) - hTabPaddingBottom,
                    hItemWidth, RoofMarker.GetLabelHeight(marker.MarkerType));

                Rect borderRect = new Rect(buttonRect.x + buttonRect.width / 2 - borderItemWidth / 2,
                                           contentPanel.y + borderMarginTop,
                                           borderItemWidth,
                                           borderItemWidth);

                if (BuildingManager.Picked.PickedMarkerType == marker.MarkerType)
                {
                    GUI.Box(borderRect, GUIContent.none, borderStyle);
                }

                if (GUI.Button(buttonRect, GUIContent.none, buttonStyle))
                {
                    BuildingManager.Picked.PickedMarkerType = marker.MarkerType;

                    UpdateInteriorWallsHeight(marker.MarkerType);
                    UpdateDoorsHeaders(marker.MarkerType);
                    InteriorDesignGUI.UpdateWalls();

                    if (BuildingManager.Picked.PickedRoof != null)
                    {
                        BuildingManager.Picked.PickedRoof.transform.localPosition =
                            BuildingManager.Picked.PickedRoofMarker(BuildingManager.Picked.PickedMarkerType).MarkerPosition;
                    }

                    SetupCeilings(BuildingManager.Picked.PickedMarkerType);

                    UpdateCeilingLampHeights();
                }

                GUIContent label1 = new GUIContent(marker.LabelText.Split(' ')[0]);
                Vector2 label1Size = buttonStyle.CalcSize(label1);
                Rect label1Rect = new Rect(buttonRect.x + buttonRect.width / 2 - label1Size.x / 2,
                                   buttonRect.y - label1Size.y * 2,
                                   label1Size.x,
                                   label1Size.y);

                GUIContent label2 = new GUIContent(marker.LabelText.Split(' ')[1]);
                Vector2 label2Size = buttonStyle.CalcSize(label2);
                Rect label2Rect = new Rect(buttonRect.x + buttonRect.width / 2 - label2Size.x / 2,
                                   buttonRect.y - label2Size.y,
                                   label2Size.x,
                                   label2Size.y);

                if (multiplier > 1)
                {
                    label1Rect.y += buttonRect.height - 10;
                    label2Rect.y += buttonRect.height - 10;
                }

                GUI.Label(label1Rect, label1, labelStyle);
                GUI.Label(label2Rect, label2, labelStyle);


                multiplier++;
            }
        }
    }

    private void UpdateCeilingLampHeights()
    {
        List<InteriorItemDefinition> ceilingLamps = new List<InteriorItemDefinition>();

        ceilingLamps = InteriorDesignGUI.GridItems.FindAll(delegate(InteriorItemDefinition p)
        {
            return p.text.Contains("Ceiling");
        });

        if (ceilingLamps != null)
        {
            foreach (InteriorItemDefinition lamp in ceilingLamps)
            {
                lamp.transform.position = lamp.ClosestMarker +
                    new Vector3(0, BuildingManager.Picked.Ceiling.localPosition.y, 0);
            }
        }

    }

    private void UpdateInteriorWallsHeight(RoofMarker.MarkerTypes p_markerTypes)
    {
        foreach (Transform item in BuildingManager.Picked.InteriorWalls)
        {
            BuildingManager.Picked.ShowWallByRoofMarker(item, BuildingManager.Picked.PickedMarkerType);
        }
    }

    public static void UpdateDoorsHeaders(RoofMarker.MarkerTypes p_markerTypes)
    {
        if (InteriorDesignGUI.GridItems == null)
            return;

        foreach (InteriorItemDefinition item in InteriorDesignGUI.GridItems)
        {
            Transform tr2 = item.transform.Find("2FootDoorHeader"),
                      tr4 = item.transform.Find("4FootDoorHeader"),
                      tr7 = item.transform.Find("7FootDoorHeader");

            if (tr2 && tr4 && tr7)
            {
                switch (p_markerTypes)
                {
                    case RoofMarker.MarkerTypes.m10Foot:
                        tr2.gameObject.layer = LayerMask.NameToLayer("Interior");
                        tr4.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr7.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        break;
                    case RoofMarker.MarkerTypes.m12Foot:
                        tr2.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr4.gameObject.layer = LayerMask.NameToLayer("Interior");
                        tr7.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        break;
                    case RoofMarker.MarkerTypes.m15Foot:
                        tr2.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr4.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr7.gameObject.layer = LayerMask.NameToLayer("Interior");
                        break;
                    default:
                        tr2.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr4.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        tr7.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        break;
                }
            }
        }
    }

    private void RoofTab()
    {
        if (!BuildingManager.Picked)
            return;

        scrollCam.cullingMask = Common.SetCullingMask("RoofScroll");
        lastRoofRect = BuildingManager.PositionRoofsInScroller(BuildingManager.Picked, scrollCam, startRoofPos);
        lastRoofRect.x -= contentPanel.x + lastRoofRect.width / 2;

        float buttonWidth = arrowButtonStyle.normal.background.width * 0.85f;
        float buttonHeight = arrowButtonStyle.normal.background.height * 0.85f;

        Vector2 leftArrowCenter = new Vector2(selectionBox.x - buttonWidth / 2, selectionBox.y + buttonHeight / 2);
        GUIUtility.RotateAroundPivot(180, leftArrowCenter);
        if (GUI.Button(new Rect(selectionBox.x - buttonWidth + 10,
                                 selectionBox.y - buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) && roofScroller < 0)
        {
            startRoofPos -= scrollCam.transform.TransformDirection(Vector3.left) * 10f;

            roofScroller += 32;
        }

        GUI.matrix = Matrix4x4.identity;

        if (GUI.Button(new Rect(selectionBox.x + selectionBox.width + 10,
                                 selectionBox.y + buttonHeight / 3,
                                 buttonWidth,
                                 buttonHeight), string.Empty, arrowButtonStyle) &&
            LastItemInView(contentPanel, lastRoofRect))
        {
            startRoofPos += scrollCam.transform.TransformDirection(Vector3.left) * 10f;
            roofScroller -= 32;
        }

        Rect insidePanel = contentPanel;
        insidePanel.x += 25;
        insidePanel.width -= 50;

        GUI.BeginGroup(insidePanel);

        foreach (RoofDefinition roof in BuildingManager.Picked.Roofs)
        {
            roof.LabelPos = scrollCam.WorldToScreenPoint(roof.transform.position);

            Rect roofRect = new Rect(roof.LabelPos.x - insidePanel.x - 70, 0, 100, 77);

            GUI.BeginGroup(roofRect);

            GUI.Label(new Rect(10, 50, 100, 10), roof.typeName, roofLabelStyle);
            GUI.Label(new Rect(10, 60, 100, 10), roof.materialName, roofLabelStyle);

            if (roof.IsPicked)
            {
                GUI.Box(new Rect(0, 10, 100, 67), string.Empty, activeBtnStyle);
            }

            GUI.EndGroup();
        }
        GUI.EndGroup();        
    }

	private void OnGUI()
	{
        GUI.depth = 1;

		if (Event.current != null &&
			Event.current.type == EventType.MouseUp &&
			selectionBox.Contains(Event.current.mousePosition))
		{
            Vector2 origCursor = Event.current.mousePosition;

            if (tabStage == TabStages.Roof)
            {
                RoofDefinition clickedRoof = BuildingManager.RoofClicked(scrollCam, origCursor);
                
                if (clickedRoof != null)
                    PickRoof(clickedRoof);
            }
		}
        
		TabScrollerView();

		// selectionSkin is active and used for Labels
		GUIStyle labelStyle = selectionSkin.GetStyle("label");

        GUIStyle borderStyle = tabSkin.GetStyle("Window");

        GUI.Box(new Rect(cam3D.rect.x * Screen.width, 
            (1 - cam3D.rect.y) * Screen.height - 130, 200, 130), GUIContent.none, borderStyle); 

		Vector2 labelSize = labelStyle.CalcSize(new GUIContent("3D View"));
		GUI.Label( new Rect( cam3D.rect.x * Screen.width + cam3D.rect.width/2 * Screen.width - labelSize.x/2,
							(1-cam3D.rect.y) * Screen.height,
							 labelSize.x,
                             labelSize.y), "3D View", labelStyle);

        if (BuildingManager.Picked != null)
        {
            Rect planRect = new Rect(280, 20, BuildingManager.Picked.plan.width, BuildingManager.Picked.plan.height);

            GUI.Label(planRect, BuildingManager.Picked.plan, GUIStyle.none);
            GUIContent content = new GUIContent("Plan View");
            labelSize = labelStyle.CalcSize(content);
            GUI.Label(new Rect(planRect.x + planRect.width / 2 - labelSize.x / 2,
                planRect.y + planRect.height, labelSize.x, labelSize.y), content, labelStyle);

            Rect frontRect = new Rect(280, 260, BuildingManager.Picked.FrontImageByPickedHeight.width, BuildingManager.Picked.FrontImageByPickedHeight.height);
            GUI.Label(frontRect, BuildingManager.Picked.FrontImageByPickedHeight, GUIStyle.none);
            content = new GUIContent("Front Elevation");
            labelSize = labelStyle.CalcSize(content);
            GUI.Label(new Rect(frontRect.x + frontRect.width / 2 - labelSize.x / 2,
                frontRect.y + frontRect.height, labelSize.x, labelSize.y), content, labelStyle);

            Rect sideRect = new Rect(680, 260, BuildingManager.Picked.SideImageByPickedHeight.width, BuildingManager.Picked.SideImageByPickedHeight.height);
            GUI.Label(sideRect, BuildingManager.Picked.SideImageByPickedHeight, GUIStyle.none);
            content = new GUIContent("Side Elevation");
            labelSize = labelStyle.CalcSize(content);
            GUI.Label(new Rect(sideRect.x + sideRect.width / 2 - labelSize.x / 2,
                sideRect.y + sideRect.height, labelSize.x, labelSize.y), content, labelStyle);
        }

	}
                       
    private void PickRoof(RoofDefinition picked)
    {
        if (BuildingManager.Picked != null)
        {
            if (BuildingManager.Picked.PickedRoof != null)
            {
                Destroy(BuildingManager.Picked.PickedRoof.gameObject);
            }

            BuildingManager.Picked.PickedRoof = (RoofDefinition)GameObject.Instantiate(picked,
                                                                        BuildingManager.Picked.transform.position,
                                                                        BuildingManager.Picked.transform.rotation);

            BuildingManager.Picked.PickedRoof.gameObject.name = "Roof (3D View)";
            BuildingManager.Picked.PickedRoof.transform.parent = BuildingManager.Picked.transform;

            BuildingManager.Picked.PickedRoof.transform.localPosition =
                BuildingManager.Picked.PickedRoofMarker(BuildingManager.Picked.PickedMarkerType).MarkerPosition + 
                    BuildingManager.Picked.PickedRoof.offSet;

            foreach (Transform tr in BuildingManager.Picked.PickedRoof.transform)
            {
                tr.gameObject.layer = LayerMask.NameToLayer("Default");
                foreach (Transform item in tr)
                {
                    item.gameObject.layer = LayerMask.NameToLayer("Default");
                }
            }
            BuildingManager.Picked.PickedRoof.LabelPos = picked.LabelPos;
        }        
    }

    private float cameraX = 0;           // Use this to rotate camera around the building by x
    private float cameraY = 0;           // Use this to rotate camera around the building by y
    private float distance = 30f;        // Distance from the Cam3D camera to the building
    public float rotateSpeed = 180;      // Cam3D rotation speed around the building
    public float zoomSpeed = 30;         // Cam3D zoom speed

    public float minCam3DDistance = 4,   // min Cam3D distance to the building
                  maxCam3DDistance = 70; // max Cam3D distance to the building

    /// <summary>
    /// 
    /// This code rotates Cam3D around the building
    /// </summary>
    /// 
    void LateUpdate()
    {
        if (!BuildingManager.Picked)
            return;

        // Zoom out
        if (Input.GetKey(KeyCode.Minus) || Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.KeypadMinus))
        {
            if (distance < maxCam3DDistance)
            {
                distance += zoomSpeed * Time.deltaTime;
            }
            else
                distance = maxCam3DDistance;
        }

        // Zoom in
        if (Input.GetKey(KeyCode.Equals) || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.KeypadPlus))
        {
            if (distance > minCam3DDistance)
            {
                distance -= zoomSpeed * Time.deltaTime;
            }
            else
                distance = minCam3DDistance;
        }

        //Detect mouse drag;
        if (Input.GetMouseButton(0))
        {
            cameraX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            cameraY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        }
        
        // Clamp rotation angle
        cameraY = InteriorDesignGUI.ClampAngle360(cameraY, 10, 85);

        
        // Get Look At center
        Vector3 buildingCenter = BuildingManager.Picked.transform.position +
            BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * BuildingManager.Picked.lookAtOffset.x +
                BuildingManager.Picked.transform.TransformDirection(Vector3.right) * BuildingManager.Picked.lookAtOffset.y;

        Debug.DrawRay(buildingCenter, new Vector3(0, 50, 0), Color.red);

        Quaternion rotation = Quaternion.Euler(cameraY, cameraX, 0);
        Vector3 newCam3DPosition = rotation * new Vector3(0.0f, 0.0f, -distance) +
            buildingCenter;

        // Apply changes 
        cam3D.transform.position = newCam3DPosition;
        cam3D.transform.rotation = rotation;

        // Set Look At
        cam3D.transform.LookAt(buildingCenter);
         
    }

    /// <summary>
    /// This State is finished if:
    ///     1. Building is picked
    ///     2. Roof is picked
    /// </summary>
    /// <returns></returns>
    internal static bool IsFinished()
    {
        return BuildingManager.Picked && BuildingManager.Picked.PickedRoof;
    }
}
