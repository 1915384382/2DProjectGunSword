using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rig;
    [Header("移动")]
    public float moveSpeed;
    public float VelocityX = 0;
    public float smothAddTime = 0.09f;
    public float smothMinusTime = 0.09f;
    public Vector2 InputOffset;
    bool canMove = true;
    [Header("跳跃")]
    public float JumpingSpeed;
    public float fallMultiplier;//下坠增加加速度   下落越来越快
    public float lowJumpMultiplier;//减少跳跃加速度  跳的越高速度越慢
    public bool hasDoubleJump;
    public bool IsJumping;
    bool canJump = true;
    [Header("触地检测")]
    public LayerMask groundLayer;
    public Vector2 pointOffSet;
    public Vector2 size;
    public bool isGrounded;
    public bool isInAir;
    [Header("冲刺")]
    public float DashCD;
    public float DashForce;
    public float FlyDashForce;
    public float DashTime;
    public bool WasDashed;
    bool hasDashed;
    public bool canDash = true;
    float dashCdCounter;
    Vector2 dir;
    [Header("蹲下")]
    public bool isCrouch;
    [Header("动画")]
    public bool faceright; //face side of sprite activated
    Animator anim;
    [Header("攻击")]
    public bool startAttack;
    public bool HasSword;
    public float SkillTime;
    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        faceright = true;
        anim = this.gameObject.GetComponent<Animator>();
    }
    void ResetDashCD() 
    {
        canDash = false;
        dashCdCounter = DashCD;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && IsJumping && isInAir && !hasDoubleJump && canJump)
        {
            rig.velocity = new Vector2(rig.velocity.x, JumpingSpeed);
            hasDoubleJump = true;
        }
        if (dashCdCounter>0)
        {
            dashCdCounter -= Time.deltaTime;
            if (dashCdCounter<=0)
            {
                canDash = true;
            }
        }
    }
    void FixedUpdate()
    {
        isGrounded = OnGround();
        if (!isGrounded)
        {
            //animCtrl.ChangeAnimatorState(State.Jump);
        }
        Move();
        Jump();
        Dash();
    }
    void Dash()
    {
        //dir = new Vector2(Mathf.RoundToInt(Input.GetAxis("Horizontal")), Mathf.RoundToInt(Input.GetAxis("Vertical")));//获取玩家当前手柄摇杆方向
        dir = new Vector2(Input.GetAxisRaw("Horizontal"), 0);// Input.GetAxisRaw("Vertical"));
        if (dir == Vector2.zero)
        {
            if (faceright)
                dir.x = 1;
            else
                dir.x = -1;
        }
        if (Input.GetAxisRaw("Jump") > 0 && !WasDashed && canDash && !hasDashed)
        {
            ResetDashCD();
            StartCoroutine(DoDash());
            WasDashed = true;
            hasDashed = true;
        }
        if (isGrounded)
        {
            hasDashed = false;
        }
        //if (isGrounded && Input.GetAxisRaw("Fire2") == 0)
        //{
        //    WasDashed = false;
        //}
    }
    IEnumerator DoDash() 
    {
        //关闭移动和跳跃
        canMove = false;
        canJump = false;
        //关闭 重力
        rig.velocity = Vector2.zero;
        //关闭重力系数
        rig.gravityScale = 0;
        //加一个力
        if (dir.y>0)
            rig.velocity = dir * FlyDashForce;
        else
            rig.velocity = dir * DashForce;
        //等待时间
        yield return new WaitForSeconds(DashTime);
        //恢复系数
        canMove = true;
        canJump = true;
        WasDashed = false;
        rig.gravityScale = 1;
    }
    public void Skill() 
    {
        StartCoroutine(DoSkill());
    }
    IEnumerator DoSkill()
    {//关闭移动和跳跃
        canMove = false;
        canJump = false;
        //关闭 重力
        rig.velocity = Vector2.zero;
        //关闭重力系数
        rig.gravityScale = 0;
        //等待时间
        yield return new WaitForSeconds(SkillTime);
        //恢复系数
        canMove = true;
        canJump = true;
        WasDashed = false;
        rig.gravityScale = 1;
    }
    #region 触地检测
    /// <summary>
    /// 触地检测
    /// </summary>
    /// <returns></returns>
    bool OnGround() 
    {
        Collider2D colliders = Physics2D.OverlapBox((Vector2)transform.position + pointOffSet, size, 0, groundLayer);
        if (colliders != null)
        {
            isInAir = false;   
            return true;
        }
        else
        {
            isInAir = true;
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube((Vector2)transform.position + pointOffSet, size);
    }
    #endregion
    #region 移动
    void Move() 
    {
        if (!canMove)
            return;
        if (Input.GetAxisRaw("Horizontal") > InputOffset.x)
        {
            if (!isCrouch)
            {
                //if (!startAttack)
                //    anim.SetBool("walk", true);//Walking animation is activated

                rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, moveSpeed * Time.fixedDeltaTime * 60, ref VelocityX, smothAddTime), rig.velocity.y);
                if (startAttack && isGrounded)
                {
                    rig.velocity = new Vector2(0, rig.velocity.y);
                }
            }
            if (faceright == false && !startAttack)
            {
                Flip();
            }
        }
        else if (Input.GetAxisRaw("Horizontal") < InputOffset.x * -1)
        {
            if (!isCrouch)
            {
                //if (!startAttack)
                //    anim.SetBool("walk", true);//Walking animation is activated
                rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, moveSpeed * Time.fixedDeltaTime * 60 * -1, ref VelocityX, smothMinusTime), rig.velocity.y);
                if (startAttack && isGrounded)
                {
                    rig.velocity = new Vector2(0, rig.velocity.y);
                }
            }
            if (faceright == true && !startAttack)
            {
                Flip();
            }
        }
        else
        {
            //anim.SetBool("walk", false);
            rig.velocity = new Vector2(Mathf.SmoothDamp(rig.velocity.x, 0, ref VelocityX, smothMinusTime), rig.velocity.y);
        }
    }
    void Flip() 
    {
        //if (isAttack)
        //    return;
        faceright = !faceright;
        //transform.Rotate(0,180,0);
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    #endregion
    #region 跳跃
    /// <summary>
    /// 跳跃
    /// </summary>
    void Jump() 
    {
        if (!canJump)
            return;
        if ((startAttack && !IsJumping))
            return;
        //if (Input.GetAxis("Jump") == 1 && RestJumpTime > 0 && IsJumping && !isGrounded)
        //{
        //    anim.SetBool("jump", true);
        //    rig.velocity = new Vector2(rig.velocity.x, JumpingSpeed);
        //    IsJumping = true;
        //    RestJumpTime--;
        //}
        if (Input.GetAxis("Fire2") == 1&& !hasDoubleJump)
        {
            if (!IsJumping)
            {
                //anim.SetBool("jump", true);
                rig.velocity = new Vector2(rig.velocity.x, JumpingSpeed);
                IsJumping = true;

            }
            //if (IsJumping && isInAir && rig.velocity.y<=0)
            //{
            //    hasDoubleJump = true;
            //    rig.velocity = new Vector2(rig.velocity.x, JumpingSpeed);
            //}
        }
        if (isGrounded)
        {
            IsJumping = false;
            //anim.SetBool("jump", false);
            hasDoubleJump = false;
        }
        if (rig.velocity.y < 0)//下落的时候
        {
            rig.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;//加速下坠
        }
        else if (rig.velocity.y > 0 && Input.GetAxis("Jump") != 1)//当玩家上升的时候而且  玩家没有按下跳跃键的时候
        {
            rig.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime;//减缓上升
        }
    }
    #endregion
}
