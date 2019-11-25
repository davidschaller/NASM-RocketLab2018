using UnityEngine;
using System;
using System.Collections.Generic;

public class Bust : ActorBase, ISceneActor, ISceneObject
{
	void Start ()
	{

	}

    #region ISceneActor Members

    void ISceneActor.MoveForward(float speed, bool fast)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.MoveForward(float speed, bool fast, bool nearWall)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.MoveBackward(float speed)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.TurnRight(float? maxAngle, Vector3 target)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.TurnLeft(float? maxAngle, Vector3 target)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.StrafeLeft(float speed)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.StrafeRight(float speed)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.MoveUp(float speed)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.MoveDown(float speed)
    {
        throw new NotImplementedException();
    }

    bool ISceneActor.ActorTypeCanStrafe()
    {
        throw new NotImplementedException();
    }

    bool ISceneActor.ActorTypeCanTurn()
    {
        throw new NotImplementedException();
    }

    bool ISceneActor.ActorTypeCanFly()
    {
        throw new NotImplementedException();
    }

    /*
    void ISceneActor.OnTriggerStay(Collider coll)
    {
        throw new NotImplementedException();
    }
     */

    void ISceneActor.ClaimPCControl()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.ReleasePCControl()
    {
        throw new NotImplementedException();
    }

    bool ISceneActor.IsPCControlled()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.Idle()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.HorseIdle()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.HorseForward(bool fast)
    {
        throw new NotImplementedException();
    }

    void ISceneActor.Dismount()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.Mount()
    {
        throw new NotImplementedException();
    }

    void ISceneActor.PlaySound(AudioClip clip)
    {
        throw new NotImplementedException();
    }

    Transform ISceneActor.GetTransform()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region ISceneObject Members

    public void GroundObject()
    {
        throw new NotImplementedException();
    }

    #endregion
}
