using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.PlayerLoop;

public class Driver : MonoBehaviour
{
    Vector3 pos;
    Vector3 forward;
    Vector3 left;
    Vector3 right;
    public float ForwardDistance = 0;
    public float LeftDistance = 0;
    public float RightDistance = 0;

    private void Update()
    {
        forward = transform.TransformDirection(Vector3.forward)*30;
        left = transform.TransformDirection(new Vector3(.5f, 0, 1))*30;
        right = transform.TransformDirection(new Vector3(-.5f, 0, 1))*30;
        
        pos = new Vector3(transform.position.x,transform.position.y+2,transform.position.z);
        
        Debug.DrawRay(pos,forward,Color.red);
        Debug.DrawRay(pos,left,Color.red);
        Debug.DrawRay(pos,right,Color.red);
        
        
        gameObject.transform.rotation = Quaternion.LookRotation(transform.forward);
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;
        
        Ray RF = new Ray(pos,forward);
        Ray RL = new Ray(pos,left);
        Ray RR = new Ray(pos,right);

        ForwardDistance = 1;
        LeftDistance = 1;
        RightDistance = 1;

        if (Physics.Raycast(pos, forward, out hit, 30))
        {
            ForwardDistance = hit.distance/30;
        }
        if (Physics.Raycast(pos, left, out hit2, 30))
        {
            RightDistance = hit2.distance/30;
        }
        if (Physics.Raycast(pos, right, out hit3, 30))
        {
            LeftDistance = hit3.distance/30;
        }

    }
    
}
