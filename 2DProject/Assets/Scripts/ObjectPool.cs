using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private static ObjectPool minstance;
    public static ObjectPool Instance
    {
        get
        {
            if (minstance == null)
            {
                return new ObjectPool();
            }
            return minstance;
        }
    }
    private void Awake()
    {
        minstance = this;
    }
    public GameObject objectToPool;
    public Dictionary<string, GameObject> objectPools = new Dictionary<string, GameObject>();

    public List<GameObject> thePool = new List<GameObject>();
    public int startCount = 30;
    public int continueCount = 5;

    void Start()
    {
        for (int i = 0; i < startCount; i++)
        {
            thePool.Add(Instantiate(objectToPool));
            thePool[i].SetActive(false);
            thePool[i].transform.parent = transform;
        }
    }
    public GameObject SpawnObject(Vector3 position,Quaternion rotation) 
    {
        GameObject ToReturn = null;
        if (thePool.Count>0)
        {
            ToReturn = thePool[0];
            thePool.RemoveAt(0);
        }
        else
        {
            if (AddToPool())
            {
                ToReturn = thePool[0];
                thePool.RemoveAt(0);
            }
        }
        ToReturn.SetActive(true);
        ToReturn.transform.position = position;
        ToReturn.transform.rotation = rotation;
        return ToReturn;
    }
    bool AddToPool() 
    {
        for (int i = 0; i < continueCount; i++)
        {
            GameObject Return = Instantiate(objectToPool);
            thePool.Add(Return);
            Return.SetActive(false);
            Return.transform.parent = transform;
        }
        if (thePool.Count>0)
            return true;
        else
            return false;
    }
    public void RevertObject(GameObject obj) 
    {
        if (obj!=null)
        {
            obj.SetActive(false);
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;
            thePool.Add(obj);
        }
    }
    void Update()
    {
        
    }
}
