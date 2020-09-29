using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager minstance;
    public static EffectManager Instance
    {
        get
        {
            if (minstance == null)
            {
                return new EffectManager();
            }
            return minstance;
        }
    }
    private void Awake()
    {
        minstance = this;
    }
    public void ShowEffect(string effectName,Vector3 _position) 
    {
        Object cubePreb = Resources.Load("Effect/Bomb", typeof(GameObject));
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        GameObject cube = Instantiate(cubePreb) as GameObject;
        cube.transform.position = _position;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
