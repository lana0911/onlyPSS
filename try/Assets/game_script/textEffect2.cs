using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI; //使用Unity UI程式庫。 (Text是UI的一部份哦! 要使用就必須要加，不然會出現錯誤!)

public class textEffect2 : MonoBehaviour
{

    public Text UIText;

    private float timer;

    // Use this for initialization

    void Start()
    {

        timer = 0.0f;

    }

    // Update is called once per frame

    void Update()
    {

        timer += Time.deltaTime * 2;

        if (timer % 2 > 1.0f)

        {

            UIText.text = "";

        }

        else
        {
            if (Gobal_TCP.pss_name == 1)
            { 

                    UIText.text = "<color=#FF7167>剪刀</color><color=#19B3B1>石頭</color><color=#FFD73D>布</color>";
            }
        

        }

    }

}