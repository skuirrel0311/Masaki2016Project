using UnityEngine;
using System.Collections.Generic;
public enum CutInState
{
    Non, In, Stop, Out
}

public class CutInEffect : MonoBehaviour
{


    [System.Serializable]
    public class CutInElement
    {
        public RectTransform CutInImage;
        public float InTime;
        public float StopTime;
        public float outTime;
        [HideInInspector]
        public bool isPlay;
        [HideInInspector]
        public float timer;
        [HideInInspector]
        public Vector2 startRectPosition;
        public CutInState cutInState;
        public void CutInRun()
        {
            if (isPlay)
            {
                //中央左
                float Center = startRectPosition.x * 0.2f;
                if (cutInState == CutInState.In)
                {
                    timer += Time.deltaTime;
                    float nextPosition = startRectPosition.x + (timer / InTime) * (Center - startRectPosition.x);
                    CutInImage.anchoredPosition = new Vector2(nextPosition, CutInImage.anchoredPosition.y);
                    if (timer > InTime)
                    {
                        cutInState = CutInState.Stop;
                    }
                }
                else if (cutInState == CutInState.Stop)
                {
                    timer += Time.deltaTime;
                    float nextPosition = Center + ((timer - InTime) / StopTime) * (-Center - Center);
                    CutInImage.anchoredPosition = new Vector2(nextPosition, CutInImage.anchoredPosition.y);
                    if (timer > InTime + StopTime)
                    {
                        cutInState = CutInState.Out;
                    }
                }
                else if (cutInState == CutInState.Out)
                {
                    timer += Time.deltaTime;
                    float nextPosition = -Center + (((timer - (InTime + StopTime)) / outTime) * (-startRectPosition.x * 2));
                    CutInImage.anchoredPosition = new Vector2(nextPosition, CutInImage.anchoredPosition.y);
                    if (timer > InTime + StopTime + outTime)
                    {
                        cutInState = CutInState.In;
                        CutInImage.anchoredPosition = startRectPosition;
                        timer = 0;
                        isPlay = false;
                        OnEnd();
                    }
                }
            }
        }

        public delegate void OnEndHandrer();

        public event OnEndHandrer OnEnd =()=>{};
    }

    [SerializeField]
    public List<CutInElement> cutInElement=new List<CutInElement>();

    private List<CutInElement> RunQue = new List<CutInElement>();

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < cutInElement.Count; i++)
        {
            cutInElement[i].startRectPosition = cutInElement[i].CutInImage.anchoredPosition;
            cutInElement[i].isPlay = false;
            cutInElement[i].timer = 0;
            cutInElement[i].cutInState = CutInState.In;
            cutInElement[i].OnEnd += () =>
            {
                RunQue.Remove(cutInElement[i]);
            };
        }

        MainGameManager mainGameManager = GameObject.Find("MainGameManager").GetComponent<MainGameManager>();
        
    }

    public void CutInPlay(RectTransform transform)
    {
        foreach (CutInElement cie in cutInElement)
        {
            if (cie.CutInImage == transform)
            {
                cie.isPlay = true;
                RunQue.Add(cie);
            }
        }
    }

    public void CutInPlay(int Index)
    {

                cutInElement[Index].isPlay = true;
                RunQue.Add(cutInElement[Index]);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (RunQue.Count > 0)
        {
            RunQue[0].CutInRun();
        }
    }
}