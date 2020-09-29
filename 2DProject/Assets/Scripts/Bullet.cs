using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public bool isRight;
    private float lifeTime = 5f;
    public int damageValue = 10;
    public ObjectPool bulletPool;
    Player gameplayer;
    Vector3 dir = Vector3.zero;
    //void Start()
    //{
    //    gameplayer = GameManager.Instance.gamePlayer;
    //    bulletPool = gameplayer.bulletPool;
    //    if (gameplayer!=null)
    //    {
    //        if (gameplayer.faceright)
    //            dir = Vector3.right;
    //        else
    //            dir = Vector3.left;
    //    }
    //}
    private void OnEnable()
    {
        gameplayer = GameManager.Instance.gamePlayer;
        bulletPool = ObjectPool.Instance;
        if (gameplayer != null)
        {
            if (gameplayer.faceright)
                dir = Vector3.right;
            else
                dir = Vector3.left;
        }
    }
    float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer>lifeTime)
        {
            Des(this.gameObject);
        }
        //if (gameplayer.transform.localScale.x == -1)
        //{
        //    transform.Translate(Vector3.left * speed * Time.deltaTime);
        //}
        //else if (gameplayer.transform.localScale.x == 1)
        //{
        //    transform.Translate(Vector3.right * speed * Time.deltaTime);
        //}
        transform.Translate(dir * speed * Time.deltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("canReduceHP"))
        {
            Actor actor = collision.gameObject.GetComponent<Actor>();
            if (actor != null)
                actor.Hurt(damageValue);
        }
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        EffectManager.Instance.ShowEffect("Bomb", transform.position);
        Des(gameObject);
    }
    public virtual void Des(GameObject obj) 
    {
        bulletPool.RevertObject(obj);
        //Destroy(obj);
    }
}
