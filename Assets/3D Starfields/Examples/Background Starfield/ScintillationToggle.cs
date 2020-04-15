using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScintillationToggle : MonoBehaviour
{
    public StarfieldBase myStarfield;
    private Toggle myToggle;

    void Awake()
    {
        myToggle = GetComponent<Toggle>();
    }

    void Start()
    {
        myToggle.isOn = myStarfield.myUseScintillation;
        myToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void OnToggleChanged(bool isOn)
    {
        myStarfield.SetScintillation(isOn);
    }
}
