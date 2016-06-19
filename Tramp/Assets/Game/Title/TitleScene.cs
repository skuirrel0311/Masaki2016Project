using UnityEngine;
using System.Collections.Generic;

public class TitleScene : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Sprites;

    //シーンの選択
    [SerializeField]
    public TitleState SceneState;

    List<SpriteSlide> slides;

    float slideTimeSpan = 0.1f;
    float timer = 0;
    bool isIn;
    bool isout;
    public bool isena=true;

    void Awake()
    {
        isena = false;
        isIn = false;
        isout = false;
        slides = new List<SpriteSlide>();
        timer = 0;
        //子のオブジェクトにスライドイン用のスクリプトをアタッチ
        foreach (GameObject go in Sprites)
        {
            SpriteSlide spriteslide = go.AddComponent<SpriteSlide>();
            slides.Add(spriteslide);
        }
    }

    //移動済みの状態にする
    public void ZeroPosition()
    {
        foreach(GameObject go in Sprites)
        {
            go.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, go.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    void Update()
    {

        if (slides.Count <= 0) return;
        //スライドアウトの処理
        if (isout)
        {
            timer += Time.deltaTime;
            int s =Mathf.Max(Mathf.Min(slides.Count-1,(int)(timer / slideTimeSpan)),0);

            slides[s].SlideOut();
            if (slides.Count == s + 1)
            {
                isout = false;
                timer = 0;
            }
        }
        //スライドインの処理
        else if (isIn)
        {
            timer += Time.deltaTime;
            int s = Mathf.Max(Mathf.Min(slides.Count - 1, (int)(timer / slideTimeSpan)), 0);
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
        if (isena) return;
        isena = true;
        timer = 0;
        isIn = true;
    }

    public void StartSlideOut()
    {
        if (isIn || isout) return;
        if (isena) return;
        timer = 0;
        isena = true;
        isout = true;
    }

    void OnDisabled()
    {
        isIn = false;
        isout = false;
    }
}
