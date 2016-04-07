using UnityEngine;
using System.Collections;

public class PlayerCreateAnchor : MonoBehaviour
{

    [SerializeField]
    GameObject InstanceAnchor;

    [SerializeField]
    float UncreateDistance = 3;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {

            if (CheckNearAnchor())
            {
                Instantiate(InstanceAnchor, transform.position + transform.forward*2+Vector3.up, transform.rotation);
            }
        }
    }


    //他のアンカーが近すぎないかチェック
    bool CheckNearAnchor()
    {
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return true;

        float MinimumDistance = 1000000;

        foreach (GameObject obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < MinimumDistance)
            {
                MinimumDistance = distance;
            }
        }

        if (MinimumDistance < UncreateDistance)
        {
            return false;
        }
        return true;
       
    }
}
