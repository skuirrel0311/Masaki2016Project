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

        col.gameObject.GetComponent<PlayerControl>().hitFix = true;
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag != "Player") return;

        state.RidePlayers.Remove(col.gameObject);
        if (!col.gameObject.GetComponent<Penetrate>().isPenetrate)
            col.gameObject.GetComponent<PlayerControl>().hitFix = false;
    }
}
