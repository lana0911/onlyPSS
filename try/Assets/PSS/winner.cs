using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class winner : MonoBehaviour
{
    Text pose;
    // Start is called before the first frame update
    void Start()
    {
        pose = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //可以顯示winner + pose
        if (Gobal_TCP.show_pose_text && Gobal_TCP.timeup == true)
        {
            if (Gobal_TCP.PSS_winer == 0)
                pose.text = "TIE 平手";
            else if (Gobal_TCP.PSS_winer == 1)
                pose.text = "YOU LOSE";
            else if (Gobal_TCP.PSS_winer == 2)
                pose.text = "YOU WIN";
        }
        else
        {
            pose.text = " ";
        }
    }
}
