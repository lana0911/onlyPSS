using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class varName
{

    //文字-------------------------------
    //遊戲名稱
    public static bool gameName = true;
    //對鏡頭文字
    public static bool gameIntro = false;
    //提示等下開始倒數
    public static bool gameIntro2 = false;
    public static bool beforeCount = false;
    
    public static bool OpenCam = false;
    //倒數結束(openpose回傳結果)
    public static bool cnt_end = false;









    //ca
    public static bool came = false; //


    public static bool playing = false; //playing
    public static int mode = -1; //playing

    //UI介面
    public static float img_dis = 0;//album iimg用
    //PSS
    public static int pss_name = 1; //一進去顯示 "剪刀石頭布"
    public static bool gameName_show = true;//"肢體剪刀時頭部"
    public static bool introTitle_show = false;//"遊戲說明"
    public static bool introContent_show = false;//"遊戲說明內容
    public static bool gift_show = false;//"小禮物"    
    public static bool pss_name_show = false;//示範時的剪刀石頭布名稱
    public static bool pss_detail_show = false;//示範時的剪刀石頭布說明  
    public static bool cnt_start = false;//倒數開始
    //PSS-model animation
    public static string modelPose = "";//
    public static string playerPose = "";//
    public static bool model_start_animation = false;//
    public static int winner = -1;//
    public static int getPrize = -1;//
    public static bool game1Over = false;


}
