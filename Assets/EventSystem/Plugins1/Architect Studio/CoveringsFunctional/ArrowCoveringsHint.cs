using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;


public static class ArrowCoveringsHint
{
    public static bool? IsInteriorCovering
    {
        get
        {
            switch (currentDirection)
            {
                case arrowDirections.down:
                    return false;
                case arrowDirections.left:
                    return false;
                case arrowDirections.right:
                    return true;
                case arrowDirections.up:
                    return true;
                default:
                    if (currentDirection == arrowDirections.none)
                        return null;
                    else
                        return true;
            }
        }
    }

    public enum arrowDirections
    {
        none,
        left,
        right,
        up,
        down,
        leftUp,
        leftDown,
        rightUp,
        rightDown
    }

    public static arrowDirections currentDirection = arrowDirections.none;

    public static Vector2 Update(Vector3 p_anchorPos, Vector2 p_v1, Vector2 p_v2, Camera p_camera)
    {
        Vector2 arrowPos = Vector3.zero;

        if ((p_v1 - p_v2).magnitude < 17 && (p_v1 - p_v2).magnitude > 15)
        {
            Vector2 screenPos = p_v1.x < p_v2.x || p_v1.y > p_v2.y ? p_v1 : p_v2;

            if (p_v1.x == p_v2.x)
            {
                if (IsBetween(Event.current.mousePosition.y, p_v1.y, p_v2.y))
                {
                    if (Event.current.mousePosition.x < screenPos.x)
                        currentDirection = arrowDirections.right;
                    else
                        currentDirection = arrowDirections.left;
                }
                else
                    currentDirection = arrowDirections.none;
            }
            else
            {
                if (IsBetween(Event.current.mousePosition.x, p_v1.x, p_v2.x))
                {
                    if (Event.current.mousePosition.y > screenPos.y)
                        currentDirection = arrowDirections.up;
                    else
                        currentDirection = arrowDirections.down;
                }
                else
                    currentDirection = arrowDirections.none;
            }

            arrowPos = screenPos;
        }
        else
        {
            Transform realMarker = null;

            float cursorToMarkerDistance = 99999;
            float minFitDistance = 15;

            Vector2 closestMarker = BuildingManager.Picked.GetClosestExteriorDoorMarker(p_camera, Event.current.mousePosition, ref cursorToMarkerDistance, ref realMarker);
            if (cursorToMarkerDistance < minFitDistance)
            {
                float angle = realMarker.rotation.eulerAngles.y;
                switch (Mathf.RoundToInt(angle))
                {
                    case 0:
                        currentDirection = arrowDirections.up;
                        break;
                    case 45:
                        currentDirection = arrowDirections.rightUp;
                        break;
                    case 90:
                        currentDirection = arrowDirections.right;
                        break;
                    case 135:
                        currentDirection = arrowDirections.rightDown;
                        break;
                    case 180:
                        currentDirection = arrowDirections.down;
                        break;
                    case 225:
                        currentDirection = arrowDirections.leftDown;
                        break;
                    case 270:
                        currentDirection = arrowDirections.left;
                        break;
                    case 315:
                        currentDirection = arrowDirections.leftUp;
                        break;
                    default:
                        currentDirection = arrowDirections.none;
                        break;
                }

                arrowPos = closestMarker;
            }
            else
                currentDirection = arrowDirections.none;
        }

        return arrowPos;
    }

    private static bool IsBetween(float pos, float point1, float point2)
    {
        return (pos > point1 && pos < point2) || (pos > point2 && pos < point1);
    }

    public static void ShowArrow(Vector2 p_arrowPos, bool p_draggingMouse, Texture2D[] p_arrows, Texture2D[] lines)
    {
        Vector2 pos = p_arrowPos;

        switch (currentDirection)
        {
            case arrowDirections.left:
                GUI.Label(new Rect(pos.x + 5, pos.y - 15, 20, 20), new GUIContent(p_draggingMouse ? lines[2] : p_arrows[2])); // lineLeft : arrowLeft
                break;
            case arrowDirections.right:
                GUI.Label(new Rect(pos.x - 18, pos.y - 15, 20, 20), new GUIContent(p_draggingMouse ? lines[3] : p_arrows[3])); // lineRight : arrowRight
                break;
            case arrowDirections.up:
                GUI.Label(new Rect(pos.x, pos.y + 5, 20, 20), new GUIContent(p_draggingMouse ? lines[0] : p_arrows[0])); // lineUp : arrowUp
                break;
            case arrowDirections.down:
                 GUI.Label(new Rect(pos.x, pos.y - 20, 20, 20), new GUIContent(p_draggingMouse ? lines[1] : p_arrows[1])); // lineDown : arrowDown
                break;
            case arrowDirections.leftUp:
                GUI.Label(new Rect(pos.x - 10, pos.y + 10, 20, 20), new GUIContent(p_draggingMouse ? lines[5] : p_arrows[6])); // lineLeft45 : arrowLeftUp
                break;
            case arrowDirections.leftDown:
                GUI.Label(new Rect(pos.x - 4, pos.y - 18, 20, 20), new GUIContent(p_draggingMouse ? lines[4] : p_arrows[4])); // lineRight45 : arrowLeftDown
                break;
            case arrowDirections.rightUp:
                GUI.Label(new Rect(pos.x - 20, pos.y - 10, 20, 20), new GUIContent(p_draggingMouse ? lines[4] : p_arrows[5])); // lineRight45 : arrowRightUp
                break;
            case arrowDirections.rightDown:
                GUI.Label(new Rect(pos.x - 20, pos.y - 10, 20, 20), new GUIContent(p_draggingMouse ? lines[5] : p_arrows[7])); // lineLeft45 : arrowRightDown
                break;
            default:
                break;
        }
    }

}

