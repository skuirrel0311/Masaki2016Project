using UnityEngine;
using System.Collections;
using GamepadInput;

public class CameraControlTest : MonoBehaviour
{
    [SerializeField]
    PlayerControl player;
    [SerializeField]
    float radius = 3;       //球体の半径(ターゲットの位置からの距離)
    [SerializeField]
    float rotationSpeed = 5;

    /// <summary>
    /// 緯度
    /// </summary>
    float latitude = 15;
    /// <summary>
    /// 経度
    /// </summary>
    float longitude = 180;

    float deg2Rad = Mathf.Deg2Rad;

    void Update()
    {
        Vector2 rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, (GamePad.Index)player.playerNum);

        latitude += -rightStick.y * rotationSpeed * Time.deltaTime;
        longitude += rightStick.x * rotationSpeed * Time.deltaTime;

        //経度には制限を掛ける
        latitude = Mathf.Clamp(latitude, -25, 80);

        SphereCameraControl();
    }

    /// <summary>
    /// 球体座標系によるカメラの制御
    /// </summary>
    void SphereCameraControl()
    {
        Vector3 cameraPosition;

        cameraPosition.x = radius * Mathf.Cos(latitude * deg2Rad) * Mathf.Sin(longitude * deg2Rad);
        cameraPosition.y = radius * Mathf.Sin(latitude * deg2Rad);
        cameraPosition.z = radius * Mathf.Cos(latitude * deg2Rad) * Mathf.Cos(longitude * deg2Rad);

        //プレイヤーの足元からY座標に+1した座標をターゲットにする
        Vector3 target = player.transform.position;
        target.y += 1.5f;

        transform.position = cameraPosition + target;
        transform.LookAt(target);
    }
}
