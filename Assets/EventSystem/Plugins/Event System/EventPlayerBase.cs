using UnityEngine;

public abstract class EventPlayerBase : MonoBehaviour
{
	public enum EventStartupType {SceneStart, Trigger, Both};
	public EventStartupType startType = EventStartupType.SceneStart;

	public abstract void PlayerTriggered();
    public abstract void Pause();
    public abstract void Resume();
	public abstract void StartOnSceneStart();
	public abstract bool Finished
	{
		get;
	}
	void Start ()
	{
		if (startType == EventStartupType.SceneStart || startType == EventStartupType.Both)
			StartOnSceneStart();
	}

}