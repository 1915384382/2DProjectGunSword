using FreeFSM;
using LogFilter;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(CharacterAnimator))]
public class UnitAI : MonoBehaviour
{
    [SerializeField]
    private ELogLevel _LogLevel; //Depth of output logging
    [SerializeField]
    private List<Transform> StopCheckers; //for checking edges of platforms
    [SerializeField]
    private LayerMask _StoperLayer; // A mask determining what is an edge of a platform
    [SerializeField]
    private float _StopCheckerRadius = 0.2f; //for checking edges of platforms
    [SerializeField]
    private float _IgnoreStopperPeriod = 0.5f; //for start moving on the edge (for fall off the platform)

    //ai states
    CAIState_Idle _idle_state;
    CAIState_Walk _walk_state;

    CFreeFSM<CAIState> _fsm;

    CharacterAnimator _ua;
    public CharacterAnimator UnitAnimator {  get { return _ua; } }

    CLogFilter _log;

    void Awake()
    {
        _log = new CLogFilter(_LogLevel, s => Debug.Log(s), e => Debug.LogError(e));
        _log.SetLogLevelTracker(() => { return _LogLevel; });

        _ua = GetComponent<CharacterAnimator>();

        _idle_state = new CAIState_Idle(this);
        _walk_state = new CAIState_Walk(this);

        _fsm = new CFreeFSM<CAIState>(_idle_state, () => { return Time.time; }, TimeSpan.FromSeconds(0));
        _fsm.OnSwitch += (o, arg) =>
        {
            OnEntryLog(arg.Switch);
            arg.Switch.Key.OnExit();
        };
    }

    public void DebugLog(string inText, ELogLevel inLevel) { _log.Log(inText, inLevel); }

    private void OnEntryLog(KeyValuePair<CAIState, CAIState> inSwitch)
    {
        DebugLog(string.Format("UnitAI: {0} ---> {1}", inSwitch.Key.GetState(), inSwitch.Value.GetState()), ELogLevel.INFO);
    }

    Collider2D CheckCollision(Vector3 inPoint, float inPointRadius, LayerMask inLayerMask)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(inPoint, inPointRadius, inLayerMask);
        Collider2D baseobj = Array.Find(colliders, c => c.gameObject != gameObject);
        return baseobj;
    }

    //define - character is on an edge or no
    public bool IsCheckStopers()
    {
        for(int i = 0; i < StopCheckers.Count; i++)
        {
            if(CheckCollision(StopCheckers[i].position, _StopCheckerRadius, _StoperLayer) != null)
                return true;
        }
        return false;
    }

    void Update()
    {
        _fsm.State.Update(Time.smoothDeltaTime);
    }

    public bool IsActive() { return _fsm.State != _idle_state; }

    public void Disable()
    {
        DebugLog(string.Format("UnitAI: {0}", MethodInfo.GetCurrentMethod().Name), ELogLevel.INFO);
        _fsm.Switch(_idle_state);
    }

    public void StartWalk(EMoveState inMoveState, bool inWalk, float inTime)
    {
        DebugLog(string.Format("UnitAI: {0}", MethodInfo.GetCurrentMethod().Name), ELogLevel.DEBUG);

        if (_fsm.Switch(_walk_state) == ESwitchResult.Passed)
        {
            _walk_state.Start(inMoveState, inWalk, _IgnoreStopperPeriod, inTime);
        }
    }

    public void OnFinishState(CAIState inAIState)
    {
        _fsm.Switch(_idle_state);
    }

}
