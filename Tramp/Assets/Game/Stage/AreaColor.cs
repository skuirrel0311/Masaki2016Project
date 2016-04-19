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
            if (g.gameObject.name != "scaffold") continue;

            g.gameObject.GetComponent<Renderer>().material.color = areaColor;
        }
    }
}
