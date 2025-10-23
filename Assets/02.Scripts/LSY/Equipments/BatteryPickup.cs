using UnityEngine;

public class BatteryPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어의 손(Hand)에 닿았는지 확인 (손에 "PlayerHand" 태그가 있어야 함!)
        if (other.CompareTag("PlayerHand"))
        {
            // 1. 현재 장착 중인 아이템을 가져와본다.
            IEquippable currentItem = InventorySystem.Instance.GetCurrentEquippedItem();

            // 2. 만약 아이템이 있고, 그게 '카메라'가 맞다면
            if (currentItem != null && currentItem is CameraController)
            {
                // 3. 카메라를 찾아서 배터리를 충전시킨다!
                CameraController camera = currentItem as CameraController;
                camera.RechargeBattery();

                // 4. 배터리 아이템은 줍고 파괴한다.
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("카메라를 들고 있을 때만 배터리를 주울 수 있습니다.");
            }
        }
    }
}