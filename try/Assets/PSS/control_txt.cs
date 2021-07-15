using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class control_txt : MonoBehaviour
{
    // Start is called before the first frame update
    float gameNameTime = 4.0f;
    void Start()
    {
        //等3秒後再關掉 "剪刀石頭布"
        StartCoroutine(showGameName(gameNameTime)); //執行 IEnumerator
    }


    IEnumerator showGameName(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Gobal_TCP.pss_name = 0;

    }

}
