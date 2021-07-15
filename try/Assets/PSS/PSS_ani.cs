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
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class PSS_ani : MonoBehaviour
{
    public Animator UnityChanControl = null;
   // public AudioSource UnityChanVoice = null;
   // public AudioClip [] UnityChanVoiceClips = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //剪刀石頭布 animation 觸發(示範)
        if (Gobal_TCP.hint3_sci == true)
        {
            Debug.Log("觸發剪刀");  
            UnityChanControl.SetBool("showSc", true);
        }
        if (Gobal_TCP.hint3_stone == true)
        {
            Debug.Log("觸發石頭");
            UnityChanControl.SetBool("showSt", true);
        }
        if (Gobal_TCP.hint3_paper == true)
        {
            Debug.Log("觸發布");
            UnityChanControl.SetBool("showPa", true);
        }
        if (Gobal_TCP.back == true)
        {
            Debug.Log("觸發Wait");
            UnityChanControl.SetBool("back", true);
        }
        //指定model 的剪刀石頭布 pose開始動作
        if (Gobal_TCP.timeup == true)
        {
            UnityChanControl.SetBool("start", true);
            //做剪刀
            if (Gobal_TCP.model_pose == "1")
            {
                UnityChanControl.SetBool("scissor", true);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", false);
                StartCoroutine(win_lose(3.5f, 1));


            }//做石頭
            else if(Gobal_TCP.model_pose == "2")
            {
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", true);
                UnityChanControl.SetBool("paper", false);
                StartCoroutine(win_lose(3.5f, 2));
            }//做布
            else if (Gobal_TCP.model_pose == "3")
            {
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", true); 
                StartCoroutine(win_lose(3.5f, 3));
                
            }
            StartCoroutine(show_pose_word(2.5f));
        }
        //if遊戲結束
        if(Gobal_TCP.reset)
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!RESET");
            Gobal_TCP.reset = false;
            Gobal_TCP.pss_name = 1;
            Gobal_TCP.hint_over = false;
            Gobal_TCP.hint2_over = false;
            Gobal_TCP.back = false;
            Gobal_TCP.hint3_paper = false;
            Gobal_TCP.hint3_over = false;
            Gobal_TCP.hint3_sci = false;
            Gobal_TCP.hint3_stone = false;
            Gobal_TCP.countdown = false;
            Gobal_TCP.timeup = false;
            Gobal_TCP.scanPose = false;
            Gobal_TCP.model_pose = "";
            Gobal_TCP.player_pose = "";
            Gobal_TCP.PSS_winer = -1;
            Gobal_TCP.show_pose_text = false;
            UnityChanControl.SetBool("scissor", false);
            UnityChanControl.SetBool("stone", false);
            UnityChanControl.SetBool("paper", false);
            UnityChanControl.SetBool("start", false);
            UnityChanControl.SetBool("showPa", false);
            UnityChanControl.SetBool("showSc", false);
            UnityChanControl.SetBool("showSt", false);
            UnityChanControl.SetBool("back", false);
            UnityChanControl.SetBool("paOver", false);
            UnityChanControl.SetBool("StOver", false);
            UnityChanControl.SetBool("ScOver", false);
            UnityChanControl.SetBool("Lose", false);
            UnityChanControl.SetBool("Win", false);
            SceneManager.LoadScene(0);

        }
    }


    //4秒後在顯示
    IEnumerator show_pose_word(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + Gobal_TCP.show_pose_text);

        Gobal_TCP.show_pose_text = true;
    }
    //4秒後在win/lose
    IEnumerator win_lose(float waitTime,int who)
    {
        yield return new WaitForSeconds(waitTime);
        Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + Gobal_TCP.show_pose_text);
        if(who==1)
        {
            UnityChanControl.SetBool("ScOver", true);

        }
        else if(who==2)
        {
            UnityChanControl.SetBool("Stover", true);
        }
        else if(who==3)
        {
            UnityChanControl.SetBool("paOver", true);
        }
        //win / lose
        if (Gobal_TCP.PSS_winer == 0)
        {
            UnityChanControl.SetBool("Win", true);
           // UnityChanVoice.clip = UnityChanVoiceClips[0];
            //UnityChanVoice.Play();


        }
        else if (Gobal_TCP.PSS_winer == 1)
        {
            UnityChanControl.SetBool("Win", true);
          //  UnityChanVoice.clip = UnityChanVoiceClips[0];
          //  UnityChanVoice.Play();
        }
        else if(Gobal_TCP.PSS_winer == 2)
        {

            UnityChanControl.SetBool("Lose", true);
         //   UnityChanVoice.clip = UnityChanVoiceClips[1];
          //  UnityChanVoice.Play();
        }
       
    }
}
