using UnityEngine;

public abstract class BaseAnomaly : MonoBehaviour
{
    [Tooltip("JSON�� anomalyId ���ڿ� �Ȱ��� ���߼���")]
    public int anomalyId; // ��� ID

    // ����� ���۵ɶ� ȣ��Ǵ� �Լ�
    public abstract void StartAnomaly();

    // ����� ������ ȣ��Ǵ� �Լ�
    public abstract void EndAnomaly();
}
