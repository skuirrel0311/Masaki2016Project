using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FixAnchorHit : MonoBehaviour
{

    [SerializeField]
    GameObject HitEffect;

    List<GameObject> connectionFlows;

    [SerializeField]
    AudioClip hitSE;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        connectionFlows = new List<GameObject>();
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    }

    public void ConnectionFlow(GameObject flow)
    {
        connectionFlows.Add(flow);
    }

    void OnTriggerEnter(Collider col)
    {
        Debug.Log("Call Fix Anchor OnTrigger");
        if (col.tag == "Ammo")
        {
            audioSource.PlayOneShot(hitSE);
            DestroyConnctions();
            Destroy(col.gameObject);
            Instantiate(HitEffect,transform.position,transform.rotation);
        }
    }

    void DestroyConnctions()
    {
        foreach (GameObject flow in connectionFlows)
        {
            if (flow != null)
                Destroy(flow);
        }

        connectionFlows.Clear();
    }
}
