using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Penetrate : NetworkBehaviour
{
    Image PenetrateGage;

    public float Energy
    {
        get
        {
            return energy;
        }
        set
        {
            energy = Mathf.Max(Mathf.Min(MaxEnergy, value), 0);
        }
    }

   [SerializeField]
   private float reduce=0.3f;

    private float energy = 0;
    [SerializeField]
    private float MaxEnergy = 100;
    private bool isPenetrate;
    // Use this for initialization
    void Start()
    {
        PenetrateGage = GameObject.Find("GunEnergy").GetComponent<Image>();
        isPenetrate = false;
        energy = MaxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        PenetrateGage.fillAmount = energy / MaxEnergy;
        if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.RightShoulder, GamepadInput.GamePadInput.Index.One)&&0<energy)
        {
            isPenetrate = !isPenetrate;
            if (isPenetrate)
            {
                GameObject[] gos = GameObject.FindGameObjectsWithTag("Flow");
                foreach (GameObject go in gos)
                {
                    if (!go.GetComponent<Flow>().isCreatePlayer)
                    {
                        go.GetComponent<Renderer>().enabled = true;
                        go.GetComponent<LineRenderer>().enabled = true;
                    }
                }

                GetComponent<PlayerControl>().hitFix = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Rigidbody>().useGravity = true;
            }
            else
            {
                StopFlowRender();
            }

        }

        if(isPenetrate)
        {
            energy-=reduce ;
            if (energy <= 0)
            {
                energy = 0;
                StopFlowRender();
            }
        }
    }

    void StopFlowRender()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Flow");
        GetComponent<PlayerControl>().hitFix = false;
        foreach (GameObject go in gos)
        {
            if (!go.GetComponent<Flow>().isCreatePlayer)
            {
                go.GetComponent<Renderer>().enabled = false;
                go.GetComponent<LineRenderer>().enabled = false;
            }
        }
    }
}
