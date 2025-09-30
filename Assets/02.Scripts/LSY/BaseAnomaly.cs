using UnityEngine;

public abstract class BaseAnomaly : MonoBehaviour
{
    [Tooltip("JSON의 anomalyId 숫자와 똑같이 맞추세요")]
    public int anomalyId; // 기믹 ID

    // 기믹이 시작될때 호출되는 함수
    public abstract void StartAnomaly();

    // 기믹이 끝날때 호출되는 함수
    public abstract void EndAnomaly();
}
