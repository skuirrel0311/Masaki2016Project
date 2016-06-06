using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
        }
    }
}
