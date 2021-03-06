﻿using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
public enum WeaponType
{
    Pistol,
    Sword,
}
//Example Script for motion (Walk, jump and dying), for dying press 'k'...
public class Player : MonoBehaviour
{
    [SerializeField] private LayerMask RayCastHit2DLayer;
    public float maxspeed; //walk speed
    public bool faceright; //face side of sprite activated
    public Transform Sword;
    public Transform shoot;
    public Transform GroundCheck;
    public Bullet bullet;
    public ObjectPool bulletPool;
    public float JumpHeight = 300;
    public bool isGrounded;
    public BoxCollider2D checkGround;

    public LayerMask playerLayerMask;

    public WeaponType weaponType;

    Animator anim;

    public float chongfengForce = 500;
    public float chongfengAmout = 5f;

    public CircleCollider2D circle;
    public float force;


    private Rigidbody2D rb2D;
    private bool jumping = false;
    private bool isdead = false;
    private bool isShifting = false;
    private bool canCut = true;
    private TrailRenderer trail;
    bool isChongFeng = false;
    int JumpTime = 2;
    int NowJumpTime = 2;
    Vector2 constVelocity = new Vector2(0, -9.8f);
    void Start()
    {
        bullet.GetComponent<Bullet>();
        faceright = true;
        anim = this.gameObject.GetComponent<Animator>();
        anim.SetBool("walk", false);//Walking animation is deactivated
        anim.SetBool("dead", false);//Dying animation is deactivated
        anim.SetBool("jump", false);//Jumping animation is deactivated
        anim.SetBool("cut", false);//Jumping animation is deactivated
        rb2D = GetComponent<Rigidbody2D>();
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if ((coll.gameObject.tag == "Obstacle" || coll.gameObject.tag == "Ground") && coll.gameObject.transform.position.y < transform.position.y && isGrounded && jumping)
        {
            jumping = false;
            NowJumpTime = JumpTime;
            anim.SetBool("jump", false);
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Ground") && collision.gameObject.transform.position.y < transform.position.y && isGrounded && jumping)
        {
            jumping = false;
            NowJumpTime = JumpTime;
            anim.SetBool("jump", false);
        }
    }
    bool quickAttack = false;
    void DoReleaseSkill()
    {
        string skillCode = "";
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            KeyCode.Alpha1.ToString();
            skillCode = "Alpha1";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            skillCode = "Alpha2";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            skillCode = "Alpha3";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            skillCode = "Alpha4";
        }
        SkillManager.Instance.UseSkill(0);
    }
    float gravity;
    float jumpgraqvity;
    float VelocityX = 0;
    float smothAddTime = 0.09f;
    float smothMinusTime = 0.09f;
    void FixedUpdate()
    {

        if (Input.GetAxis("Horizontal") > 0)
        {
            rb2D.velocity = new Vector2(Mathf.SmoothDamp(rb2D.velocity.x, maxspeed * Time.fixedDeltaTime * 60, ref VelocityX, smothAddTime), rb2D.velocity.y);
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            rb2D.velocity = new Vector2(Mathf.SmoothDamp(rb2D.velocity.x, maxspeed * Time.fixedDeltaTime * 60*-1, ref VelocityX, smothMinusTime), rb2D.velocity.y);
        }
        else if (Input.GetAxis("Horizontal") == 0)
        {
            rb2D.velocity = new Vector2(Mathf.SmoothDamp(rb2D.velocity.x, 0, ref VelocityX, smothMinusTime), rb2D.velocity.y);
        }
        //float XValue = Input.GetAxis("Horizontal");
        //rb2D.velocity = new Vector2(XValue * maxspeed * Time.deltaTime, rb2D.velocity.y);
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    rb2D.velocity = new Vector2(rb2D.velocity.x, JumpHeight * Time.deltaTime);
        //}




        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    skill = !skill;
        //}
        //if (isdead == false && !isChongFeng)
        //{
        //    if (Input.GetKey("b"))
        //    {
        //        anim.SetBool("dead", true);
        //        isdead = true;
        //    }
        //    ChongFeng();
        //    float move = 0;// Input.GetAxis("Horizontal");
        //    Move(ref move);
        //    move *= maxspeed;

        //    Jump(move, ref finalGravity);
        //    Attack();
        //    rb2D.velocity = new Vector2(move, finalGravity);

        //    if (Input.GetKeyDown(KeyCode.Alpha0))
        //    {
        //        quickAttack = !quickAttack;
        //    }
        //}
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    Time.timeScale *= -1;
        //}
    }
    void Move(ref float move)
    {
        if (Input.GetKey(KeyCode.A))
        {
            move = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            move = 1;
        }

        if (move > 0)//右边
        {
            anim.SetBool("walk", true);//Walking animation is activated
            if (faceright == false)
            {
                Flip();
            }
        }
        else if (move == 0)//停止
        {
            anim.SetBool("walk", false);
        }
        else if ((move < 0))//左边
        {
            anim.SetBool("walk", true);
            if (faceright == true)
            {
                Flip();
            }
        }
    }
    void Attack()
    {
        if (Input.GetKey(KeyCode.J))
        {
            switch (weaponType)
            {
                case WeaponType.Pistol:
                    Shoot();
                    break;
                case WeaponType.Sword:
                    Cut();
                    break;
                default:
                    break;
            }
        }
    }
    float jumpTimne = 0.5f;
    float jumpTimneCounter = 0;
    void Jump(float vector2X,ref float finalGragity)
    {
        if (Input.GetKeyDown("k"))
        {
            jumping = true;
            anim.SetBool("jump", true);
            if (isGrounded && !jumping)
            {
                finalGragity = jumpgraqvity;
                NowJumpTime--;

                jumpTimneCounter = jumpTimne;
            }
            else if (!isGrounded && NowJumpTime > 0)
            {
                finalGragity = jumpgraqvity;

                jumpTimneCounter = jumpTimne;
                NowJumpTime--;
            }
        }
        if (jumpTimneCounter > 0)
        {
            jumpTimneCounter -= Time.deltaTime;
            if (jumpTimneCounter<=0)
            {
                finalGragity = gravity;
            }
        }
    }
    float chongfengCD = 3;
    float chongfengCDTimer = 0;
    bool canUseChongFegn = true;
    void ChongFeng()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canUseChongFegn)
        {
            isChongFeng = true;
            trail.enabled = true;
            canUseChongFegn = false;
            chongfengCDTimer = 0;
        }
        chongfengCDTimer += Time.deltaTime;
        if (chongfengCDTimer >= chongfengCD)
        {
            canUseChongFegn = true;
        }
        if (isChongFeng)
        {
            Vector2 dir = Vector2.zero;
            if (faceright)
                dir = Vector2.right;
            else
                dir = Vector2.left;
            rb2D.AddRelativeForce(dir * force, ForceMode2D.Force);
            //rb2D.AddForce(dir * force, ForceMode2D.Impulse);

            //Vector3 chongfengPos = (Vector2)transform.position + dir * chongfengAmout;
            //RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, dir, chongfengAmout, RayCastHit2DLayer);
            //if (raycastHit2D.collider != null)
            //{
            //    Debug.Log(raycastHit2D.collider.name);
            //    Debug.Log(raycastHit2D.collider.name);
            //    chongfengPos = raycastHit2D.point;
            //}
            //rb2D.MovePosition(chongfengPos);

            isChongFeng = false;
            trail.enabled = false;
        }
    }
    void Flip()
    {
        if (isAttack)
            return;
        faceright = !faceright;
        //transform.Rotate(0,180,0);
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    bool isAttack = false;
    float shootCD = 0.1f;
    float shootCDTimer = 0f;
    bool canShoot = true;

    void Shoot()
    {
        if (canShoot)
        {
            bulletPool.SpawnObject(shoot.position, shoot.rotation);
            canShoot = false;
            shootCDTimer = 0f;
        }
        else
        {
            shootCDTimer += Time.deltaTime;
            if (shootCDTimer >= shootCD)
                canShoot = true;
        }
    }
    void Cut()
    {
        if (canCut)
        {
            if (quickAttack)
            {
                anim.speed = 2;
                anim.SetBool("cut", true);
            }
            else
            {
                anim.speed = 1;
                anim.SetBool("cut", true);
            }
            isAttack = true;
            canCut = false;
        }
    }
    void attackEnd()//绑定的动画事件
    {
        anim.SetBool("cut", false);
        canCut = true;
        isAttack = false;
    }
    void DoSwordHurt()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll((Vector2)circle.transform.position + circle.offset, circle.radius);
        for (int i = 0; i < targets.Length; i++)
        {
            Actor actor = targets[i].transform.GetComponent<Actor>();
            Enemy enemy = targets[i].transform.GetComponent<Enemy>();
            Player player = targets[i].transform.GetComponent<Player>();
            if (actor != null && enemy != null)
            {
                actor.Hurt(20);
            }
            if (actor != null)
            {
                if (player == null)
                    EffectManager.Instance.ShowEffect("Bomb", actor.transform.position);
            }
        }
    }
}
