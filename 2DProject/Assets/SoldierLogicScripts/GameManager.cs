using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager minstance;
    public static GameManager Instance
    {
        get
        {
            if (minstance == null)
            {
                return new GameManager();
            }
            return minstance;
        }
    }
    public Camera mainCamara;
    private void Awake()
    {
        minstance = this;
    }
    public Player gamePlayer;
    public float duration;
    public float strength;
    public int vibrate;
    public float randomnes;


    private int instanceID = 100;
    public int GetInstance() 
    {
        return Instance.instanceID++;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
public class SkillPool : MonoBehaviour
{

    private static SkillPool minstance;
    public static SkillPool Instance
    {
        get
        {
            if (minstance == null)
            {
                return new SkillPool();
            }
            return minstance;
        }
    }
    public void GetSkill(string name,Transform target) 
    {
        Object cubePreb = Resources.Load("Skill/"+ name, typeof(GameObject));
        //用加载得到的资源对象，实例化游戏对象，实现游戏物体的动态加载
        GameObject cube = Instantiate(cubePreb) as GameObject;
        if (target!=null)
        {
            cube.transform.position = target.position;
            cube.transform.rotation = target.rotation;
            cube.transform.localScale = target.localScale;
        }
        else
        {
            cube.transform.position = Vector3.zero;
            cube.transform.rotation = Quaternion.identity;
            cube.transform.localScale = Vector3.one;
        }

    }
}
