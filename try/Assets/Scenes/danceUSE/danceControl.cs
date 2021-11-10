using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading;
using System;
using UnityEngine.Video;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using System.Text;
using System.Collections.Concurrent;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class danceControl: MonoBehaviour
{
    //img
    public GameObject blackBg;
    public Text mention; //mwntion
    public Text overWord;
    public Text str_core; //分數 (字串)
    public Text judge; //評語 (字串)
    int int_core = 0 ; //分數 (字串)
    int show_cnt;//"timeUP"顯示多久
    //影片
    public VideoPlayer vgoodBg; 
    public VideoPlayer vnormalBg; 
    public VideoPlayer vbadBg; 
    //public VideoPlayer vPlayer;
    public GameObject Ogood;
    public GameObject Obad;
    public GameObject Onor;
    public GameObject ready;
    public GameObject shine;
    int shin_n = 0;
    void Start()
    {
        //vPlayer.Stop();
        //img
        //ready.SetActive(false);
        blackBg.SetActive(false);
        shine.SetActive(false);
        InvokeRepeating("shining", 0.1f, 0.5f);
        overWord.text = "";
        str_core.text = "";
        show_cnt = 0;
        //video
        //vPlayer = GetComponent<VideoPlayer>();
        vgoodBg = GetComponent<VideoPlayer>();
        vnormalBg = GetComponent<VideoPlayer>();
        vbadBg = GetComponent<VideoPlayer>();
        Ogood.SetActive(false);
        Obad.SetActive(false);
        Onor.SetActive(false);
        //vPlayer.playOnAwake = false;
       // vPlayer.loopPointReached += EndReached;

    }
    //影片播完
    void EndReached(UnityEngine.Video.VideoPlayer vp)
    {
        
        Debug.Log("over!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        Gobal_TCP.videoOver = true;
       // vPlayer.Stop();

        loadOver();
    }
    void Update()
    {
        //舉手開始跳舞
        if (Gobal_TCP.handup)
        {
            Debug.Log("play!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            mention.text = "";
            CancelInvoke("shining");
            shine.SetActive(false);
            ready.SetActive(false);
            //GetComponent<AudioSource>().Play();
          //  vPlayer.Play();
            Gobal_TCP.handup = false;
        }
        if(Gobal_TCP.tu)
        {
            loadOver();
        }
    }
    void loadOver()
    {
        Debug.log("收到影片結束");
        blackBg.SetActive(true);
        InvokeRepeating("showHide", 0.5f, 0.5f);
    }
    void showHide()
    {
        if (show_cnt < 6)
        {
            show_cnt++;
            if (overWord.text == "")
            {
                overWord.text = "Time UP!"; //將 mytext內容改變
            }
            else
            {
                overWord.text = ""; //將 mytext內容改成空的
            }
        }
        else
        {
            overWord.text = "";
            CancelInvoke("showHide");
            showCore();
        }
    }
    void showCore()
    {
        InvokeRepeating("adder", 0.1f, 0.03f);
    }

    void adder()
    {
        string str = Convert.ToString(int_core);
        if (int_core < (int)Gobal_TCP.Dcore/1.2 )
        {
            
            str_core.text = str;
            int_core++;
        }
        else if(int_core >= (int)Gobal_TCP.Dcore / 1.2 && int_core <= Gobal_TCP.Dcore)
        {
            str_core.text = str;
            int_core++;
            CancelInvoke("adder");
            InvokeRepeating("adder", 0.1f, 0.15f);
        }
        else
        {
            CancelInvoke("adder");
            bgWithCore();
        }
    }
    void bgWithCore()
    {
        //90~100分
        if (Gobal_TCP.Dcore >= 90 && Gobal_TCP.Dcore <= 100)
        {
            judge.text = "太厲害了!!";
            Ogood.SetActive(true);
            vgoodBg.Play();
            StartCoroutine(wait(2.5f));
        }
        //60~90分
        else if (Gobal_TCP.Dcore < 90 && Gobal_TCP.Dcore >= 60)
        {
            judge.text = "還不錯喔";
            Onor.SetActive(true);
            vnormalBg.Play();
            StartCoroutine(wait(2.5f));
        }
        //60分下
        else if (Gobal_TCP.Dcore <= 60)
        {
            Obad.SetActive(true);
            judge.text = "多多加油";
            vbadBg.Play();
            StartCoroutine(wait(4.0f));
        }
    }
    void shining ()
    {

        if(shin_n == 0)
        {
            shine.SetActive(true);
            shin_n = 1;
        }
        else
        {
            shine.SetActive(false);
            shin_n = 0;
        }
    }
    IEnumerator wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //規0
        Gobal_TCP.handup = false;
        Gobal_TCP.videoOver = false;
        Gobal_TCP.Dcore = 0;
        Gobal_TCP.game2Over = true;
        varName.mode = 0;
    }
}
