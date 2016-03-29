using UnityEngine;
using System.Collections;

/// <summary>
/// 敵AI(巡回)
/// </summary>
public class StateWander : State<Enemy>
{
    public StateWander(Enemy owner)
        : base(owner)
    {

    }

    public override void Enter()
    {
        owner.navMeshAgent.destination = GetRandomPosition();
    }

    public override void Execute()
    {
        //目的地に近づいたか？
        if (IsNearThePosition(owner.navMeshAgent.destination, 5))
        {
            //目的地を再設定
            owner.navMeshAgent.destination = GetRandomPosition();
        }
        //プレイヤーに近づいたか？
        if (IsNearThePosition(owner.player.transform.position, 10))
        {
            //追跡する
            owner.ChangeState(EnemyState.Pursuit);
        }
    }

    public override void Exit()
    {
    }

    private Vector3 GetRandomPosition()
    {
        Vector2 fieldSize = new Vector2(30,75);
        return new Vector3(Random.Range(-fieldSize.x,fieldSize.x),0,Random.Range(-fieldSize.y,fieldSize.y));
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
