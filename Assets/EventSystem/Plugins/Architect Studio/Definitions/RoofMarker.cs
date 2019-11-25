using UnityEngine;

public class RoofMarker
{
    public enum MarkerTypes
    {
        m8Foot = 0,
        m10Foot,
        m12Foot,
        m15Foot
    }

    private Vector3 markerPosition;
    public Vector3 MarkerPosition
    {
        get
        {
            return markerPosition;
        }
        set
        {
            markerPosition = value;
        }
    }

    private string labelText;
    public string LabelText
    {
        get
        {
            return labelText;
        }
        set
        {
            labelText = value;
        }
    }

    private MarkerTypes markerType;
    public MarkerTypes MarkerType
    {
        get
        {
            return markerType;
        }
        set
        {
            markerType = value;
        }
    }

    public int sortKey = 0;

    public static string GetWallExtension(MarkerTypes markerTypes)
    {
        switch (markerTypes)
        {
            case MarkerTypes.m10Foot:
                return "2FootWallExtension";
            case MarkerTypes.m12Foot:
                return "4FootWallExtension";
            case MarkerTypes.m15Foot:
                return "7FootWallExtension";
            default:
                return "noWallExtension";
        }
    }

    public static float GetLabelHeight(MarkerTypes markerTypes)
    {
        switch (markerTypes)
        {
            case MarkerTypes.m10Foot:
                return 35f;
            case MarkerTypes.m12Foot:
                return 41f;
            case MarkerTypes.m15Foot:
                return 47f;
            default:
                return 23f;
        }
    }
}
