﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    // playerの速さ
    public float speed = 5.0f;
    public float turnSpeed = 1f;

    private Rigidbody rb;

    //public static PlayerMove instancePM;

    private bool _playerMove = true;
    public bool CanPlayerMove { get { return _playerMove; } set { _playerMove = value; } }


    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        move(speed);
    }

    private void move(float speed)
    {
        if (!_playerMove) {
            //rb.velocity = Vector3.zero;
            return;
        }
        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 move = new Vector3(horizontal, 0, vertical).normalized;
        

        rb.velocity = move * speed;
        //Debug.Log("プレイヤーのスピードは"+speed+"です");
        if (move!= Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(move, Vector3.up), turnSpeed);
    }
}