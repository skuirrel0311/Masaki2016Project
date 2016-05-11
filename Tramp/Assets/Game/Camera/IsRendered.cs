using UnityEngine;
using System.Collections;

public class IsRendered : MonoBehaviour
{
    private bool oldIsRenderd;
    private bool isRendered;
    /// <summary>
    /// 前のフレームでカメラに映っているか？
    /// </summary>
    public bool WasRendered { get { return oldIsRenderd; } }

    void Start()
    {
        isRendered = false;
        oldIsRenderd = false;
    }

    void Update()
    {
        oldIsRenderd = isRendered;
        //このあとにOnWillRenderObjectが呼ばれるのでfalseを入れておく
        isRendered = false;
    }

    //アップデートの後に呼ばれる
    void OnWillRenderObject()
    {
        if (Camera.current.name == "ThirdPersonCamera")
        {
            isRendered = true;
        }
    }
}
