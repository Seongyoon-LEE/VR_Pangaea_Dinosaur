using UnityEngine;

public class RevolverAttach : MonoBehaviour
{
    [Header("손 위치 지정")]
    [Tooltip("총이 붙을 부모가 될 손 오브젝트 (예: RightHand Controller)")]
    public Transform handParent;

    [Header("위치 및 회전 보정값")]
    [Tooltip("손을 기준으로 총이 얼마나 떨어져 있을지 위치 오프셋")]
    public Vector3 positionOffset = Vector3.zero;

    [Tooltip("손을 기준으로 총을 얼마나 회전시킬지 회전 오프셋")]
    public Vector3 rotationOffset = Vector3.zero;

    // 이 함수를 호출하면 마법이 일어난다!
    public void Attach()
    {
        if (handParent == null)
        {
            Debug.LogError("손(Hand Parent)이 지정되지 않았습니다!", this.gameObject);
            return;
        }

        // 1. 총을 손의 자식으로 만든다.
        transform.SetParent(handParent);

        // 2. 인스펙터에서 설정한 값으로 로컬 위치를 바로잡는다.
        transform.localPosition = positionOffset;

        // 3. 인스펙터에서 설정한 값으로 로컬 회전을 바로잡는다. (오일러 각도를 쿼터니언으로)
        transform.localRotation = Quaternion.Euler(rotationOffset);

        Debug.Log($"{this.name}을(를) 손에 부착했습니다.");
    }

    // 아이템을 내려놓을 때 호출할 함수
    public void Detach()
    {
        // 부모 관계를 끊어서 다시 독립적인 오브젝트로 만든다.
        transform.SetParent(null);
        Debug.Log($"{this.name}을(를) 손에서 뗐습니다.");
    }
}