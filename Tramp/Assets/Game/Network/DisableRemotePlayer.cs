using UnityEngine;
using UnityEngine.Networking;

public class DisableRemotePlayer : NetworkBehaviour 
{
    [SerializeField]
    Behaviour[] behaviours;

    void Start()
    {
        if (!isLocalPlayer)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = false;
            }
        }
    }

    //アプリケーションが選択されている場合はtrueが来る
    void OnApplicationFocus(bool focusStatus)
    {
        if (isLocalPlayer)
        {
            foreach (var behaviour in behaviours)
            {
                behaviour.enabled = focusStatus;
            }
        }
    }
}
