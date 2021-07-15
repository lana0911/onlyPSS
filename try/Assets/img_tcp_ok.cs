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



public class img_tcp_ok : MonoBehaviour
{
    RectTransform rt;
    public int dis = 0;
    public float scale = 4.0f;
    //
    public string IP = "192.168.56.1";
    public int Port = 8000;
    public Socket client;
    // public string msg;


    void Awake()
    {
        rt = GetComponent<RectTransform>();
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IP, Port);
        //client.Send(Encoding.UTF32.GetBytes("img"));
        Time.fixedDeltaTime = 0.5F;

    }

    void FixedUpdate()
    {


        if (client.Connected)
        {
            //recv
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            string msg = Encoding.UTF8.GetString(bytes, 0, count);
            //  Debug.Log("Gett Data:");
           // Debug.Log(msg);

            if (msg.Length < 10)
            {
                dis = (Convert.ToInt32(msg));
                if (dis > 42000) scale = 3.8f;
                else if(dis > 39000) scale = 4.2f;
                else if (dis < 39000 && dis >= 20000) scale =4.8f;
                else if (dis < 20000) scale = 5.5f;

                rt.localScale = new Vector3(scale, scale, scale);
            }

            /*  if (dis > 40000) scale = 5.5f;
             else if (dis < 40000 && dis >= 20000) scale = 8.5f;
             else if (dis < 20000) scale = 11.5f;

             rt.localScale = new Vector3(scale, scale, scale);*/
            /*
                        dis = Int32.Parse(msg);
                        if (dis > 40000) scale = 5.5f;
                        else if (dis < 40000 && dis >= 20000) scale = 8.5f;
                        else if (dis < 20000) scale = 11.5f;

                        rt.localScale = new Vector3(scale, scale, scale);
                    */

        }
        else
        {
            Debug.Log("CLose Connect"); client.Close();
        }


    }
}
