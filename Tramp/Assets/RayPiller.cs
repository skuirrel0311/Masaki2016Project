using UnityEngine;
using System.Collections;

public class RayPiller : MonoBehaviour
{

    [SerializeField]
    AppealAreaState appealArea=null;

    [SerializeField]
    Color n_Color=Color.white;

    [SerializeField]
    Color t_Color= Color.white;

    [SerializeField]
    Color h_Color= Color.white;

    private Renderer render;

    // Use this for initialization
    void Start()
    {
        render = GetComponent<Renderer>();
        render.materials[0].EnableKeyword("_EMISSION");
        render.materials[0].SetColor("_Color", n_Color);
        render.materials[0].SetColor("_EmissionColor", n_Color);
    }

    // Update is called once per frame
    void Update()
    {
        if (appealArea.isOccupation)
        {
            if(appealArea.isOccupiers)
            {
                render.materials[0].SetColor("_Color", t_Color);
                render.materials[0].SetColor("_EmissionColor", t_Color);
            }
            else
            {
                render.materials[0].SetColor("_Color", h_Color);
                render.materials[0].SetColor("_EmissionColor", h_Color);
            }
        }
        else
        {
            render.materials[0].SetColor("_Color", n_Color);
            render.materials[0].SetColor("_EmissionColor", n_Color);
        }
    }
}
