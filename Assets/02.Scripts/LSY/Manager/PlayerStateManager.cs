using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Rendering;

public class PlayerStateManager : MonoBehaviour
{
    public static PlayerStateManager Instance;

    [Header("숨기 기능 참조")] 
    public InputActionReference hideAction; 
    public GameObject hidePromptPanel; 
    public TextMeshProUGUI hidePromptText;
    
    [Header("플레이어 참조")]
    public CharacterController playerCharacterController;

    public GameObject leftHandModel; 
    public GameObject rightHandModel;

    private HidingSpot currentHidingSpot; // 지금 내가 서 있는 숨는 장소

    [Header("플레이어 손 레이 참조")]
    public XRRayInteractor leftHandRay; 
    public XRRayInteractor rightHandRay;
    public PlayerState CurState { get; private set; }

    public event Action<PlayerState> OnStateChanged;

    [Header("포스트 프로세싱 참조")] 
    public Volume globalPostProcessVolume;
    public VolumeProfile normalProfile; 
    public VolumeProfile nightVisionProfile; 

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
        if (hideAction != null)
        {
            hideAction.action.performed += OnHidePressed; // "눌렸을 때 OnHidePressed 함수 실행!"
            hideAction.action.Enable(); // 액션 활성화!
        }
    }
    private void OnDestroy()
    {
        if (hideAction != null)
        {
            hideAction.action.performed -= OnHidePressed;
        }
    }

    void Start()
    {
        // 게임 시작시 맨손
        ChangeState(PlayerState.Hand);
        if (hidePromptPanel != null) hidePromptPanel.SetActive(false);
    }
    public void SetCurrentHidingSpot(HidingSpot spot)
    {
        currentHidingSpot = spot;

        // 1. 숨을 곳이 생겼고, 내가 아직 안 숨었다면
        if (currentHidingSpot != null && CurState != PlayerState.Hiding)
        {
            hidePromptText.text = "Press [LT] to Hide"; //  텍스트 바꾸기
            hidePromptPanel.SetActive(true); //  UI 켜기
        }
        // 2. 숨을 곳에서 멀어졌다면
        else
        {
            hidePromptPanel.SetActive(false); //  UI 끄기
        }
    }
    private void OnHidePressed(InputAction.CallbackContext context)
    {
        // 1. (이미 숨어있는 상태에서 누름) -> "나오기"
        if (CurState == PlayerState.Hiding)
        {
            if (currentHidingSpot != null)
            {
                // "캐비닛아! 나 내보내줘!"
                currentHidingSpot.ExitHide(playerCharacterController);

                // "나오자마자" UI 다시 띄우기
                hidePromptText.text = "Press [LT] to Hide";
                hidePromptPanel.SetActive(true);
            }
        }
        // 2. (숨을 곳 근처에서 누름) -> "숨기"
        else if (currentHidingSpot != null && CanPerformAction()) // 행동 가능한 상태일 때만!
        {
            // "캐비닛아! 나 숨겨줘!"
            currentHidingSpot.EnterHide(playerCharacterController);

            // "숨자마자" UI 텍스트 바꾸기
            hidePromptText.text = "Press [LT] to Exit";
            hidePromptPanel.SetActive(true); // (이미 켜져있겠지만 확실하게!)
        }
    }
    public void ChangeState(PlayerState newState)
    {
        // 이미 같은 상태라면 아무것도 안함 (최적화)
        if (CurState == PlayerState.Revolver && newState == PlayerState.Hand) return;
        if (CurState == newState) return;

        CurState = newState;
        print($"[PlayerStateManager] State Changed to: {CurState}");

        VolumeProfile targetProfile = normalProfile;

        // 새로운 상태에 따라서 양손 레이를 껐다 켠다!
        switch (newState)
        {
            case PlayerState.Hand:      // 맨손 상태
            case PlayerState.Inventory: // 인벤토리 열었을 때
                SetHandRays(true); // 양손 레이 켜기 
                break;

            case PlayerState.Revolver:  // 총 들었을 때
            case PlayerState.Lighter:  // 라이터 들었을 때
            case PlayerState.Camera:   // 카메라 들었을 때
                SetHandRays(false); // 양손 레이 끄기 
                targetProfile = normalProfile;
                break;

            case PlayerState.CameraView: // 카메라 뷰 모드일 때
            case PlayerState.Hiding: // 숨었을때 
                SetHandRays(false); // 양손 레이 끄기 
                SetHandModels(false); // 손 모델 끄기! (손/아이템 숨기기!)
                targetProfile = nightVisionProfile;
                break;

            default:
                SetHandRays(true); // 기본값은 켜기
                SetHandModels(true);
                targetProfile = normalProfile;
                break;
        }
        if (globalPostProcessVolume != null && targetProfile != null)
        {
            globalPostProcessVolume.profile = targetProfile;
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
    private void SetHandModels(bool isEnabled)
    {
        if (leftHandModel != null)
        {
            leftHandModel.SetActive(isEnabled);
        }
        if (rightHandModel != null)
        {
            rightHandModel.SetActive(isEnabled);
        }
    }
}
