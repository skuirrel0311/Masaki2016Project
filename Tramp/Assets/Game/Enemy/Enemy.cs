using UnityEngine;
using System.Collections;

public enum EnemyState
{
    Wander,     //巡回
    Pursuit,    //追尾
    Attack      //攻撃
}

public class Enemy : EnemyBase<Enemy,EnemyState>
{
    public float speed = 5;
    public GameObject player;
    public NavMeshAgent navMeshAgent;
    
    void Start()
    {
        player = GameObject.Find("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();

        stateManager = new StateManager<Enemy>();

        //ステートのリストにEnemyStateと同じ順番で入れる
        stateList.Add(new StateWander(this));
        stateList.Add(new StatePursuit(this));
        stateList.Add(new StateAttack(this));

        ChangeState(EnemyState.Wander);
    }

    void Update()
    {
        stateManager.Update();
    }
}
