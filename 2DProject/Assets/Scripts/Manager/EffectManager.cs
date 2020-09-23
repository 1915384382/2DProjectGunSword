using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    private void Awake()
    {
        Instance = this;
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
