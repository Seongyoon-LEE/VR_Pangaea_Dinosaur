using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiganotosaurusFOV : DinoFOV
{
    private Vector3 initPos = Vector3.zero;
    public float minSqrDist = 0f;
    protected override void Start()
    {
        isRandom = true;
        base.Start();
    }

    protected override void OnPlayerRecognized()
    {
        base.OnPlayerRecognized();

        // 처음 인식되었을 때 초기 위치값 저장
        if (initPos == Vector3.zero)
        {
            initPos = playerTr.position;
            return;
        }

        // 이후에 위치값으로 거리를 계산해서 움직인 판정
        float distSqr = (initPos - playerTr.position).sqrMagnitude;
        // 일정 거리 이동하면 사망처리
        if (distSqr >= minSqrDist)
        {
            // 사망처리
            print("사망");
        }
    }
    protected override void OnPlayerLost()
    {
        base.OnPlayerLost();
        initPos = Vector3.zero;
    }
}
