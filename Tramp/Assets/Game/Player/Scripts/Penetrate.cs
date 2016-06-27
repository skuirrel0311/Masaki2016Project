﻿using UnityEngine;
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

    [SerializeField]
    private Sprite ClientGage;

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

    public bool isPenetrate;

    [SerializeField]
    AudioClip enableSE;
    [SerializeField]
    AudioClip disableSE;
    AudioSource enableAudioSource;
    AudioSource disableAudioSource;

    Image Outgage;

    [SerializeField]
    Sprite defaultGage;

    [SerializeField]
    Sprite UseSprite;

    // Use this for initialization
    void Start()
    {
        PenetrateGage = GameObject.Find("GunEnergy").GetComponent<Image>();
        Outgage = GameObject.Find("GunMeter").GetComponent<Image>();

        if (!isServer)
        {
            PenetrateGage.sprite = ClientGage;
        }

        isPenetrate = false;
        energy = MaxEnergy;
        PenetrateEffect.SetActive(false);
        enableAudioSource = GetComponent<PlayerSound>().EnableAudioSource;
        disableAudioSource = GetComponent<PlayerSound>().DisableAudioSource;

        enableAudioSource.clip = enableSE;
        disableAudioSource.clip = disableSE;

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
        if (GamepadInput.GamePadInput.GetButtonDown(GamepadInput.GamePadInput.Button.RightShoulder, GamepadInput.GamePadInput.Index.One) && 0 < energy)
        {
            isPenetrate = !isPenetrate;
            //開始
            if (isPenetrate)
            {
                //renderer.material = penetrateMaterial;
                InitializeParticle();
                PenetrateEffect.SetActive(true);
                enableAudioSource.Play();
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

                Outgage.sprite = UseSprite;
            }
            //終了
            else
            {
                StopFlowRender();
            }
        }

        if (isPenetrate)
        {
            energy -= reduce;

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
        disableAudioSource.Play();
        Outgage.sprite = defaultGage;
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
            if (isPenetrate) yield break;
            float progress = 1 - (time / 1);
            ring.startLifetime = 1.5f * progress * progress;
            time += Time.deltaTime;
            yield return null;
        }
        if (isPenetrate) yield break;
        InitializeParticle();
    }

    void InitializeParticle()
    {
        ring.startLifetime = 1.5f;
        lightParticle.SetActive(true);
        PenetrateEffect.SetActive(false);
    }
}
