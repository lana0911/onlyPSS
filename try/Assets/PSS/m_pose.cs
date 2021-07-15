using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class m_pose : MonoBehaviour
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
        if (Gobal_TCP.show_pose_text && Gobal_TCP.timeup ==true)
        {
            if (Gobal_TCP.model_pose == "1")
                pose.text = "剪刀Sicssor";
            else if (Gobal_TCP.model_pose == "2")
                pose.text = "石頭Stone";
            else if (Gobal_TCP.model_pose == "3")
                pose.text = "布Paper";
        }
        else
        {
            pose.text = " ";
        }
    }
}
