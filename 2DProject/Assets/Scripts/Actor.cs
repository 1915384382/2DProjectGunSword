using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Actor : MonoBehaviour
{
    public float HP = 100f;
    public float MaxHP = 100f;
    bool isdead;
    void Start()
    {
    }

    void Update()
    {
        
    }
    #region 受伤 死亡
    public void Hurt(int value)
    {
        GameManager.Instance.mainCamara.DOShakePosition(GameManager.Instance.duration, GameManager.Instance.strength, GameManager.Instance.vibrate, GameManager.Instance.randomnes);
        HP -= value;
        if (HP <= 0)
        {
            DoDie();
        }
    }
    void DoDie()
    {
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
