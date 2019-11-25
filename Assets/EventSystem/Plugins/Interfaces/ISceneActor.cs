using UnityEngine;
using System;

public interface ISceneActor
{
	void MoveForward(float speed, bool fast);
	void MoveForward(float speed,float AnimSpeed, bool fast);
    void MoveForward(float speed, bool fast, bool nearWall);
	void MoveBackward(float speed);		
	void MoveBackward(float speed, float AnimSpeed);		
	void TurnRight(float? maxAngle, Vector3 target);
    void TurnLeft(float? maxAngle, Vector3 target);		
	void StrafeLeft(float speed);		
	void StrafeRight(float speed);		
	void MoveUp(float speed);		
	void MoveDown(float speed);		
	bool ActorTypeCanStrafe();		
	bool ActorTypeCanTurn();		
	bool ActorTypeCanFly();		
	//void OnTriggerStay(Collider coll);
	void ClaimPCControl();
	void ReleasePCControl();
	bool IsPCControlled();
    void Idle();
    void HorseIdle();
    void HorseForward(bool fast);
    void Dismount();
    void Mount();
    void PlaySound(AudioClip clip);
	Transform GetTransform();
}
