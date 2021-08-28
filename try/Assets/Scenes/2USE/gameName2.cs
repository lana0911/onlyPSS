using System.Collections;

using System.Collections.Generic;
using System.IO;
using UnityEngine;

using UnityEngine.UI; //使用Unity UI程式庫。 (Text是UI的一部份哦! 要使用就必須要加，不然會出現錯誤!)

public class gameName2 : MonoBehaviour
{

    public Text NameText;
    public Text Intro_titleText;
    public Text breforeCountText;
    public Text countNum;
    public Camera m_camera;
    //text use
    private float timer;
    int time_int = 17;
    public Animator UnityChanControl = null;
    //pose
    public Text pPose;
    public Text mPose;
    public Text Winner;
    //img
    public GameObject candy;
    public GameObject bg2_img;
    public GameObject takeGift; 
    public GameObject bg2_lose;
    public GameObject noGift;
    public GameObject UI_imgs;
    RectTransform rt;
    RectTransform rtBg;
    public Text backto;
    double y = -213;
    int direct = 0;
    int downCnt = 0;
    float scale = 0.0f;
    float scale_Candy = 0.0f;
    void Start()
    {
        NameText.text = "";
        countNum.text = "";
        breforeCountText.text = ""; 
        Intro_titleText.text = "";
        backto.text = "";
        callCountdown();
        rt = candy.GetComponent<RectTransform>();
        rtBg = bg2_img.GetComponent<RectTransform>();
        candy.SetActive(false);
        UI_imgs.SetActive(true);
        bg2_img.SetActive(false);
        takeGift.SetActive(false);
        bg2_lose.SetActive(false);
        noGift.SetActive(false);
    }

    void Update()
    {
        //遊戲名稱
        if (varName.gameName)
        {
            timer += Time.deltaTime * 2;

            if (timer % 2 > 1.0f)
            {
                NameText.text = "";
            }
            else
            {
                //Debug.Log(timer);
                NameText.text = "肢體<color=#FF7167>剪刀</color><color=#19B3B1>石頭</color><color=#FFD73D>布</color>";
                if (timer > 5.5)
                {
                    NameText.text = "";
                    varName.gameName = false;
                    varName.gameIntro = true;
                }
            }
        }
        //提示對準鏡頭
        if (varName.gameIntro)
        {
            Intro_titleText.text = "請站在畫面中央...";
            StartCoroutine(wait(3f, 1));

        }
        //提示等下開始倒數
        if (varName.gameIntro2)
        {
            breforeCountText.text = "待會開始倒數\n別忘記擺動作喔";
            StartCoroutine(wait(4f, 2));
        }
        //開始倒數
        if (varName.beforeCount)
        {
            Debug.Log("update");

            // countNum.text = "5";
            //StartCoroutine(wait(3f, 3));
            varName.beforeCount = false;

        }
        //倒數結束收到Server傳來的poses
        if(varName.model_start_animation)
        {
            varName.model_start_animation = false;
            Debug.Log("model_start_animation");
            UnityChanControl.SetBool("start", true);
            //做剪刀
            if (varName.modelPose == "剪刀 Scissor")
            {
                UnityChanControl.SetBool("scissor", true);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", false);
                //StartCoroutine(win_lose(3.5f, 1));


            }//做石頭
            else if (varName.modelPose == "石頭 Stone")
            {
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", true);
                UnityChanControl.SetBool("paper", false);
                // StartCoroutine(win_lose(3.5f, 2));
            }//做布
            else if (varName.modelPose == "布 Paper")
            {
                UnityChanControl.SetBool("scissor", false);
                UnityChanControl.SetBool("stone", false);
                UnityChanControl.SetBool("paper", true);
                // StartCoroutine(win_lose(3.5f, 3));

            }
            //等1.5秒在收動作
            StartCoroutine(wait(3.0f, 4));
            //等1.5秒在print中文pose

        }
    }
    void callCountdown()
    {
        InvokeRepeating("countdown", 1, 1);

    }

    IEnumerator wait(float waitTime, int type)
    {
        yield return new WaitForSeconds(waitTime);
        // Debug.Log("wait....over");
        if (type == 1)//"請站在畫面中央..." 顯示3秒
        {
            Debug.Log("type1");
            Intro_titleText.text = "";
            varName.gameIntro = false;
            varName.gameIntro2 = true;
        }
        if (type == 2)//"待會開始倒數別忘記擺動作喔" 顯示3秒
        {
            Debug.Log("type2");
            breforeCountText.text = "";
            varName.gameIntro2 = false;
            varName.beforeCount = true;
        }
        if (type == 3)//"倒數完關鏡頭
        {
            Debug.Log("type3");
            countNum.text = "";
            varName.beforeCount = false;
            //varName.came = true;
        }
        if (type == 4) //
        {
            Debug.Log("type4");

            pPose.text = varName.playerPose;
            mPose.text = varName.modelPose;

            //收Pose
            if (varName.modelPose == "剪刀 Scissor")
                UnityChanControl.SetBool("ScOver", true);

            else if (varName.modelPose == "石頭 Stone")
                UnityChanControl.SetBool("StOver", true);

            else if (varName.modelPose == "布 Paper")
                UnityChanControl.SetBool("paOver", true);


            //在等1.5s 印中文結果
            StartCoroutine(wait(1.5f, 5));
        }
        if (type == 5)
        {
            Debug.Log("type5");
            if (varName.winner == 1)//model win
            {
                Winner.text = "YOU LOSE";
                UnityChanControl.SetBool("Win", true);
            }
            else if (varName.winner == 0)//model win
            {
                Winner.text = "TIE";
                UnityChanControl.SetBool("Win", true);
            }
            else if (varName.winner == 2)//player win
            {
                Winner.text = "YOU WIN";
                UnityChanControl.SetBool("Lose", true);
            }
            ////在等2s 進入頒發獎品
            StartCoroutine(wait(3.0f, 6)); 
        }
        if (type == 6)
        {
            ceremony();
        }
        if (type == 7)
        {
            backto.text = "回主畫面中...";
            varName.game1Over = true;
            //3s後回UI
            StartCoroutine(wait(1.5f, 8));
        }
        if (type == 8)//just stop for candy go down
        {

            //規0
            varName.gameName = true;
            varName.gameIntro = false;
            varName.gameIntro2 = false;
            varName.beforeCount = false;
            varName.OpenCam = false;
            varName.cnt_end = false;

            varName.came = false;
            varName.gameName_show = true;
            varName.pss_name = 1;
            varName.introTitle_show = false;
            varName.introContent_show = false;
            varName.gift_show = false;
            varName.pss_name_show = false;
            varName.pss_detail_show = false;
            varName.cnt_start = false;
            varName.cnt_end = false;
            varName.modelPose = "";
            varName.playerPose = "";
            varName.model_start_animation = false;
            varName.winner = -1;
            varName.getPrize = -1;
            varName.game1Over = false;



            varName.mode = 0;
        }
    }
    // //頒發獎品-----------------------------------------------------------------------
    void ceremony()
    {
        //UI_imgs.SetActive(false);
        varName.came = true;

        if (varName.winner == 2 || varName.winner == 0)//pleyer win or tie 有獎品
        {
            candy.SetActive(true);
            bg2_img.SetActive(true);
            takeGift.SetActive(true);

            InvokeRepeating("goDown", 1, 0.00008f);
           // InvokeRepeating("goDown_Forlight", 1, 0.005f);


        }
        else if (varName.winner == 1)//model win 無獎品
        {
            noGift.SetActive(true);
            bg2_lose.SetActive(true);
            candy.SetActive(true);
            InvokeRepeating("goDown", 1, 0.00008f);
           // InvokeRepeating("goDown_Forlight", 1, 0.005f);

        }

    }
    //----------------------------------------------------------------------------------
/*    void bgScale()
    {

        //Debug.Log("scale=" + scale);
        Debug.Log("y=" + y);
        scale += 10.0f;
        scale_Candy += 1.2f;
        if (scale >= 50)
        {
            takeGift.SetActive(true);
            CancelInvoke("bgScale");
        }
        rt.localScale = new Vector3(scale_Candy, scale_Candy, scale_Candy);
        rtBg.localScale = new Vector3(scale, scale, scale);
    }*/
        //----------------------------------------------------------------------------------
    void goDown()
    {

        //Debug.Log("scale=" + scale);
        Debug.Log("y=" + y);
       // scale -= 0.005f;
        if(downCnt==8)
        {
            CancelInvoke("goDown");
            //1.5s後印 "回主畫面"
            StartCoroutine(wait(0.05f, 7));
        }
        if(direct==0)
        {
            y -= 0.5;
            if(y<-243)
            {
                direct = 1;
                downCnt++;
            }
        }
        if (direct == 1)
        {
            y += 0.5;
            if (y > -213)
            {
                downCnt++;
                direct = 0;
            }
        }
        rt.localPosition = new Vector3((float)7, (float)y, 0);
        //rt.localScale = new Vector3(scale, scale, scale);
        //rt.localPosition = new Vector3(-26, y, 0);
        //if()
        /*
                if (y <= -350)
                {
                    CancelInvoke("goDown");
                    CancelInvoke("goDown_Forlight");
                    if (varName.winner == 2 || varName.winner == 0)//pleyer win or tie 有獎品
                        heart.SetActive(true);
                    else
                        cry.SetActive(true);

                    //

                    //1.5s後印 "回主畫面"
                    StartCoroutine(wait(1.5f, 7));

                }*/

    }
    void countdown()
    {
        Debug.Log("start cd" + time_int);
        time_int -= 1;
        if (time_int <= 5)
        {
            countNum.text = time_int + "";
        }
        if (time_int == 0)
        {
            Debug.Log("==" + time_int);
            countNum.text = "";
            CancelInvoke("countdown");

            //倒數完截圖
            CapImg();

            //截完圖再告訴Server
            varName.cnt_end = true;//告訴openpose可掃描
        }
    }
    //截圖------------------------------------------------------------
    void CapImg()
    {
        string path = "D:/screenshot" + "/Shot.png";
        Debug.Log("Path : " + Application.persistentDataPath);
        Debug.Log("Path2 : " + Application.streamingAssetsPath);
        Debug.Log("123");
        RenderTexture rt = new RenderTexture((int)(Screen.width), (int)(Screen.height), 0);
        Camera cam = Camera.main;
        m_camera.targetTexture = rt;
        m_camera.Render();
        m_camera.targetTexture = null;

        RenderTexture.active = rt;

        Texture2D t = new Texture2D((int)(Screen.width), (int)(Screen.height), TextureFormat.RGB24, false);
        t.ReadPixels(new Rect(0, 0, (int)(Screen.width), (int)(Screen.height)), 0, 0);
        t.Apply(false, false);


        m_camera.targetTexture = null;

        byte[] bytes = t.EncodeToPNG();
        File.WriteAllBytes(path, bytes);

    }

}
/*
public Text Gift;
private float timer;
int gamName_cnt = 0;
//img
public GameObject img;
public GameObject trash_img;
public GameObject phone_img;
public GameObject candy2_img;
public GameObject bg2_img;
public GameObject lose_img;
public GameObject win_img; 
public GameObject heart;
public GameObject cry;
public GameObject light;

// public GameObject fire;
public Animator UnityChanControl = null;
//content
public Text content;
private string words; 
private float timer2 = 0.01f;
private bool isPrint = false;
private float perCharSpeed = 0.5f;
private float timer3 = 0.01f;
private bool isPrint2 = false;
private float perCharSpeed2 = 0.5f;

//示範
int lineCount = 0;
int lineCount2 = 0;
public Text PSS_name ;
public Text PSS_detail ;
StreamReader sr;
StreamReader sr2;

//count donw
public Text CountD; 
int time_int = 4;

//pose
public Text pPose;
public Text mPose;
public Text Winner;
//PRIZE
//public Text prizeText;
RectTransform rt;
RectTransform rt_light;
float scale = 1.6f;
int y = 174;
public Text backto;
float z = 0;


void Start()
{
    //content
    words = "利用身體動作玩剪刀石頭布\n" +
        "待會由unity chan示範動作\n"
        + "請在計時結束前擺好姿勢!\n"
        + "勝利可以獲得周邊";
    isPrint = true;
    timer = 0.0f;
    // Gift.text.SetActive(false);
    Gift.text = "";

    rt = candy2_img.GetComponent<RectTransform>();
    rt_light = light.GetComponent<RectTransform>();
    img.SetActive(false);

    trash_img.SetActive(false);
    phone_img.SetActive(false);
    candy2_img.SetActive(false);
    bg2_img.SetActive(false);
    lose_img.SetActive(false);
    win_img.SetActive(false);
    heart.SetActive(false);
    cry.SetActive(false);
    light.SetActive(false);
    //fire.SetActive(false);

    backto.text = "";
}

// Update is called once per frame

void Update()
{
    ///"肢體剪刀時頭部"
    if (varName.gameName_show)
    {

        timer += Time.deltaTime * 2;
        gamName_cnt++;
        if (timer % 2 > 1.0f)

        {

            NameText.text = "";

        }

        else
        {

            //Debug.Log(timer);
            NameText.text = "肢體\n<color=#FF7167>剪刀</color><color=#19B3B1>石頭</color><color=#FFD73D>布</color>";
            if(timer > 5.5)
            {
                NameText.text = "";
                varName.gameName_show = false;
                varName.introTitle_show = true;
            }
        }
    }
    //"遊戲說明"
    if(varName.introTitle_show)
    {

        Intro_titleText.text = "遊戲說明";
        StartCoroutine(wait(1.5f,1));

    }
    //遊戲說明內容
    if(varName.introContent_show)
    {
        printText(words);
    }

    //切換到示範
    if(varName.pss_detail_show && varName.pss_name_show)
    {
        Intro_titleText.text = "動作示範";
        //Debug.Log("動作示範");
        StartCoroutine(Display1());
        StartCoroutine(Display2());
    }

    //model展示指定動作 (收到openpose給的答案)
    if(varName.model_start_animation)
    {
        varName.model_start_animation = false;

        Debug.Log("model_start_animation");
        UnityChanControl.SetBool("start", true);
        //做剪刀
        if (varName.modelPose == "剪刀 Scissor")
        {
            UnityChanControl.SetBool("scissor", true);
            UnityChanControl.SetBool("stone", false);
            UnityChanControl.SetBool("paper", false);
            //StartCoroutine(win_lose(3.5f, 1));


        }//做石頭
        else if (varName.modelPose == "石頭 Stone")
        {
            UnityChanControl.SetBool("scissor", false);
            UnityChanControl.SetBool("stone", true);
            UnityChanControl.SetBool("paper", false);
           // StartCoroutine(win_lose(3.5f, 2));
        }//做布
        else if (varName.modelPose == "布 Paper")
        {
            UnityChanControl.SetBool("scissor", false);
            UnityChanControl.SetBool("stone", false);
            UnityChanControl.SetBool("paper", true);
           // StartCoroutine(win_lose(3.5f, 3));

        }
        //等1.5秒在收動作
        StartCoroutine(wait(3.0f, 4));
        //等1.5秒在print中文pose

    }



}



IEnumerator wait(float waitTime,int type)
{
   // Debug.Log("wait....");
    yield return new WaitForSeconds(waitTime);
   // Debug.Log("wait....over");
    if(type==1)//
    {
        //Intro_titleText.text = "";
        //varName.introTitle_show = false;
        varName.introContent_show = true;
    }
    if(type==2)// 切換示範
    {
        Intro_titleText.text = "";
        NameText.text = "";
        content.text = "";
        varName.introTitle_show = false;
        varName.introContent_show = false;
        Gift.text = "";
        img.SetActive(false);

        //啟動
        //PSS_detail.text = "剪刀";
        varName.pss_detail_show = true;
        varName.pss_name_show = true;

    }
    if(type==4) //
    {
        Debug.Log("type4");

        pPose.text = varName.playerPose;
        mPose.text = varName.modelPose;

        //收Pose
        if (varName.modelPose == "剪刀 Scissor")
            UnityChanControl.SetBool("ScOver", true);

        else if (varName.modelPose == "石頭 Stone")
            UnityChanControl.SetBool("StOver", true);

        else if (varName.modelPose == "布 Paper")
            UnityChanControl.SetBool("paOver", true);


        //在等1.5s 印中文結果
        StartCoroutine(wait(1.5f, 5));
    }
    if(type==5)
    {
        Debug.Log("type5");
        if (varName.winner == 1)//model win
        {
            Winner.text = "YOU LOSE";
            UnityChanControl.SetBool("Win", true);
        }
        else if (varName.winner == 0)//model win
        {
            Winner.text = "TIE";
            UnityChanControl.SetBool("Win", true);
        }
        else if (varName.winner == 2)//player win
        {
            Winner.text = "YOU WIN";
            UnityChanControl.SetBool("Lose", true);
        }
        ////在等2s 進入頒發獎品
        StartCoroutine(wait(3.0f, 6));


    }
    if(type == 6)// 進入頒發獎品
    {
        varName.getPrize = varName.winner;
        ceremony();
    }
    if(type==7)
    {
        backto.text = "回主畫面中...";
        varName.game1Over = true;
        //3s後回UI
        StartCoroutine(wait(3.0f,8));
    }
    if(type==8)//just stop for candy go down
    {

        //規0
        varName.came = false;
        varName.gameName_show = true;
        varName.pss_name = 1;
        varName.introTitle_show = false;
        varName.introContent_show = false;
        varName.gift_show = false;
        varName.pss_name_show = false;
        varName.pss_detail_show = false;
        varName.cnt_start = false;
        varName.cnt_end = false;
        varName.modelPose = "";
        varName.playerPose = "";
        varName.model_start_animation = false;
        varName.winner = -1;
        varName.getPrize = -1;
        varName.game1Over = false;



        varName.mode = 0;
    }

}



// //頒發獎品-----------------------------------------------------------------------
void ceremony ()
{
    varName.came = true;
    bg2_img.SetActive(true);
    light.SetActive(true);
    //fire.SetActive(true);
    if (varName.winner == 2 || varName.winner == 0)//pleyer win or tie 有獎品
    {
        win_img.SetActive(true);
        phone_img.SetActive(true);
        candy2_img.SetActive(true);


        InvokeRepeating("goDown", 1, 0.008f);
        InvokeRepeating("goDown_Forlight", 1, 0.005f);


    }
    else if (varName.winner == 1)//model win 無獎品
    {

        lose_img.SetActive(true);
        trash_img.SetActive(true);
        candy2_img.SetActive(true);
        InvokeRepeating("goDown", 1, 0.008f);
        InvokeRepeating("goDown_Forlight", 1, 0.005f);

    }

}
//----------------------------------------------------------------------------------
void goDown()
{

    Debug.Log("scale=" + scale); 
    Debug.Log("y=" + y);
    scale -= 0.005f;
    y -= 3;
    rt.localScale = new Vector3(scale, scale, scale);
    rt.localPosition = new Vector3(-26, y, 0);

    if (y <= -350)
    {
        CancelInvoke("goDown");
        CancelInvoke("goDown_Forlight");
        if (varName.winner == 2 || varName.winner == 0)//pleyer win or tie 有獎品
            heart.SetActive(true);
        else
            cry.SetActive(true);

        //

        //1.5s後印 "回主畫面"
        StartCoroutine(wait(1.5f, 7));

    }

}
void goDown_Forlight()
{
    z += 0.7f;
    rt_light.localRotation = Quaternion.Euler(0, 0, z);
}

//一行一行-----------------------------------------------------------------------
IEnumerator Display1()
{
    varName.pss_name_show = false;
       sr = new StreamReader(Application.dataPath + "/name.txt");
    //建立一個流，用于讀取行數
    StreamReader srLine = new StreamReader(Application.dataPath + "/name.txt");
    //循環來讀取行數，直到為null停止
    while (srLine.ReadLine() != null)
    {
        lineCount++;
    }
    //關閉並釋放流
    srLine.Close();
    srLine.Dispose();
    //Game name show over
    for (int i = 0; i < lineCount; i++)
    {
        string tempText = sr.ReadLine();
        PSS_name.text = tempText.Split('$')[0];
       // Debug.Log(PSS_name.text);
        //也就是
        float tempTime;
        //將文中的那個$3中的3讀取出來
        if (float.TryParse(tempText.Split('$')[1], out tempTime))
        {
            //協程等待
            yield return new WaitForSeconds(tempTime);
        }
        if (i == 0)
        {
            Debug.Log("觸發剪刀");
            UnityChanControl.SetBool("showSc", true);
        }
        if (i == 2)
        {
            Debug.Log("觸發石頭");
            UnityChanControl.SetBool("showSt", true);
        }
        if (i == 4)
        {
            Debug.Log("觸發布");
            UnityChanControl.SetBool("showPa", true);
        }
        if (i == 6)
        {
            Debug.Log("觸發布");
            UnityChanControl.SetBool("back", true);
        }
        if (i == 16)
        {
            Intro_titleText.text = " ";
            varName.cnt_start = true;
            InvokeRepeating("countdown", 1, 1);
        }
    }


    //關閉並釋放流
    sr.Close();
    sr.Dispose();
}

IEnumerator Display2()
{
  sr2 = new StreamReader(Application.dataPath + "/detail.txt");
  //建立一個流，用于讀取行數
  StreamReader srLine2 = new StreamReader(Application.dataPath + "/detail.txt");
  //循環來讀取行數，直到為null停止
  while (srLine2.ReadLine() != null)
  {
      lineCount2++;
  }
  //關閉並釋放流
  srLine2.Close();
  srLine2.Dispose();
  //Game name show over
  for (int i = 0; i < lineCount2; i++)
  {
      string tempText = sr2.ReadLine();
      PSS_detail.text = tempText.Split('$')[0];
     // Debug.Log(PSS_detail.text);
      //也就是
      float tempTime;
      //將文中的那個$3中的3讀取出來
      if (float.TryParse(tempText.Split('$')[1], out tempTime))
      {
          //協程等待
          yield return new WaitForSeconds(tempTime);
      }


  }


  //關閉並釋放流
  sr2.Close();
  sr2.Dispose();
}
//-----------------------------------------------------------------------


//逐字--------------------------------------------------------------------------
void printText(string words)
{
    try
    {
        if (isPrint)
        {

            content.text = words.Substring(0, (int)(perCharSpeed * timer2));//截取
           // Debug.Log(words.Substring(0, (int)(perCharSpeed * timer2)));
            timer2 += Time.deltaTime * 30;

        }
    }
    catch (System.Exception)
    {
        printEnd();
    }
}

void printEnd()
{
    Gift.text = "小禮物！";
    img.SetActive(true);
    isPrint = false;


    //格n秒 切換示範
    StartCoroutine(wait(3.0f, 2));



}
//----------------------------------------------------------------------------------------

//倒數--------------------------------------------------------------------------------------
void countdown()
{


    //Debug.Log("start cd" + time_int);
    time_int -= 1;
    if (time_int <= 5)
    {
        CountD.text = time_int + "";
    }

    if (time_int == 0)
    {

        CountD.text = "";
        CancelInvoke("countdown");
        varName.cnt_end = true;//告訴openpose可掃描
    }




}
//----------------------------------------------------------------------------------------
}
*/