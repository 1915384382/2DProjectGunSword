using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : MonoBehaviour
{
    public int HP;

    Animator anim;
    private CircleCollider2D collider;
    private Rigidbody2D rb;
    public Transform target;
    bool isdead;
    // Start is called before the first frame update
    void Start()
    {
        isdead = false;
        collider = this.gameObject.GetComponent<CircleCollider2D>();
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = this.gameObject.GetComponent<Animator>();
        anim.SetBool("walk", false);//Walking animation is deactivated
        anim.SetBool("dead", false);//Dying animation is deactivated
        anim.SetBool("jump", false);//Jumping animation is deactivated
    }

    // Update is called once per frame
    void Update()
    {
        if (!isdead)
        {
            if (target.position.x < transform.position.x)
            {
                Vector3 theScale = transform.localScale;
                theScale.x = -1;
                transform.localScale = theScale;
            }
            else
            {
                Vector3 theScale = transform.localScale;
                theScale.x = 1;
                transform.localScale = theScale;
            }
            //if (Input.GetKey("l"))
            //{
            //    DoDie();
            //}
        }
    }
    //void DoDie() 
    //{
    //    anim.SetBool("dead", true);
    //    isdead = true;
    //    if (collider != null)
    //        collider.enabled = false;
    //    rb.velocity = Vector2.zero;
    //    rb.gravityScale = 0;
    //    Invoke("DoSmall", 2);
    //}
    //void DoSmall() 
    //{
    //    transform.DOScale(0.01f, 1).OnComplete(Dead);// => Dead;
    //}
    //void Dead() 
    //{
    //    if (transform.gameObject.activeSelf)
    //        transform.gameObject.SetActive(false);
    //}
    //public void Hurt(int value) 
    //{
    //    HP -= value;
    //    if (HP<=0)
    //    {
    //        DoDie();
    //    }
    //}
}
