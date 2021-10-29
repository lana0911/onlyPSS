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
    public string IP ;
    public int Port ;
    public int mode_int = 0;
    public Socket client;
    int i = 0;
    string msg = "";
    string pose = "";
    int now_cnt = Gobal_TCP.text_cnt;
    int winner;

    static GameManager instance;
    //TCP so0cket 多載用
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
            client.Send(Encoding.UTF8.GetBytes("shot;"));
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


            //收到訊息範例 : text;welcome
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            msg = Encoding.UTF8.GetString(bytes, 0, count);
            //切割
            string[] msg_split = msg.Split(';');
            //Debug.Log("完整" + msg);
            //Debug.Log("msg[0]" + msg_split[0]);
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
                else if (msg_split[0] == "game2")
                {
                    Debug.Log("收到game2");
                    loadToDance();
                }

                //接收到openpose 的 回傳資料
                if (msg_split[0]=="pose")
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
                    //Debug.Log("scale收(dis)" + dis);
                    varName.img_dis = dis;
                }
                //(舉手)收到要開始跳舞
                if (msg_split[0] == "handup")
                {
                    Debug.Log("handup!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Gobal_TCP.handup = true;
                }
                //收到跳舞分數
                if (msg_split[0] == "Dcore")
                {
                    Debug.Log("Dcore!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    string str  = msg_split[1];
                    Gobal_TCP.Dcore = (Convert.ToInt32(str));
                    Debug.Log("Dcore=!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
                ///收關節點資料
                if (msg_split[0] == "k")
                {
                    //Debug.Log("關節點" + msg);
                    string[] sArray = msg.Split(';');
                    Gobal_TCP.leftShoulderPos.x = float.Parse(sArray[1]);
                    Gobal_TCP.leftShoulderPos.y = float.Parse(sArray[2]);
                    Gobal_TCP.leftShoulderPos.z = float.Parse(sArray[3]);
                    Gobal_TCP.leftForeArmPos.x = float.Parse(sArray[4]);
                    Gobal_TCP.leftForeArmPos.y = float.Parse(sArray[5]);
                    Gobal_TCP.leftForeArmPos.z = float.Parse(sArray[6]);
                    Gobal_TCP.leftHandPos.x = float.Parse(sArray[7]);
                    Gobal_TCP.leftHandPos.y = float.Parse(sArray[8]);
                    Gobal_TCP.leftHandPos.z = float.Parse(sArray[9]);

                    Gobal_TCP.headPos.x = float.Parse(sArray[10]);
                    Gobal_TCP.headPos.y = float.Parse(sArray[11]);
                    Gobal_TCP.headPos.z = float.Parse(sArray[12]);
                    Gobal_TCP.neckPos.x = float.Parse(sArray[13]);
                    Gobal_TCP.neckPos.y = float.Parse(sArray[14]);
                    Gobal_TCP.neckPos.z = float.Parse(sArray[15]);
                    Gobal_TCP.spine2Pos.x = float.Parse(sArray[16]);
                    Gobal_TCP.spine2Pos.y = float.Parse(sArray[17]);
                    Gobal_TCP.spine2Pos.z = float.Parse(sArray[18]);

                    Gobal_TCP.rightShoulderPos.x = float.Parse(sArray[19]);
                    Gobal_TCP.rightShoulderPos.y = float.Parse(sArray[20]);
                    Gobal_TCP.rightShoulderPos.z = float.Parse(sArray[21]);
                    Gobal_TCP.rightForeArmPos.x = float.Parse(sArray[22]);
                    Gobal_TCP.rightForeArmPos.y = float.Parse(sArray[23]);
                    Gobal_TCP.rightForeArmPos.z = float.Parse(sArray[24]);
                    Gobal_TCP.rightHandPos.x = float.Parse(sArray[25]);
                    Gobal_TCP.rightHandPos.y = float.Parse(sArray[26]);
                    Gobal_TCP.rightHandPos.z = float.Parse(sArray[27]);

                    Gobal_TCP.leftupLegPos.x = float.Parse(sArray[28]);
                    Gobal_TCP.leftupLegPos.y = float.Parse(sArray[29]);
                    Gobal_TCP.leftupLegPos.z = float.Parse(sArray[30]);
                    Gobal_TCP.leftLegPos.x = float.Parse(sArray[31]);
                    Gobal_TCP.leftLegPos.y = float.Parse(sArray[32]);
                    Gobal_TCP.leftLegPos.z = float.Parse(sArray[33]);
                    Gobal_TCP.leftFootPos.x = float.Parse(sArray[34]);
                    Gobal_TCP.leftFootPos.y = float.Parse(sArray[35]);
                    Gobal_TCP.leftFootPos.z = float.Parse(sArray[36]);

                    Gobal_TCP.rightupLegPos.x = float.Parse(sArray[37]);
                    Gobal_TCP.rightupLegPos.y = float.Parse(sArray[38]);
                    Gobal_TCP.rightupLegPos.z = float.Parse(sArray[39]);
                    Gobal_TCP.rightLegPos.x = float.Parse(sArray[40]);
                    Gobal_TCP.rightLegPos.y = float.Parse(sArray[41]);
                    Gobal_TCP.rightLegPos.z = float.Parse(sArray[42]);
                    Gobal_TCP.rightFootPos.x = float.Parse(sArray[43]);
                    Gobal_TCP.rightFootPos.y = float.Parse(sArray[44]);
                    Gobal_TCP.rightFootPos.z = float.Parse(sArray[45]);

                    //print("Gobal_TCP.leftShoulderPos" + Gobal_TCP.leftShoulderPos);
                    print("Gobal_TCP.leftForeArmPos" + Gobal_TCP.leftForeArmPos);
                    print("Gobal_TCP.rightFootPos" + Gobal_TCP.rightFootPos);
                    print("Gobal_TCP.leftFootPos" + Gobal_TCP.leftFootPos);
                }

            }else
            {
                client.Close();
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
    void loadToDance()
    {
        varName.mode = 2;
    }
    //--UI--跑馬燈
    void pamadan(string content)
    {
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!PamaDan center = " + content);
       
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