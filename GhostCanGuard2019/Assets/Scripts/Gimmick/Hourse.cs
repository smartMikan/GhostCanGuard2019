﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hourse : MonoBehaviour
{
    [SerializeField]
    private bool IfEnable = false;      //使いできますか(UIエフェクト、光、説明文、移動メニュー)
    private bool IfActivated = false;   //使う中ですか
    private bool IfBacking = false;
    private Rigidbody HourseRB;
    private Renderer HourseRenderer;
    public static Hourse instanceHourse;
   
    //馬の速さ
    [SerializeField]
    private float HourseSpeed = 10.0f;
    //馬の移動時間
    public float HourseMoveTime = 3.0f;
    public float HourseBackTime = 2.0f;
    public float DispearAlpha =70;
    float floorheight = 0;

    private float radius = 0.5f;
    [SerializeField]
    private PlayerMove playerMove;
    [SerializeField]
    private GameObject Player;
    [SerializeField]
    private GameObject Saddle;                          //馬の鞍（くら）
    private Vector3 StartPosition = Vector3.zero;       //馬の初期位置

    void OnEnable()
    {
        StartPosition = transform.position;
        IfEnable = true;
        HourseRB = GetComponent<Rigidbody>();
        HourseRenderer = GetComponent<Renderer>();
    }


    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
    //    if (!IsHourseMove) return;
    // Move(HourseSpeed, HourseMoveTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IfActivated && IfEnable &&other.tag=="Player")
        {
            IfEnable = false;
            MoveOrient = Vector3.right.normalized;
            GetOnHourse(Player);
           // StartCoroutine(HourseMove(HourseMoveTime));
            IfActivated = true;
            //IsHourseMove = true;
            //StartCoroutine(HourseMove(other.gameObject, HourseSpeed, GetOrient()));
            
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        
        
    }


    Vector3 MoveOrient = Vector3.zero;
    Vector3 LeftOrient = Vector3.zero;
    
    //private Vector3 GetOrient()                                                  
    //{// 右
    //    if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
    //    {
    //        return Vector3.right.normalized;
    //    }
    //    // 左
    //    else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
    //    {
    //        return Vector3.left.normalized;
    //    }
    //    // 上
    //    else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
    //    {
    //        return Vector3.forward.normalized;
    //    }
    //    // 下
    //    else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
    //    {
    //        return Vector3.back.normalized;
    //    }
    //    else return MoveOrient.normalized;
        
    //}
   private bool GetKeyInput(Vector3 input)
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return true;
        else return false;
    }
    float Stime = 0;
    private void Move(float speed,Vector3 orient)
    {
        float movedistance = (speed / Mathf.Sqrt(2.0f) * Time.deltaTime);
        RaycastHit hit;
        if (!IfBacking)
        {
            if (Physics.Raycast(transform.position, orient, out hit, movedistance))
            {
                Debug.Log(hit.point);
                Debug.Log(hit.distance);
                Debug.Log(hit.collider);
                movedistance = Mathf.Clamp(movedistance, 0, hit.distance - radius > 0 ? hit.distance - radius : 0);
            }
        }
       
        Vector3 Direction = transform.position + orient.normalized * movedistance;
        Debug.Log(orient);
        transform.position = Vector3.MoveTowards(transform.position, Direction,movedistance);
    }
    private void MoveUpdate()
    {
        if (IfActivated)
        {
            Move(HourseSpeed,MoveOrient);
            Stime += Time.fixedDeltaTime;
            if (!IfBacking) 
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    IfBacking = true;
                    Debug.Log("Start Back");
                    LeftOrient = Vector3.right;
                    //MoveOrient = Vector3.zero;
                    GetOffHourse(Player, LeftOrient);
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    IfBacking = true;
                    Debug.Log("Start Back");
                    LeftOrient = Vector3.left;
                    //MoveOrient = Vector3.zero;
                    GetOffHourse(Player, LeftOrient);
                }
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    IfBacking = true;
                    Debug.Log("Start Back");
                    LeftOrient = Vector3.up;
                    //MoveOrient = Vector3.zero;
                    GetOffHourse(Player, LeftOrient);
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    IfBacking = true;
                    Debug.Log("Start Back");
                    LeftOrient = Vector3.down;
                    //MoveOrient = Vector3.zero;
                    GetOffHourse(Player, LeftOrient);
                }

            }
           
            else if (Stime > HourseMoveTime && !IfBacking)
            {
                IfBacking = true;
                LeftOrient = MoveOrient;
                //MoveOrient = Vector3.zero;
                GetOffHourse(Player, LeftOrient);
            }

            if (IfBacking)
            {
                Back();
            }

        }
        else return;
    }

    private void Back()
    {
        
        Debug.Log("Start Back");
        Stime += Time.fixedDeltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, StartPosition, HourseSpeed * Time.deltaTime);       //元の位置に戻る
        Move(HourseSpeed, MoveOrient);
       
        //if (transform.position == StartPosition)
        if(Stime> HourseBackTime+HourseMoveTime)
        {
            transform.position = StartPosition;
            IfBacking = false;
            Debug.Log("Backing End");
           
            IfActivated = false;

            IfEnable = true;
            Debug.Log("Reade to Reuse");
            Stime = 0;
        }
    }
    //IEnumerator HourseMove(float time)                    //IEnumerator を使う時間制御　　まだ未完成
    //{
    //    Stime += Time.fixedDeltaTime;
    //    HourseRB = GetComponent<Rigidbody>();
    //    HourseRB.velocity = MoveOrient * HourseSpeed;
    //    yield return new WaitForSeconds(time);
    //    HourseRB.velocity = Vector3.zero;

    //}
    private void GetOnHourse(GameObject player)
    {
        tag = "Player";
        playerMove.IsPlayerMove = false;
        player.transform.position = Saddle.transform.position;
        player.transform.SetParent(this.transform);
        player.tag = "Default Banned By Portal";
        Debug.Log("Saddle Set");
    } 
    private void GetOffHourse(GameObject player,Vector3 orient)
    {
        tag = "Gimmik";
        player.transform.position += orient * transform.localScale.magnitude;
        player.transform.parent = null;
        player.tag = "Player";
        player.transform.position = new Vector3(player.transform.position.x, floorheight, player.transform.position.z);
        playerMove.IsPlayerMove = true;
    }
    public void OnOff()      
    {
        IfEnable = !IfEnable;
    }


}