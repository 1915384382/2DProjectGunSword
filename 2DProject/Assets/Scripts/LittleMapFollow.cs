using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleMapFollow : MonoBehaviour
{
    public Transform tran;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tran!=null)
            transform.position = new Vector3(tran.position.x, tran.position.y, transform.position.z);
    }
}
