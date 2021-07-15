/*using System.Collections;
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
using Cysharp.Threading.Tasks;

public class tcp_dis : MonoBehaviour
{
    RectTransform rt;
    public int dis = 0;
    public float scale = 5.5f;
    //
    public string IP = "192.168.56.1";
    public int Port = 8000;
    public Socket client;
    public string msg;


    async void Update()
    {
        Thread.Sleep(2000);
        await Changing();
        
        //await UniTask.Delay(TimeSpan.FromSeconds(2f));
        //await UniTask.Yield();

    }
    async void  Start()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(new IPEndPoint(IPAddress.Parse(IP), Port));
        

    }

    async UniTask Changing()
    {

        //recv
        var bytes = new byte[1024];
        var count = client.Receive(bytes);
       // var count = client.Read(bytes, 0, client.ReceiveBufferSize);
        string msg = Encoding.UTF8.GetString(bytes, 0, count);
        if (msg != null)
        {

            Debug.Log(msg);
        }
        else
        {
            Debug.Log("CLose Connect"); client.Close();
        }


    }
}
*/