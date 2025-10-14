using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorTest : MonoBehaviour
{// 이동 속도 (Inspector에서 조절)
    public float moveSpeed = 5.0f;

    // Rigidbody 컴포넌트
    private Rigidbody rb;

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("CapsuleMovement 스크립트는 Rigidbody 컴포넌트가 필요합니다!");
        }

        // Rigidbody를 사용할 때 회전(기울어짐)을 막습니다.
        rb.freezeRotation = true;
    }

    // 물리 업데이트는 FixedUpdate에서 처리하는 것이 좋습니다.
    void FixedUpdate()
    {
        // 1. 입력 처리
        // 수평 입력 (A/D)
        float horizontalInput = Input.GetAxis("Horizontal");
        // 수직 입력 (W/S)
        float verticalInput = Input.GetAxis("Vertical");

        // 2. 이동 방향 계산
        // y축은 0으로 설정하여 공중으로 뜨지 않게 합니다.
        Vector3 moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        // 월드 좌표계 기준이 아닌, 오브젝트의 앞 방향 기준으로 이동하도록 변환
        Vector3 movement = transform.TransformDirection(moveDirection) * moveSpeed;

        // 3. Rigidbody를 이용한 이동 적용
        // 현재 Rigidbody의 y축 속도를 유지하여 중력 및 점프(추후 구현 시)를 처리합니다.
        // Rigidbody의 velocity(속도)를 직접 설정하여 이동합니다.
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // 4. 회전 (선택 사항: 이동 방향으로 플레이어를 돌립니다)
        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            // Slerp 대신 RotateTowards를 사용하거나, Rigidbody의 회전 기능을 사용해 부드럽게 회전시킵니다.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }
}
