using UnityEngine;
using System.Collections;

/// <summary>
/// 照準の線の描画Nozzleにアタッチする
/// </summary>
public class AlignmentLine : MonoBehaviour {

    LineRenderer lineRenderer;

	// Use this for initialization
	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        RaycastHit hit;
        lineRenderer.SetPosition(0,transform.position);

        //直線上にオブジェクトがあればそこまでのラインを描画させる
        if (Physics.Raycast(transform.position,transform.forward, out hit))
        {
            Transform objectHit = hit.transform;
            lineRenderer.SetPosition(1,hit.point);
        }
        else
        {
            lineRenderer.SetPosition(1,transform.position+(transform.forward*100));
        }
    }
}
