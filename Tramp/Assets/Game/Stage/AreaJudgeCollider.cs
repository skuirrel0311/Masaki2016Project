using UnityEngine;
using System.Collections;

public class AreaJudgeCollider : MonoBehaviour
{
    AppealAreaState state;
    
    void Start()
    {
        state = gameObject.GetComponentInParent<AppealAreaState>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Player") return;

        state.RidePlayers.Add(col.gameObject);
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;

        state.RidePlayers.Remove(col.gameObject);
    }
}
