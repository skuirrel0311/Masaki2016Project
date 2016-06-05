using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapPlayerPosition : MonoBehaviour
{

    [SerializeField]
    RectTransform HostPlayerSprite;

    [SerializeField]
    RectTransform ClientPlayerSprite;

    GameObject HostPlayerObject;
    GameObject ClientPlayerObject;

    // Use this for initialization
    void Start()
    {
        SetPlayerObjects();
    }

    // Update is called once per frame
    void Update()
    {
        if (HostPlayerObject == null || ClientPlayerObject == null)
        {
            SetPlayerObjects();
        }
        else
        {
            HostPlayerSprite.anchoredPosition = new Vector2(HostPlayerObject.transform.position.x, HostPlayerObject.transform.position.z);
            ClientPlayerSprite.anchoredPosition = new Vector2(ClientPlayerObject.transform.position.x, ClientPlayerObject.transform.position.z);
        }
    }

    void SetPlayerObjects()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        if (gos.Length < 2) return;
        foreach (GameObject go in gos)
        {
            if (go.GetComponent<PlayerControl>().isLocalPlayer)
            {
                HostPlayerObject = go;
            }
            else
            {
                ClientPlayerObject = go;
            }
        }
    }
}
