using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Penetrate : NetworkBehaviour
{
    [SerializeField]
    private float reduce = 0.3f;

    [SerializeField]
    private float MaxEnergy = 100;

    [SerializeField]
    private GameObject PenetrateEffect;

    private ParticleSystem ring;
    private GameObject lightParticle;

    [SerializeField]
    Material penetrateMaterial;

    Renderer renderer;
    Material defaultMaterial;

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

    private bool isPenetrate;

    [SerializeField]
    AudioClip enableSE;
    [SerializeField]
    AudioClip disableSE;
    AudioSource audioSource;
    // Use this for initialization
    void Start()
    {
        PenetrateGage = GameObject.Find("GunEnergy").GetComponent<Image>();
        isPenetrate = false;
        energy = MaxEnergy;
        PenetrateEffect.SetActive(false);
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        ring = PenetrateEffect.transform.FindChild("ring").GetComponent<ParticleSystem>();
        lightParticle = PenetrateEffect.transform.FindChild("light").gameObject;

        //renderer = GetComponentInChildren<Renderer>();
        //defaultMaterial = renderer.material;
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
                //renderer.material = penetrateMaterial;
                PenetrateEffect.SetActive(true);
                audioSource.PlayOneShot(enableSE);
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
            energy-=reduce;

            if (energy <= 0)
            {
                energy = 0;
                isPenetrate = false;
                StopFlowRender();
            }
        }
    }

    void StopFlowRender()
    {
        //renderer.material = defaultMaterial;
        audioSource.PlayOneShot(disableSE);
        StartCoroutine("ReversePlayParticle");
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

    IEnumerator ReversePlayParticle()
    {
        lightParticle.SetActive(false);
        float time = 0;
        while (time < 1)
        {
            float progress = 1 - (time / 1);
            ring.startLifetime = 1.5f * progress * progress;
            time += Time.deltaTime;
            yield return null;
        }
        ring.startLifetime = 1.5f;
        lightParticle.SetActive(true);
        PenetrateEffect.SetActive(false);
    }
}
