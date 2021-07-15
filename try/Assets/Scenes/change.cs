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
        if (Gobal_TCP.game_mode == 0)
        {
            Gobal_TCP.game_mode = 5;
            SceneManager.LoadScene(0);
            Debug.Log("switch Sence 0 (UI)");
        }
        else if (Gobal_TCP.game_mode == 1)
        {
            Gobal_TCP.game_mode = 5;
            SceneManager.LoadScene(1);
            Debug.Log("switch Sence 1 (PSS Game)");
        }
        else if (Gobal_TCP.game_mode == 2)
        {
            Gobal_TCP.game_mode = 5;
            SceneManager.LoadScene(2);
            Debug.Log("switch Sence 2 (Dance Game)");
        }

    }
}
