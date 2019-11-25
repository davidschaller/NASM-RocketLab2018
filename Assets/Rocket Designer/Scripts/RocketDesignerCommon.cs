using UnityEngine;

public class RocketDesignerCommon
{
    public const string NORMAL = "idle",
                        HOVER = "hover",
                        PRESSED = "pressed";

    public static UIImageButton SetButton(Transform panel, string buttonName, AudioClip clickAudioClip)
    {
        UIImageButton uiImageButton = null;

        Transform trButton = panel.Find(buttonName);

        if (trButton)
        {
            uiImageButton = trButton.gameObject.AddComponent<UIImageButton>();

            UIButtonSound uiSoundbutton = trButton.gameObject.AddComponent<UIButtonSound>();

            if (clickAudioClip)
                uiSoundbutton.audioClip = clickAudioClip;

            Transform idle = trButton.Find(NORMAL);
            if (idle)
            {
                foreach (Transform tr in idle)
                {
                    if (tr.name.Contains(NORMAL))
                    {
                        uiImageButton.target = tr.GetComponent<UISprite>();
                        uiImageButton.normalSprite = uiImageButton.target.spriteName;

                        BoxCollider boxCollider = uiImageButton.gameObject.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                        boxCollider.size = new Vector3(uiImageButton.target.transform.localScale.x, uiImageButton.target.transform.localScale.y, 1);
                        break;
                    }
                }

                Transform hover = trButton.Find(HOVER);
                if (hover)
                {
                    foreach (Transform tr in hover)
                    {
                        if (tr.name.Contains(HOVER))
                        {
                            uiImageButton.hoverSprite = tr.GetComponent<UISprite>().spriteName;
                            break;
                        }
                    }
                }
                else
                    uiImageButton.hoverSprite = idle.GetComponentInChildren<UISprite>().spriteName;

                Transform pressed = trButton.Find(PRESSED);
                if (pressed)
                {
                    foreach (Transform tr in pressed)
                    {
                        if (tr.name.Contains(PRESSED))
                        {
                            uiImageButton.pressedSprite = tr.GetComponent<UISprite>().spriteName;
                            break;
                        }
                    }
                }
                else
                    uiImageButton.pressedSprite = idle.GetComponentInChildren<UISprite>().spriteName;

                if (hover)
                    GameObject.Destroy(hover.gameObject);

                if (pressed)
                    GameObject.Destroy(pressed.gameObject);
            }
        }

        return uiImageButton;
    }

    public static UIImageButton MakeButton(Transform trButton, AudioClip clickAudioClip)
    {
        UIImageButton uiImageButton = null;

        if (trButton)
        {
            uiImageButton = trButton.gameObject.AddComponent<UIImageButton>();

            UIButtonSound uiSoundbutton = trButton.gameObject.AddComponent<UIButtonSound>();

            if (clickAudioClip)
                uiSoundbutton.audioClip = clickAudioClip;

            Transform idle = trButton.Find(NORMAL);
            if (idle)
            {
                bool gotIdle = false;
                foreach (Transform tr in idle)
                {
                    UISprite uiSprite = tr.GetComponent<UISprite>();

                    if (uiSprite)
                    {
                        uiImageButton.target = tr.GetComponent<UISprite>();
                        uiImageButton.normalSprite = uiImageButton.target.spriteName;

                        if (uiSprite.atlas.GetSprite(uiImageButton.target.spriteName) == null)
                        {
                            Debug.LogError("The atlas '" + uiSprite.atlas.name + "' doesn't have sprite with name '" + uiImageButton.target.spriteName + "'. It means that '" + trButton.name + "' background is invisible! ", trButton);
                        }

                        BoxCollider boxCollider = uiImageButton.gameObject.AddComponent<BoxCollider>();
                        boxCollider.isTrigger = true;
                        boxCollider.size = new Vector3(uiImageButton.target.transform.localScale.x, uiImageButton.target.transform.localScale.y, 1);

                        gotIdle = true;
                        break;
                    }
                }

                Transform hover = trButton.Find(HOVER);
                if (hover)
                {
                    foreach (Transform tr in hover)
                    {
                        UISprite uiSprite = tr.GetComponent<UISprite>();

                        if (uiSprite)
                        {
                            uiImageButton.hoverSprite = tr.GetComponent<UISprite>().spriteName;

                            if (uiSprite.atlas.GetSprite(uiImageButton.target.spriteName) == null)
                            {
                                Debug.LogError("The atlas '" + uiSprite.atlas.name + "' doesn't have sprite with name '" + uiImageButton.target.spriteName + "'. It means that '" + trButton.name + "' background is invisible! ", trButton);
                            }

                            break;
                        }
                    }
                }
                else
                    uiImageButton.hoverSprite = uiImageButton.normalSprite;

                Transform pressed = trButton.Find(PRESSED);
                if (pressed)
                {
                    foreach (Transform tr in pressed)
                    {
                        UISprite uiSprite = tr.GetComponent<UISprite>();

                        if (uiSprite)
                        {
                            uiImageButton.pressedSprite = tr.GetComponent<UISprite>().spriteName;

                            if (uiSprite.atlas.GetSprite(uiImageButton.target.spriteName) == null)
                            {
                                Debug.LogError("The atlas '" + uiSprite.atlas.name + "' doesn't have sprite with name '" + uiImageButton.target.spriteName + "'. It means that '" + trButton.name + "' background is invisible! ", trButton);
                            }
                            break;
                        }
                    }
                }
                else
                    uiImageButton.pressedSprite = uiImageButton.normalSprite;

                if (hover)
                    GameObject.Destroy(hover.gameObject);

                if (pressed)
                    GameObject.Destroy(pressed.gameObject);
            }
        }

        return uiImageButton;
    }
}
