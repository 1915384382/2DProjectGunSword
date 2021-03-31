using UnityEngine;
using System;
using System.Collections.Generic;
using FreeFSM;
using LogFilter;

public enum EAnimState { Idle, Walk, Run, Jump, FlyUp, FlyDown, FlyUpSide, FlyDownSide, Landing, PushButton, Climb, ClimbOnFly }

public class CAnimFSM
{
    static bool IsCloseControlState(EAnimState inSt) { return inSt == EAnimState.Landing; }
    static bool IsFlyState(EAnimState inSt) { return inSt == EAnimState.FlyUp || inSt == EAnimState.FlyDown || inSt == EAnimState.FlyUpSide || inSt == EAnimState.FlyDownSide; }
    static bool IsMoveState(EAnimState inSt) { return inSt == EAnimState.Walk || inSt == EAnimState.Run; }

    CharacterAnimator _owner;

    CFreeFSM<EAnimState> _fsm;

    public EAnimState CurrentState { get { return _fsm.State; } }

    public CAnimFSM(CharacterAnimator owner)
    {
        _owner = owner;

        //configure fsm
        //1. Get time from Unity time, for possible change scale of time.
        //2. Set switching time like transition duration in Unity Animator
        _fsm = new CFreeFSM<EAnimState>(EAnimState.Idle, () => { return Time.time; }, TimeSpan.FromSeconds(_owner.GetSwitchAnimationPeriod()));
        _fsm.OnSwitch += (o, arg) =>
            {
                OnEntryLog(arg.Switch); //write in console
                _owner.OnChangeState(arg.Switch.Key, arg.Switch.Value); //send event to Unity Animator

                //block any switch before end of animation of this state
                _fsm.SetAnySwitchBlocked(arg.Switch.Value == EAnimState.Jump || 
                                        arg.Switch.Value == EAnimState.Landing ||
                                        arg.Switch.Value == EAnimState.Climb ||
                                        arg.Switch.Value == EAnimState.ClimbOnFly ||
                                        arg.Switch.Value == EAnimState.PushButton);
            };

        //close these switches as impossible
        _fsm.CloseSwitch(EAnimState.Idle, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.Walk, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.Run, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.Jump, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.FlyUp, EAnimState.Jump);
        _fsm.CloseSwitch(EAnimState.FlyUp, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.FlyUpSide, EAnimState.Jump);
        _fsm.CloseSwitch(EAnimState.FlyUpSide, EAnimState.Landing);
        _fsm.CloseSwitch(EAnimState.FlyDown, EAnimState.Jump);
        _fsm.CloseSwitch(EAnimState.FlyDownSide, EAnimState.Jump);
    }

    private void OnEntryLog(KeyValuePair<EAnimState, EAnimState> inSwitch)
    {
        _owner.LogInfo(string.Format("AnimFSM: {0} ---> {1} (IsGround {2}, VSpeed {3}, HSpeed {4})", inSwitch.Key, inSwitch.Value, _owner.IsGround(), _owner.GetCurrentVSpeed(), _owner.GetCurrentHState()), ELogLevel.IMPORTANT_INFO);
    }

    public bool StartClimb()
    {
        return TrySwitchState(EAnimState.Climb);
    }

    public bool StartClimbOnFly()
    {
        return TrySwitchState(EAnimState.ClimbOnFly);
    }

    public void StartJump()
    {
        if (_owner.IsGround())
            TrySwitchState(EAnimState.Jump);
    }

    public void StartPushButton()
    {
        if (_owner.IsGround())
            TrySwitchState(EAnimState.PushButton);
    }

    EAnimState _lastclosedstate = EAnimState.Idle;

    private bool TrySwitchState(EAnimState inState)
    {
        //Debug.Log(string.Format("TrySwitchState: {0}", inState));

        ESwitchResult res = _fsm.Switch(inState);

        if (res != ESwitchResult.Passed && inState != _lastclosedstate)
        {
            _owner.LogInfo(string.Format("Closed state: {0}, reason {1}", inState, res), ELogLevel.INFO);
            _lastclosedstate = inState;
        }

        return res == ESwitchResult.Passed;
    }

    //choose best state now
    EAnimState GetNeedState()
    {
        EAnimState res;
        if (_owner.IsGround())
        {
            if (IsFlyState(_fsm.State) && _owner.GetCurrentVSpeed() < _owner.GetLandingSpeed())
            {//if fall was too fast
                _owner.LogInfo(string.Format("GetCurrentVSpeed(): {0}", _owner.GetCurrentVSpeed()), ELogLevel.DEBUG);
                res = EAnimState.Landing;
            }
            else if (IsFlyState(_fsm.State) && !_owner.IsMovePushPresent())
                res = EAnimState.Idle; //landing without horizontal move
            else if (!_owner.IsCurrentHStateStand()) //if the character is moving
                if(_owner.IsWalkOn()) //in walk state
                    res = EAnimState.Walk;
                else
                    res = EAnimState.Run;
            else
                res = EAnimState.Idle;
        }
        else
        {
            if (_owner.GetCurrentVSpeed() > 0) //flying up
            {
                if (_owner.IsCurrentHStateStand())
                    res = EAnimState.FlyUp;
                else
                    res = EAnimState.FlyUpSide;
            }
            else
            {
                if (_owner.IsCurrentHStateStand())
                    res = EAnimState.FlyDown;
                else
                    res = EAnimState.FlyDownSide;
            }
        }
        return res;
    }

    // Update is called once per frame
    public void Update(float inTime)
    {
        if (_fsm.IsSwitchBlocked())
            return;

        EAnimState tr = GetNeedState();
        TrySwitchState(tr);
    }

    public bool IsCloseControlState() { return IsCloseControlState(_fsm.State); }
    public bool IsMoveState() { return IsMoveState(_fsm.State); }
    public bool IsClimbState() { return _fsm.State == EAnimState.Climb || _fsm.State == EAnimState.ClimbOnFly; }

    public void OnJumpAnimationEnd()
    {
        _fsm.SetAnySwitchBlocked(false);
    }

    public void OnLandingAnimationEnd()
    {
        _fsm.SetAnySwitchBlocked(false);
    }

    public void OnPushButtonAnimationEnd()
    {
        _fsm.SetAnySwitchBlocked(false);
    }

    public void OnClimbEnd()
    {
        _fsm.SetAnySwitchBlocked(false);
    }

}
