using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    public float rotationSpeed = 5;
    private float rotationY;

    private Vector3 oldPlayerPosition;
    private GameObject player;
    private GameObject nearAnchor;

    void Start()
    {
        rotationY = 0;
        player = GameObject.Find("Player");
        oldPlayerPosition = player.transform.position;
    }

    void Update()
    {
        PlayerTrace();

        if (Input.GetKeyDown(KeyCode.R))
        {
            nearAnchor = GetNearAnchor();
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (nearAnchor == null) return;
            if (Input.GetKeyDown(KeyCode.D))
            {
                Destroy(nearAnchor);
            }
            Vector3 vec = nearAnchor.transform.position - transform.position;
            transform.rotation = Quaternion.Euler(0,VectorToAngle(vec.x,vec.z), 0);
            return;
        }

        float mouseX = Input.GetAxis("Horizontal2");

        rotationY += mouseX * rotationSpeed;

        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    private float VectorToAngle(float a,float b)
    {
         return Mathf.Atan2(a, b) * 180 / Mathf.PI;
    }

    //todo:CreateFlowとメソッドがかぶっているのでライブラリを作成する
    GameObject GetNearAnchor()
    {
        GameObject gameObject=null;
        //一番近いアンカーを探す
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Anchor");
        if (objects.Length <= 0) return null;

        float distance = 1000000;

        foreach (GameObject obj in objects)
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < distance)
            {
                Vector3 vec = obj.transform.position - transform.position;
                distance = vec.magnitude;
                gameObject = obj;
            }
        }

        return gameObject;
    }

    private void PlayerTrace()
    {
        Vector3 movement = player.transform.position - oldPlayerPosition;

        //プレイヤーが移動していなかったら終了
        if (movement.magnitude == 0) return;

        //プレイヤーについていく
        transform.position += movement;

        oldPlayerPosition = player.transform.position;
    }
}
