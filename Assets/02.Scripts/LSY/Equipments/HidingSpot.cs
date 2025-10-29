using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [Header("플레이어가 숨을 위치")]
    public Transform hidingTransform; //  아까 만든 HidingTransform을 끌어다 놓기!

    [Header("플레이어가 나올 위치")]
    public Transform exitTransform; 

    [Header("지금 숨어있는 플레이어")]
    public CharacterController hiddenPlayer; //  지금 숨은 플레이어 (디버깅용)

    private Transform playerOriginalParent; //  플레이어의 원래 부모 (기억용)

    // 1. 플레이어가 감지 영역에 들어왔을 때
    private void OnTriggerEnter(Collider other)
    {
        //  플레이어 태그 확인! (CharacterController에 "Player" 태그가 있어야 해!)
        if (other.CompareTag("Player"))
        {
            // "매니저님! 저 숨을 수 있는 캐비닛이에요!" 하고 보고하기
            PlayerStateManager.Instance.SetCurrentHidingSpot(this);
        }
    }

    // 2. 플레이어가 감지 영역에서 나갔을 때
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // "매니저님! 저 이제 얘랑 멀어졌어요!" 하고 보고하기
            PlayerStateManager.Instance.SetCurrentHidingSpot(null);
        }
    }

    // 3. "숨어!" (매니저가 호출할 함수)
    public void EnterHide(CharacterController player)
    {
        hiddenPlayer = player;
        playerOriginalParent = player.transform.parent; // 원래 부모 기억

        player.enabled = false; //  캐릭터 컨트롤러를 "잠깐 꺼서" 물리 무시!
        player.transform.SetParent(hidingTransform); // 캐비닛 안으로 텔레포트
        player.transform.localPosition = Vector3.zero;
        player.transform.localRotation = Quaternion.identity;

        // "매니저님! 저 Hiding 상태로 바꿔주세요!"
        PlayerStateManager.Instance.ChangeState(PlayerState.Hiding);
    }

    // 4. "나와!" (매니저가 호출할 함수)
    public void ExitHide(CharacterController player)
    {
        player.transform.SetParent(playerOriginalParent); // 원래 부모로 복귀
        if (exitTransform != null)
        {
            player.transform.position = exitTransform.position;
            player.transform.rotation = exitTransform.rotation;
        }
        else
        {
            player.transform.position = this.transform.position;
        }
        player.enabled = true; //  캐릭터 컨트롤러 다시 켜기!

        hiddenPlayer = null;
        PlayerStateManager.Instance.ChangeState(PlayerState.Hand); // 맨손 상태로 복귀
    }
}
