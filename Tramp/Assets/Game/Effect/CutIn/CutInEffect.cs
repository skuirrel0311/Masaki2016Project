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
                if (cutInState == CutInState.In)
                {
                    timer += Time.deltaTime;
                    float nextPosition = startRectPosition.x + (timer / InTime) * (0 - startRectPosition.x);
                    CutInImage.anchoredPosition = new Vector2(nextPosition, CutInImage.anchoredPosition.y);
                    if (timer > InTime)
                    {
                        cutInState = CutInState.Stop;
                    }
                }
                else if (cutInState == CutInState.Stop)
                {
                    timer += Time.deltaTime;
                    if (timer > InTime + StopTime)
                    {
                        cutInState = CutInState.Out;
                    }
                }
                else if (cutInState == CutInState.Out)
                {
                    timer += Time.deltaTime;
                    float nextPosition = startRectPosition.x + (((timer - (InTime + StopTime)) / outTime) * (0 - startRectPosition.x)) + (0 - startRectPosition.x);
                    CutInImage.anchoredPosition = new Vector2(nextPosition, CutInImage.anchoredPosition.y);
                    if (timer > InTime + StopTime + outTime)
                    {
                        cutInState = CutInState.In;
                        CutInImage.anchoredPosition = startRectPosition;
                        timer = 0;
                        isPlay = false;
                    }
                }
            }
        }
    }

    [SerializeField]
    public List<CutInElement> cutInElement=new List<CutInElement>();

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < cutInElement.Count; i++)
        {
            cutInElement[i].startRectPosition = cutInElement[i].CutInImage.anchoredPosition;
            cutInElement[i].isPlay = false;
            cutInElement[i].timer = 0;
            cutInElement[i].cutInState = CutInState.In;
        }

    }

    public void CutInPlay(RectTransform transform)
    {
        foreach (CutInElement cie in cutInElement)
        {
            if (cie.CutInImage == transform)
            {
                cie.isPlay = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(CutInElement cie in cutInElement)
        {
            cie.CutInRun();
        }
    }
}