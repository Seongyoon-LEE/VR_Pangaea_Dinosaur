using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // 싱글톤

    [Header("알 수집")]
    public int maxEggs = 5; // 목표 알 개수
    public int currentEggs = 4; // 현재 모은 알 개수

    [Header("문 열기 이벤트")]
    public UnityEvent OnAllEggsCollected; // 5/5가 되면 여기서 쾅! 신호를 쏠 거야!

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    private void Start()
    {
        // 게임 시작할 때 UI를 '0/5'로 초기화!
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEggCount(currentEggs, maxEggs);
        }
    }

    // 'DinoEgg.cs'가 이 함수를 부를 거야!
    public void CollectEgg()
    {
        currentEggs++; // 알 개수 1 증가!

        // 1. UIManager한테 "UI 갱신해!"라고 시키기
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateEggCount(currentEggs, maxEggs);
        }

        // 2. 5/5가 되었는지 확인!
        if (currentEggs >= maxEggs)
        {
            Debug.Log("모든 알 수집 완료! 문 열기 이벤트를 실행합니다!");

            // 3. 문 열기 이벤트 쾅! 쏘기!
            if (OnAllEggsCollected != null)
            {
                OnAllEggsCollected.Invoke();
            }
        }
    }
    public void Die()
    {
        SceneManager.LoadScene("LobbyScene");
    }
}