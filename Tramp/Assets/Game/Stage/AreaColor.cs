using UnityEngine;
using System.Collections;

public class AreaColor : MonoBehaviour
{
    [SerializeField]
    Color areaColor;

    void Start()
    {
        foreach(Transform g in transform)
        {
            if (g.gameObject.tag != "Scaffold" && g.gameObject.tag != "Box") continue;

            g.gameObject.GetComponent<Renderer>().material.color = areaColor;

            if(g.gameObject.tag == "Scaffold")
            {
                g.gameObject.GetComponent<Renderer>().material.color = new Color(areaColor.r, areaColor.g, areaColor.b, 0.7f);
            }
        }
    }
}
