using UnityEngine;

// BaseAnomaly�� ��ӹ޾Ƽ� �츮 �ý����� �Ϻΰ� �� �� �־�!
public class TestAnomaly_ColorCube : BaseAnomaly
{
    [Header("�׽�Ʈ ���")]
    // �ν����Ϳ��� ������ �ٲ� ť�긦 ������ �� �ž�.
    public MeshRenderer targetCubeRenderer;

    private Color originalColor; // ���� ������ ������ �� ����

    void Awake()
    {
        // �׽�Ʈ�� ����, ť�갡 �Ҵ� �� ������ �����θ� ã�� ������!
        if (targetCubeRenderer == null)
        {
            targetCubeRenderer = GetComponentInChildren<MeshRenderer>();
        }
        // ���� ������ �̸� ������ �־� ���߿� �ǵ��� �� �ְ���?
        originalColor = targetCubeRenderer.material.color;
    }

    // ��� ����!
    public override void StartAnomaly()
    {
        // ť�긦 ���� ��� ���������� �ٲ۴�!
        targetCubeRenderer.material.color = Color.red;
        Debug.Log($"<�׽�Ʈ> ��� #{anomalyId}�� �ߵ�! ť�� ������ ���������� ����˴ϴ�.");
    }

    // ��� ����! (���󺹱�)
    public override void EndAnomaly()
    {
        // ť�� ������ ������� �ǵ�����.
        targetCubeRenderer.material.color = originalColor;
        Debug.Log($"<�׽�Ʈ> ��� #{anomalyId}�� ����! ť�� ������ ������� ���ƿɴϴ�.");
    }
}