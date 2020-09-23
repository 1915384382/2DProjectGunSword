using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBullet : Bullet
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }
        if (collision.gameObject.CompareTag("enemy"))
        {
            Actor actor = collision.gameObject.GetComponent<Actor>();
            if (actor != null)
                actor.Hurt(damageValue);
        }
        Object cubePreb = Resources.Load("Effect/Bomb", typeof(GameObject));
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        GameObject cube = Instantiate(cubePreb) as GameObject;
        //GameObject bomb = Resources.Load("Effect/Bomb") as GameObject;
        cube.transform.position = transform.position;
        //EffectManager.Instance.ShowEffect("Bomb", transform.position);
    }

}
