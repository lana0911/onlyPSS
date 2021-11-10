using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var path = @"D:\example.txt";
        var txt = File.ReadAllText(path);
        Debug.Log(txt);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
