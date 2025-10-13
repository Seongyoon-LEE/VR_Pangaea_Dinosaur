using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI 구성 요소")]
    public GameObject inventoryPanel; // 인벤토리 패널
    public Transform buttonParent; // 버튼 부모 객체

    [Header("체력, 스테미너 UI 매니저")]
    public GameObject healthUIPanel; // 체력 UI 패널
    public GameObject staminaUIPanel; // 스테미너 UI 패널

    // 인벤토리가 열려 있는지 상태 알려주는 변수
    public bool IsOpen { get; private set; }

    Button[] itemButtons; // 아이템 버튼 배열
    void Start()
    {
        CloseInventory(); // 시작할때 인벤토리 닫기

        itemButtons = buttonParent.GetComponentsInChildren<Button>();
        for (int i = 0; i < itemButtons.Length; i++)
        {
            int itemIndex = i; // 클로저 문제 해결용 임시 변수
            itemButtons[i].onClick.AddListener(() => OnItemButtonClicked(itemIndex));
        }
    }
    // 인벤토리를 켜고 끄는 기능
    public void ToggleInventory()
    {
        // 인벤토리가 열려 있으면 닫고, 닫혀 있으면 열기
        if (IsOpen)
            CloseInventory();
        else
            OpenInventory();
    }
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        IsOpen = true;

        // 체력, 스테미너 UI 숨기기
        if (healthUIPanel != null) healthUIPanel.SetActive(false);
        if (staminaUIPanel != null) staminaUIPanel.SetActive(false);

        print("인벤토리 열림 (체력/스테미너 UI 숨김");
    }
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        IsOpen = false;
        // 체력, 스테미너 UI 다시 보이기
        if (healthUIPanel != null) healthUIPanel.SetActive(true);
        if (staminaUIPanel != null) staminaUIPanel.SetActive(true);
        print("인벤토리 닫힘");
    }

    // 아이템 버튼 클릭시 호출되는 함수
    void OnItemButtonClicked(int itemIndex)
    {
        Debug.Log($"아이템 버튼 {itemIndex} 클릭됨");
        InventorySystem.Instance.EquipItem(itemIndex);
        CloseInventory(); // 아이템 장착 후 인벤토리 닫기
    }
}
