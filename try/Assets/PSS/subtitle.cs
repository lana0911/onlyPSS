using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class subtitle : MonoBehaviour
{
    //用來顯示字幕的TextUi
    public Text Titles;
    //檔案流,用于讀取文本
    StreamReader sr;
    //文本中的字幕的行數
    int lineCount=0;


    void Start()
    {
        StartCoroutine(Display());
    }

    IEnumerator Display()
    {
        sr= new StreamReader(Application.dataPath + "/text.txt");
        //建立一個流，用于讀取行數
        StreamReader srLine = new StreamReader(Application.dataPath + "/text.txt");
        //循環來讀取行數，直到為null停止
        while(srLine.ReadLine()!=null)
        {
            lineCount++;
        }
        //關閉並釋放流
        srLine.Close();
        srLine.Dispose();
        for (int i = 0; i < lineCount; i++)
        {
            string tempText = sr.ReadLine();
            Titles.text = tempText.Split('$')[0];
            Debug.Log(Titles.text);
            //也就是
            float tempTime = 0.5f;
            //將文中的那個$3中的3讀取出來
            if(float.TryParse(tempText.Split('$')[1], out tempTime))
            {
                //協程等待
                yield return new WaitForSeconds(1);
            }
        }

        //關閉並釋放流
        sr.Close();
        sr.Dispose();
    }
}