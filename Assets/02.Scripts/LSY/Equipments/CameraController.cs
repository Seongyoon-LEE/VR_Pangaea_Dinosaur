using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class CameraController : MonoBehaviour, IEquippable
{
    [Header("핵심 부품")]
    public GameObject cameraEffect; // 카메라 이펙트 오브젝트

    public Slider batterySlider; // 배터리 잔량 슬라이더

    [Header("배터리 설정")]
    public float maxBattery = 100f; // 최대 배터리 용량
    public float drainRate = 5f; // 배터리 소모 속도 (초당)
    private float currentBattery; // 현재 배터리 잔량

    [Header("입력 액션")]
    public InputActionReference useAction; // 사용 입력 액션

    [Header("장착 보정값")]
    public Vector3 positionOffset = Vector3.zero; // 장착 위치 오프셋
    public Vector3 rotationOffset = Vector3.zero; // 장착 회전 오프셋

    [Header("카메라 모드 참조")]
    public GameObject cameraUIPanel; // 아까 만든 'Camcorder_View' UI 패널
    public GameObject heldCameraModel; // 손에 들고있는 3D 카메라 모델링

    [Header("포스트 프로세싱")]
    public Volume globalPostProcessVolume; // 씬에 있는 Global Volume
    public VolumeProfile normalProfile;     // 평상시 프로필
    public VolumeProfile nightVisionProfile; // 나이트비전 프로필

    private bool isCameraOn = false; // 카메라 켜짐 상태 여부

    private void Awake()
    {
        currentBattery = maxBattery; // 초기 배터리 잔량 설정
        
        UpdateBatteryUI(); // UI 초기화
    }
    public void Equip(Transform handParent)
    {
        transform.SetParent(handParent);
        transform.localPosition = positionOffset;
        transform.localRotation = Quaternion.Euler(rotationOffset);
        gameObject.SetActive(true);

        useAction.action.performed += OnUsePressed;
        useAction.action.Enable();

        // 카메라든 상태 
        PlayerStateManager.Instance.ChangeState(PlayerState.Camera);
        if(batterySlider != null)
            batterySlider.gameObject.SetActive(true);
    }
    void ToggleCameraView(bool isOn)
    {
        if (isOn) //  카메라 켤 때
        {
            // 1. 상태 변경 (레이저 끄기 등)
            PlayerStateManager.Instance.ChangeState(PlayerState.CameraView);
            // 2. 손에 든 3D 모델링 숨기기 (UI로 대체하니까!)
            heldCameraModel.SetActive(false);
            // 3. 카메라 UI 켜기
            cameraUIPanel.SetActive(true);
            // 4. 나이트비전 효과 켜기
            globalPostProcessVolume.profile = nightVisionProfile;
        }
        else //  카메라 끌 때
        {
            // 1. 상태 복구
            PlayerStateManager.Instance.ChangeState(PlayerState.Hand); // 혹은 PlayerState.Camera (들고만 있는 상태)
                                                                       // 2. 손에 든 3D 모델링 다시 보이기
            heldCameraModel.SetActive(true);
            // 3. 카메라 UI 끄기
            cameraUIPanel.SetActive(false);
            // 4. 평상시 효과로 복구
            globalPostProcessVolume.profile = normalProfile;
        }
    }
    public void Unequip()
    {
        useAction.action.performed -= OnUsePressed;
        useAction.action.Disable();

        TurnOffCamera(); // 아이템 집어 넣을때 끄기
        transform.SetParent(null);
        gameObject.SetActive(false);
        if (batterySlider != null)
            batterySlider.gameObject.SetActive(false);
    }
    // 매 프레임 배터리 소모 체크
    void Update()
    {
        if (isCameraOn)
        {
            currentBattery -= drainRate * Time.deltaTime;
            UpdateBatteryUI();

            // 배터리 다 썼는지 체크
            if(currentBattery <= 0)
            {
                print("카메라 배터리 방전");
                currentBattery = 0;
                TurnOffCamera();

                // 배터리 다썼으니 맨손으로
                InventorySystem.Instance.UnequipCurrentItem();
                // 카메라 내리는 애니메이션
            }
        }
    }
    private void OnUsePressed(InputAction.CallbackContext context)
    {
        // 행동 불가 상태일땐 무시
        if (!PlayerStateManager.Instance.CanPerformAction()) return;

        // 켜라고 하는데 배터리가 없으면 무시
        if (!isCameraOn && currentBattery <= 0)
        {
            print("배터리 없어 킬 수 없음");
            return;
        }
        // 토글 
        isCameraOn = !isCameraOn;

        if (isCameraOn) TurnOnCamera();
        else TurnOffCamera();
    }
    void TurnOnCamera()
    {
        isCameraOn = true;
        // TOOD: 카메라 소리 재생
    }
    void TurnOffCamera()
    {
        isCameraOn = false;
        // 소리 끄는 재생
    }
    // 배터리 주울때 호출할 함수
    public void RechargeBattery()
    {
        currentBattery = maxBattery;
        UpdateBatteryUI();
        print("배터리 완충!");
    }
    // 배터리 UI 업데이트
    void UpdateBatteryUI()
    {
        if (batterySlider != null)
        {
            batterySlider.value = currentBattery / maxBattery;
        }
    }
}
