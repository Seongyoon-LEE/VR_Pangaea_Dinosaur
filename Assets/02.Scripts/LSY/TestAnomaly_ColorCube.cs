using UnityEngine;

// BaseAnomaly를 상속받아서 우리 시스템의 일부가 될 수 있어!
public class TestAnomaly_ColorCube : BaseAnomaly
{
    [Header("테스트 대상")]
    // 인스펙터에서 색깔을 바꿀 큐브를 연결해 줄 거야.
    public MeshRenderer targetCubeRenderer;

    private Color originalColor; // 원래 색깔을 저장해 둘 변수

    void Awake()
    {
        // 테스트를 위해, 큐브가 할당 안 됐으면 스스로를 찾게 만들자!
        if (targetCubeRenderer == null)
        {
            targetCubeRenderer = GetComponentInChildren<MeshRenderer>();
        }
        // 원래 색깔을 미리 저장해 둬야 나중에 되돌릴 수 있겠지?
        originalColor = targetCubeRenderer.material.color;
    }

    // 기믹 시작!
    public override void StartAnomaly()
    {
        // 큐브를 눈에 띄는 빨간색으로 바꾼다!
        targetCubeRenderer.material.color = Color.red;
        Debug.Log($"<테스트> 기믹 #{anomalyId}번 발동! 큐브 색깔이 빨간색으로 변경됩니다.");
    }

    // 기믹 종료! (원상복구)
    public override void EndAnomaly()
    {
        // 큐브 색깔을 원래대로 되돌린다.
        targetCubeRenderer.material.color = originalColor;
        Debug.Log($"<테스트> 기믹 #{anomalyId}번 종료! 큐브 색깔이 원래대로 돌아옵니다.");
    }
}