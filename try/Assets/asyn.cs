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
//using Cysharp.Threading.Tasks;

public class asyn : MonoBehaviour
{
    RectTransform rt;
    public int dis = 0;
    public float scale = 5.5f;
    //
    public string IP = "192.168.56.1";
    public int Port = 8000;
    public Socket client;
    public string msg;


    private void Update()
    {
        Debug.Log("Gett Data:");
        Changing();
        Thread.Sleep(10);

    }
    void Start()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));

    }

    public void Changing()
    {

        //recv
        var bytes = new byte[1024];
        var count = client.Receive(bytes);
        string msg = Encoding.UTF8.GetString(bytes, 0, count);
        if (client.Connected)
        {
           
            Debug.Log(msg);
        }
        else
        {
            Debug.Log("CLose Connect"); client.Close();
        }
       

    }
}
