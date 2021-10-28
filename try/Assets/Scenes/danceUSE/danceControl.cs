using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class danceControl: MonoBehaviour
{ 
    public VideoPlayer vPlayer; //¼v¤ù
    void Start()
    {
        vPlayer = GetComponent<VideoPlayer>();
        vPlayer.playOnAwake = false;
    }
    void Update()
    {
        if (Gobal_TCP.handup)
        {
            Debug.Log("play!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

            //GetComponent<AudioSource>().Play();
            vPlayer.Play();
        }

        if (Input.GetKeyDown("space"))
        {
            print("space key was pressed");
        }
    }

}
