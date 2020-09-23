using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject objectToPool;

    public List<GameObject> thePool = new List<GameObject>();
    public int startCount;

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
        GameObject ToReturn;
        if (thePool.Count>0)
        {
            ToReturn = thePool[0];
            thePool.RemoveAt(0);
        }
        else
        {
            ToReturn = Instantiate(objectToPool);
            thePool.Add(ToReturn);
            ToReturn.SetActive(false);
            ToReturn.transform.parent = transform;
        }
        ToReturn.SetActive(true);
        ToReturn.transform.position = position;
        ToReturn.transform.rotation = rotation;

        return ToReturn;
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
