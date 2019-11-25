using UnityEngine;
using System.Collections;

public class MixingTransformTests : MonoBehaviour
{
    public Animation anim;

    public AnimationClip clip;

    public Transform mixingTr;

    public int layer = 1;

    // Use this for initialization
    void OnEnable()
    {
        Debug.LogError("MixingTransformTests OnEnable");

        if (anim[clip.name] == null)
        {
            anim.AddClip(clip, clip.name);
        }

        anim[clip.name].weight = 1;

        //anim.Blend(clip.name, 0.0f, 0.3f);
        anim[clip.name].AddMixingTransform(mixingTr);
        anim[clip.name].layer = layer;

        anim.CrossFade(clip.name, .5f, PlayMode.StopSameLayer);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
