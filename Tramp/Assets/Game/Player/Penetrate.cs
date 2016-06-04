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

    private float energy = 0;
    private float MaxEnergy = 100;

    // Use this for initialization
    void Start()
    {
        PenetrateGage = GameObject.Find("GunEnergy").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;
        PenetrateGage.fillAmount = energy / MaxEnergy;
        if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.RightShoulder, GamepadInput.GamePadInput.Index.One)&&0<energy)
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
        }
        else if (GamepadInput.GamePadInput.GetButton(GamepadInput.GamePadInput.Button.RightShoulder, GamepadInput.GamePadInput.Index.One))
        {
            energy--;
            if (energy <= 0)
            {
                energy = 0;
                StopFlowRender();
            }
        }
        else if (GamepadInput.GamePadInput.GetButtonUp(GamepadInput.GamePadInput.Button.RightShoulder, GamepadInput.GamePadInput.Index.One))
        {
            StopFlowRender();
        }
    }

    void StopFlowRender()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Flow");
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
