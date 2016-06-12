using UnityEngine;
using System.Collections;

public class SpriteSlide : MonoBehaviour
{

    Vector2 StartPosition;
    bool isIn;
    bool isout;
    float timer;

    void Start()
    {
        StartPosition = GetComponent<RectTransform>().anchoredPosition;
        timer = 0;
    }

    public void Update()
    {
        if (isout)
        {
            timer += Time.deltaTime * 5;
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(new Vector2(0, StartPosition.y), StartPosition, timer);
            if (timer > 1)
            {
                timer = 0;
                isIn = false;
            }
        }
        else if (isIn)
        {
            timer += Time.deltaTime * 5;
            GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(StartPosition, new Vector2(0, StartPosition.y), timer);
            if (timer > 1)
            {
                timer = 0;
                isIn = false;
            }
        }
    }

    public void SlideIn()
    {
        if (isIn || isout) return;
        isIn = true;
        timer = 0;
    }

    public void SlideOut()
    {
        if (isIn || isout) return;
        isout = true;
        timer = 0;
    }

    public void Disabled()
    {
        GetComponent<RectTransform>().anchoredPosition = StartPosition;
    }
}
