using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telpo : MonoBehaviour
{
    [Header("이동할 목적지 박스")]
    public Transform targetBox;

    private bool canTeleport = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canTeleport)
        {
            // 이동 전, 양쪽 모두 잠시 텔레포트 비활성화
            Telpo targetTeleport = targetBox.GetComponent<Telpo>();
            canTeleport = false;
            targetTeleport.canTeleport = false;

            // 플레이어 위치 이동
            other.transform.position = targetBox.position;

            // 둘 다 0.5초 뒤에 다시 활성화
            Invoke(nameof(EnableTeleport), 0.5f);
            targetTeleport.Invoke(nameof(targetTeleport.EnableTeleport), 0.5f);
        }
    }

    private void EnableTeleport()
    {
        canTeleport = true;
    }
}
