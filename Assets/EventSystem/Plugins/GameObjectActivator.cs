using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectActivator : MonoBehaviour
{
    const string GO_NAME = "Game Object Activator",
                 CANNONS_PROGRESS_BAR = "Cannons Progress Bar";

    public static GameObjectActivator DoActivation(GameObject target, int recursiveLevel, string ezDialogName)
    {
        GameObject go = new GameObject(GO_NAME);

        GameObjectActivator obj = go.AddComponent<GameObjectActivator>();

        obj.Init(target, recursiveLevel, ezDialogName);

        return obj;
    }

    UIButton continueButton;
    UIProgressBar progressBar;

    void Init(GameObject target, int recursiveLevel, string ezDialogName)
    {
        this.target = target;
        this.recursiveLevel = recursiveLevel;

        if (!string.IsNullOrEmpty(ezDialogName))
        {
            GameObject goEzDialog = GameObject.Find(ezDialogName);

            if (!goEzDialog)
                goEzDialog = GameObject.Find(ezDialogName + "(Clone)");

            if (!goEzDialog)
                goEzDialog = GameObject.Find(ezDialogName + " (Clone)");

            if (goEzDialog)
            {
                foreach (Transform tr in goEzDialog.transform)
                {
                    if (tr.GetComponent<UIButton>())
                    {
                        continueButton = tr.GetComponent<UIButton>();
                        continueButton.gameObject.SetActiveRecursively(false);
                    }
                }

                GameObject goProgress = GameObject.Find(CANNONS_PROGRESS_BAR);

                if (goProgress)
                {
                    progressBar = goProgress.GetComponent<UIProgressBar>();
                    
                    if (progressBar)
                    {
                        progressBar.transform.position = continueButton.transform.position;
                        progressBar.Value = 0;

                        progressBar.gameObject.layer = LayerMask.NameToLayer("GUI");
                        foreach (Transform tr in progressBar.transform)
                        {
                            tr.gameObject.layer = LayerMask.NameToLayer("GUI");
                        }
                    }
                }
            }
        }

        StartCoroutine(CountPartsCoroutine(target.transform, 0));
    }

    int countringCoroutines = 0;
    IEnumerator CountPartsCoroutine(Transform root, int level)
    {
        countringCoroutines++;

        if (level == recursiveLevel || root.childCount == 0)
        {
            parts++;
        }
        else
        {
            level++;
            foreach (Transform tr in root)
            {
                StartCoroutine(CountPartsCoroutine(tr, level));
                yield return 0;
            }
        }

        countringCoroutines--;

        if (root.gameObject == target)
        {
            while (countringCoroutines > 1)
                yield return 0;

            StartActivation();
        }
    }

    public static int ProcessPercentage { get; private set; }

    public GameObject target;

    public int recursiveLevel = 2;

    public int parts = 0,
               doneParts = 0;

    void StartActivation()
    {
        StartCoroutine(ActivationCoroutine(target.transform, 0));
    }

    void FinishActivation()
    {
        if (continueButton)
        {
            continueButton.gameObject.layer = LayerMask.NameToLayer("GUI");
            foreach (Transform tr in continueButton.transform)
                tr.gameObject.layer = LayerMask.NameToLayer("GUI");

            continueButton.gameObject.SetActiveRecursively(true);
        }

        if (progressBar)
        {
            progressBar.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
            foreach (Transform tr in progressBar.transform)
                tr.gameObject.layer = LayerMask.NameToLayer("TransparentFX");
        }

        Destroy(gameObject);
    }

    IEnumerator ActivationCoroutine(Transform root, int level)
    {
        if (level == recursiveLevel || root.childCount == 0)
        {
            doneParts++;
            progressBar.Value = (float)doneParts / parts;

            root.gameObject.SetActiveRecursively(true);
        }
        else
        {
            level++;
            foreach (Transform tr in root)
            {
                StartCoroutine(ActivationCoroutine(tr, level));
                yield return 0;
            }
        }

        if (root.gameObject == target)
        {
            while (doneParts < parts)
                yield return 0;

            root.gameObject.active = true;

            FinishActivation();
        }
        else
            root.gameObject.active = true;
    }
}
