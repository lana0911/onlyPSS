using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
public class text_two : MonoBehaviour
{
    Text words;
    float leftX;
    public float speed = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        leftX = 172 * -1;
        words = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Translate(-10 * speed, 0, 0);
        float nowX = this.gameObject.transform.localPosition.x;
        if (nowX < leftX)
        {
            this.gameObject.transform.Translate(1600, 0, 0);
        }

        words.text = Gobal_TCP.text2;
    }
}