using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LandscapingGUI : MonoBehaviour
{
    public GUISkin buttonSkin,
                   hintSkin,
                   labelSkin;

    public Camera cam3D,                 // Use the same '3D Building Camera' Game Object
                  previewCamera;         // Use this to view 3d model of the selected item

    private float cameraX = 0;           // Use this to rotate camera around the building by x
    private float cameraY = 0;           // Use this to rotate camera around the building by y
    private float distance = 50f;        // Distance from the Cam3D camera to the building

    private static int selectedGridItemIndex = -1;
    private static List<ItemDefinition> gridItems;      // All selected interior items are here
    public static List<ItemDefinition> GridItems
    {
        get
        {
            return gridItems;
        }
        set
        {
            gridItems = value;
        }
    }
    private static GameObject gridItemsContaiter;

    private static ItemDefinition selectedItem;
    public static ItemDefinition SelectedItem
    {
        get
        {
            return selectedItem;
        }
        private set { }
    }


    private GUIStyle itemStyle,
                     landscapingMenuStyle,
                     pickedItemLabelStyle,
                     switcherLeft,
                     switcherLeftActive,
                     switcherRight,
                     switcherRightActive,
                     view3DStyle,
                     boldHintStyle,
                     buttonStyle;

    private LayoutGridLandscapings oLayoutGridLandscapings;

    void Awake()
    {
        ArchitectStudioGUI.RegisterStateScript(ArchitectStudioGUI.States.Landscaping, GetComponent<LandscapingGUI>());

        // Start in plan view
        Vector3 angles = cam3D.transform.eulerAngles;
        cameraX = angles.y;
        cameraY = angles.x;

        GetComponent<Camera>().enabled = false;

        gridItemsContaiter = GameObject.Find("Grid Items");

        gridItems = new List<ItemDefinition>();
        LandscapingManager.Init();

        StartCoroutine(TryLoadSavedLandscaping());

        if (!previewPlaceholder)
            previewPlaceholder = GameObject.Find("Preview Placeholder");

        itemStyle = buttonSkin.GetStyle("item");
        landscapingMenuStyle = hintSkin.GetStyle("interiorMenu");
        pickedItemLabelStyle = labelSkin.GetStyle("pickedItemLabel");

        switcherLeft = hintSkin.GetStyle("switcherLeft");
        switcherLeftActive = new GUIStyle(switcherLeft);
        switcherLeftActive.normal = switcherLeft.active;
        switcherRight = hintSkin.GetStyle("switcherRight");
        switcherRightActive = new GUIStyle(switcherRight);
        switcherRightActive.normal = switcherRight.active;
        boldHintStyle = labelSkin.GetStyle("boldHint");
        buttonStyle = buttonSkin.GetStyle("button");

        view3DStyle = hintSkin.GetStyle("view3d");

        oLayoutGridLandscapings = transform.GetComponent<LayoutGridLandscapings>();
    }

    private Rect gridRect,
                 view3DRect;

    void OnEnable()
    {
        if (oLayoutGridLandscapings)
            oLayoutGridLandscapings.enabled = true;
        LandscapingManager.ActivateLandscapings();

        if (!BuildingManager.Picked)
            return;

        GetComponent<Camera>().enabled = true;

        gridRect = GetComponent<Camera>().pixelRect;
        gridRect.y = 25;
        gridRect.height = gridRect.width - 93;
        gridRect.x += 10;
        gridRect.width -= 20;

        if (CameraSwitcher.Is3DView)
        {
            cam3D = CameraSwitcher.ExpandView(cam3D);
            cam3D.enabled = true;
        }
        else
        {
            if (BuildingManager.Picked.PickedRoof)
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");

            cam3D = CameraSwitcher.RestoreView(cam3D);
            cam3D.gameObject.SetActiveRecursively(false);
        }

        PreviewSelectedItem(null);
        if (previewCamera)
        {
            previewCamera.transform.position -= Vector3.forward * 15;
            previewCamera.transform.position += Vector3.up * 15 ;
        }

        BuildingManager.Picked.UpdateLayer("Landscaping");
    }

    void OnDisable()
    {
        GetComponent<Camera>().enabled = false;

        if (cam3D)
        {
            cam3D = CameraSwitcher.RestoreView(cam3D);
        }

        if (previewCamera)
        {
            previewCamera.transform.position += Vector3.forward * 15;
            previewCamera.transform.position -= Vector3.up * 15;
        }

        PreviewSelectedItem(null);

        LandscapingManager.DeactivateLandscapings();
        if (oLayoutGridLandscapings)
            oLayoutGridLandscapings.enabled = false;

        CameraSwitcher.Is3DView = false;
    }

    private float selectionPanelWidth = 190,
                  selectionPanelHeight = 440;


    private Rect rightPanel;



    void RightInterface()
    {
        rightPanel = new Rect(Screen.width - selectionPanelWidth - 40,
                    50, selectionPanelWidth, selectionPanelHeight);

        GUI.Box(rightPanel, GUIContent.none, landscapingMenuStyle);

        LanscapingItems();
    }

    public float itemWidth = 40,
                 itemHeight = 50;


    private void LanscapingItems()
    {
        int vmult = 0,
            hmult = 0;

        for (int i = 0; i < LandscapingManager.LandscapingList.Count; i++)
        {
            LandscapingManager.LandscapingList[i].ItemRect = 
                new Rect(rightPanel.x + 25 + (itemWidth + 10) * (i % 3 == 0 ? hmult = 0 : ++hmult),
                                        rightPanel.y + 25 + (itemHeight + 10) * (i % 3 == 0 ? vmult = i / 3 : vmult),
                                        itemWidth,
                                        itemHeight);

            DrawItem(LandscapingManager.LandscapingList[i], true, itemStyle);
        }
    }

    private ItemDefinition DrawItem(ItemDefinition item, bool copy, GUIStyle itemStyle)
    {
        if (copy)
        {
            if (Event.current.type == EventType.MouseDown &&
                    Event.current.button == 0 &&
                        item.ItemRect.Contains(Event.current.mousePosition))
            {
                selectedItem = item;

                gridItems.Add(item.CreateCopy(gridItemsContaiter.transform, GUIStyle.none));
            }
        }
        else
        {
            if (item.IsGrabbed)
            {
                selectedGridItemIndex = gridItems.IndexOf(item);

                item.ItemRect = new Rect(Event.current.mousePosition.x - dx, Event.current.mousePosition.y - dy,
                                item.ItemRect.width, item.ItemRect.height);

                item.ClosestMarker = GetComponent<Camera>().ScreenToWorldPoint(new Vector2(item.ItemRect.x + item.ItemRect.width / 2,
                   Screen.height - (item.ItemRect.y + item.ItemRect.height / 2)));

                // Modify item model on the floor
                item.ClosestMarker = new Vector3(item.ClosestMarker.x, BuildingManager.Picked.Floor.position.y, item.ClosestMarker.z);
            }

            if (Event.current.type == EventType.MouseDown &&
                Event.current.button == 0 &&
                item.ItemRect.Contains(Event.current.mousePosition))
            {
                item.IsGrabbed = true;
                dx = Mathf.Abs(Event.current.mousePosition.x - item.ItemRect.x);
                dy = Mathf.Abs(Event.current.mousePosition.y - item.ItemRect.y);
            }
            else if (Event.current.type == EventType.MouseUp &&
                     Event.current.button == 0 &&
                     item.IsGrabbed)
            {
                if (!item.IsInside(gridRect))
                {
                    item.RestorePosition();
                }

                item.IsGrabbed = false;
            }
        }

        GUIStyle selectedItemStyle = new GUIStyle(itemStyle);
        selectedItemStyle.normal = selectedItemStyle.active;

        GUIStyle currentStyle = GUIStyle.none;

        if (selectedItem != null)
        {
            if (item == selectedItem)
                currentStyle = selectedItemStyle;
            else
                currentStyle = itemStyle;
        }
        else
            currentStyle = itemStyle;

        GUI.Box(item.ItemRect, item.image ? new GUIContent(item.image) : new GUIContent(item.name), currentStyle);

        if (selectedItem != null)
        {
            Rect itemTextRect = new Rect(rightPanel.x + 25, rightPanel.y + rightPanel.height - 30, 140, 16);

            GUI.Label(itemTextRect, !string.IsNullOrEmpty(selectedItem.text) ? selectedItem.text : selectedItem.name, pickedItemLabelStyle);
        }

        return item;
    }

    private float dx, dy; // Use this to fit item position


    private void DrawGridItems(bool restoring)
    {
        for (int i = 0; i < gridItems.Count; i++)
        {
            if (!restoring)
            {
                // Draw item label
                gridItems[i] = DrawItem(gridItems[i], false, GUIStyle.none);
            }

            // Instantiate item model
            Quaternion itemRotation;
            PositionItemModel(gridItems[i], out itemRotation);

            // Or remove it, if it's outside of the building
            if (!restoring && !gridItems[i].IsGrabbed && !gridItems[i].IsInside(gridRect))
            {
                selectedGridItemIndex = -1;
                GameObject.Destroy(gridItems[i].gameObject);

                gridItems.Remove(gridItems[i]);
                break;
            }
        }
    }

    /// <summary>
    /// 
    /// Instantiate interior item model on the floor
    /// </summary>
    /// <param name="item"></param>
    private Vector3 PositionItemModel(ItemDefinition item, out Quaternion rotation)
    {
        item.transform.GetChild(0).position = item.ClosestMarker + item.modelOffSet;
        rotation = item.transform.GetChild(0).rotation;

        return item.transform.GetChild(0).position;
    }

    private int switcher3dIndex = 0;                // for 3d switcher button: 0 is PLAN VIEW
    private Texture2D selTex = new Texture2D(2, 2); // background blink texture for selected item
    private float selIAlpha = 0;                    // transparent variable for selTex texture: 0 - 1f
    private float dTime = 0;
    public float alphaDelay = 0.2f;                 // Selected item background blink time
    private int sign = 1;   

    private void DrawGridButtons()
    {
        if (!CameraSwitcher.Is3DView)
        {
            // This code draws selection background for selected item:

            if (selectedGridItemIndex >= 0)
            {
                if (selTex == null)
                {
                    selTex = UpdateTexture(selIAlpha);
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
                        selTex = UpdateTexture(selIAlpha);

                        dTime = 0;
                    }
                }

                GUIStyle selectionRectStyle = new GUIStyle();
                selectionRectStyle.normal.background = selTex;

                Rect selectionRect = gridItems[selectedGridItemIndex].ItemRect;

                if (gridItems[selectedGridItemIndex].image != null)
                    selectionRect.height = selectionRect.width;


                GUI.Box(selectionRect, GUIContent.none, selectionRectStyle);
            }
        }

        // Delete button, only in plan view
        GUIContent deleteBtnContent = new GUIContent("DELETE");
        Vector2 deleteBtnSize = buttonStyle.CalcSize(deleteBtnContent);
        Rect deleteBtnRect = new Rect(gridRect.x + gridRect.width - deleteBtnSize.x - 30,
            gridRect.y + gridRect.height - 28, deleteBtnSize.x, deleteBtnSize.y);

        if (GUI.Button(deleteBtnRect, deleteBtnContent, buttonStyle))
        {
            if (selectedGridItemIndex >= 0)
            {
                GameObject.Destroy(gridItems[selectedGridItemIndex].gameObject);
                gridItems.RemoveAt(selectedGridItemIndex);
                selectedGridItemIndex = -1;
            }
        }

        GUIContent[] switcher3d = new GUIContent[2] { new GUIContent("PLAN VIEW"), new GUIContent("3D VIEW") };

        Rect switcherBtnRect = new Rect(gridRect.x + 105,
           gridRect.y + gridRect.height - 27, 161, 22);

        switcher3dIndex = Common.SelectionGrid(switcherBtnRect, switcher3dIndex, switcher3d,
            switcherLeft, switcherLeftActive, switcherRight, switcherRightActive);
            
            //GUI.SelectionGrid(switcherBtnRect, switcher3dIndex, switcher3d, 2, buttonStyle);

        // Set 3D View
        if (switcher3dIndex == 0)
        {
            CameraSwitcher.Is3DView = false;
        }
        else
        {
            CameraSwitcher.Is3DView = true;
        }
    }

    private Texture2D UpdateTexture(float p_selIAlpha)
    {
        Texture2D result = new Texture2D(2, 2);

        for (int i = 0; i <= result.height; i++)
            for (int j = 0; j <= result.width; j++)
            {
                result.SetPixel(i, j, new Color(1f, 1f, 1f, p_selIAlpha));
            }

        result.Apply();

        return result;
    }


    void OnGUI()
    {
        GUI.depth = 1;

        if (!loadingFinished || BuildingManager.Picked == null)
            return;

        if (CameraSwitcher.Is3DView)
        {
            previewCamera.enabled = false;

            cam3D = CameraSwitcher.ExpandView(cam3D);

            cam3D.gameObject.SetActiveRecursively(true);

            view3DRect = cam3D.pixelRect;
            view3DRect.y = Screen.height - cam3D.pixelRect.y - cam3D.pixelRect.height;

            GUI.Box(view3DRect, GUIContent.none, view3DStyle);

            if (InteriorDesignGUI.SwitcherRoofIndex == 0)
            {
                if (BuildingManager.Picked.PickedRoof)
                    BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Default");
            }
            else
            {
                if (BuildingManager.Picked.PickedRoof)
                    BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");
            }

            DrawRoofSwitcher();

            Rect boldHintRect = new Rect(gridRect.x, gridRect.y + gridRect.height + 5, 700, 20);

            GUI.Label(boldHintRect, "Click-drag to rotate your view. Use your keyboard + and - keys to zoom in and out.", boldHintStyle);
        }
        else
        {
            previewCamera.enabled = true;

            if (BuildingManager.Picked.PickedRoof)
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");

            if (selectedItem != null && selectedItem.model != null)
                PreviewSelectedItem(selectedItem);
            else
                PreviewSelectedItem(null);

            cam3D = CameraSwitcher.RestoreView(cam3D);
            cam3D.gameObject.SetActiveRecursively(false);

            if (BuildingManager.Picked.PickedRoof)
                BuildingManager.Picked.PickedRoof.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Invisible");

            RightInterface();
            DrawGridItems(false);

            //cam3D.enabled = false;
        }

        DrawGridButtons();
    }

    private void DrawRoofSwitcher()
    {
        Rect switcherRoofRect = new Rect(gridRect.x + 265,
            gridRect.y + gridRect.height - 27, 161, 22);

        InteriorDesignGUI.SwitcherRoofIndex = 
            Common.SelectionGrid(switcherRoofRect, InteriorDesignGUI.SwitcherRoofIndex, switcherRoof,
                 switcherLeft, switcherLeftActive, switcherRight, switcherRightActive);        
    }


    public float rotateSpeed = 130;      // Cam3D rotation speed around the building
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
        cameraY = ClampAngle(cameraY, 10, 85);

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
    /// 
    /// Clamp cam3D rotation angle
    /// </summary>
    /// 
    public static float ClampAngle(float p_angle, float p_min, float p_max)
    {
        if (p_angle < -360)
            p_angle += 360;

        if (p_angle > 360)
            p_angle -= 360;

        return Mathf.Clamp(p_angle, p_min, p_max);
    }

    private bool loadingFinished = false;

    public IEnumerator TryLoadSavedLandscaping()
    {

        if (LoadAS3DManager.oGameData != null && LoadAS3DManager.oGameData.oExterior != null 
            && LoadAS3DManager.oGameData.oLandscape != null)
        {
            while (BuildingManager.Picked == null)
            {
                loadingFinished = false;
                yield return 0;
            }

            foreach (SimpleItem simpleItem in LoadAS3DManager.oGameData.oLandscape.landscapeItems)
            {
                ItemDefinition origItem = LandscapingManager.LandscapingList.Find(
                    delegate(ItemDefinition p) {return p.gameObject.name.Equals(simpleItem.name); });

                if (origItem)
                {
                    gridItems.Add(origItem.CreateCopy(gridItemsContaiter.transform,
                        GUIStyle.none,
                            Common.StringToVector3(simpleItem.position),
                                Common.StringToRect(simpleItem.rect)));

                }
            }

            DrawGridItems(true);

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

    private float previewAngle = 1;
    GameObject previewPlaceholder;

    GUIContent[] switcherRoof = new GUIContent[2] { new GUIContent("ROOF ON"), new GUIContent("ROOF OFF") };

    private void PreviewSelectedItem(ItemDefinition p_selectedItem)
    {
        foreach (InteriorItemDefinition item in gridItems)
            item.DeactivateRenderers();

        if (!previewPlaceholder)
            previewPlaceholder = GameObject.Find("Preview Placeholder");

        if (!previewPlaceholder)
            return;

        ItemDefinition previewModel = null;

        previewModel = previewPlaceholder.GetComponentInChildren<ItemDefinition>();

        if (p_selectedItem != null)
        {
            p_selectedItem.ActivateRenderers();

            if (!previewModel || (previewModel && p_selectedItem.model != previewModel.model))
            {
                if (previewModel)
                    DestroyImmediate(previewModel.gameObject);

                previewModel = p_selectedItem.CreateCopy(previewPlaceholder.transform, GUIStyle.none,
                     previewPlaceholder.transform.position, new Rect());

                foreach (Transform tr in previewModel.transform)
                {
                    tr.gameObject.layer = LayerMask.NameToLayer("Preview");
                    if (tr.childCount > 0)
                    {
                        foreach (Transform child in tr)
                        {
                            child.gameObject.layer = LayerMask.NameToLayer("Preview");

                            if (child.childCount > 0)
                            {
                                foreach (Transform part in child)
                                {
                                    part.gameObject.layer = LayerMask.NameToLayer("Preview");
                                }
                            }
                        }
                    }
                }

                previewCamera.fieldOfView = previewModel.FieldOfView;
                previewCamera.transform.LookAt(previewModel.transform);
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
}
