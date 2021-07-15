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
/*
line 127: 修改顯示勝負時間
line 53:修改model做動作
*/
/*

public class PSS_contrrol : MonoBehaviour
{

    //animation
    public Animator UnityChanControl = null;
    public int player = 0;
    public int model = 0;
    int winner = -1;//贏家

    //TCP socket
    public string IP = "192.168.56.1";
    public int Port = 8000;
    public Socket client;

    //other
    public bool start = false; //update用
    public float waitTime = 5.0f;
    //TEXT
    public GameObject player_text;
    public GameObject model_text;
    //指定string內容
    public string str_player = " ";
    public string str_model = " ";
    float model_move_time = 16.0f;
    float show_winner_time = 18.0f;


    // Start is called before the first frame update
    void Start()
    {


        //animation 變數初始化
        start = false;

        //建立連線
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IP, Port);

        Thread t = new Thread(recvData);
        t.Start();
       

    }
    
    // Update is called once per frame
    void Update()
    {
        StartCoroutine(tcpStart(model_move_time)); //執行 IEnumerator
        if (start == true)
        {

 
            Debug.Log("showPse()::model==" + model);
            //剪刀石頭布 animation 觸發
            if (model == 1)
            {
                UnityChanControl.SetBool("start", true);
                UnityChanControl.SetBool("scissor", true);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", false);
                Debug.Log("剪刀");
                str_model = "剪刀Scissor";

            }
            else if (model == 3)
            {
                UnityChanControl.SetBool("start", true);
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", true);
                str_model = "布Paper";
                Debug.Log("布");

            }
            else if(model == 2)
            {
                UnityChanControl.SetBool("start", true);
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", true);
                UnityChanControl.SetBool("paper", false);
                str_model = "石頭Stone";
                Debug.Log("石頭");
            }

            if (player == 1)
            {
                str_player = "剪刀Scissor";
            }
            else if (player == 2)
            {
                str_player = "石頭Stone";
            }
            else if (player == 3)
            {
                str_player = "布Paper";
            }

           
            start = false;
            

        }
        else
        {
            Debug.Log("Hi");


        }
        //等3秒後再顯示
        StartCoroutine(winner_show(show_winner_time)); //執行 IEnumerator

    }


    //等待server傳剪刀石頭布資訊
    void recvData()
    {


        client.Send(Encoding.UTF32.GetBytes("PSS"));
        if (client.Connected)
        {
            Debug.Log("RECV:");
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            string msg = Encoding.UTF8.GetString(bytes, 0, count);

            if (msg != null)
            {
                Debug.Log("msg[0]" + msg[0]);//model的pose
                Debug.Log("msg[2]" + msg[2]);//player的pose
                //轉為int型態
                model = (Convert.ToInt32(msg[0])) - 48;
                player = (Convert.ToInt32(msg[2])) - 48;
                //計算贏家是誰
                whoIsWinner();
                
                //給model指定動作
                
                //showPose();

            }

        }
    }
    void whoIsWinner()
    {
        //(剪刀,石頭,布) = (1,2,3)
        //winner : 0=平手, 1=model, 2=player
       
        //model出剪刀
        if (model==1 )
        {
            if (player == 1)
                winner = 0;
            else if (player == 2)
                winner = 2;
            else
                winner = 1;
        }
        //model出石頭
        else if (model == 2)
        {
            if (player == 1)
                winner = 1;
            else if (player == 2)
                winner = 0;
            else
                winner = 2;
        }
        //model出布
        else if (model==3)
        {
            if (player == 1)
                winner = 2;
            else if (player == 2)
                winner = 1;
            else
                winner = 0;
        }
      
    }

    //10秒後開始move
    IEnumerator tcpStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        start = true;
    }
    //3秒後在顯示pose && 勝負
    IEnumerator winner_show(float waitTime2)
    {
        yield return new WaitForSeconds(waitTime2);

        //GET
        GameObject.Find("player_pose_text").GetComponent<Text>().text = str_player;
        GameObject.Find("model_pose_text").GetComponent<Text>().text = str_model;
        mode_num.effect = winner;
        Debug.Log(" mode_num.effect:" + mode_num.effect);
    }


    */

    /*
void showPose()
{
    Debug.Log("showPse()::model==" + model);
    if (model == 1)
    {
        scissors();
        Debug.Log("剪刀");
    }
    else if (model == 2)
    {
        Stone();
        Debug.Log("石頭");
    }
    else if (model == 3)
    {
        Paper();
        Debug.Log("布");
    }
}*/

    /*
    //剪刀石頭布 animation 觸發
    public void scissors()
    {
        UnityChanControl.SetBool("scissor", true);
        UnityChanControl.SetBool("stone", false);
        UnityChanControl.SetBool("paper", false);

    }
    public void Stone()
    {
        UnityChanControl.SetBool("scissor", false);
        UnityChanControl.SetBool("stone", true);
        UnityChanControl.SetBool("paper", false);

    }
    public void Paper()
    {
        UnityChanControl.SetBool("scissor", false);
        UnityChanControl.SetBool("stone", false);
        UnityChanControl.SetBool("paper", true);
    }
    */


//}
