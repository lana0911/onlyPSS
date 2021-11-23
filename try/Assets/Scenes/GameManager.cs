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

    //file text write
    string path = @"d:\澶у\onlyPSS\onlyPSS\\test1.txt";
	string[] createText = {""};

    static GameManager instance;
    //TCP so0cket 锟絟锟斤拷锟斤拷
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
            Debug.Log("锟絉锟斤拷" + ScenName + "锟斤拷" + name);
            Destroy(gameObject);
        }

    }
    /// <summary>
    /// ////////////////////////////////////////////////////////
    /// </summary>
    //锟絅server锟斤拷锟絜锟斤拷s锟絎锟絟
    void Update()
    {
        /*//锟斤拷锟斤拷锟斤拷锟斤拷
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
        //锟斤拷锟統锟脚锟斤拷锟結锟斤拷
        if (varName.cnt_end)
        {
            //锟絠锟紻server锟絠锟紿锟斤拷
            client.Send(Encoding.UTF8.GetBytes("shot;"));
            varName.cnt_end = false;


        }
        if (varName.game1Over && fi==0)//锟絠锟紻server锟紺锟斤拷锟斤拷锟斤拷
        {
            client.Send(Encoding.UTF8.GetBytes("over;"));
            //fi = 1;
            varName.game1Over = false;
        }
        if (Gobal_TCP.game2Over && fi == 0)//锟絠锟紻server锟紺锟斤拷锟斤拷锟斤拷
        {
            client.Send(Encoding.UTF8.GetBytes("over2;"));
            //fi = 1;
            Gobal_TCP.game2Over = false;
        }

    }
    //锟截ミ硈锟絬
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
            //锟斤拷锟斤拷 scale;123456
            string[] msg_split = msg.Split(';');
            //Debug.Log("锟斤拷锟斤拷" + msg);
           // Debug.Log("msg[0]" + msg_split[0]);
            //Debug.Log("msg[1]" + msg_split[1]);


            if (msg != null)
            {
                //锟斤拷锟斤拷锟斤拷openpose 锟斤拷 锟絕锟角革拷锟�
                if (msg_split[0] == "scale")
                {
                    //Debug.Log("scale锟斤拷" + msg_split[1]);
                    dis = (Convert.ToInt32(msg_split[1]));
                    //Debug.Log("scale锟斤拷(dis)" + dis);
                    varName.img_dis = dis;
                }


            }

        }
       
    }*/
    //锟絡锟介Μ锟斤拷锟�
    void recvData()
    {
        int im = 1;
        //锟絠锟斤拷锟絆unity 锟捷狾
        client.Send(Encoding.UTF8.GetBytes("1"));

        // Thread t_img = new Thread(recvIMG);
        //t_img.Start();
        float dis;
        while (client.Connected)
        {


            //锟斤拷锟斤拷T锟斤拷锟絛锟斤拷 : text;welcome
            var bytes = new byte[1024];
            var count = client.Receive(bytes);
            msg = Encoding.UTF8.GetString(bytes, 0, count);
            //锟斤拷锟斤拷
            string[] msg_split = msg.Split(';');
            //Debug.Log("锟斤拷锟斤拷" + msg);
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
                    Debug.Log("锟斤拷锟斤拷game1");
                    loadToPSS();
                }
                else if (msg_split[0] == "game2")
                {
                    Debug.Log("锟斤拷锟斤拷game2");
                    loadToDance();
                }

                //锟斤拷锟斤拷锟斤拷openpose 锟斤拷 锟絕锟角革拷锟�
                if (msg_split[0]=="pose")
                {
                    Debug.Log("Scan锟斤拷锟紾" + msg_split[1]);
                    //锟揭帮拷model锟斤拷锟斤拷锟絯锟绞
                    
                    //锟絇锟絖锟斤拷墓
                    whoWin(msg_split[1]);
                }                
                if (msg_split[0] == "scale")
                {
                    //Debug.Log("scale锟斤拷" + msg_split[1]);
                    dis = (Convert.ToInt32(msg_split[1]));
                    //Debug.Log("scale锟斤拷(dis)" + dis);
                    varName.img_dis = dis;
                }
                //(锟絴锟斤拷)锟斤拷锟斤拷n锟絵锟絣锟斤拷锟絉
                if (msg_split[0] == "handup")
                {
                    Debug.Log("handup!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    Gobal_TCP.handup = true;
                }
                //锟斤拷锟斤拷锟斤拷R锟斤拷锟斤拷
                if (msg_split[0] == "Dcore")
                {
                    Debug.Log("Dcore!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    string str  = msg_split[1];
                    //Gobal_TCP.Dcore = (Convert.ToInt32(str));
                    Debug.Log("Dcore=!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
                ///锟斤拷锟斤拷锟絗锟絀锟斤拷锟�
                if (msg_split[0] == "k")
                {
                    //file text write
                    if(Gobal_video.status == 1 && Gobal_video.timeFlag == 1) {
                        string appendText = msg + Environment.NewLine;
                        // print(msg);
                        File.AppendAllText(path, appendText, Encoding.UTF8);
                        Gobal_video.timeFlag = 0;
                        // print("gameManage second");
                    }

                    //Debug.Log("锟斤拷锟絗锟絀" + msg);
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
                    // print("Gobal_TCP.leftForeArmPos" + Gobal_TCP.leftForeArmPos);
                    // print("Gobal_TCP.rightFootPos" + Gobal_TCP.rightFootPos);
                    // print("Gobal_TCP.leftFootPos" + Gobal_TCP.leftFootPos);
                }

            }else
            {
                client.Close();
            }

        }
    }
    ////锟絇锟絖锟斤拷墓
    void whoWin(string pose)
    {
        //1st: player 2nd:model
        string[] P = pose.Split(' ');
        string player = P[0];
        string model = P[1] ;
        
        
        //winner : 0=锟斤拷锟斤拷, 1=model, 2=player
        //model锟絏锟脚
        if (model == "1")
        {
            varName.modelPose = "锟脚 Scissor";
            if (player == "1")
            {
                varName.playerPose = "锟脚 Scissor";
                varName.winner = 0;
            }
                
            else if (player == "2")
            {
                varName.winner = 2;
                varName.playerPose = "锟斤拷锟結 Stone";

            }
            else
            {
                varName.winner = 1;
                varName.playerPose = "锟斤拷 Paper";

            }
        }
        //model锟絏锟斤拷锟結
        else if (model == "2")
        {
            varName.modelPose = "锟斤拷锟結 Stone";
            if (player == "1")
            {
                varName.winner = 1;
                varName.playerPose = "锟脚 Scissor";

            }
            else if (player == "2")
            { 
                varName.winner = 0;
                varName.playerPose = "锟斤拷锟結 Stone";
            }
            else
            {
                varName.winner = 2;
                varName.playerPose = "锟斤拷 Paper";
            }
        }
        //model锟絏锟斤拷
        else if (model == "3")
        {
            varName.modelPose = "锟斤拷 Paper";
            if (player == "1")
            {
                varName.winner = 2;
                varName.playerPose = "锟脚 Scissor";
            }
            else if (player == "2")
            {
                varName.winner = 1;
                varName.playerPose = "锟斤拷锟結 Stone";
            }
            else
            {
                varName.winner = 0;
                varName.playerPose = "锟斤拷 Paper";

            }
        }
        Debug.Log("winner=" + varName.winner);
        //1.锟斤拷model锟斤拷锟斤拷锟絯锟绞
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
    //--UI--锟絔锟斤拷锟絆
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