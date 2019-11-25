using UnityEngine;
using System.Collections;

public class ItemDefinition : MonoBehaviour
{
    public int FieldOfView = 60;
    public bool useCustomFieldOfView = false;

    public GameObject model;
    public Texture2D image;
    public string text;
    public int sortKey = 0;

    public Vector3 modelOffSet;
    public Vector2 labelOffSet;

    private Rect itemRect;
    public Rect ItemRect
    {
        get
        {
            return itemRect;
        }
        set
        {
            itemRect = value;
        }
    }

    public Vector3 scale = Vector3.one;

    private Rect oldRect = new Rect();
    private Vector3 oldMarker = Vector3.zero;

    private bool isGrabbed = false;
    public bool IsGrabbed
    {
        get
        {
            return isGrabbed;
        }
        set
        {
            if (value)
                SavePosition(itemRect, closestMarker); 
            else
                ForgetPosition();

            isGrabbed = value;
        }
    }

    public void SavePosition(Rect p_currentRect, Vector3 p_currentMarker)
    {
        oldRect = p_currentRect;
        oldMarker = p_currentMarker;
    }

    public void RestorePosition()
    {
        if (oldRect != new Rect() && oldMarker != Vector3.zero)
        {
            itemRect = oldRect;
            closestMarker = oldMarker;
        }
    }

    private void ForgetPosition()
    {
        oldRect = new Rect();
        oldMarker = Vector3.zero;
    }

    private Vector3 closestMarker = Vector3.zero;
    public Vector3 ClosestMarker
    {
        get
        {
            return closestMarker;
        }
        set
        {
            closestMarker = value;
        }
    }

    public void Deactivate()
    {
        gameObject.SetActiveRecursively(false);
    }

    public void Activate()
    {
        Debug.Log("Activating " + gameObject.name);
        gameObject.SetActiveRecursively(true);
    }

    protected Rect GetOriginalItemRect(Rect p_rect, GUIStyle p_style, GUIContent p_content, Vector2 p_offSet)
    {
        Vector2 size = Common.GetContentSize(p_style, p_content);

        return new Rect(p_rect.x + p_offSet.x,
                        p_rect.y + p_offSet.y,
                        size.x,
                        size.y);
    }


    internal ItemDefinition CreateCopy(Transform p_parent, GUIStyle p_style)
    {
        ItemDefinition newItem = (ItemDefinition)GameObject.Instantiate(this);
 
        newItem.ItemRect = GetOriginalItemRect(this.ItemRect,
                                       p_style,
                                       newItem.image ? new GUIContent(newItem.image) : new GUIContent(newItem.text),
                                       labelOffSet);

        newItem.transform.localScale = newItem.scale;

        if (p_parent)
        {
            newItem.transform.parent = p_parent;
        }

        return newItem;
    }

    internal ItemDefinition CreateCopy(Transform p_parent, GUIStyle p_style, Vector3 newPosition, Rect newRect)
    {
        ItemDefinition newItem = (ItemDefinition)GameObject.Instantiate(this, newPosition, this.transform.rotation);

        newItem.transform.localScale = newItem.scale;

        newItem.itemRect = newRect;

        if (p_parent)
        {
            newItem.transform.parent = p_parent;
        }

        newItem.ClosestMarker = newPosition;

        return newItem;
    }

    public bool IsInside(Rect p_rect)
    {
        if (((itemRect.x > p_rect.x && itemRect.x < p_rect.x + p_rect.width) ||
            (itemRect.x + itemRect.width > p_rect.x && itemRect.x + itemRect.width < p_rect.x + p_rect.width)) &&
            ((itemRect.y > p_rect.y && itemRect.y < p_rect.y + p_rect.height) ||
            (itemRect.y + itemRect.height > p_rect.y && itemRect.y + itemRect.height < p_rect.y + p_rect.height)))
        {
            return true;
        }
        else
            return false;
    }

    private Transform instantiatedModel = null;
    public Transform Model
    {
        get
        {
            if (!instantiatedModel)
            {
                instantiatedModel = transform.Find("Model");
            }

            return instantiatedModel;
        }
    }

    public void ActivateRenderers()
    {
        if (!this)
            return;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = true;
    }

    public void DeactivateRenderers()
    {
        if (!this)
            return;

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
            renderer.enabled = false;
    }
}
