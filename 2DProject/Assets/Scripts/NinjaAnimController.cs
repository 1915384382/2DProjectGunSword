using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    Run,
    Crouch,
    CrouchDash,
    SwordIdle,
    Jump,
    HoldSword,
    HoldSwordHurt,
    SwordSkill,
    SwordCrouch,
    SwordCrouchDash,
    SwordAttack,
    SwordRedAttack,
    SwordRun,
    SwordJump,
    SwordJumpSkill,
    SwordJumpAttack,
    SwordStrike,
}
public class NinjaAnimController : MonoBehaviour
{
    Animator anim;
    PlayerController controller;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
    }
    bool hasChangeState = false;
    void Update()
    {
        bool canChangeState = false;
        //跑步
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && controller.isGrounded)
        {
            canChangeState = false;
            if (!controller.startAttack)
            {
                if (hasChangeState)
                {
                    anim.SetBool("SwordRun", true);
                }
                else
                    anim.SetBool("Run", true);
            }
        }
        else
        {
            canChangeState = true;
            if (!controller.startAttack)
            {
                if (hasChangeState)
                    anim.SetBool("SwordRun", false);
                else
                    anim.SetBool("Run", false);
            }
        }
        //跳跃
        if (controller.IsJumping)
        {
            canChangeState = false;
            if (hasChangeState)
                anim.SetBool("SwordJump", true);
            else
            anim.SetBool("Jump", true);
        }
        else
        {
            canChangeState = true;
            if (hasChangeState)
                anim.SetBool("SwordJump", false);
            else
                anim.SetBool("Jump", false);
            if (Input.GetKey(KeyCode.LeftControl))
            {
                canChangeState = false;
                if (hasChangeState)
                    anim.SetBool("SwordCrouch", true);
                else
                    anim.SetBool("Crouch", true);
                controller.isCrouch = true;
            }
        }
        //下蹲
        if (Input.GetKey( KeyCode.LeftControl) && controller.isGrounded)
        {
            canChangeState = false;
            if (hasChangeState)
                anim.SetBool("SwordCrouch", true);
            else
                anim.SetBool("Crouch", true);
            controller.isCrouch = true;
        }
        else
        {
            canChangeState = true;
            if (hasChangeState)
                anim.SetBool("SwordCrouch", false);
            else
                anim.SetBool("Crouch", false);
            controller.isCrouch = false;
        }
        if (controller.isCrouch && Input.GetMouseButtonDown(1) && controller.canDash)
        {
            if (hasChangeState)
                anim.SetBool("SwordCrouch", false);
            else
                anim.SetBool("Crouch", false);
            if (hasChangeState)
                anim.SetFloat("SwordCrouchDash", 1);
            else
                anim.SetFloat("CrouchDash", 1);
        }
        else if (!controller.WasDashed)
        {
            if (hasChangeState)
                anim.SetFloat("SwordCrouchDash", 0);
            else
                anim.SetFloat("CrouchDash", 0);
        }
        //转换姿势
        if (Input.GetKeyDown(KeyCode.C) && canChangeState && !anim.GetBool("SwordIdle"))
        {
            anim.SetBool("SwordIdle", true);
            hasChangeState = true;
        }
        else if (Input.GetKeyDown(KeyCode.C) && canChangeState && anim.GetBool("SwordIdle"))
        {
            anim.SetBool("SwordIdle", false);
            hasChangeState = false;
        }
        if (controller.isGrounded)
        {
            if (Input.GetMouseButton(0) && hasChangeState && !controller.WasDashed && !anim.GetBool("SwordCrouch") && !controller.startAttack && string.IsNullOrEmpty(currentState))
            {
                StartAttack();
            }
        }
        if (anim.GetBool(currentState) == false)
        {
            currentState = "";
        }
    }
    string currentState = "";
    public void StartAttack() 
    {
        anim.SetBool("SwordAttack", true);
        currentState = "SwordAttack";
        controller.startAttack = true;
    }
    public void AttackTrigger() 
    {

    }
    public void AttackEnd()
    {
        anim.SetBool("SwordAttack", false);
        controller.startAttack = false;
    }

}
