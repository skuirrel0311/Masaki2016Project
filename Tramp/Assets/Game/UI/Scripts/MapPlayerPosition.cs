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
    }

    public void SetHostPlayer(GameObject go)
    {
        HostPlayerObject = go;
    }

    public void SetClientPlayer(GameObject go)
    {
        ClientPlayerObject = go;
    }

    // Update is called once per frame
    void Update()
    {
        if (HostPlayerObject == null || ClientPlayerObject == null) return;
        HostPlayerSprite.anchoredPosition = new Vector2(HostPlayerObject.transform.position.x, HostPlayerObject.transform.position.z);
        ClientPlayerSprite.anchoredPosition = new Vector2(ClientPlayerObject.transform.position.x, ClientPlayerObject.transform.position.z);

        HostPlayerSprite.localRotation = Quaternion.Euler(0,0,HostPlayerObject.transform.localRotation.y);
        ClientPlayerSprite.localRotation = Quaternion.Euler(0, 0, ClientPlayerObject.transform.localRotation.y);


    }
}
