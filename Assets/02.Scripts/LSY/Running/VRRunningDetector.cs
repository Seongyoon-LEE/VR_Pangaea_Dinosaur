using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRunningDetector : MonoBehaviour
{
    [Header("감지할 컨트롤러")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("달리기 감지 민감도")]
    public float runningSpeedThreshold = 2.5f; // 이 값으로 민감도 조절

    [Range(0f, 1f)]
    public float smoothingFactor = 0.1f; // 부드럽게 하기 위한 계수

    // 현재 플레이어가 달리는중인지 여부 변수
    public bool IsRunning { get; private set; }

    Vector3 prevLeftHandPos;
    Vector3 prevRightHandPos;
    float smoothedHandSpeed;

    void Start()
    {
        // 첫 프레임 값 튐 방지
        prevLeftHandPos = leftHand.position;
        prevRightHandPos = rightHand.position;
    }

    void Update()
    {
        // 각 손의 수직(Y축) 이동 속도 계산
        // (현재 Y위치 - 이전 프레임 Y위치) / 시간 = 속도
        float leftHandSpeedY = (leftHand.position.y - prevLeftHandPos.y) / Time.deltaTime;
        float rightHandSpeedY = (rightHand.position.y - prevRightHandPos.y) / Time.deltaTime;

        // 다음 프레임 계산을 위해 현재 위치를 이전위치로 저장
        prevLeftHandPos = leftHand.position;
        prevRightHandPos = rightHand.position;

        float currentHandSpeed = Mathf.Abs(leftHandSpeedY) + Mathf.Abs(rightHandSpeedY);

        // 값이 떨리지 않도록 부드럽게 러프 활용
        smoothedHandSpeed = Mathf.Lerp(smoothedHandSpeed, currentHandSpeed, smoothingFactor);

        // 부드러워진 속도 값이 정한 Threshold를 넘는지 확인
        if(smoothedHandSpeed > runningSpeedThreshold)
        {
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }
    }
}
