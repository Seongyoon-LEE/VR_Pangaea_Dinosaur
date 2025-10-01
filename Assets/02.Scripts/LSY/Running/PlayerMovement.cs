using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("속도 설정")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;

    [Header("참조 스크립트")]
    public VRRunningDetector runningDetector; // 달리기 감지
    public PlayerStamina playerStamina; // 체력 관리
    public Transform mainCameraTransform; // 플레이어 시점(카메라)
    public InputActionReference moveActionReference; // 왼쪽 조이스틱 입력 액션

    CharacterController characterController;
    Vector3 playerVelocity; // 중력 처리 변수
    float gravityValue = -9.81f; 

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void OnEnable()
    {
        moveActionReference.action.Enable();
    }
    private void OnDisable()
    {
        moveActionReference.action.Disable();
    }
    void Update()
    {
        // 손을 흔들고 있는가?
        bool wantsToRun = runningDetector.IsRunning;
        // 달릴 스테미너가 있는가?
        bool canRun = playerStamina.HasStamina;
        // 실제로 달리고 있는가?
        bool isActuallyRunning = wantsToRun && canRun;
        playerStamina.SetRunningState(isActuallyRunning);
        // 현재 상태에 맞춰 목표 속도를 결정
        float targetSpeed = isActuallyRunning ? runSpeed : walkSpeed;

        Vector2 joystickInput = moveActionReference.action.ReadValue<Vector2>();

        // 카메라가 바라보는 방향으로 앞/옆 방향 계산
        // (카메라 숙여도 바닥으로 안꺼지게 Y축은 0으로 고정)
        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // (조이스틱 Y 앞/뒤 , X 좌/우) 방향으로 이동 벡터 계산
        Vector3 desireMoveDirection = forward * joystickInput.y + right * joystickInput.x;

        characterController.Move(desireMoveDirection * targetSpeed * Time.deltaTime);

        // 중력 처리
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
