using UnityEngine;
using TMPro; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤으로 만들기

    [Header("UI 참조")]
    public TextMeshProUGUI eggCountText; // 인스펙터에서 0/5 텍스트 끌어다 놓기!

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // '총괄 매니저'가 이 함수를 불러서 UI를 갱신시킬 거야!
    public void UpdateEggCount(int currentAmount, int maxAmount)
    {
        if (eggCountText != null)
        {
            eggCountText.text = $"{currentAmount}/{maxAmount}";
        }
    }
}