using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Actor : MonoBehaviour
{
    private Collider2D collider;
    Animator anim;
    public float HP = 100f;
    public float MaxHP = 100f;
    bool isdead;
    void Start()
    {

        anim = this.gameObject.GetComponent<Animator>();
        anim.SetBool("walk", false);//Walking animation is deactivated
        anim.SetBool("dead", false);//Dying animation is deactivated
        anim.SetBool("jump", false);//Jumping animation is deactivated
    }

    void Update()
    {
        
    }
    #region 受伤 死亡
    public void Hurt(int value)
    {
        HP -= value;
        if (HP <= 0)
        {
            DoDie();
        }
    }
    void DoDie()
    {
        anim.SetBool("dead", true);
        isdead = true;
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        Invoke("DoSmall", 2);
    }
    void DoSmall()
    {
        transform.DOScale(0.01f, 1).OnComplete(Dead);// => Dead;
    }
    void Dead()
    {
        if (transform.gameObject.activeSelf)
            transform.gameObject.SetActive(false);
    }
    #endregion
}
