using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// 就是利用Stack的原理来获取历史位置
/// 如果同时有动画，把动画倒放就是
public class TBPlayer : MonoBehaviour
{
    public int Speed = 3;



    public int RotateSpeed = 100;



    public bool ShouldLimit = false;



    public int Limit = 100;
    //可以存放的坐标上限

    private
    List<Vector3> HistoryPos;



    private
    List<Vector3> HistoryRot;



    private
    bool
     _IsTimeBack =
    false
    ;






    void Start()
    {
        HistoryPos =
        new
        List<Vector3>();



        HistoryRot =
        new
        List<Vector3>();



    }
    void Update()
    {



        if (_IsTimeBack)
            TimeBack();
        else
            ControlPos();
    }






    void ControlPos()
    {
        //Pos
        Vector3 pos = this.transform.position;
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (Mathf.Abs(horizontal) > 0.0001f)
        //左右移动
        {
            pos.x += Time.deltaTime * horizontal * Speed;
        }
        if (Mathf.Abs(vertical) > 0.0001f)
        //上下移动
        {
            pos.y += Time.deltaTime * vertical * Speed;
        }
        this.transform.position = pos;
        HistoryPos.Add(pos);
        //Rotation
        Quaternion rot = this.transform.rotation;
        Vector3 rotv = rot.eulerAngles;

        float rotate = Input.GetAxis("Fire1");
        if (Mathf.Abs(rotate) > 0.0001f)
        {
            rotv.z += Time.deltaTime * rotate * RotateSpeed;
        }
        rot = Quaternion.Euler(rotv);
        this.transform.rotation = rot;
        HistoryRot.Add(rotv);
        if (ShouldLimit && HistoryPos.Count > Limit)
        {
            HistoryPos.RemoveAt(0);
            HistoryRot.RemoveAt(0);
        }
    }






    void TimeBack()
    {
        if (HistoryPos.Count > 0)
        {
            int index = HistoryPos.Count - 1;
            this.transform.position = HistoryPos[index];
            HistoryPos.RemoveAt(index);
        }
        if (HistoryRot.Count > 0)
        {
            int index = HistoryRot.Count - 1;
            this.transform.eulerAngles = HistoryRot[index];
            HistoryRot.RemoveAt(index);
        }
    }
    void OnGUI()
    {
        if (GUILayout.Button("时间倒流"))
        {
            _IsTimeBack = true;
        }



        if
        (GUILayout.Button(
        "Reset"
        ))



        {



            HistoryRot.Clear();



            HistoryPos.Clear();



            _IsTimeBack =
            false
            ;



        }



    }
}


 