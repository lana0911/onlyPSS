using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class change : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (varName.mode == 0)
        {
            SceneManager.LoadScene(0);
            Debug.Log("switch Sence 0 (UI)");
            varName.mode = -1;
        }
        else if (varName.mode == 1)
        {
            SceneManager.LoadScene(1);
            Debug.Log("switch Sence 1 (PSS Game)");
            varName.mode = -1;
        }
        else if (varName.mode == 2)
        {
            SceneManager.LoadScene(2);
            Debug.Log("switch Sence 3 (Dance Game)");
            varName.mode = -1;
        }

    }
}
