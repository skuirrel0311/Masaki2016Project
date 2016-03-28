using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum EnemyState
{
    Wander,     //巡回
    Pursuit,    //追尾
    Attack,     //攻撃
    Death       //死亡
}

public class Enemy : EnemyBase<Enemy,EnemyState>
{
    public GameObject player;
    public float speed = 5;

    void Start()
    {
        player = GameObject.Find("Player");

        stateList.Add(new StateWander(this));

        stateManager = new StateManager<Enemy>();

        ChangeState(EnemyState.Wander);
    }
}
