using UnityEngine;
using System.Collections;

public interface IEvent
{
	void OnEnter();
	void OnExecute();
	void OnExit();
	bool EventIsFinished();
    void OnReset();
    void OnTerminate();
}
