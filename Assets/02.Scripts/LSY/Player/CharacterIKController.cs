
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class CharacterIKController : MonoBehaviour
{
    private Animator animator;

    // 각 손이 따라가야 할 목표 지점
    private Transform rightHandTarget = null;
    private Transform leftHandTarget = null;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        // 오른손 목표가 지정되었다면?
        if (rightHandTarget != null)
        {
            // IK 가중치를 1(100%)로 설정해서 IK를 완전히 활성화!
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
            // 오른손을 목표 지점의 위치와 회전으로 이동!
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }
        else // 목표가 없다면 IK를 비활성화해서 손이 원래 애니메이션을 따라가게 함
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }

        // 왼손도 똑같이!
        if (leftHandTarget != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }

    // 외부에서 "손, 이 목표를 따라가!" 라고 명령할 함수
    public void SetHandTarget(AvatarIKGoal goal, Transform target)
    {
        if (goal == AvatarIKGoal.RightHand) rightHandTarget = target;
        else if (goal == AvatarIKGoal.LeftHand) leftHandTarget = target;
    }

    // 외부에서 "이제 그만 따라가도 돼!" 라고 명령할 함수
    public void ClearHandTarget(AvatarIKGoal goal)
    {
        if (goal == AvatarIKGoal.RightHand) rightHandTarget = null;
        else if (goal == AvatarIKGoal.LeftHand) leftHandTarget = null;
    }
}