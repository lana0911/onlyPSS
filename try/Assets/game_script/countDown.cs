using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.UI; //使用Unity UI程式庫。

public class countDown : MonoBehaviour
{

    int time_int = 6;
    Text time_UI;
    

    void Start()
    {
        time_UI = GetComponent<Text>();
        InvokeRepeating("timer", 1, 1);

    }

    void timer()
    {
        if (Gobal_TCP.countdown)
        {
            time_int -= 1;
            if (time_int <= 5)
            {
                time_UI.text = time_int + "";
            }

            if (time_int == 0)
            {

                time_UI.text = "";
                Gobal_TCP.timeup = true;
                Gobal_TCP.countdown = false;
                CancelInvoke("timer");

            }

        }

    }

}