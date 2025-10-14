using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCorridor : MonoBehaviour
{
    //정면에서 플레이어를 보면 추적 상태
    //나랑 목표의 거리부터 체크 : 거리가 맞을 경우, 로컬 기준으로 각도 체크 : 각도까지 맞을 경우, 사이에 장애물이 없는지 체크 
    //시야에 안닿는곳에서 길리슈트를 입으면 추적 해제 / 시야란 위에 적은 기준
    //평시에 배회상태
    private void Start()
    {
        
    }
}
