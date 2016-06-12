using UnityEngine;
using System.Collections.Generic;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Sprites;

    List<SpriteSlide> slides;

    float slideTimeSpan = 0.1f;
    float timer = 0;
    bool isIn;
    bool isout;
    bool isena = false;

    void Awake()
    {
        isIn = false;
        isout = false;
        slides = new List<SpriteSlide>();

        //子のオブジェクトにスライドイン用のスクリプトをアタッチ
        foreach (GameObject go in Sprites)
        {
            SpriteSlide spriteslide = go.AddComponent<SpriteSlide>();
            slides.Add(spriteslide);
        }
    }
    void Update()
    {
        //スライドアウトの処理
        if (isout)
        {
            timer += Time.deltaTime;
            int s = (int)(timer / slideTimeSpan);
            slides[s].SlideOut();
            if (slides.Count == s + 1)
            {
                isout = false;
            }
        }
        //スライドインの処理
        else if (isIn)
        {
            timer += Time.deltaTime;
            int s = (int)(timer / slideTimeSpan);
            slides[s].SlideIn();
            if (slides.Count == s + 1)
            {
                isIn = false;
            }
        }
    }

    public void StartSlideIn()
    {
        if (isIn || isout) return;
        timer = 0;
        isIn = true;
    }

    public void StartSlideOut()
    {
        if (isIn || isout) return;
        timer = 0;
        isout = true;
    }

    void OnEnable()
    {
        if (isena) return;
        StartSlideIn();
        isena = true;
    }
}
