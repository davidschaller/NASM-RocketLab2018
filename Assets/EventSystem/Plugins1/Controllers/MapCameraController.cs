using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapCameraController : MonoBehaviour
{
    public Camera mapCamera;
    public Texture2D playerMarker;

    private Dictionary<Transform, Texture2D> markers;

    private bool isVisible = true;
    private Transform target;
    private Rect mapRect;

    public static MapCameraController Main {get; private set;}

    //private GUIStyle mapStyle;   

    private void Awake()
    {
        if (Main == null)
            Main = this;

        if (markers == null)
            markers = new Dictionary<Transform, Texture2D>();
    }

    private void Start()
    {
        SetGUIStyles();
    }

    private void SetGUIStyles()
    {
        /*
        if (SkinManager.IsActive)
        {
            mapStyle = SkinManager.Main.GetStyle("Map", "Crypto");
        }
        else
            Debug.LogWarning("SkinManager is not active");
         */
    }

    private void Update()
    {
        if (mapCamera == null)
        {
            Debug.Log("Map Camera is NULL");
            return;
        }

        if (Input.GetKeyDown("m"))
        {
            isVisible = !isVisible;
        }

        if (isVisible && !mapCamera.enabled)
        {
            mapCamera.enabled = true;
        }
        else if (!isVisible && mapCamera.enabled)
        {
            mapCamera.enabled = false;
        }

        if (target == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
                target = playerGO.transform;

            return;
        }

        mapCamera.transform.position = new Vector3(target.position.x, GetComponent<Camera>().transform.position.y, target.position.z);
    }
    private void OnGUI()
    {
        GUI.depth = 10;

        mapRect = new Rect(mapCamera.rect.x * Screen.width, (1 - mapCamera.rect.y) * Screen.height - 190, 190, 190);
        //GUI.Label(mapRect, GUIContent.none, mapStyle);

        if (markers != null)
        {
            foreach (KeyValuePair<Transform, Texture2D> marker in markers)
            {
                Vector2 pos = GetMarkerPosition(marker.Key);
                Vector2 mapPos = GetMapMarkerPosition(mapRect, pos);

                GUI.DrawTexture(new Rect(mapPos.x - marker.Value.width / 2, mapPos.y - marker.Value.height / 2, 
                    marker.Value.width, marker.Value.height), marker.Value);
            }
        }
    }

    private Vector2 GetMarkerPosition(Transform tr)
    {
        Vector2 result = Vector2.zero;

        if (mapCamera != null)
        {
            result = mapCamera.WorldToScreenPoint(tr.position);
            result.y = Screen.height - result.y;
        }

        return result;
    }

    private Vector2 GetMapMarkerPosition(Rect mapRect, Vector2 pos)
    {
        Vector2 result = new Vector2(pos.x, pos.y);

        if (pos.x < mapRect.x)
            result.x = mapRect.x;
        
        if (pos.x > mapRect.x + mapRect.width)
            result.x = mapRect.x + mapRect.width;

        if (pos.y < mapRect.y + 10)
            result.y = mapRect.y + 10;

        if (pos.y > mapRect.y + mapRect.height)
            result.y = mapRect.y + mapRect.height;
        

        return result;
    }

    private void LateUpdate()
    {

    }

    public static void AddMarker(Transform tr, Texture2D texture)
    {
        if (Main != null)
        {
            if (Main.markers == null)
                Main.markers = new Dictionary<Transform, Texture2D>();

            if (texture != null)
            {
                Main.markers.Add(tr, texture);
            }
            else
                Debug.Log("texture is NULL");
        }
        else
            Debug.Log("MapCameraController is NULL");
    }

    public static void RemoveMarker(Transform tr)
    {
        if (Main.markers == null)
            Main.markers = new Dictionary<Transform, Texture2D>();

        if (Main.markers.ContainsKey(tr))
            Main.markers.Remove(tr);
    }


    public void Disable()
    {
        mapCamera.enabled = false;
        enabled = false;
    }

    public void Enable()
    {
        mapCamera.enabled = true;
        enabled = true;
    }
}
