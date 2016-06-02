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
                Material mat = g.GetComponent<Renderer>().material;
                mat.SetFloat("_Mode", 2);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                mat.color = new Color(areaColor.r, areaColor.g, areaColor.b, 0.7f);
            }
        }
    }
}
