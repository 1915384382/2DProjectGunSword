using FreeFSM;
using LogFilter;
using System.Collections.Generic;
using UnityEngine;

public enum EPCStateType { Stay, DirectMove, WaitingTurn, WaitingAIWalk }

public abstract class CPCState
{
    protected PlayerControl _owner;
    protected CharacterAnimator _character;

    protected CPCState(PlayerControl inOwner, CharacterAnimator inChAn) { _owner = inOwner; _character = inChAn; }

    public abstract EPCStateType GetStateType();
    public abstract void Update(float inFrameTime, EMoveState state);
    public virtual void OnEnter(EMoveState inMoveState) { _owner.Timers.Reset(GetStateType()); }
    public virtual void OnExit() { }
}

public class CPCState_Stay : CPCState
{
    public CPCState_Stay(PlayerControl inOwner, CharacterAnimator inChAn) : base(inOwner, inChAn) { }

    public override EPCStateType GetStateType() { return EPCStateType.Stay; }
    public override void Update(float inFrameTime, EMoveState state)
    {
        if (state == EMoveState.Stand)
            return;

        if (_character.IsCurrentHStateStand())
        {
            bool fr = state == EMoveState.Right;
            if (_character.FacingRight != fr)
                _owner.SetNewState(EPCStateType.WaitingTurn, state);
            else
                _owner.SetNewState(EPCStateType.WaitingAIWalk, state);
        }
        else
            _owner.SetNewState(EPCStateType.DirectMove, state);
    }
}

public class CPCState_DirectMove : CPCState
{
    public CPCState_DirectMove(PlayerControl inOwner, CharacterAnimator inChAn) : base(inOwner, inChAn) { }

    public override EPCStateType GetStateType() { return EPCStateType.DirectMove; }

    public override void OnEnter(EMoveState inMoveState)
    {
        base.OnEnter(inMoveState);
        _owner.AIDisable();
        _character.SetMoveControl(inMoveState, "PlayerControl");
    }

    public override void Update(float inFrameTime, EMoveState state)
    {
        if (state == EMoveState.Stand)
        {
            if (!_owner.IsAIActive())
                _character.SetMoveControl(state, "PlayerControl");

            _owner.SetNewState(EPCStateType.Stay, state);
            return;
        }

        if(_character.MoveControl != state)
            _character.SetMoveControl(state, "PlayerControl");
    }
}

public class CPCState_WaitingTurn : CPCState
{
    public CPCState_WaitingTurn(PlayerControl inOwner, CharacterAnimator inChAn) : base(inOwner, inChAn) { }

    public override EPCStateType GetStateType() { return EPCStateType.WaitingTurn; }

    public override void OnEnter(EMoveState inMoveState) { base.OnEnter(inMoveState); _character.Flip(); }

    public override void Update(float inFrameTime, EMoveState state)
    {
        if (state == EMoveState.Stand)
        {
            _owner.SetNewState(EPCStateType.Stay, state);
            return;
        }

        if (_owner.Timers.Update(GetStateType(), _owner.TurnPeriod, inFrameTime))
            _owner.SetNewState(EPCStateType.DirectMove, state);
    }
}

public class CPCState_WaitingAIWalk : CPCState
{
    public CPCState_WaitingAIWalk(PlayerControl inOwner, CharacterAnimator inChAn) : base(inOwner, inChAn) { }

    public override EPCStateType GetStateType() { return EPCStateType.WaitingAIWalk; }

    public override void OnEnter(EMoveState inMoveState)
    {
        base.OnEnter(inMoveState);
        _owner.StartAIWalk(inMoveState);
    }

    public override void Update(float inFrameTime, EMoveState state)
    {
        if (state == EMoveState.Stand)
        {
            _owner.SetNewState(EPCStateType.Stay, state);
            return;
        }

        if (_owner.Timers.Update(GetStateType(), _owner.MovePeriod, inFrameTime))
            _owner.SetNewState(EPCStateType.DirectMove, state);
    }
}

[RequireComponent(typeof(CharacterAnimator))]
[RequireComponent(typeof(UnitAI))]
public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private ELogLevel _LogLevel; //Depth of output logging

    [SerializeField]
    public float MoveThreshold = 0.2f;
    [SerializeField]
    public float TurnPeriod = 0.2f;
    [SerializeField]
    public float MovePeriod = 0.3f;
    [SerializeField]
    public float AutoWalkTime = 0.3f;

    CharacterAnimator _character;

    UnitAI _ai;

    CFreeFSMbase<CPCState, EMoveState> _fsm;

    List<CPCState> _states;

    CLogFilter _log;

    CTimeCounters<EPCStateType> _timers;
    public CTimeCounters<EPCStateType> Timers { get { return _timers; } }

    private void Awake()
    {
        _timers = new CTimeCounters<EPCStateType>();
        _character = GetComponent<CharacterAnimator>();
        _ai = GetComponent<UnitAI>();

        _log = new CLogFilter(_LogLevel, s => Debug.Log(s), e => Debug.LogError(e));
        _log.SetLogLevelTracker(() => { return _LogLevel; });

        _states = new List<CPCState>();
        _states.Add(new CPCState_DirectMove(this, _character));

        _fsm = new CFreeFSMbase<CPCState, EMoveState>(GetState(EPCStateType.Stay));
        _fsm.OnSwitch += (o, arg) =>
        {
            OnEntryLog(arg.Switch);
            arg.Switch.Key.OnExit();
            arg.Switch.Value.OnEnter(arg.Package);
        };
    }

    CPCState GetState(EPCStateType inStateType)
    {
        CPCState state = _states.Find(s => s.GetStateType() == inStateType);
        if (state != null)
            return state;

        switch (inStateType)
        {
            case EPCStateType.Stay: state = new CPCState_Stay(this, _character); break;
            case EPCStateType.DirectMove: state = new CPCState_DirectMove(this, _character); break;
            case EPCStateType.WaitingAIWalk: state = new CPCState_WaitingAIWalk(this, _character); break;
            case EPCStateType.WaitingTurn: state = new CPCState_WaitingTurn(this, _character); break;
        }

        _states.Add(state);
        return state;
    }

    internal void SetNewState(EPCStateType inStateType, EMoveState state)
    {
        _fsm.Switch(GetState(inStateType), state);
    }

    internal void StartAIWalk(EMoveState inMoveState)
    {
        if (_character.IsCurrentHStateStand())
            _ai.StartWalk(inMoveState, true, AutoWalkTime);
    }

    public void DebugLog(string inText, ELogLevel inLevel) { _log.Log(inText, inLevel); }
    private void OnEntryLog(KeyValuePair<CPCState, CPCState> inSwitch)
    {
        DebugLog(string.Format("PlayerControl: {0} ---> {1}", inSwitch.Key.GetStateType(), inSwitch.Value.GetStateType()), ELogLevel.INFO);
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");
        EMoveState movestate = move < -MoveThreshold ? EMoveState.Left : move > MoveThreshold ? EMoveState.Right : EMoveState.Stand;

        _fsm.State.Update(Time.smoothDeltaTime, movestate);

        if (Input.GetButton("Jump"))
        {
            AIDisable();
            _character.StartJump();
        }

        if (Input.GetButton("Submit"))
        {
            AIDisable();
            _character.StartPushButton();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            AIDisable();
            _character.SwitchWalkOn();
        }
    }

    public bool IsAIActive() { return _ai.IsActive(); }

    public void AIDisable()
    {
        if(_ai.IsActive())
            _ai.Disable();
    }
}

