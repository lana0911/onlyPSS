using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class stream_Read : MonoBehaviour
{
    //用來顯示字幕的TextUi
    public Text Titles;
    //檔案流,用于讀取文本
    StreamReader sr;
    //文本中的字幕的行數
    int lineCount = 0;
    bool start_n = true;


    void Start()
    {
        
    }



    void Update()
    {
        if(start_n==true)
        {
            if (Gobal_TCP.pss_name == 0)
            {         
                StartCoroutine(Display());

                start_n = false;
            }

          
        }
 
    }

    IEnumerator Display()
    {
        sr = new StreamReader(Application.dataPath + "/text0.txt");
        //建立一個流，用于讀取行數
        StreamReader srLine = new StreamReader(Application.dataPath + "/text0.txt");
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
                Titles.text = tempText.Split('$')[0];
                Debug.Log(Titles.text);
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
        sr.Close();
        sr.Dispose();
    }
}