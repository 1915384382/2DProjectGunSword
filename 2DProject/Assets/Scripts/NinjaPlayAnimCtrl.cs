using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State 
{
    Idle,
    Run,
    Jump,
    Crouch,
    CrouchDash,

    SwordIdle,
    SwordRun,
    SwordJump,
    SwordCrouch,
    SwordCrouchDash,
    SwordAttack01,
    SwordRedAttack,
    SwordIdleHurt,
    SwordSkill,
    SwordStrike,
    SwordJumpSkill,
    SwordJumpAttack01,

    HoldSwordIdle,
    HoldSwordHurt,
}
public class NinjaPlayAnimCtrl : MonoBehaviour
{
    public const float AttackAniDefaultTime = 0.5f;
    public const float SkillAniDefaultTime = 0.6f;
    Animator anim;
    PlayerController controller;
    State state;
    string currAttackState = "";
    bool SwordState = false;
    bool canUseSkill = true;
    float skillCD = 3f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        state = State.Idle;
        anim.Play("Idle");
    }
    bool CanAttack { get { return SwordState && !controller.startAttack && !controller.isCrouch && !controller.WasDashed; } }
    bool CanSkill { get { return SwordState && !controller.startAttack && !controller.isCrouch && !controller.WasDashed && canUseSkill; } }
    bool CanMove { get { return controller.isGrounded && !controller.isCrouch && !controller.startAttack; } }
    bool CanCrouch { get { return !controller.startAttack && !controller.IsJumping; } }
    bool CanChangeState { get { return !controller.startAttack; } }
    void Update()
    {
        if (skillCD > 0)
        {
            skillCD -= Time.deltaTime;
            if (skillCD<=0)
                canUseSkill = true;
        }

        //攻击
        if (CanAttack)
        {
            if (Input.GetMouseButton(0))
            {
                if (controller.isGrounded)
                {
                    StartAttack(State.SwordAttack01);
                }
                else
                {
                    StartAttack(State.SwordJumpAttack01);
                }
            }
            else if (Input.GetMouseButton(2))
            {
                if (controller.isGrounded)
                    StartAttack(State.SwordRedAttack);
            }
        }
        //技能
        if (CanSkill)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                canUseSkill = false;
                skillCD = 3f;
                controller.Skill();
                if (controller.isGrounded)
                {
                    StartAttack(State.SwordSkill);
                }
                else
                {
                    StartAttack(State.SwordJumpSkill);
                }
            }
        }
        //移动
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && CanMove)
        {
            if (!SwordState)
                ChangeAnimatorState(State.Run);
            else
                ChangeAnimatorState(State.SwordRun);
        }

        //跳跃
        if (controller.IsJumping)
        {
            if (!controller.startAttack)
            {
                if (!SwordState)
                    ChangeAnimatorState(State.Jump);
                else
                    ChangeAnimatorState(State.SwordJump);
            }
        }
        if (controller.isGrounded)
        {
            if (IsAnimatorState(State.SwordJumpAttack01))
            {
                currAttackState = "";
                controller.startAttack = false; 
                SetIdleAnimatorState();
            }
        }

        //蹲伏
        if (Input.GetKey( KeyCode.LeftControl) && CanCrouch)
        {
            controller.isCrouch = true;
        }
        else
        {
            controller.isCrouch = false;
        }
        //蹲伏冲刺
        if (controller.isCrouch)
        {
            if (controller.WasDashed)
            {
                if (!SwordState)
                    ChangeAnimatorState(State.CrouchDash);
                else
                    ChangeAnimatorState(State.SwordCrouchDash);
            }
            else
            {
                if (!SwordState)
                    ChangeAnimatorState(State.Crouch);
                else
                    ChangeAnimatorState(State.SwordCrouch);
            }
        }

        //不蹲伏  不跳  不攻击  在地上 不冲刺  
        if (!controller.isCrouch && !controller.IsJumping && !controller.startAttack && controller.isGrounded && !controller.WasDashed&& !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            SetIdleAnimatorState();
        }
        //切换状态  拿剑 不拿剑
        if (Input.GetKeyDown( KeyCode.C) && CanChangeState)
        {
            SwordState = !SwordState;
            SetIdleAnimatorState();
        }
        //当前攻击状态
        if (string.IsNullOrEmpty(currAttackState))
        {
            controller.startAttack = false;
        }

    }
    public void StartAttack(State state)
    {
        ChangeAnimatorState(state);
        controller.startAttack = true;
        currAttackState = state.ToString();
    }
    public void AttackAnimTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll((Vector2)transform.position + pointOffSet, size, 0, enemyLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            Actor enemy = colliders[i].GetComponent<Actor>();
            if (enemy!=null)
            {
                enemy.Hurt(10);
            }
        }
    }
    public void AttackAnimEnd()
    {
        currAttackState = "";
        if (IsAnimatorState( State.SwordSkill) || IsAnimatorState(State.SwordJumpSkill))
        {

        }
        SetIdleAnimatorState();
    }
    void SetIdleAnimatorState() 
    {
        if (!SwordState)
        {

            ChangeAnimatorState(State.Idle);
        }
        else
        {

            ChangeAnimatorState(State.SwordIdle);
        }
    }
    public void ChangeAnimatorState(State _state) 
    {
        if (_state == state)
        {
            return;
        }
        else
        {
            state = _state;
            anim.Play(state.ToString());
        }
    }
    public bool IsAnimatorState(State _state) 
    {
        if (state == _state)
        {
            return true;
        }
        return false;
    }

    public Vector2 pointOffSet;
    public LayerMask enemyLayer;
    public Vector2 size;
    bool OnGround()
    {
        Collider2D colliders = Physics2D.OverlapBox((Vector2)transform.position + pointOffSet, size, 0, enemyLayer);
        if (colliders != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube((Vector2)transform.position + pointOffSet, size);
    }
}
