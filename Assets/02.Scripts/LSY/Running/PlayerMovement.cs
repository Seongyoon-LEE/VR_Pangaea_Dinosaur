using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("�ӵ� ����")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 5.0f;

    [Header("���� ��ũ��Ʈ")]
    public VRRunningDetector runningDetector; // �޸��� ����
    public PlayerStamina playerStamina; // ü�� ����
    public Transform mainCameraTransform; // �÷��̾� ����(ī�޶�)
    public InputActionReference moveActionReference; // ���� ���̽�ƽ �Է� �׼�

    CharacterController characterController;
    Vector3 playerVelocity; // �߷� ó�� ����
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
        // ���� ���� �ִ°�?
        bool wantsToRun = runningDetector.IsRunning;
        // �޸� ���׹̳ʰ� �ִ°�?
        bool canRun = playerStamina.HasStamina;
        // ������ �޸��� �ִ°�?
        bool isActuallyRunning = wantsToRun && canRun;
        playerStamina.SetRunningState(isActuallyRunning);
        // ���� ���¿� ���� ��ǥ �ӵ��� ����
        float targetSpeed = isActuallyRunning ? runSpeed : walkSpeed;

        Vector2 joystickInput = moveActionReference.action.ReadValue<Vector2>();

        // ī�޶� �ٶ󺸴� �������� ��/�� ���� ���
        // (ī�޶� ������ �ٴ����� �Ȳ����� Y���� 0���� ����)
        Vector3 forward = mainCameraTransform.forward;
        Vector3 right = mainCameraTransform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // (���̽�ƽ Y ��/�� , X ��/��) �������� �̵� ���� ���
        Vector3 desireMoveDirection = forward * joystickInput.y + right * joystickInput.x;

        characterController.Move(desireMoveDirection * targetSpeed * Time.deltaTime);

        // �߷� ó��
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
        playerVelocity.y += gravityValue * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
