using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class Gobal_TCP
{
//UI �]���Otext -------------------------------------------------------------------------------
    public static string text1 = "";
    public static string text2 = "";
    public static int text_cnt = 0;//��ӽ��yshowte text
    //scene Control
    public static int game_mode = 0;//��ӽ��yshowte text
//-------------------------------------------------------------------------------


//PSS-------------------------------------------------------------------------------
    public static int pss_name = 1; //�@�i�h��� "�ŤM���Y��"
    public static bool hint_over = false;
    public static bool hint2_over = false;
    public static bool back = false;
    //
    public static bool hint3_over = false;
    public static bool hint3_paper = false;
    public static bool hint3_sci = false;
    public static bool hint3_stone = false; 
    //�}�l�p�� + �ɶ���
    public static bool countdown = false;
    public static bool timeup = false;
    //�i�Dserver�i�H��pose��T�F
    public static bool scanPose = false;
    //server �ǨӪ�pose
    public static string model_pose = "";
    public static string player_pose = "";
    //winner �O��
    public static int PSS_winer = -1;
    //UI�i���server���w�ʧ@�W��text
    public static bool show_pose_text = false;
    //RESET
    public static bool reset = false;

    //-------------------------------------------------------------------------------
    public static bool game2Over = false;


    //���`�I-------------------------

    public static bool handup = false;//�|����R
    public static bool videoOver = false;//���R�v������
    public static int Dcore = 0;//���R�v������

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
