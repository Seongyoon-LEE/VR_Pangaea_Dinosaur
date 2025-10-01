using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRunningDetector : MonoBehaviour
{
    [Header("������ ��Ʈ�ѷ�")]
    public Transform leftHand;
    public Transform rightHand;

    [Header("�޸��� ���� �ΰ���")]
    public float runningSpeedThreshold = 2.5f; // �� ������ �ΰ��� ����

    [Range(0f, 1f)]
    public float smoothingFactor = 0.1f; // �ε巴�� �ϱ� ���� ���

    // ���� �÷��̾ �޸��������� ���� ����
    public bool IsRunning { get; private set; }

    Vector3 prevLeftHandPos;
    Vector3 prevRightHandPos;
    float smoothedHandSpeed;

    void Start()
    {
        // ù ������ �� Ʀ ����
        prevLeftHandPos = leftHand.position;
        prevRightHandPos = rightHand.position;
    }

    void Update()
    {
        // �� ���� ����(Y��) �̵� �ӵ� ���
        // (���� Y��ġ - ���� ������ Y��ġ) / �ð� = �ӵ�
        float leftHandSpeedY = (leftHand.position.y - prevLeftHandPos.y) / Time.deltaTime;
        float rightHandSpeedY = (rightHand.position.y - prevRightHandPos.y) / Time.deltaTime;

        // ���� ������ ����� ���� ���� ��ġ�� ������ġ�� ����
        prevLeftHandPos = leftHand.position;
        prevRightHandPos = rightHand.position;

        float currentHandSpeed = Mathf.Abs(leftHandSpeedY) + Mathf.Abs(rightHandSpeedY);

        // ���� ������ �ʵ��� �ε巴�� ���� Ȱ��
        smoothedHandSpeed = Mathf.Lerp(smoothedHandSpeed, currentHandSpeed, smoothingFactor);

        // �ε巯���� �ӵ� ���� ���� Threshold�� �Ѵ��� Ȯ��
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
