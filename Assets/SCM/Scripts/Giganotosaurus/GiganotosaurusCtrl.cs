using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GiganotosaurusCtrl : MonoBehaviour
{
    private int hashIsWalk = Animator.StringToHash("IsWalk");
    Transform playerTr = null;
    Rigidbody rb;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerTr != null)
        {
            float playerZSpeed = Mathf.Abs(GetZAxisMovementSpeed());
            float currentZ = playerTr.position.z * 0.96f;

            Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, currentZ);

            transform.position = targetPosition;

            if (rb != null)
            {
                animator.SetBool(hashIsWalk, playerZSpeed > 0f);
            }
        }
        else
        {
            animator.SetBool(hashIsWalk, false);
        }
    }

    public void FindOut(GameObject tr)
    {
        playerTr = (tr != null) ? tr.transform : null;
        rb = (tr != null) ? tr.GetComponent<Rigidbody>() : null;
    }

    public float GetZAxisMovementSpeed()
    {
        if (rb == null)
        {
            print("rb null");
            return 0f;
        }

        // 1. 플레이어의 로컬 앞 방향 벡터
        Vector3 forward = transform.forward;

        // 2. 플레이어의 현재 속도 벡터
        Vector3 velocity = rb.velocity;

        // 3. 속도 벡터를 플레이어의 로컬 좌표계로 변환 (로컬 Z축 속도를 얻기 위함)
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        // 4. 로컬 Z축 속도 (앞뒤 이동 속도)만 리턴
        // 이 값은 앞으로 가면 양수, 뒤로 가면 음수가 됩니다.
        return localVelocity.z;
    }
}
