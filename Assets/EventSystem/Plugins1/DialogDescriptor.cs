using UnityEngine;
using System.Collections;

public class DialogDescriptor : MonoBehaviour
{
	//public TaskDescriptor taskToCloseOnFinish;
	public TextAsset dialogXML;
	public bool linearDialog = false;
	public AnimationClip[] animations;
    public LipSynchDefinition[] lipSynchAnims;
	public string noMoreQuestions = "I have no more questions";
	public bool mustFinishAllQuestions = false;
	public bool dialogAvailableOnce = false;

	bool dialogDone = false;
	public bool DialogDone
	{
		get
		{
			return dialogDone;
		}
		set
		{
			dialogDone = value;
		}
	}

    [System.Serializable]
    public class LipSynchDefinition
    {
        public AudioClip audioClip;
        public TextAsset lipSynchXML;
        public string animationClipName;
    }
}
