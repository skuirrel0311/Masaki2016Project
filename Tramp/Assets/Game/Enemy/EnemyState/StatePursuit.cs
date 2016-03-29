using UnityEngine;
using System.Collections;

/// <summary>
/// 敵AI(追跡)
/// </summary>
public class StatePursuit : State<Enemy>
{
    IEnumerator findPlayer;

    public StatePursuit(Enemy owner)
        :base(owner)
    {

    }

    public override void Enter()
    {
        findPlayer = FindPlayer();
    }

    public override void Execute()
    {
        bool isNear = IsNearThePosition(owner.player.transform.position, 10);

        if (isNear == false)
        {
            //視界にプレイヤーがいない
            owner.ChangeState(EnemyState.Wander);
        }
        else
        {
            findPlayer.MoveNext();
        }
    }

    public override void Exit()
    {
    }

    /// <summary>
    /// プレイヤーの座標を目的地に再設定します
    /// </summary>
    private IEnumerator FindPlayer()
    {
        while (true)
        {
            //目的地をプレイヤーに設定
            owner.navMeshAgent.destination = owner.player.transform.position;
            yield return new WaitForSeconds(2);
        }
    }

    /// <summary>
    /// ターゲットに指定した距離より近い場合はtrueを返します
    /// </summary>
    protected bool IsNearThePosition(Vector3 targetPosition, float distance)
    {
        float distanceToTarget = (owner.transform.position - targetPosition).magnitude;
        if (distanceToTarget > distance) return false;
        return true;
    }

}
