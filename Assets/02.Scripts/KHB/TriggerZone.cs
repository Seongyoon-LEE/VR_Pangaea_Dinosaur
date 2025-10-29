using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingTriggerZone : MonoBehaviour
{
    [Header("엔딩 씬 이름")]
    public string endingSceneName = "EndingScene"; // Inspector에서 실제 엔딩 씬 이름으로 바꿔주세요.

    [Header("트리거 설정")]
    [Tooltip("트리거가 Player에게 닿았을 때만 작동할 태그")]
    public string playerTag = "Player"; // Player 오브젝트의 태그를 'Player'로 설정해주세요.

    // 이 변수는 알이 5개 모였는지 여부를 저장합니다.
    private bool isReadyToFinish = false;

    void Start()
    {
        // 1. GameManager가 준비되었는지 확인
        if (GameManager.Instance != null)
        {
            // 2. GameManager의 'OnAllEggsCollected' 이벤트에 함수 연결!
            // 알이 5개 모이는 즉시 isReadyToFinish를 true로 설정합니다.
            GameManager.Instance.OnAllEggsCollected.AddListener(OnAllEggsCollected);

            // 만약 이미 5개가 모인 상태로 씬이 로드되면 바로 상태를 업데이트 (안전장치)
            if (GameManager.Instance.currentEggs >= GameManager.Instance.maxEggs)
            {
                isReadyToFinish = true;
            }
        }

        // *주의: 이 오브젝트에는 Collider 컴포넌트가 붙어있고 'Is Trigger'가 체크되어 있어야 합니다.
        // Rigidbody도 함께 붙여주세요. (Unity Trigger 동작의 필수 조건)
    }

    // GameManager의 이벤트가 호출되면 실행될 함수
    private void OnAllEggsCollected()
    {
        Debug.Log("엔딩존: 모든 알 수집 신호 수신. 엔딩 준비 완료!");
        isReadyToFinish = true;
    }

    // 다른 오브젝트와 충돌했을 때 (트리거)
    public void OnTriggerEnter(Collider other)
    {
        // 1. 플레이어 태그와 일치하는지 확인
        if (other.CompareTag(playerTag))
        {
            Debug.Log("플레이어가 엔딩 존에 진입했습니다.");

            // 2. 알 수집 조건이 충족되었는지 확인
            if (isReadyToFinish)
            {
                Debug.Log("엔딩 조건 충족! 씬 이동을 시작합니다.");

                // 3. 씬 이동 실행
                SceneManager.LoadScene(endingSceneName);
            }
            else
            {
                // 디버깅 메시지: 아직 알이 부족함
                Debug.LogWarning("엔딩 진입 실패. 알이 " + (GameManager.Instance.maxEggs - GameManager.Instance.currentEggs) + "개 부족합니다.");
            }
        }
    }
}