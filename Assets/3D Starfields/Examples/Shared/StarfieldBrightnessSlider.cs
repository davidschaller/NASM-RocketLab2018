using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StarfieldBrightnessSlider : MonoBehaviour
{
    private Slider mySlider;
    private RandomStarfield myStarfield;
    private ParticleSystemRenderer myStarfieldRenderer;

    void Awake()
    {
        mySlider = GetComponent<Slider>();
        myStarfield = FindObjectOfType<RandomStarfield>();
        myStarfieldRenderer = myStarfield.GetComponent<ParticleSystemRenderer>();

        mySlider.onValueChanged.AddListener(ValueChanged);
    }

    void Start()
    {
        mySlider.value = 0.5f;
    }

    void ValueChanged(float newValue)
    {
        float alphaPower = (1 - newValue) * 5 + 1;
        myStarfieldRenderer.sharedMaterial.SetFloat("_AlphaPow", alphaPower);
    }
}
