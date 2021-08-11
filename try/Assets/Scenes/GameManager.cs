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
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
    //cama
    int fi = 0;
    public AudioSource Voice = null;
    public AudioClip[] VoiceClips = null;
    //int game = mode_num.mode;
    public string IP = "192.168.56.1";
    public int Port = 8000;
    public int mode_int = 0;
    public Socket client;
    int i = 0;
    string msg = "";
    string pose = "";
    int now_cnt = Gobal_TCP.text_cnt;
    int winner;

    static GameManager instance;
    //TCP socket 多載用
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            name = "first_game";
        }
        else if (this != instance && instance != null)
        {
            string ScenName = SceneManager.GetActiveScene().name;
            Debug.Log("刪除" + ScenName + "的" + name);
            Destroy(gameObject);
        }

    }
    /// <summary>
    /// ////////////////////////////////////////////////////////
    /// </summary>
    //將server內容更新上去
    void Update()
    {
        /*//場景切換
        if (Gobal_TCP.game_mode == 0)
        {
            SceneManager.LoadScene(0);
            Debug.Log("switch Sence 0 (UI)");
        }
        else if (Gobal_TCP.game_mode == 1)
        {
            SceneManager.LoadScene(1);
            Debug.Log("switch Sence 1 (PSS Game)");
        }
        else if (Gobal_TCP.game_mode == 2)
        {
            SceneManager.LoadScene(2);
            Debug.Log("switch Sence 2 (Dance Game)");
        }*/
        //掃描剪刀石頭布
        if (varName.cnt_end)
        {
            //告訴server可以掃
            client.Send(Encoding.UTF8.GetBytes("pose;"));
            varName.cnt_end = false;


        }
        if (varName.game1Over && fi==0)//告訴server遊戲結束
        {
            client.Send(Encoding.UTF8.GetBytes("over;"));
            //fi = 1;
            varName.game1Over = false;
        }

    }
    //建立連線
    void Start()
    {
        Debug.Log("hi" + i);
        i++;
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(IP, Port);
        Thread t = new Thread(recvData);
       
        t.Start();
        

    }
    /*void recvIMG()
    {
        Debug.Log("recvIMG start");
        while (client.Connected)
        {
            
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            float dis;
            msg = Encoding.UTF8.GetString(bytes, 0, count);
            //切割 scale;123456
            string[] msg_split = msg.Split(';');
            //Debug.Log("完整" + msg);
           // Debug.Log("msg[0]" + msg_split[0]);
            //Debug.Log("msg[1]" + msg_split[1]);


            if (msg != null)
            {
                //接收到openpose 的 回傳資料
                if (msg_split[0] == "scale")
                {
                    //Debug.Log("scale收" + msg_split[1]);
                    dis = (Convert.ToInt32(msg_split[1]));
                    //Debug.Log("scale收(dis)" + dis);
                    varName.img_dis = dis;
                }


            }

        }
       
    }*/
    //迴圈收資料
    void recvData()
    {
        int im = 1;
        //告知是unity 看板
        client.Send(Encoding.UTF8.GetBytes("1"));
        // Thread t_img = new Thread(recvIMG);
        //t_img.Start();
        float dis;
        while (client.Connected)
        {
            Debug.Log("switch recv:");
            //收到訊息範例 : text;welcome
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            msg = Encoding.UTF8.GetString(bytes, 0, count);
            //切割
            string[] msg_split = msg.Split(';');
            Debug.Log("完整" + msg);
            Debug.Log("msg[0]" + msg_split[0]);
            //Debug.Log("msg[1]" + msg_split[1]);

            
            if (msg != null)
            {
                if (msg_split[0] == "text")
                {
                    pamadan(msg_split[1]);
                }
                else if(msg_split[0] == "game1")
                {
                    Debug.Log("收到game1");
                    loadToPSS();
                }
                //接收到openpose 的 回傳資料
                if(msg_split[0]=="pose")
                {
                    Debug.Log("Scan結果" + msg_split[1]);
                    //啟動model做指定動作
                    
                    //判斷輸贏
                    whoWin(msg_split[1]);
                }                
                if (msg_split[0] == "scale")
                {
                    //Debug.Log("scale收" + msg_split[1]);
                    dis = (Convert.ToInt32(msg_split[1]));
                    Debug.Log("scale收(dis)" + dis);
                    varName.img_dis = dis;
                }


            }

        }
    }
    ////判斷輸贏
    void whoWin(string pose)
    {
        //1st: player 2nd:model
        string[] P = pose.Split(' ');
        string player = P[0];
        string model = P[1] ;
        
        
        //winner : 0=平手, 1=model, 2=player
        //model出剪刀
        if (model == "1")
        {
            varName.modelPose = "剪刀 Scissor";
            if (player == "1")
            {
                varName.playerPose = "剪刀 Scissor";
                varName.winner = 0;
            }
                
            else if (player == "2")
            {
                varName.winner = 2;
                varName.playerPose = "石頭 Stone";

            }
            else
            {
                varName.winner = 1;
                varName.playerPose = "布 Paper";

            }
        }
        //model出石頭
        else if (model == "2")
        {
            varName.modelPose = "石頭 Stone";
            if (player == "1")
            {
                varName.winner = 1;
                varName.playerPose = "剪刀 Scissor";

            }
            else if (player == "2")
            { 
                varName.winner = 0;
                varName.playerPose = "石頭 Stone";
            }
            else
            {
                varName.winner = 2;
                varName.playerPose = "布 Paper";
            }
        }
        //model出布
        else if (model == "3")
        {
            varName.modelPose = "布 Paper";
            if (player == "1")
            {
                varName.winner = 2;
                varName.playerPose = "剪刀 Scissor";
            }
            else if (player == "2")
            {
                varName.winner = 1;
                varName.playerPose = "石頭 Stone";
            }
            else
            {
                varName.winner = 0;
                varName.playerPose = "布 Paper";

            }
        }
        Debug.Log("winner=" + varName.winner);
        //1.讓model做指定動作
        varName.model_start_animation = true;


    }
    void loadToPSS()
    {

        varName.mode = 1;
    }

    //--UI--跑馬燈
    void pamadan(string content)
    {
        Debug.Log("PamaDan center = " + content);
       
        Debug.Log("Gobal_TCP.text_cnt = " + Gobal_TCP.text_cnt);
        if (Gobal_TCP.text_cnt == now_cnt)
        {
           if (Gobal_TCP.text_cnt % 2 == 0)
                {
                    Gobal_TCP.text1 = content;
                    Gobal_TCP.text_cnt++;
                    now_cnt = Gobal_TCP.text_cnt;
                }
                else if (Gobal_TCP.text_cnt % 2 == 1)
                {
                    Gobal_TCP.text2 = content;
                    Gobal_TCP.text_cnt++;
                    now_cnt = Gobal_TCP.text_cnt;
                }
        }
     
    }




}