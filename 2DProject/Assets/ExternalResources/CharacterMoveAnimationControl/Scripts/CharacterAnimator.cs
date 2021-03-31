using LogFilter;
using System;
using System.Reflection;
using UnityEngine;

public enum EMoveState { Left, Stand, Right }

public class CharacterAnimator : MonoBehaviour
{

    [SerializeField]
    private ELogLevel _LogLevel; //Depth of output logging

    [SerializeField]
    private float _SwitchAnimPeriod = 0.15f;

    [SerializeField]
    private float _RunSpeed = 3.5f;
    [SerializeField]
    private float _WalkSpeed = 1.5f;

    [SerializeField]
    private float _accel = 6.0f; //horizontal inertial param
    [SerializeField]
    private float _brake = 6.0f; //horizontal inertial param

    private bool _walk_on = false; //walk or run

    private EMoveState _move_control = EMoveState.Stand; //horizontal moving was setted from outside
    public EMoveState MoveControl { get { return _move_control; } }

    private EMoveState _move = EMoveState.Stand; //horizontal moving

    private CScalarVelocityIntegrator _velocity; //horizontal speed [units per second] for moving, has inertia

    [SerializeField]
    private float _MaxYSpeed = 10f; //bound vertical speed
    [SerializeField]
    private float _JumpForce = 400f; // Amount of force added when the player jumps.
    [SerializeField]
    private float _GroundCheckSkipTimeAfterJump = 0.5f; //after start of jump, in during this time script doesn't check ground. in otherwise script will be set Idle state after jump animation finished and when ground check will be in ground yet.
    [SerializeField]
    private float _LandingSpeed = -0.5f; //threshold of vetical speed when after landing will be playing landing animation

    [SerializeField]
    private float _GroundCheckRadius = 0.2f; //for ground checking
    [SerializeField]
    private float _CeilingCheckRadius = 0.1f; //for ceiling checking, preventing jump under low roof


    [SerializeField]
    private LayerMask _GroundLayer; // A mask determining what is ground to the character
    [SerializeField]
    private LayerMask _ClimbLayer; // A mask determining places for clamber up

    private CAnimFSM _fsm; //state machine for animation control

    private bool _grounded; // Whether or not the player is grounded.

    private CCheckProbe _groundprobe; //transform + radius for ground checking
    private CCheckProbe _ceilingprobe; //...for ceiling checking
    private CCheckProbe _frontprobe; //...for front obstacle checking and climb places
    private CCheckProbe _climbprobe; //...for climb places checking from ground
    private CCheckProbe _flyclimbprobe; //...for climb places checking on fly

    private Animator _anim; // Reference to the player's animator component.
    private Rigidbody2D _Rigidbody2D; // Reference to the player's rigidbody component.
    private bool _FacingRight = true;  // For determining which way the player is currently facing.
    public bool FacingRight {  get { return _FacingRight; } }


    //Delays between when Player pushes a button "forward" in front of an obstacle and before how character decides to climb on the obstacle.
    [SerializeField]
    private float _StayToClimbDelay = 0.3f; //character is on the ground.
    [SerializeField]
    private float _FlyToClimbDelay = 0.1f; //character is flying.

    //Climbing parameters
    private Vector3 _ClimbStart; //start position for climbing state
    private Vector3 _ClimbTarget; //finish position for climbing state
    [SerializeField]
    private float ClimbShiftPeriod = 0.1f; //time of first part of climb - shifting to start point
    [SerializeField]
    private float _ClimbPeriod = 0.5f; //second part: how long time climbing from the ground
    [SerializeField]
    private float _ClimbPeriodOnFly = 0.7f; //second part: how long time climbing on the fly
    [SerializeField]
    private float _ClimbGrabRadius = 0.05f; //parameter for check an opportunity of climb
    Transform _FinalClimbTargetPoint; //using in climb update
    bool _climbshift = true; //switcher from first part of climb to second part
    [SerializeField]
    private bool _AutoClimbOnFly = true; //character will be try to grab and climb when he can.

    enum ETimeCounters { Climb, PushObs, Move, GroundCheck } //types of time counter
    CTimeCounters<ETimeCounters> _timers;

    [SerializeField]
    private Transform GroundCheck; //child transform for ground check
    [SerializeField]
    private Transform CeilingCheck; //child transform for ceiling check
    [SerializeField]
    private Transform FrontCheck; //child transform for front obstacle check (stop or climb)
    [SerializeField]
    private Transform ClimbCheck; //child transform for front obstacle check climb
    [SerializeField]
    private Transform FlyClimbCheck; //child transform for check an oppotunity of climb
    [SerializeField]
    private Transform ClimbPoint; //child transform defines finish climb point (climb from the ground)
    [SerializeField]
    private Transform ClimbPointOnFly; //child transform defines finish climb point (climb from on the fly)

    CLogFilter _log; //for control count of messages in console

    private void Awake()
    {
        _log = new CLogFilter(_LogLevel, s => Debug.Log(s), e => Debug.LogError(e));
        _log.SetLogLevelTracker(() => { return _LogLevel; });

        _timers = new CTimeCounters<ETimeCounters>();
        _timers.SetTime(ETimeCounters.GroundCheck, _GroundCheckSkipTimeAfterJump);
        _grounded = true;

        //if (_Graph != null)
        //    _graph = _Graph.GetComponent<DrawGraphicsScript>();

        // Setting up references.
        _groundprobe = new CCheckProbe(GroundCheck, _GroundCheckRadius);
        _ceilingprobe = new CCheckProbe(CeilingCheck, _CeilingCheckRadius);
        _frontprobe = new CCheckProbe(FrontCheck, _ClimbGrabRadius);
        _climbprobe = new CCheckProbe(ClimbCheck, _ClimbGrabRadius);
        _flyclimbprobe = new CCheckProbe(FlyClimbCheck, _ClimbGrabRadius);

        _anim = GetComponent<Animator>();
        _Rigidbody2D = GetComponent<Rigidbody2D>();

        _velocity = new CScalarVelocityIntegrator(0, _accel, _brake);

        _fsm = new CAnimFSM(this);
    }

    #region methods for outer scripts
    public bool IsWalkOn() { return _walk_on; }

    //switch from run to walk and back again
    public void SwitchWalkOn()
    {
        LogInfo(string.Format("{0}, was {1} to {2}", MethodInfo.GetCurrentMethod().Name, _walk_on, !_walk_on), ELogLevel.INFO);
        _walk_on = !_walk_on;
    }
    public void SetWalkOn(bool inValue)
    {
        LogInfo(string.Format("{0}, was {1} to {2}", MethodInfo.GetCurrentMethod().Name, _walk_on, inValue), ELogLevel.INFO);
        _walk_on = inValue;
    }

    public bool IsMovePushPresent() { return _move_control != EMoveState.Stand; }

    //main way for moving control
    //inSource - for trace and debug
    public void SetMoveControl(EMoveState inMove, string inSource)
    {
        //animation state can't be under control
        if (_fsm.IsCloseControlState())
            return;

        if (inMove != _move_control)
            LogInfo(string.Format("{0}, inMove {1} from {2}", MethodInfo.GetCurrentMethod().Name, inMove, inSource), ELogLevel.DEBUG);

        _move_control = inMove;
    }

    public void StartJump()
    {
        if (_fsm.IsCloseControlState())
            return;

        //before jump character checks ceiling
        if (CheckGround(_ceilingprobe))
            return;

        //1. character tries to climb to the upper platform
        //2. character can jump only from the ground
        if (!TryClimbOnFly() && _grounded)
            _fsm.StartJump();
    }

    //start special animation state
    public void StartPushButton()
    {
        if (_fsm.IsCloseControlState())
            return;
        _fsm.StartPushButton();
    }

    #endregion

    #region linked with event in animation
    private void OnJumpAnimationEnd()
    {
        _fsm.OnJumpAnimationEnd();

        if (!_grounded)
            return;
        // Add a vertical force to the player.
        _grounded = false;
        _timers.Reset(ETimeCounters.GroundCheck);
        _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, _Rigidbody2D.velocity.y + 0.01f);
        _Rigidbody2D.AddForce(new Vector2(0f, _JumpForce));
    }

    private void OnLandingAnimationEnd()
    {
        _fsm.OnLandingAnimationEnd();
    }

    private void OnPushButtonAnimationEnd()
    {
        _fsm.OnPushButtonAnimationEnd();
    }

    #endregion

    #region helper methods

    Collider2D CheckCollision(CCheckProbe inProbe, LayerMask inLayerMask)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(inProbe.Point.position, inProbe.Radius, inLayerMask);
        Collider2D baseobj = Array.Find(colliders, c => c.gameObject != gameObject);
        return baseobj;
    }

    //check collision with ground layer
    bool CheckGround(CCheckProbe inProbe) { return CheckCollision(inProbe, _GroundLayer) != null; }

    //switch character collision
    void SetCollisionEnabled(bool inValue)
    {
        Collider2D[] colliders = transform.GetComponents<Collider2D>();
        foreach (var c in colliders)
            c.enabled = inValue;
    }

    //character pushes on the obstacle in front of him
    bool IsPushingFrontObstacle(EMoveState inMovePushValue)
    {
        if (inMovePushValue == EMoveState.Stand)
            return false;

        if (inMovePushValue == EMoveState.Left == _FacingRight)
            return false;

        return CheckGround(_frontprobe);
    }

    void ResetAllSpeed()
    {
        _move = EMoveState.Stand;
        _move_control = EMoveState.Stand;
        _velocity.SetNewVelocity(0);
    }
    #endregion

    #region Climbing

    //collision with special corners of platforms for climb
    Collider2D CheckGrabCorner(CCheckProbe inProbe) { return CheckCollision(inProbe, _ClimbLayer); }

    bool TryClimbFromGround()
    {
        //character is in required position
        Collider2D col = CheckGrabCorner(_climbprobe);
        if (col == null)
            return false;

        //ClimbPoint defined and animation fsm could turn into climb state
        if (ClimbPoint != null && _fsm.StartClimb())
        {
            OnStartClimb(ClimbPoint, _climbprobe.Point, col);
            return true;
        }
        return false;
    }

    bool TryClimbOnFly()
    {
        Collider2D col = CheckGrabCorner(_flyclimbprobe);
        if (col == null)
            return false;

        if (ClimbPointOnFly != null && _fsm.StartClimbOnFly())
        {
            OnStartClimb(ClimbPointOnFly, _flyclimbprobe.Point, col);
            return true;
        }
        return false;
    }

    void OnStartClimb(Transform inClimbTargetPoint, Transform inClimbGrabPoint, Collider2D inCollider)
    {
        _climbshift = true; //first part of climb starts
        _FinalClimbTargetPoint = inClimbTargetPoint; //set in memory final transform for second part

        //our goal is to combine character's grab point and corner point
        _ClimbTarget = inCollider.transform.position - inClimbGrabPoint.position + transform.position; 
        _ClimbStart = transform.position;
        _timers.Reset(ETimeCounters.Climb); //reset time counter
        SetCollisionEnabled(false); //switch off character's collision
        ResetAllSpeed();
    }

    //character's climb to a platform
    void UpdateClimb(float inUpdateTime)
    {
        //choose period of climb
        float preiod = _climbshift ? ClimbShiftPeriod : _fsm.CurrentState == EAnimState.Climb ? _ClimbPeriod : _ClimbPeriodOnFly;

        //increase timer and if period finished...
        if (_timers.Update(ETimeCounters.Climb, preiod, inUpdateTime))
        {
            if (_climbshift)
            {//... then switch from first part to second
                _climbshift = false;
                _timers.Reset(ETimeCounters.Climb);

                transform.position = _ClimbTarget; //set position in right place
                _ClimbStart = transform.position; //start from this place
                _ClimbTarget = _FinalClimbTargetPoint.position; //finish in point where now set final transform (ClimbPoint or ClimbPointOnFly)
            }
            else
            {//... then finish climb
                SetCollisionEnabled(true);
                _fsm.OnClimbEnd();

                _Rigidbody2D.velocity = Vector2.zero;

                LogInfo(string.Format("{0}: OnClimbEnd: move {1}, move_control {2}", MethodInfo.GetCurrentMethod().Name, _move, _move_control), ELogLevel.INFO);
            }
        }
        else
        {
            //if time wasn't ended
            _Rigidbody2D.velocity = Vector2.zero;
            float part = _timers.GetPassedTime(ETimeCounters.Climb) / preiod; //how part of time passed
            transform.position = Vector3.Lerp(_ClimbStart, _ClimbTarget, part); //set position between start of state and end

            LogInfo(string.Format("{0}: _climbshift {1}, part {2}", MethodInfo.GetCurrentMethod().Name, _climbshift, part), ELogLevel.DEBUG);
        }
    }
    #endregion

    #region Updates
    //main update
    private void FixedUpdate()
    {
        float ticktime = Time.fixedDeltaTime;

        //define - character is on ground or not, sometimes we must not do it (after begin of jump)
        if (_timers.Update(ETimeCounters.GroundCheck, _GroundCheckSkipTimeAfterJump, ticktime))
            _grounded = CheckGround(_groundprobe);

        if (_fsm.IsClimbState())
            UpdateClimb(ticktime); //character is climbing
        else if (_grounded)
            UpdateGround(ticktime); //or character is moving
        else
            UpdateFly(ticktime); //or character is flying

        //select right animation
        _fsm.Update(ticktime);
    }

    //void Update()
    //{
    //    //float ticktime = Time.smoothDeltaTime;
    //    if (_graph)
    //    {
    //        //var lst = new List<KeyValuePair<int, float>>();
    //        //lst.Add(new KeyValuePair<int, float>(0, _move));
    //        //lst.Add(new KeyValuePair<int, float>(1, _move_accell));
    //        //lst.Add(new KeyValuePair<int, float>(2, animspeed));
    //        //_graph.AddValue(_updatecount, lst);
    //        _updatecount++;
    //    }
    //}
    //int _updatecount = 0;

    //update if character is in the fly
    void UpdateFly(float inUpdateTime)
    {
        //bound vertical speed
        float fVertSpeed = Mathf.Min(_MaxYSpeed, Mathf.Abs(_Rigidbody2D.velocity.y)) * Mathf.Sign(_Rigidbody2D.velocity.y);

        _Rigidbody2D.velocity = new Vector2(_Rigidbody2D.velocity.x, fVertSpeed);

        if (_AutoClimbOnFly) //if character must cling and climb himself
            TryClimbOnFly(); //try to do it
        else
        {
            //waiting for when Player pushes obstacle enough time
            if (IsMovePushPresent())
            {
                if (_timers.Update(ETimeCounters.PushObs, _FlyToClimbDelay, inUpdateTime))
                    TryClimbOnFly();
            }
            else
                _timers.Reset(ETimeCounters.PushObs);
        }
    }

    //update if character is on the ground
    void UpdateGround(float inUpdateTime)
    {
        if (_fsm.IsCloseControlState()) 
            _move = EMoveState.Stand; //if character closed for control
        else 
            _move = _move_control; //set outer state

        bool pushing_obst = IsPushingFrontObstacle(_move);
        if (pushing_obst)
        {//if the player makes the character run into an obstacle
            _move = EMoveState.Stand; //character will not do it

            //if the player do it enough time the character will be try to climb
            float climbperiod = _fsm.CurrentState == EAnimState.Run ? _FlyToClimbDelay : _StayToClimbDelay;
            if(_timers.Update(ETimeCounters.PushObs, climbperiod, inUpdateTime))
                TryClimbFromGround();
            //LogInfo(string.Format("TimeCounters PushObs: {0}", _timers.GetPassedTime(ETimeCounters.PushObs)));
        }
        else
            _timers.Reset(ETimeCounters.PushObs);

        // Move the character
        float dir = _move == EMoveState.Stand ? 0 : _move == EMoveState.Right ? 1 : -1;
        float fNeedHorSpeed = dir * (_walk_on ? _WalkSpeed : _RunSpeed);
        float currspeed = _velocity.Update(inUpdateTime, fNeedHorSpeed); //changing velocity of character with acceleration

        if (currspeed > 0 && _fsm.CurrentState == EAnimState.Idle) //checking brake in console
            LogInfo(string.Format("_move {0}, currspeed {1}", _move, currspeed), ELogLevel.DEBUG);

        //bound vertical speed
        float fVertSpeed = Mathf.Min(_MaxYSpeed, Mathf.Abs(_Rigidbody2D.velocity.y)) * Mathf.Sign(_Rigidbody2D.velocity.y);
        _Rigidbody2D.velocity = new Vector2(currspeed, fVertSpeed);

        //rotate character in accordance with the movement
        if (_move != EMoveState.Stand && _move == EMoveState.Right != _FacingRight)
            Flip();
    }

    public void Flip()
    {
        LogInfo(string.Format("{0}: _move {1}", MethodInfo.GetCurrentMethod().Name, _move), ELogLevel.DEBUG);

        // Switch the way the player is labelled as facing.
        _FacingRight = !_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    #endregion

    #region IAnimFSMOwner
    public float GetSwitchAnimationPeriod() { return _SwitchAnimPeriod; }

    public void LogInfo(string inText, ELogLevel inLevel) { _log.Log(inText, inLevel); }
    public void LogError(string inText) { _log.LogError(inText); }

    public EMoveState GetCurrentHState() { return _move; }
    public bool IsCurrentHStateStand() { return _move == EMoveState.Stand; }
    public float GetCurrentVSpeed() { return _Rigidbody2D.velocity.y / _MaxYSpeed; }

    public float GetLandingSpeed() { return _LandingSpeed; }

    public bool IsGround() { return _grounded; }

    //animation fsm set new state for unity animator
    public void OnChangeState(EAnimState oldState, EAnimState NewState)
    {
        //Debug.Log(string.Format("Set Animator Trigger {0}", NewState));
        _anim.SetTrigger(NewState.ToString());
    }
    #endregion
}

