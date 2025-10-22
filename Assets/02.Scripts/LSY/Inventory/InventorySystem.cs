using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance; // 싱글톤 인스턴스

    [Header("아이템 목록")]
    public GameObject[] items; // 아이템 프리팹 배열

    [Header("입력 액션")]
    public InputActionReference inventoryToggleAction; // 인벤토리 토글 액션

    [Header("참조")]
    public InventoryUIManager uiManager; // 인벤토리 UI 매니저

    public Transform handTransform; // 아이템을 들 위치    

    int currentEquippedItemIndex = -1; // 현재 장착된 아이템 인덱스
    RevolverAttach curRevolverAttach; // 현재 장착된 리볼버 스크립트

    public GameObject leftHandModel; // 왼손 모델
    public GameObject rightHandModel; // 오른손 모델

    private void Awake()
    {
        if(Instance == null) Instance = this;
    }
    void Start()
    {
        // 게임 시작시 모든 아이템 비활성화
        foreach(var item in items)
        {
            item.SetActive(false);
        }
    }
    private void OnEnable()
    {
        inventoryToggleAction.action.performed += OnInventoryTogglePressed;
        inventoryToggleAction.action.Enable();
    }
    private void OnDisable()
    {
        inventoryToggleAction.action.performed -= OnInventoryTogglePressed;
        inventoryToggleAction.action.Disable();
    }
    // 인벤토리 열기 버튼 눌렀을때
    void OnInventoryTogglePressed(InputAction.CallbackContext context)
    {
        if (!uiManager.IsOpen)
        {
            UnequipCurrentItem();
        }
        // UI 매니저에게 인벤토리를 토글하라고 명령
        uiManager.ToggleInventory();
    }
    // 특정 번호의 아이템을 장착하는 기능
    public void EquipItem(int itemIndex)
    {
        // 잘못된 번호거나 이미 같은 아이템을 들고 있다면 아무것도 안함
        if (itemIndex < 0 || itemIndex >= items.Length || itemIndex == currentEquippedItemIndex)
            return;
        // 이전에 들고 있던 아이템이 있다면 집어넣기
        UnequipCurrentItem();

        if (leftHandModel != null)
            leftHandModel.SetActive(false); // 손 모델 숨기기
        if(rightHandModel != null)
            rightHandModel.SetActive(false); // 손 모델 숨기기

        GameObject itemToEquip = items[itemIndex];
        currentEquippedItemIndex = itemIndex;

        // 아이템을 활성화!
        itemToEquip.SetActive(true);

        // 아이템에 붙어있는 RevolverAttach 스크립트를 찾아서 Attach 함수를 호출!
        var attachable = itemToEquip.GetComponent<RevolverAttach>();
        if (attachable != null)
        {
            attachable.Attach();
        }
    }
    // 현재 들고 있는 아이템을 집어넣는 기능
    void UnequipCurrentItem()
    {
        if (currentEquippedItemIndex != -1)
        {
            // 내려놓을 아이템에서 RevolverAttach 스크립트를 찾아서 Detach 함수를 호출!
            var attachable = items[currentEquippedItemIndex].GetComponent<RevolverAttach>();
            if (attachable != null)
            {
                attachable.Detach();
            }

            items[currentEquippedItemIndex].SetActive(false);
            currentEquippedItemIndex = -1;

            if (leftHandModel != null)
                leftHandModel.SetActive(true); // 손 모델 숨기기
            if (rightHandModel != null)
                rightHandModel.SetActive(true); // 손 모델 숨기기
        }
    }
}
