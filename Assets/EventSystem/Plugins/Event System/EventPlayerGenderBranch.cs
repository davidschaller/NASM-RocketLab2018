using UnityEngine;

public class EventPlayerGenderBranch : EventPlayerBase
{
	public EventPlayerBase maleEventPlayer;
	public EventPlayerBase femaleEventPlayer;
	public override bool Finished
	{
		get
		{
			if (SaveManager.Gender == Gender.Male && maleEventPlayer)
				return maleEventPlayer.Finished;
			else if (SaveManager.Gender == Gender.Female && femaleEventPlayer)
				return femaleEventPlayer.Finished;
			else
				return true;
		}
	}

	public override void PlayerTriggered()
	{
		Trigger();
	}
	
	public override void StartOnSceneStart()
	{
		Trigger();
	}

	void Trigger ()
	{
		if (SaveManager.Gender == Gender.Male)
		{
			Debug.Log("Playing male event player", gameObject);
			if (maleEventPlayer)
				maleEventPlayer.PlayerTriggered();
			else
				Debug.LogWarning("No male event player for gender branching EP", gameObject);
		}
		else
		{
			Debug.Log("Playing female event player");
			if (femaleEventPlayer)
				femaleEventPlayer.PlayerTriggered();
			else
				Debug.LogWarning("No female event player for gender branching EP", gameObject);
		}
	}

    public override void Pause()
    {
        throw new System.NotImplementedException();
    }

    public override void Resume()
    {
        throw new System.NotImplementedException();
    }
}
