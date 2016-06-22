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



        public void CutInRun(List<CutInElement> elements)
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
                        elements.Remove(this);
                    }
                }
            }
        }
    }

    [SerializeField]
    public List<CutInElement> cutInElement = new List<CutInElement>();

    private List<CutInElement> RunQue = new List<CutInElement>();

    GameObject mainGameManager;
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
        if(mainGameManager==null)
        {
            SetMainGameManagerEvent();
        }

        if (RunQue.Count > 0)
        {
            RunQue[0].CutInRun(RunQue);
        }
    }

    void SetMainGameManagerEvent()
    {
        //イベントに処理を設定
        mainGameManager = GameObject.Find("MainGameManager");
        if (mainGameManager == null) return;

        MainGameManager mgm = mainGameManager.GetComponent<MainGameManager>();

        RemainingTime Timer = GameObject.Find("Canvas").GetComponent<RemainingTime>();

        Timer.OnOneMinHandler += () =>
        {
            CutInPlay(0);
        };
        Timer.OnTenSceHandler += () =>
        {
            CutInPlay(1);
        };
        mgm.OnOccupiedingHnadler += () =>
        {
            CutInPlay(3);
        };

        mgm.OnOccupiedHnadler += () =>
        {
            CutInPlay(2);
        };
    }
}