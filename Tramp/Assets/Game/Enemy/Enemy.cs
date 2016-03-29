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
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        player = GameObject.Find("Player");
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        navMeshAgent.destination = player.transform.position;
    }
}
