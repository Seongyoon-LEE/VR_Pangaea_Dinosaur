using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;

    [Header("플레이어 손 레이 참조")]
    public XRRayInteractor leftHandRay; 
    public XRRayInteractor rightHandRay;
    public PlayerState CurState { get; private set; }

    public event Action<PlayerState> OnStateChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 게임 시작시 맨손
        ChangeState(PlayerState.Hand);
    }
    public void ChangeState(PlayerState newState)
    {
        // 이미 같은 상태라면 아무것도 안함 (최적화)
        if (CurState == PlayerState.Revolver && newState == PlayerState.Hand) return;
        if (CurState == newState) return;

        CurState = newState;
        print($"[PlayerStateManager] State Changed to: {CurState}");

        // 새로운 상태에 따라서 양손 레이를 껐다 켠다!
        switch (newState)
        {
            case PlayerState.Hand:      // 맨손 상태
            case PlayerState.Inventory: // 인벤토리 열었을 때
                SetHandRays(true); // 양손 레이 켜기 
                break;

            case PlayerState.Revolver:  // 총 들었을 때
            case PlayerState.Lighter:  // 라이터 들었을 때
            case PlayerState.Hiding:    // 숨었을 때
                SetHandRays(false); // 양손 레이 끄기 
                break;

            default:
                SetHandRays(true); // 기본값은 켜기
                break;
        }
        OnStateChanged?.Invoke(newState);
    }
    private void SetHandRays(bool isEnabled)
    {
        if (leftHandRay != null)
        {
            leftHandRay.enabled = isEnabled;
        }
        if (rightHandRay != null)
        {
            rightHandRay.enabled = isEnabled;
        }
    }
    public bool CanPerformAction()
    {
        switch (CurState)
        {
            case PlayerState.Inventory:
            case PlayerState.Hiding:
                return false;

            // 그 외엔 행동 가능 
            default:
                return true;
        }
    }
}
