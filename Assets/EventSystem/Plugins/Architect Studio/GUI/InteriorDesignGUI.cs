using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteriorDesignGUI : MonoBehaviour
{
    #region Public parameters

    // Skins
    public GUISkin buttonSkin,
                   selectionSkin,
                   hintSkin,
                   labelSkin;

    // Hint image under the grid
    public Texture2D cell;

    // Preview Camera
    public Camera previewCamera;

    // 3D building view
    public Camera cam3D;

    public float panelWidth = 190,
                 panelHeight = 380;

    public float itemWidth = 40,
                 itemHeight = 50,
                 itemMargin = 10;

    public float panelPaddingLeft = 25,
                 panelPaddingTop = 15,
                 panelPaddingButtom = 30,
                 panelMarginRight = 40,
                 panelMarginTop = 120;

    public string roomText = "Think about where each room should go in the house. Drag room labels to the floorplan to \"rough out\" the room arrangement.",
                  linearFeetText = "linear feet:",
                  linearMetersText = "linear meters:",
                  squareText = "= 3 square feet";

    public float panelMenuSpace = 30;
    public float minFitDistance = 10;

    // Selected item background blink time
    public float alphaDelay = 0.2f;
    // Cam3D rotation speed around the building
    public float rotateSpeed = 130;
    // Cam3D zoom speed
    public float zoomSpeed = 30;

    // min Cam3D distance to the building
    public float minCam3DDistance = 4,
    // max Cam3D distance to the building
                 maxCam3DDistance = 70;

    public string imagePrintUrl = "http://architectstudio3d.org/DB2/Print.php/?id={0}";

    #endregion

    #region Private static parameters

    // History of picked Tabs is here
    private static List<Tabs> pickedTabs;
    private static Tabs currentTab = Tabs.Rooms;

    private static int pickedGridItemIndex = -1;
    // All selected interior items are here
    private static List<InteriorItemDefinition> gridItems;

    private static int furnishingsIndex = 0, // 0..6 - furnishings
                       openingsIndex = 7;    // 7..8 - openings

    private static GameObject gridItemsContaiter,
                              interiorItemsContainer;

    private static InteriorItemDefinition selectedItem;
    // for ROOF ON/ROOF OFF buttons: 0 is ROOF ON
    private static int switcherRoofIndex = 0;

    #endregion

    #region Public static parameters

    public static Tabs CurrentTab
    {
        get
        {
            return currentTab;
        }
    }
    
    public static bool IsWallDrawing
    {
        get
        {
            return selectedItem != null && selectedItem.tab == Tabs.Walls && selectedItem.subtab == SubTabs.Draw;
        }
    }
    public static bool IsWallErasing
    {
        get
        {
            return selectedItem != null && selectedItem.tab == Tabs.Walls && selectedItem.subtab == SubTabs.Erase;
        }
    }
    public static bool IsWallAction
    {
        get
        {
            return selectedItem != null &&
                     selectedItem.tab == Tabs.Walls &&
                        (selectedItem.subtab == SubTabs.Draw ||
                          selectedItem.subtab == SubTabs.Erase ||
                            (selectedItem.subtab == SubTabs.Wall_Coverings && !selectedItem.asNavigation));
        }
    }
    public static bool IsWallCoveringsDrawing
    {
        get
        {
            return selectedItem != null &&
                        selectedItem.tab == Tabs.Walls &&
                            selectedItem.subtab == SubTabs.Wall_Coverings &&
                                !selectedItem.asNavigation;
        }
    }
    public static bool IsFloorDrawing
    {
        get
        {
            return selectedItem != null &&
                      selectedItem.tab == Tabs.Floors &&
                          (selectedItem.subtab == SubTabs.Draw || selectedItem.subtab == SubTabs.Erase);
        }
    }
    public static bool IsCoveringsAction
    {
        get
        {
            return selectedItem != null &&
                      selectedItem.tab == Tabs.Walls &&
                         selectedItem.subtab == SubTabs.Wall_Coverings &&
                            !selectedItem.asNavigation;
        }
    }
    public static List<InteriorItemDefinition> GridItems
    {
        get
        {
            return gridItems;
        }
    }
    public static InteriorItemDefinition SelectedItem
    {
        get
        {
            return selectedItem;
        }
    }
    public static int SwitcherRoofIndex
    {
        get
        {
            return switcherRoofIndex;
        }
        set
        {
            switcherRoofIndex = value;
        }
    }

    #endregion

    #region Private parameters

    // Rotate camera around the building by x
    private float cameraX = 0;
    // Rotate camera around the building by y       
    private float cameraY = 0;
    // Distance from Cam3D camera to the building
    private float cameraDistance = 50f;

    private GUIStyle buttonStyle,
                     interiorButtonStyle,
                     activeInteriorButtonStyle,
                     interiorMenuStyle,
                     roomTextStyle,
                     roomLabelStyle,
                     pickedRoomLabelStyle,
                     drawStyle,
                     eraseStyle,
                     coveringsStyle,
                     itemStyle,
                     eraseFloorStyle,
                     arrowStyle,
                     arrowLabelStyle,
                     pickedTabStyle,
                     pickedItemLabelStyle,
                     switcherLeft,
                     switcherLeftActive,
                     switcherRight,
                     switcherRightActive,
                     hintLabelStyle,
                     view3DStyle,
                     boldHintStyle,
                     headerStyle,
                     centeredHeaderStyle;

    private Rect gridRect,
                 view3DRect;

    private AS3DLayoutGrid layoutGrid;
    private PrinterManager printerManager;

    private GameObject previewPlaceholder;
    
    private string printRandomID = string.Empty;
    
    private Rect rightPanel;

    // Use this to set item position to the cursor properly
    private float dx, dy;
    private float cursorToMarkerDistance = 999;
    private bool noOtherConains = true;

    private bool modifiedOpeningsDetected = false;

    // for 3d switcher button: 0 is PLAN VIEW
    private int switcher3dIndex = 0;   
    // background blink texture for selected item          
    private Texture2D selTex;
    // transparent variable for selTex texture: 0 - 1f
    private float selIAlpha = 0;                    
    private float dTime = 0;
    // multiplier: 1 or -1 always
    private int sign = 1;
    private float previewAngle = 1;
    private bool loadingFinished = false;

    #endregion

    #region Constants

    private const string PREVIEW_PLACEHOLDER = "Preview Placeholder";
    private const int MAX_FURNISHINGS_INDEX = 6,
                      MAX_OPENINGS_INDEX = 8,
                      MIN_OPENINGS_INDEX = MAX_FURNISHINGS_INDEX + 1;

    private const float gridCorrectionY = 25,
                        gridCorrectionX = 10,
                        gridCorrectionWidth = 20,
                        gridCorrectionHeight = 93;

    private const float panelMenuYOffset = 5;

    #endregion

    #region Enums

    public enum Tabs
    {
        Rooms,
        Walls,
        Floors,
        Openings,
        Furnishings,
    }

    public enum SubTabs
    {
        Bathrooms,
        Kitchens,
        Seating,
        Tables,
        Lamps,
        Bedrooms,
        Misc,
        Doors,
        Windows,
        Draw,
        Erase,
        Wall_Coverings
    }

    public enum DragType
    {
        None,
        Copy,
        Replace
    }

    #endregion

    private void Awake ()
	{
		ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.Interior, GetComponent<InteriorDesignGUI>());

        // Start in plan view
        Vector3 angles = cam3D.transform.eulerAngles;
        cameraX = angles.y;
        cameraY = angles.x;

        GetComponent<Camera>().enabled = false;

        gridItems = new List<InteriorItemDefinition>();

        StartCoroutine(StartInitialization());

        pickedTabs = new List<Tabs>();
        pickedTabs.Add(currentTab);

        SetGUIStyles();

        layoutGrid = GetComponent<AS3DLayoutGrid>();

        selTex = new Texture2D(2, 2);
	}

    private void SetGUIStyles()
    {
        buttonStyle = buttonSkin.GetStyle("button");

        interiorButtonStyle = hintSkin.GetStyle("interiorButton");
        activeInteriorButtonStyle = new GUIStyle(interiorButtonStyle);
        activeInteriorButtonStyle.normal = interiorButtonStyle.active;
        activeInteriorButtonStyle.hover = activeInteriorButtonStyle.normal;

        interiorMenuStyle = hintSkin.GetStyle("interiorMenu");
        roomTextStyle = labelSkin.GetStyle("roomtext");

        roomLabelStyle = labelSkin.GetStyle("roomlabel");

        pickedRoomLabelStyle = new GUIStyle(roomLabelStyle);
        pickedRoomLabelStyle.normal = roomLabelStyle.focused;

        drawStyle = buttonSkin.GetStyle("draw");
        eraseStyle = buttonSkin.GetStyle("erase");
        coveringsStyle = buttonSkin.GetStyle("coverings");
        itemStyle = buttonSkin.GetStyle("item");
        eraseFloorStyle = buttonSkin.GetStyle("eraseFloor");
        arrowStyle = selectionSkin.GetStyle("arrow");
        arrowLabelStyle = labelSkin.GetStyle("arrow");
        pickedTabStyle = labelSkin.GetStyle("pickedTab");
        pickedItemLabelStyle = labelSkin.GetStyle("pickedItemLabel");

        switcherLeft = hintSkin.GetStyle("switcherLeft");
        switcherLeftActive = new GUIStyle(switcherLeft);
        switcherLeftActive.normal = switcherLeft.active;
        switcherRight = hintSkin.GetStyle("switcherRight");
        switcherRightActive = new GUIStyle(switcherRight);
        switcherRightActive.normal = switcherRight.active;

        hintLabelStyle = labelSkin.GetStyle("hintLabel");
        boldHintStyle = labelSkin.GetStyle("boldHint");
        view3DStyle = hintSkin.GetStyle("view3d");

        headerStyle = hintSkin.GetStyle("label");

        centeredHeaderStyle = new GUIStyle(headerStyle);
        centeredHeaderStyle.alignment = TextAnchor.MiddleCenter;
    }

	private void OnEnable ()
	{
        layoutGrid.enabled = true;

        InteriorItemManager.ActivateAllInteriorItems();

        if (!BuildingManager.Picked)
            return;

        GetComponent<Camera>().enabled = true;

        ApplyGridRect(GetComponent<Camera>().pixelRect);

        if (CameraSwitcher.Is3DView)
        {
            cam3D = CameraSwitcher.ExpandView(cam3D);
            cam3D.enabled = true;
            previewCamera.enabled = false;
            
            view3DRect = cam3D.pixelRect;
        }
        else
        {
            if (BuildingManager.Picked.PickedRoof)
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");

            cam3D = CameraSwitcher.RestoreView(cam3D);

            cam3D.gameObject.SetActiveRecursively(false);
            previewCamera.enabled = true;
        }

        PreviewSelectedItem(null);

        BuildingManager.Picked.UpdateLayer("Interior");

        printerManager = GameObject.FindObjectOfType(typeof(PrinterManager)) as PrinterManager;
        printRandomID = Guid.NewGuid().ToString();

        rightPanel = new Rect(Screen.width - panelWidth - panelMarginRight, panelMarginTop, panelWidth, panelHeight);
	}

	private void OnDisable ()
	{
        GetComponent<Camera>().enabled = false;

        if (cam3D)
        {
            cam3D = CameraSwitcher.RestoreView(cam3D);
        }
        PreviewSelectedItem(null);

        InteriorItemManager.DeactivateAllInteriorItems();
		if (layoutGrid != null)
			layoutGrid.enabled = false;

        CameraSwitcher.Is3DView = false;
	}

    private void RightPanel()
    {
        GUI.Box(rightPanel, GUIContent.none, interiorMenuStyle);

        currentTab = PanelMenu(currentTab);

        if (!pickedTabs.Contains(currentTab))
            pickedTabs.Add(currentTab);

        switch (currentTab)
        {
            case Tabs.Walls:
                WallsTab();
                break;
            case Tabs.Floors:
                FloorsTab();
                break;
            case Tabs.Openings:
                OpeningsTab();
                break;
            case Tabs.Furnishings:
                FurnishingsTab();
                break;
            default:
                RoomsTab();
                break;
        }
    }

    private void RoomsTab()
    {
        float descriptionHeight = roomTextStyle.CalcHeight(new GUIContent(roomText), rightPanel.width - panelMarginRight * 2);
        Rect descriptionRect = new Rect(rightPanel.x + panelPaddingLeft, rightPanel.y + panelPaddingTop, rightPanel.width - panelPaddingLeft * 2, descriptionHeight);

        GUI.Label(descriptionRect, roomText, roomTextStyle);

        List<InteriorItemDefinition> roomsList = new List<InteriorItemDefinition>();
        roomsList = InteriorItemManager.GetItemsByTabs(Tabs.Rooms, null, false);

        for (int i = 0; i < roomsList.Count; i++)
        {
            Vector2 itemSize = roomLabelStyle.CalcSize(new GUIContent(roomsList[i].text));

            roomsList[i].ItemRect = new Rect(rightPanel.x + rightPanel.width / 2 - itemSize.x / 2,
                                             rightPanel.y + descriptionRect.height + panelPaddingTop * 2 + (itemSize.y + itemMargin) * i,
                                             itemSize.x, itemSize.y);

            DrawItem(roomsList[i], DragType.Copy, true, roomLabelStyle);
        }
    }

    private void WallsTab()
    {
        List<InteriorItemDefinition> wallButtonList = new List<InteriorItemDefinition>();
        wallButtonList = InteriorItemManager.GetItemsByTabs(Tabs.Walls, null, true);

        wallButtonList.Sort(delegate(InteriorItemDefinition p1, InteriorItemDefinition p2)
                                { return p1.sortKey.CompareTo(p2.sortKey); });

        float marginLeft = (rightPanel.width - (drawStyle.fixedWidth * 3)) / 2;

        wallButtonList[0].ItemRect = 
            new Rect(rightPanel.x + marginLeft, rightPanel.y + panelPaddingTop, drawStyle.fixedWidth, drawStyle.fixedHeight);
        DrawItem(wallButtonList[0], DragType.None, true, drawStyle);

        wallButtonList[1].ItemRect = 
            new Rect(rightPanel.x + marginLeft + wallButtonList[0].ItemRect.width, rightPanel.y + panelPaddingTop, eraseStyle.fixedWidth, eraseStyle.fixedHeight);
        DrawItem(wallButtonList[1], DragType.None, true, eraseStyle);

        wallButtonList[2].ItemRect = new Rect(rightPanel.x + marginLeft + wallButtonList[0].ItemRect.width +
            wallButtonList[1].ItemRect.width, rightPanel.y + panelPaddingTop, coveringsStyle.fixedWidth, coveringsStyle.fixedHeight);
        DrawItem(wallButtonList[2], DragType.None, true, coveringsStyle);

        if (selectedItem != null && selectedItem.subtab == SubTabs.Wall_Coverings)
        {
            int vmult2 = 0,
                hmult2 = 0;

            List<InteriorItemDefinition> coveringsList = new List<InteriorItemDefinition>();
            coveringsList = InteriorItemManager.GetItemsByTabs(Tabs.Walls, SubTabs.Wall_Coverings, false);

            for (int j = 0; j < coveringsList.Count; j++)
            {
                coveringsList[j].ItemRect = new Rect(rightPanel.x + panelPaddingLeft + (itemWidth + itemMargin) * (j % 3 == 0 ? hmult2 = 0 : ++hmult2),
                                            rightPanel.y + panelPaddingTop * 2 + drawStyle.fixedHeight + (itemHeight + itemMargin) * (j % 3 == 0 ? vmult2 = j / 3 : vmult2),
                                            itemWidth,
                                            itemHeight);

                DrawItem(coveringsList[j], DragType.None, true, itemStyle);
            }
        }
    }

    private void FloorsTab()
    {
        List<InteriorItemDefinition> eraseFloors = 
            InteriorItemManager.GetItemsByTabs(Tabs.Floors, SubTabs.Erase, false); // Actually only one button, 
                                                                                   // so get it by 0 index from the list

        if (eraseFloors.Count > 0)
        {
            eraseFloors[0].ItemRect = new Rect(rightPanel.x + rightPanel.width / 2 - eraseFloorStyle.fixedWidth / 4,
                                          rightPanel.y + panelPaddingTop, eraseFloorStyle.fixedHeight, eraseFloorStyle.fixedWidth);

            DrawItem(eraseFloors[0], DragType.None, true, eraseFloorStyle);
        }

        List<InteriorItemDefinition> floorsList = new List<InteriorItemDefinition>();
        floorsList = InteriorItemManager.GetItemsByTabs(Tabs.Floors, SubTabs.Draw, false);

        int vmult = 0,
            hmult = 0;

        for (int i = 0; i < floorsList.Count; i++)
        {
            floorsList[i].ItemRect = new Rect(rightPanel.x + panelPaddingLeft + (itemWidth + itemMargin) * (i % 3 == 0 ? hmult = 0 : ++hmult),
                                            rightPanel.y + panelPaddingTop * 2 + eraseFloorStyle.fixedHeight + (itemHeight + itemMargin) * (i % 3 == 0 ? vmult = i / 3 : vmult),
                                            itemWidth,
                                            itemHeight);


            DrawItem(floorsList[i], DragType.None, true, itemStyle);
        }
    }

    private void OpeningsTab()
    {
        openingsIndex = GUIHelpers.DrawSubTabSwitcher(openingsIndex, MIN_OPENINGS_INDEX, MAX_OPENINGS_INDEX, arrowStyle, arrowLabelStyle, pickedTabStyle, rightPanel);

        List<InteriorItemDefinition> openingsList = new List<InteriorItemDefinition>();
        openingsList = InteriorItemManager.GetItemsByTabs(Tabs.Openings, (SubTabs)Enum.GetValues(typeof(SubTabs)).GetValue(openingsIndex), false);
        openingsList.Sort(delegate(InteriorItemDefinition p1, InteriorItemDefinition p2)
        {
            return p1.sortKey.CompareTo(p2.sortKey);
        });


        int vmult = 0,
            hmult = 0;

        for (int i = 0; i < openingsList.Count; i++)
        {
            openingsList[i].ItemRect = new Rect(rightPanel.x + panelPaddingLeft + (itemWidth + itemMargin) * (i % 3 == 0 ? hmult = 0 : ++hmult),
                                        rightPanel.y + itemMargin + (itemHeight + itemMargin) * (i % 3 == 0 ? vmult = i / 3 : vmult),
                                        itemWidth, itemHeight);

            DrawItem(openingsList[i], DragType.Copy, true, itemStyle);
        }

        DrawFooterLabel();
    }

    private void FurnishingsTab()
    {
        furnishingsIndex = GUIHelpers.DrawSubTabSwitcher(furnishingsIndex, 0, MAX_FURNISHINGS_INDEX, arrowStyle, arrowLabelStyle, pickedTabStyle, rightPanel);

        List<InteriorItemDefinition> furnishingsList = new List<InteriorItemDefinition>();
        furnishingsList = InteriorItemManager.GetItemsByTabs(Tabs.Furnishings, (SubTabs)Enum.GetValues(typeof(SubTabs)).GetValue(furnishingsIndex), false);

        furnishingsList.Sort(delegate(InteriorItemDefinition p1, InteriorItemDefinition p2)
        { return p1.sortKey.CompareTo(p2.sortKey); });

        int vmult = 0,
            hmult = 0;

        for (int i = 0; i < furnishingsList.Count; i++)
        {
            furnishingsList[i].ItemRect = new Rect(rightPanel.x + panelPaddingLeft + (itemWidth + itemMargin) * (i % 3 == 0 ? hmult = 0 : ++hmult),
                                        rightPanel.y + itemMargin + (itemHeight + itemMargin) * (i % 3 == 0 ? vmult = i / 3 : vmult),
                                        itemWidth, itemHeight);

            DrawItem(furnishingsList[i], DragType.Copy, true, itemStyle);
        }

        DrawFooterLabel();
    }

    private void DrawFooterLabel()
    {
        if (selectedItem != null)
        {
            Rect itemTextRect = new Rect(rightPanel.x + panelPaddingLeft, rightPanel.y + rightPanel.height - panelPaddingButtom, rightPanel.width - panelPaddingLeft * 2, pickedItemLabelStyle.CalcSize(new GUIContent(selectedItem.text)).y);

            GUI.Label(itemTextRect, !string.IsNullOrEmpty(selectedItem.text) ? selectedItem.text : selectedItem.name, pickedItemLabelStyle);
        }
    }

    private Tabs PanelMenu(Tabs currentTab)
    {
        for (int i = 0; i < Enum.GetNames(typeof(Tabs)).Length; i++)
        {
            Vector2 buttonCenter = new Vector2(rightPanel.x + panelPaddingLeft + (panelMenuSpace) * i, rightPanel.y + panelMenuYOffset);
            GUIUtility.RotateAroundPivot(-60, buttonCenter);
            if (GUI.Button(new Rect(buttonCenter.x,
                                    buttonCenter.y,
                                    100,
                                    20), Enum.GetNames(typeof(Tabs))[i].ToUpper(),
                                    Enum.Equals(currentTab, Enum.GetValues(typeof(Tabs)).GetValue(i)) ?
                                    activeInteriorButtonStyle : interiorButtonStyle))
            {
                pickedGridItemIndex = -1;
                selectedItem = null;
                return (Tabs)Enum.GetValues(typeof(Tabs)).GetValue(i);
            }

            GUI.matrix = Matrix4x4.identity;
        }

        return currentTab;
    }

    private InteriorItemDefinition DrawItem(InteriorItemDefinition item, DragType dragType, bool isButton, GUIStyle itemStyle)
    {
        switch (dragType)
        {
            case DragType.Copy:
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && item.ItemRect.Contains(Event.current.mousePosition))
                {
                    if (isButton)
                    {
                        selectedItem = item;
                    }
                    gridItems.Add(item.CreateCopy(gridItemsContaiter.transform, item.image != null ? GUIStyle.none : itemStyle));
                }
                break;
            case DragType.Replace:
                if (item.IsGrabbed)
                {
                    pickedGridItemIndex = gridItems.IndexOf(item);

                    Vector3 realMarker = Vector3.zero;

                    Vector3 mpos = Common.CursorTo3D(Event.current.mousePosition);

                    Vector3 mWorld = GetComponent<Camera>().ScreenToWorldPoint(mpos);
                    Vector3 anchorPos = layoutGrid.GetNearestIntersection(mWorld);
                    
                    Vector2 closestMarker = BuildingManager.Picked.GetClosestDoorMarker(GetComponent<Camera>(), Event.current.mousePosition, anchorPos, out cursorToMarkerDistance, out realMarker);

                    if (cursorToMarkerDistance < minFitDistance && item.tab == Tabs.Openings)
                    {
                        item.ClosestMarker = realMarker;

                        Vector2 offset = Vector2.zero;

                        if (item.subtab == SubTabs.Doors)
                            offset = GetOffSetByAngle(Convert.ToInt32(item.Angle));

                        item.ItemRect = new Rect(closestMarker.x - item.ItemRect.width / 2 + offset.x, closestMarker.y - item.ItemRect.height / 2 + offset.y, item.ItemRect.width, item.ItemRect.height);

                        ExteriorDesignGUI.UpdateDoorsHeaders(BuildingManager.Picked.PickedMarkerType);
                    }
                    else
                    {
                        item.ItemRect = new Rect(Event.current.mousePosition.x - dx, Event.current.mousePosition.y - dy, item.ItemRect.width, item.ItemRect.height);

                        if (GetComponent<Camera>().enabled)
                        {
                            item.ClosestMarker = GetComponent<Camera>().ScreenToWorldPoint(new Vector2(item.ItemRect.x + item.ItemRect.width / 2, Screen.height - (item.ItemRect.y + item.ItemRect.height / 2)));

                            // Modify item model on the floor
                            item.ClosestMarker = new Vector3(item.ClosestMarker.x, BuildingManager.Picked.Floor.position.y, item.ClosestMarker.z);
                        }
                    }
                }

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                        item.ItemRect.Contains(Event.current.mousePosition) && currentTab != Tabs.Walls && currentTab != Tabs.Floors && item.tab == currentTab && noOtherConains)
                {
                    noOtherConains = false;
                    item.IsGrabbed = true;
                    dx = Mathf.Abs(Event.current.mousePosition.x - item.ItemRect.x);
                    dy = Mathf.Abs(Event.current.mousePosition.y - item.ItemRect.y);
                }
                else if (Event.current.type == EventType.MouseUp && Event.current.button == 0 && item.IsGrabbed)
                {
                    noOtherConains = true;
                    if (!item.IsInside(gridRect))
                        item.RestorePosition();

                    item.IsGrabbed = false;
                    modifiedOpeningsDetected = true;
                }
                break;
            default:
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && item.ItemRect.Contains(Event.current.mousePosition))
                {
                    if (isButton)
                        selectedItem = item;
                }
                break;
        }

        GUIStyle currentStyle = Common.ApplyStyle(itemStyle, item, selectedItem);

        Vector2 rotateCenterOffset = Vector2.zero;
        if (item.subtab == SubTabs.Doors)
            rotateCenterOffset = new Vector2(-8, 0);

        Vector2 itemCenter = new Vector2(item.ItemRect.x + item.ItemRect.width / 2 + rotateCenterOffset.x, item.ItemRect.y + item.ItemRect.height / 2 + rotateCenterOffset.y);

        GUIUtility.RotateAroundPivot(item.Angle, itemCenter);

        if (item.transform)
        {
            Quaternion itemRotation = new Quaternion();
            itemRotation.eulerAngles = new Vector3(0, (item.Angle + 90), 0);
            item.transform.rotation = itemRotation;
        }
        GUI.Box(item.ItemRect, item.image ? new GUIContent(item.image) : new GUIContent(item.text), currentStyle);

        GUI.matrix = Matrix4x4.identity;

        return item;
    }

    /// <summary>
    /// Get symbol offset
    /// </summary>
    /// <param name="p_angle"></param>
    /// 
    private Vector2 GetOffSetByAngle(int angle)
    {
        switch (angle)
        {
            case 0:
                return new Vector2(7, -7);
            case 90:
                return new Vector2(0, 0);
            case 180:
                return new Vector2(7, -7);
            case 270:
                return new Vector2(0, 0f);
            case 45:
                return new Vector2(8, 0);
            case 135:
                return new Vector2(4, -4);
            case 225:
                return new Vector2(0, 8);
            case 315:
                return new Vector2(4, -4);
            default:
                return new Vector2(0, 0);

        }
    }

    private void DrawGridItems(bool restoring)
    {
        for (int i = 0; i < gridItems.Count; i++)
        {
            if (!restoring)
            {
                // Draw item label
                gridItems[i] = DrawItem(gridItems[i], DragType.Replace, false, gridItems[i].image != null ? GUIStyle.none : pickedRoomLabelStyle);
            }

            if (gridItems[i].Model)
            {
                // Instantiate item model
                Quaternion itemRotation;
                PositionItem(gridItems[i], out itemRotation);
            }
            
            // Or remove it, if it's outside of the building
            if (!restoring && !gridItems[i].IsGrabbed && !gridItems[i].IsInside(gridRect))
            {
                pickedGridItemIndex = -1;
                GameObject.Destroy(gridItems[i].gameObject);
                
                gridItems.Remove(gridItems[i]);
                break;
            }
        }
    }

    private static int ClampAngle90(float p_angle)
    {
        int result = Mathf.RoundToInt(p_angle);

        if (Mathf.Abs(180 - result) < 5 || Mathf.Abs(result) < 5)
            result = 0;
        else if (Mathf.Abs(90 - result) < 5 || Mathf.Abs(270 - result) < 5)
            result = 90;

        return result;
    }

    public static void UpdateWalls()
    {
        WallManager.MakeWallsVisible();
        foreach (InteriorItemDefinition item in gridItems)
        {
            if (item.tab == Tabs.Openings)
            {
                UpdateWallsWithOpenings(item.transform.position, item.transform.rotation, item);
            }
        }
    }

    private static void UpdateWallsWithOpenings(Vector3 itemPosition, Quaternion itemRotation, InteriorItemDefinition item)
    {
        int itemAngle = ClampAngle90(itemRotation.eulerAngles.y);

        UpdateInteriorWalls(itemPosition, item, itemAngle);

        UpdateExteriorWalls(itemPosition, item, itemAngle);
    }

    private static void UpdateInteriorWalls(Vector3 itemPosition, InteriorItemDefinition item, int itemAngle)
    {
        foreach (Transform tr in BuildingManager.Picked.InteriorWalls)
        {
            if ((tr.position - itemPosition).magnitude < 1f)
            {
                UpdateOpeningsWallCoverings(item, tr, tr.Find(tr.name).GetComponent<Renderer>().materials[0], tr.Find(tr.name).GetComponent<Renderer>().materials[1], false);

                int trAngle = ClampAngle90(tr.rotation.eulerAngles.y);
                if (Mathf.Abs(trAngle - itemAngle) < 5)
                    BuildingManager.Picked.ShowWallByRoofMarker(tr, null);
            }
        }
    }

    private static void UpdateExteriorWalls(Vector3 itemPosition, InteriorItemDefinition item, int itemAngle)
    {
        foreach (Transform tr in BuildingManager.Picked.ExteriorWalls)
        {
            if (tr.Find("DoorMarker") && (tr.Find("DoorMarker").position - itemPosition).magnitude <= 0.4f)
            {
                UpdateOpeningsWallCoverings(item, tr.Find("DoorMarker"),
                    tr.GetComponent<Renderer>().materials[CorruptedParser.IsCorrupted(tr) ? 0 : 1],
                    tr.GetComponent<Renderer>().materials[CorruptedParser.IsCorrupted(tr) ? 1 : 0], true);

                int thisAngle = ClampAngle90(tr.Find("DoorMarker").rotation.eulerAngles.y);

                if (Mathf.Abs(itemAngle - thisAngle) < 5 || Mathf.Abs(itemAngle - thisAngle) == 180)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("Invisible");

                    int currIndex = BuildingManager.Picked.ExteriorWalls.IndexOf(tr);

                    if (currIndex < BuildingManager.Picked.ExteriorWalls.Count - 1)
                    {
                        Transform prevDoorMarker = BuildingManager.Picked.ExteriorWalls[currIndex + 1];

                        int prevAngle = ClampAngle90(prevDoorMarker.Find("DoorMarker").eulerAngles.y);

                        if (Mathf.Abs(thisAngle - prevAngle) < 5)
                        {
                            prevDoorMarker.gameObject.layer = LayerMask.NameToLayer("Invisible");
                        }
                    }
                }
            }
        }        
    }

    /// <summary>
    /// Reconcile coverings between openings walls and wall sections.
    /// 
    /// The door can be rotated by 180 degrees so door's interior coverings won't match with wall's one.
    /// Need to swap materials quickly.
    /// </summary>
    /// <param name="p_item">Openings: Door/Window</param>
    /// <param name="p_wallSection">Wall section where openings is positioned</param>
    private static void UpdateOpeningsWallCoverings(InteriorItemDefinition p_item, Transform p_wallSection, Material p_exterior, Material p_inteiror, bool p_hideExtentions)
    {
        Transform model = p_item.Model;
        if (!model)
            return;

        Transform wall = model.Find("Wall");

        Renderer doorRenderer = null;

        if (wall)
            doorRenderer = wall.GetComponent<Renderer>();
        else
            doorRenderer = model.GetComponent<Renderer>();
         

        bool revertMaterials = false;
        if (Math.Abs(p_item.transform.rotation.eulerAngles.y - p_wallSection.rotation.eulerAngles.y) > 170)
            revertMaterials = true;

        if (doorRenderer)
        {
            Material[] materials = doorRenderer.materials;
            materials[revertMaterials || p_item.isCorrupted ? 0 : 1] = p_exterior;
            materials[revertMaterials || p_item.isCorrupted ? 1 : 0] = p_inteiror;
            doorRenderer.materials = materials;

            Transform[] trs = {p_item.transform.Find("2FootDoorHeader"),
                              p_item.transform.Find("4FootDoorHeader"),
                              p_item.transform.Find("7FootDoorHeader")};

            foreach (Transform doorHeader in trs)
            {
                if (p_hideExtentions)
                {
                    doorHeader.gameObject.layer = LayerMask.NameToLayer("Invisible");
                }
                else
                {
                    Material[] doorHeaderMaterials = doorHeader.GetComponent<Renderer>().materials;

                    // I found that "2FootDoorHeader" materials are inverted
                    if (doorHeader.name.Equals("2FootDoorHeader"))
                    {
                        doorHeaderMaterials[0] = materials[0];
                        doorHeaderMaterials[1] = materials[1];
                    }
                    else
                    {
                        doorHeaderMaterials[0] = materials[1];
                        doorHeaderMaterials[1] = materials[0];
                    }
                    doorHeader.GetComponent<Renderer>().materials = doorHeaderMaterials;
                }
            }
        }
    }

    /// <summary>
    /// 
    /// Instantiate interior item transform on the floor
    /// </summary>
    /// <param name="item"></param>
    private Vector3 PositionItem(InteriorItemDefinition item, out Quaternion rotation)
    {
        // Set item model's position here

        // Hack for ceiling lamps. Should attach them to the ceilings. Default ModelOffSet param
        // doesn't work here since height can be different
        if (item.text.Contains("Ceiling"))
        {
            item.transform.position = item.ClosestMarker + GetCeilingsOffSet();
        }
        else
        {
            item.transform.position = item.ClosestMarker + item.modelOffSet;
        }
        rotation = item.transform.rotation;

        if (item && item.transform)
            return item.transform.position;
        else
            return Vector3.zero;
    }

    private Vector3 GetCeilingsOffSet()
    {
        return new Vector3(0, BuildingManager.Picked.Ceiling.localPosition.y, 0);
    }
    
    /// <summary>
    /// 
    /// Draw PLAN VIEW/3D VIEW buttons, ROOF ON/ROOF OFF, ROTATE and DELETE buttons
    /// </summary>
    private void DrawGridButtons()
    {
        if (!CameraSwitcher.Is3DView)
        {
            previewCamera.enabled = true;
            // This code draws selection background for selected item:

            if (pickedGridItemIndex >= 0)
            {
                if (selTex == null)
                {
                    selTex = Common.UpdateTexture(selIAlpha);
                }
                else
                {
                    dTime += Time.deltaTime;

                    if (dTime >= alphaDelay)
                    {
                        if (selIAlpha <= 0)
                            sign = 1;
                        else if (selIAlpha >= 1)
                            sign = -1;

                        selIAlpha += 0.1f * sign;
                        selTex = Common.UpdateTexture(selIAlpha);

                        dTime = 0;
                    }
                }

                GUIStyle selectionRectStyle = new GUIStyle();
                selectionRectStyle.normal.background = selTex;

                Rect selectionRect = gridItems[pickedGridItemIndex].ItemRect;
                Vector2 itemCenter = new Vector2(selectionRect.x + selectionRect.width / 2, selectionRect.y + selectionRect.height / 2);

                GUIUtility.RotateAroundPivot(gridItems[pickedGridItemIndex].Angle, itemCenter);
                GUI.Box(selectionRect, GUIContent.none, selectionRectStyle);
                GUI.matrix = Matrix4x4.identity;
            }

            // Delete button, only in plan view
            GUIContent deleteBtnContent = new GUIContent("DELETE");
            Vector2 deleteBtnSize = buttonStyle.CalcSize(deleteBtnContent);
            Rect deleteBtnRect = new Rect(gridRect.x + gridRect.width - deleteBtnSize.x - 5,
                gridRect.y + gridRect.height - 28, deleteBtnSize.x, deleteBtnSize.y);

            if (GUI.Button(deleteBtnRect, deleteBtnContent, buttonStyle))
            {
                if (pickedGridItemIndex >= 0)
                {
                    GameObject.Destroy(gridItems[pickedGridItemIndex].gameObject);
                    gridItems.RemoveAt(pickedGridItemIndex);
                    pickedGridItemIndex = -1;
                }
            }

            // Rotate button, only in plan view
            GUIContent rotateBtnContent = new GUIContent("ROTATE");
            Vector2 rotateBtnSize = buttonStyle.CalcSize(rotateBtnContent);
            Rect rotateBtnRect = new Rect(deleteBtnRect.x - rotateBtnSize.x - 5,
                 deleteBtnRect.y, rotateBtnSize.x, rotateBtnSize.y);

            if (GUI.Button(rotateBtnRect, rotateBtnContent, buttonStyle))
            {
                if (pickedGridItemIndex >= 0)
                {
                    gridItems[pickedGridItemIndex].Angle += 45;
                    modifiedOpeningsDetected = true;
                }
            }
        }
        else
        {
            previewCamera.enabled = false;

            GUIContent[] switcherRoof = new GUIContent[2] 
                { new GUIContent("ROOF ON"), new GUIContent("ROOF OFF") };

            Rect switcherRoofRect = new Rect(gridRect.x + 265, gridRect.y + gridRect.height - 27, 161, 22);

            switcherRoofIndex = Common.SelectionGrid(switcherRoofRect, switcherRoofIndex, switcherRoof,
                switcherLeft, switcherLeftActive, switcherRight, switcherRightActive);

            if (BuildingManager.Picked.PickedRoof != null)
            {
                if (switcherRoofIndex == 0)
                    BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
                else
                    BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");
            }

            Rect boldHintRect = new Rect(gridRect.x, gridRect.y + gridRect.height + 5, 700, 20);

            GUI.Label(boldHintRect, "Click-drag to rotate your view. Use your keyboard + and - keys to zoom in and out.", boldHintStyle);
        }

        GUIContent[] switcher3d = new GUIContent[2]
            { new GUIContent("PLAN VIEW"), new GUIContent("3D VIEW") };

        Rect switcherBtnRect = new Rect(gridRect.x + 105,
            gridRect.y + gridRect.height - 27, 161, 22);


        switcher3dIndex = Common.SelectionGrid(switcherBtnRect, switcher3dIndex, switcher3d,
            switcherLeft, switcherLeftActive, switcherRight, switcherRightActive);

        // set 3D View
        if (switcher3dIndex == 0)
        {
            CameraSwitcher.Is3DView = false;
        }
        else
        {
            CameraSwitcher.Is3DView = true;
        }
    }

	private void OnGUI()
	{
        GUI.depth = 1;

        if (!loadingFinished || BuildingManager.Picked == null)
            return;

        if (CameraSwitcher.Is3DView)
        {
            cam3D = CameraSwitcher.ExpandView(cam3D);
            cam3D.gameObject.SetActiveRecursively(true);
            view3DRect = cam3D.pixelRect;
            view3DRect.y = Screen.height - cam3D.pixelRect.y - cam3D.pixelRect.height;

            GUI.Box(view3DRect, GUIContent.none, view3DStyle);
        }
        else
        {
            if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
            {
                if (selectedItem != null && selectedItem.model != null)
                    PreviewSelectedItem(selectedItem);
                else
                    PreviewSelectedItem(null);

                cam3D = CameraSwitcher.RestoreView(cam3D);
                cam3D.gameObject.SetActiveRecursively(false);

                if (BuildingManager.Picked.PickedRoof)
                    BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");

                RightPanel();
            }
            DrawGridItems(false);
            {
                if (modifiedOpeningsDetected)
                    UpdateWalls();

                modifiedOpeningsDetected = false;
            }
            DrawHintLabels();
        }

        if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Design)
        {
            DrawGridButtons();
        }

        if (ArchitectStudioGUI.Mode == ArchitectStudioGUI.modes.Print)
        {
            previewCamera.enabled = false;

            if (!printerManager.IsFinished && !printerManager.IsInProcess)
            {
                printerManager.IsInProcess = true;
                StartCoroutine(ScreenCapture());
            }
            else if (!printerManager.IsFinished && printerManager.IsInProcess)
            {
                if (printerManager.CanShowNotification)
                {
                    ShowProcessing("Prepare to print...");
                }
            }
            else if (printerManager.IsFinished && !printerManager.IsInProcess)
            {
                ArchitectStudioGUI.Mode = ArchitectStudioGUI.modes.Design;
                printerManager.IsFinished = false;
                ArchitectStudioGUI.ToLastState();
                previewCamera.enabled = true;

                //if (oPrinterManager.IsSuccess)
                //{
                    Application.OpenURL(string.Format(imagePrintUrl, printRandomID));
                //}
            }
        }
	}

    private void DrawHintLabels()
    {
        Rect hintRect = new Rect(gridRect.x + 5,
                                    gridRect.y + gridRect.height - 40,
                                    100, 20);

        GUI.Label(hintRect, linearFeetText, hintLabelStyle);

        Rect linearValueRect = hintRect;
        linearValueRect.x += hintLabelStyle.CalcSize(new GUIContent(linearFeetText)).x + 5;

        int linearFeet = AS3DLayoutGrid.GetWallLength();
        GUI.Label(linearValueRect, linearFeet.ToString(), hintLabelStyle);

        hintRect.y += 15;

        GUI.Label(hintRect, linearMetersText, hintLabelStyle);

        Rect meterValueRect = hintRect;
        meterValueRect.x += hintLabelStyle.CalcSize(new GUIContent(linearMetersText)).x + 5;

        float meterFeet = Common.FeetToMeter(linearFeet);
        GUI.Label(meterValueRect, meterFeet.ToString("##.#"), hintLabelStyle);

        hintRect.y += 25;
        hintRect.x -= 15;
        GUI.Label(hintRect, cell, GUIStyle.none);

        hintRect.x += 20;

        GUI.Label(hintRect, squareText, hintLabelStyle);

    }

    private void PreviewSelectedItem(InteriorItemDefinition p_selectedItem)
    {
        foreach (InteriorItemDefinition item in gridItems)
            item.DeactivateRenderers();

        if (previewPlaceholder == null)
        {
            previewPlaceholder = GameObject.Find(PREVIEW_PLACEHOLDER);

            if (previewPlaceholder == null)
                return;
        }

        InteriorItemDefinition previewModel = null;

        previewModel = previewPlaceholder.GetComponentInChildren<InteriorItemDefinition>();

        if (p_selectedItem != null)
        {
            p_selectedItem.ActivateRenderers();

            if (!previewModel || (previewModel && p_selectedItem.model != previewModel.model))
            {
                if (previewModel)
                    DestroyImmediate(previewModel.gameObject);

                previewModel = p_selectedItem.CreateCopy(previewPlaceholder.transform, GUIStyle.none,
                    previewAngle, previewPlaceholder.transform.position, new Rect());

                
                foreach (Transform tr in previewModel.transform)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("Preview");
                    if (tr.childCount > 0)
                    {
                        foreach (Transform child in tr)
                            child.gameObject.layer = LayerMask.NameToLayer("Preview");
                    }
                }

                if (previewCamera)
                {
                    if (previewModel.useCustomFieldOfView)
                        previewCamera.fieldOfView = previewModel.FieldOfView;
                    else
                        previewCamera.fieldOfView = GetFieldOfView(previewModel.subtab);

                    previewCamera.transform.LookAt(previewModel.transform);
                }
            }
            else if (previewModel)
            {
                previewModel.transform.Rotate(Vector3.up, previewAngle);
            }
        }
        else
        {
            if (previewModel != null)
                DestroyImmediate(previewModel.gameObject);
        }
    }

    private float GetFieldOfView(SubTabs subTab)
    {
        switch (subTab)
        {
            case SubTabs.Bathrooms:
                return 40;
            case SubTabs.Kitchens:
                return 40;
            case SubTabs.Bedrooms:
                return 40;
            case SubTabs.Doors:
                return 40;
            case SubTabs.Lamps:
                return 20;
            case SubTabs.Windows:
                return 40;
            case SubTabs.Seating:
                return 20;
            case SubTabs.Tables:
                return 20;
            default:
                return 60;
        }
    }

    /// <summary>
    /// 
    /// This code rotates Cam3D around the building
    /// </summary>
    /// 
    private void LateUpdate()
    {
        if (!loadingFinished || BuildingManager.Picked == null)
            return;

        // Zoom out
        if (Input.GetKey(KeyCode.Minus) || Input.GetAxis("Mouse ScrollWheel") > 0 || Input.GetKey(KeyCode.KeypadMinus))
        {
            if (cameraDistance < maxCam3DDistance)
                cameraDistance += zoomSpeed * Time.deltaTime;
            else
                cameraDistance = maxCam3DDistance;
        }

        // Zoom in
        if (Input.GetKey(KeyCode.Equals) || Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKey(KeyCode.KeypadPlus))
        {
            if (cameraDistance > minCam3DDistance)
                cameraDistance -= zoomSpeed * Time.deltaTime;
            else
                cameraDistance = minCam3DDistance;
        }

        //Detect mouse drag;
        if (Input.GetMouseButton(0))
        {
            cameraX += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;
            cameraY -= Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;
        }

        // Clamp rotation angle
        cameraY = ClampAngle360(cameraY, 10, 85);

        // Get Look At center
        Vector3 buildingCenter = BuildingManager.Picked.transform.position +
            BuildingManager.Picked.transform.TransformDirection(Vector3.forward) * BuildingManager.Picked.lookAtOffset.x +
                BuildingManager.Picked.transform.TransformDirection(Vector3.right) * BuildingManager.Picked.lookAtOffset.y;

        Quaternion rotation = Quaternion.Euler(cameraY, cameraX, 0);
        Vector3 newCam3DPosition = rotation * new Vector3(0.0f, 0.0f, -cameraDistance) +
            buildingCenter;

        // Apply changes 
        cam3D.transform.position = newCam3DPosition;
        cam3D.transform.rotation = rotation;

        // Set Look At
        cam3D.transform.LookAt(buildingCenter);
    }

    private void ShowProcessing(string p_text)
    {
        Rect processingRect = new Rect(Screen.width / 2 - 300 / 2 + 100, Screen.height / 2 - 300 / 2, 300, 200);

        GUI.Box(processingRect, GUIContent.none, hintSkin.GetStyle("needsbox"));

        Rect insideProcessingRect = new Rect(processingRect.x + processingRect.width / 4, processingRect.y + processingRect.height / 4, processingRect.width / 2, processingRect.height / 1.5f);

        GUILayout.BeginArea(insideProcessingRect);

        GUILayout.Label(p_text, centeredHeaderStyle);

        GUILayout.EndArea();
    }

    public IEnumerator ScreenCapture()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        byte[] bytes = tex.EncodeToPNG();

        // Do stuff with bytes
        //ScreenShotMaker.SaveLocalCopy(bytes);
        printerManager.PreparePrint(printRandomID, bytes);

        Destroy(tex);

        printerManager.CanShowNotification = true;
    } 

    /// <summary>
    /// 
    /// Clamp cam3D rotation angle
    /// </summary>
    /// 
    public static float ClampAngle360 (float p_angle, float p_min, float p_max) {
       if (p_angle < -360)
          p_angle += 360;

       if (p_angle > 360)
          p_angle -= 360;

       return Mathf.Clamp (p_angle, p_min, p_max);
    }

    public static void ResetGridItems()
    {
        pickedGridItemIndex = -1;
        gridItems.Clear();

        if (gridItemsContaiter)
        {
            foreach (Transform tr in gridItemsContaiter.transform)
            {
                GameObject.Destroy(tr.gameObject);
            }
        }
    }

    public IEnumerator StartInitialization()
    {
        while (gridItemsContaiter == null)
        {
            gridItemsContaiter = GameObject.Find("Grid Items");
            yield return 0;
        }

        while (interiorItemsContainer == null)
        {
            interiorItemsContainer = GameObject.Find("Interior Items");
            yield return 0;
        }

        StartCoroutine(TryLoadSavedInterior());
    }

    // TODO: Need to separate all tryload functions
    private IEnumerator TryLoadSavedInterior()
    {
        InteriorItemManager.Init(interiorItemsContainer);
        InteriorItemManager.InstantiateInteriorItems();

        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.oExterior != null && LoadAS3DManager.oGameData.oInterior != null)
        {
            while (BuildingManager.Picked == null)
            {
                loadingFinished = false;
                yield return 0;
            }

            // Load floor tiles
            BuildingManager.Picked.FloorTiles = LoadAS3DManager.oGameData.oInterior.floorTiles;

            // Load interior items
            foreach (InteriorItem interiorItem in LoadAS3DManager.oGameData.oInterior.InteriorItems)
            {
                InteriorItemDefinition match = InteriorItemManager.InteriorItemsList.Find(
                    delegate(InteriorItemDefinition p) { return p.gameObject.name.Equals(interiorItem.name); });

                if (match != null)
                {
                    InteriorItemDefinition copy = match.CreateCopy(gridItemsContaiter.transform,
                        match.image != null ? GUIStyle.none : pickedRoomLabelStyle,
                            interiorItem.angle,
                                Common.StringToVector3(interiorItem.position),
                                    Common.StringToRect(interiorItem.rect));

                    gridItems.Add(copy);
                }
            }

            // Load walls
            WallManager.RemoveAllWalls();
            foreach (List<string> fakeWall in LoadAS3DManager.oGameData.oInterior.walls)
            {
                List<WallSection> wall = new List<WallSection>();

                foreach (string fakeVector in fakeWall)
                    wall.Add(new WallSection().Deserialize(fakeVector));

                WallManager.Walls.Add(wall);
            }
            
            // Load exterior wall coverings
            CoveringsManager.Coverings.Clear();
            List<WallSection> exteriorCoverings = new List<WallSection>();
            foreach (string fakeCoverings in LoadAS3DManager.oGameData.oInterior.exteriorCoverings)
            {
                exteriorCoverings.Add(new WallSection().Deserialize(fakeCoverings));
            }
            CoveringsManager.Coverings.AddRange(exteriorCoverings);

            
            // Instantiate wall section models
            AS3DLayoutGrid oAS3DLayoutGrid =
                (AS3DLayoutGrid)GameObject.FindObjectOfType(typeof(AS3DLayoutGrid));

            oAS3DLayoutGrid.LoadWallPrefabs();
            
            foreach (List<WallSection> wall in WallManager.Walls)
                WallManager.InstantiateWallSections(wall, oAS3DLayoutGrid.WallPrefabs);

            // Apply exterior coverings
            CoveringsManager.LoadCoverings(GetComponent<Camera>());
            // Draw grid items
            DrawGridItems(true);

            InteriorDesignGUI.UpdateWalls();

            loadingFinished = true;
            enabled = false;
        }
        else
        {
            loadingFinished = true;
            enabled = false;
            yield return 0;
        }
    }

    /// <summary>
    /// This State is finished if user has clicked by all 5 tabs.
    /// </summary>
    /// <returns></returns>
    internal static bool IsFinished()
    {
        return pickedTabs.Count == 5;
    }

    private void ApplyGridRect(Rect rect)
    {
        gridRect = rect;
        gridRect.y = gridCorrectionY;
        gridRect.height = gridRect.width - gridCorrectionHeight;
        gridRect.x += gridCorrectionX;
        gridRect.width -= gridCorrectionWidth;
    }
}