using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infomation {
    public Vector3 position;
    public Quaternion rotation;
    public Infomation(Vector3 _pos, Quaternion _rot) {
        position = _pos;
        rotation = _rot;
    }
}
//public class TimeController : MonoBehaviour {
//    public bool isRewinding = false;//用来判断是否需要时光逆流
//    public float recordTime;//时光逆流时间

//    private List<Infomation> informations;
//    private Rigidbody2D rb;

//    // Use this for initialization
//    void Start()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        informations = new List<Infomation>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        //按下shift开始时光倒流
//        if (Input.GetKeyDown(KeyCode.LeftShift))
//        {
//            StartRewind();
//        }
//        //松开时停止
//        if (Input.GetKeyUp(KeyCode.LeftShift))
//        {
//            StopRewind();
//        }
//    }

//    void FixedUpdate()
//    {
//        if (isRewinding)
//            Rewind();
//        else
//            Record();

//    }

//    /// <summary>
//    /// 开始时光逆流
//    /// </summary>
//    private void StartRewind()
//    {
//        isRewinding = true;
//        rb.isKinematic = true;//使物体不受力
//    }

//    /// <summary>
//    /// 停止时光逆流
//    /// </summary>
//    private void StopRewind()
//    {
//        isRewinding = false;
//        rb.isKinematic = false;//物体开始受力
//    }

//    /// <summary>
//    /// 时光逆流
//    /// </summary>
//    private void Rewind()
//    {
//        //记录点数量大于0时才可以倒流
//        if (informations.Count > 0)
//        {
//            Infomation information = informations[0];
//            transform.position = information.position;
//            transform.rotation = information.rotation;
//            informations.RemoveAt(0);
//        }
//    }

//    /// <summary>
//    /// 记录物体的信息
//    /// </summary>
//    private void Record()
//    {
//        if (informations.Count > Mathf.Round(recordTime / Time.fixedDeltaTime))
//        {
//            informations.RemoveAt(informations.Count - 1);
//        }
//        informations.Insert(0, new Infomation(transform.position, transform.rotation));
//    }
//}




public class TimeKeyframe
{
    public Vector3 position;
    public Vector3 rotation;
    private int v1;
    private int v2;

    public TimeKeyframe(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
public class TimeController : MonoBehaviour
{
    //public ArrayList playerPosition;
    //public ArrayList playerRotation;
    public Transform player;
    public ArrayList keyframes;
    public int keyFrame = 1;  //表示FixedUpdate函數每被調用5次，就記錄數據；
    private int frameCounter = 0;

    public bool isReversing = false;


    private Camera camera;

    private int reverseCounter = 0;

    private Vector3 currentPosition;
    private Vector3 previousPosition;
    private Vector3 currentRotation;
    private Vector3 previousRotation;

    private bool firstRun = true;
    void Start()
    {
        //playerPosition = new ArrayList();
        //playerRotation = new ArrayList();
        keyframes = new ArrayList();
        camera = Camera.main;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Backspace))
        {
            isReversing = true;
        }
        else
        {
            isReversing = false;
            firstRun = true;
        }
    }
    void FixedUpdate()
    {
        if (!isReversing) //時間倒回未啓用則記錄數據
        {
            //if (frameCounter < keyFrame)
            //{
            //    frameCounter += 1;
            //}
            //else
            {
                frameCounter = 0;
                //playerPosition.Add(player.transform.position);
                //playerRotation.Add(player.transform.localEulerAngles);
                keyframes.Add(new TimeKeyframe(player.position, player.localEulerAngles));
            }

        }
        else  //時間倒囘開始
        {
            if (reverseCounter > 0)
            {
                reverseCounter -= 1;
            }
            else
            {
                //player.transform.position = (Vector3)playerPosition[playerPosition.Count - 1];
                //playerPosition.RemoveAt(playerPosition.Count - 1);

                //player.transform.localEulerAngles = (Vector3)playerRotation[playerRotation.Count - 1];
                //playerRotation.RemoveAt(playerRotation.Count - 1);
                RestorePositions();
                reverseCounter = keyFrame;
            }
            if (firstRun)
            {
                firstRun = false;
                RestorePositions();
            }
            float interPolation = (float)reverseCounter / (float)keyFrame;
            player.position = Vector3.Lerp(previousPosition, currentPosition, interPolation);
            player.localEulerAngles = Vector3.Lerp(previousRotation, currentRotation, interPolation);
        }
        if (keyframes.Count > 128) //只記錄游戲結束前128個數據
        {
            keyframes.RemoveAt(0);
        }
    }

    void RestorePositions()
    {
        int lastIndex = keyframes.Count - 1;
        int secondToLastIndex = keyframes.Count - 2;
        if (secondToLastIndex >= 0)
        {
            currentPosition = (keyframes[lastIndex] as TimeKeyframe).position;
            previousPosition = (keyframes[secondToLastIndex] as TimeKeyframe).position;

            currentRotation = (keyframes[lastIndex] as TimeKeyframe).rotation;
            previousRotation = (keyframes[secondToLastIndex] as TimeKeyframe).rotation;
            keyframes.RemoveAt(lastIndex);
        }
    }
}

