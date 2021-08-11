using UnityEngine;
using System.Collections;

public class camera : MonoBehaviour
{

    WebCamTexture c;

    void Start()
    {

        WebCamDevice[] wcd = WebCamTexture.devices;

        if (wcd.Length == 0)
        {
            print("找不到實體攝影機");
        }
        else
        {
            foreach (WebCamDevice wc in wcd)
            {
                print("找不到實體攝影機" + wc.name);
            }
            print("----------------------------------------------------------------");
            print("目前使用的攝影機是：" + wcd[0].name);
            c = new WebCamTexture(wcd[0].name);
            GetComponent<Renderer>().material.mainTexture = c;
            GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Unlit (Supports Lightmap)");
            transform.localScale = new Vector3((float)c.width / c.height, 1);
            c.Play();
        }
    }
    void Update()
    {



        if(varName.came == true)
        {
            c.Stop();
        }
    }
    void OnDisable()
    {
        if (c != null)
        {
            c.Stop();
        }
    }

}

