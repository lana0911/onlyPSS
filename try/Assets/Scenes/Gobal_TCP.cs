using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class Gobal_TCP
{
//UI 跑馬燈text -------------------------------------------------------------------------------
    public static string text1 = "";
    public static string text2 = "";
    public static int text_cnt = 0;//兩個輪流showte text
    //scene Control
    public static int game_mode = 0;//兩個輪流showte text
//-------------------------------------------------------------------------------


//PSS-------------------------------------------------------------------------------
    public static int pss_name = 1; //一進去顯示 "剪刀石頭布"
    public static bool hint_over = false;
    public static bool hint2_over = false;
    public static bool back = false;
    //
    public static bool hint3_over = false;
    public static bool hint3_paper = false;
    public static bool hint3_sci = false;
    public static bool hint3_stone = false; 
    //開始計時 + 時間到
    public static bool countdown = false;
    public static bool timeup = false;
    //告訴server可以傳pose資訊了
    public static bool scanPose = false;
    //server 傳來的pose
    public static string model_pose = "";
    public static string player_pose = "";
    //winner 是誰
    public static int PSS_winer = -1;
    //UI可顯示server指定動作名稱text
    public static bool show_pose_text = false;
    //RESET
    public static bool reset = false;

    //-------------------------------------------------------------------------------
    public static bool game2Over = false;


    //關節點-------------------------

    public static bool handup = false;//舉手跳舞
    public static bool videoOver = false;//跳舞影片播完
    public static int Dcore = 0;//跳舞影片播完

    public static Vector3 leftShoulderPos;
    public static Vector3 leftForeArmPos;
    public static Vector3 leftHandPos;

    public static Vector3 headPos;
    public static Vector3 neckPos;
    public static Vector3 spine2Pos;

    public static Vector3 rightShoulderPos;
    public static Vector3 rightForeArmPos;
    public static Vector3 rightHandPos;

    public static Vector3 leftupLegPos;
    public static Vector3 leftLegPos;
    public static Vector3 leftFootPos;


    public static Vector3 rightupLegPos;
    public static Vector3 rightLegPos;
    public static Vector3 rightFootPos;
    

}
