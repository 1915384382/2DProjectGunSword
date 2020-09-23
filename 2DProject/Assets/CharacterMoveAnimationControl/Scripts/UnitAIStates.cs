using LogFilter;

public enum EAIState { Idle, Walk }

public abstract class CAIState
{
    protected UnitAI _owner;

    protected CAIState(UnitAI ua) { _owner = ua; }

    public abstract EAIState GetState();
    public abstract void Update(float inFrameTime);
    public abstract void OnExit();
}

public class CAIState_Idle : CAIState
{
    public CAIState_Idle(UnitAI ua) : base(ua) { }

    public override EAIState GetState() { return EAIState.Idle; }
    public override void Update(float inFrameTime) { }
    public override void OnExit() { }
}

//goal for this state is moving the character during required time (walk or run)
public class CAIState_Walk : CAIState
{
    public CAIState_Walk(UnitAI ua) : base(ua) { }

    public override EAIState GetState() { return EAIState.Walk; }

    EMoveState _MoveState;
    float _walkperiod;
    float _ignorestoppersperiod;
    bool _was_walk;

    enum ETimeCounters { WalkTime, IgnoreStoppers }
    CTimeCounters<ETimeCounters> _timers = new CTimeCounters<ETimeCounters>();

    //init state method
    //inIgnoreStopperPeriod - from unity inspector
    public void Start(EMoveState inMoveState, bool inWalk, float inIgnoreStopperPeriod, float inTime)
    {
        _was_walk = _owner.UnitAnimator.IsWalkOn();
        _owner.UnitAnimator.SetWalkOn(inWalk);

        _ignorestoppersperiod = inIgnoreStopperPeriod;

        _MoveState = inMoveState;
        _walkperiod = inTime;
        _timers.Reset(ETimeCounters.WalkTime);

        if (_owner.IsCheckStopers()) //if the character stands on an edge right now
        {
            _timers.Reset(ETimeCounters.IgnoreStoppers); //ai will be ignore this some time
            _owner.DebugLog("CAIState_Walk.Start: CheckStoppers true", ELogLevel.DEBUG);
        }
        else
        {
            _timers.SetTime(ETimeCounters.IgnoreStoppers, _ignorestoppersperiod); //AI will be checking an edge at once
            _owner.DebugLog("CAIState_Walk.Start: CheckStoppers false", ELogLevel.DEBUG);
            _owner.DebugLog(string.Format("CAIState_Walk._timers.IgnoreStoppers.GetPassedTime {0}", _timers.GetPassedTime(ETimeCounters.IgnoreStoppers)), ELogLevel.DEBUG);
        }

        _owner.UnitAnimator.SetMoveControl(_MoveState, "UnitAI"); //start moving
    }

    public override void OnExit()
    {//stop moving
        _owner.UnitAnimator.SetMoveControl(EMoveState.Stand, "UnitAI");
        _owner.UnitAnimator.SetWalkOn(_was_walk);
    }

    public override void Update(float inFrameTime)
    {
        //moving and checking move time and edges of platforms
        bool bWalkTimeFinish = _timers.Update(ETimeCounters.WalkTime, _walkperiod, inFrameTime);
        bool bCheckStoppers = _timers.Update(ETimeCounters.IgnoreStoppers, _ignorestoppersperiod, inFrameTime);
        _owner.DebugLog(string.Format("CAIState_Walk._timers.IgnoreStoppers.GetPassedTime {0}", _timers.GetPassedTime(ETimeCounters.IgnoreStoppers)), ELogLevel.DEBUG);

        bool bStopByCheck = false;
        if (!bWalkTimeFinish && bCheckStoppers)
            bStopByCheck = _owner.IsCheckStopers();

        _owner.DebugLog(string.Format("CAIState_Walk.Update: bCheckStoppers {0}, bStopByCheck {1}", bCheckStoppers, bStopByCheck), ELogLevel.DEBUG);

        if (bWalkTimeFinish || bStopByCheck)
        {
            _owner.OnFinishState(this);
            return;
        }
    }
}
