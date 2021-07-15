using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System;
using UnityEngine.Events;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;



public class img_scale: MonoBehaviour
{
    RectTransform rt;
    public float scale = 4.0f;
    //
    // public string msg;
    float up = 5.0f;
    float low = 3.0f;
    bool add = true;
    bool sub = false;
    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {

        if(add)
            scale += 0.01f;
        if(sub)
            scale -= 0.01f;

        rt.localScale = new Vector3(scale, scale, scale);
        //Debug.Log("Scale="+scale);
        if(scale >= 5.1)
        {
            add = false;
            sub = true;
        }
        if (scale <= 3.0)
        {
            sub = false;
            add = true;
        }
    }
  



}
