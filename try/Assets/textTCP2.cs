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
using System.Collections.Concurrent;
public class textTCP2 : MonoBehaviour
{
    Vector3 receivedPos = Vector3.zero;
    public int dis = 0;
    public int size = 18;
    public float scale = 5.0f;
    public float speed = 0.5f;
    float leftX;
    //
    Text words;
    public string IP = "192.168.56.1";
    public string message;
    public int Port = 8000;
    public Socket client;
    public int msg_do = 0;
    ConcurrentQueue<string> que = new ConcurrentQueue<string>();
    void Start()
    {
        leftX = 172 * -1;
        
        words = GetComponent<Text>();
        Debug.Log("Starting ::start");
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IP, Port);
        Thread t = new Thread(recvData);
        t.Start();
        client.Send(Encoding.UTF32.GetBytes("text2"));

    }
    void Update()
    {
        this.gameObject.transform.Translate(-10 * speed, 0, 0);
        float nowX = this.gameObject.transform.localPosition.x;
        if (nowX < leftX)
        {
            this.gameObject.transform.Translate(1600, 0, 0);
        }
        if (msg_do == 1)
        {
            que.TryDequeue(out message);
            words.text = message;
            msg_do = 0;
        }

    }

    void recvData()
    {
        while (client.Connected)
        {
            Debug.Log("RECV:");
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            string msg = Encoding.UTF8.GetString(bytes, 0, count);
            if (msg != null)
            {
                Debug.Log(msg);
                que.Enqueue(msg);
                msg_do = 1;

            }

        }



    }
}
