﻿using UnityEngine;
using System.Collections;

public class StateWander : State<Enemy>
{
    private Vector3 targetPosition;

    public StateWander(Enemy owner)
        : base(owner)
    {

    }

    public override void Enter()
    {
        targetPosition = GetRandomPosition();
    }

    public override void Execute()
    {
        if (IsNearThePosition(targetPosition, 5))
        {
            //目的地を再設定
            targetPosition = GetRandomPosition();
        }

        // 目標地点の方向を向く
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - owner.transform.position);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime * 360);

        // 前方に進む
        owner.transform.Translate(Vector3.forward * owner.speed * Time.deltaTime);
    }

    public override void Exit()
    {
    }

    private Vector3 GetRandomPosition()
    {
        float fieldSize = 10;
        return new Vector3(Random.Range(-fieldSize,fieldSize),0,Random.Range(-fieldSize,fieldSize));
    }

    private bool IsNearThePosition(Vector3 targetPosition, float distance)
    {
        float distanceToTarget = (owner.transform.position - targetPosition).magnitude;
        if (distanceToTarget > distance) return false;
        return true;
    }
}