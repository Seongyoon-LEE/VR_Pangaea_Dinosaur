using UnityEngine;

public class IKGrabbable : MonoBehaviour
{
    [Header("IK 손잡이 위치")]
    public Transform rightHandGrip;
    public Transform leftHandGrip;

    private CharacterIKController characterIK;
    private Animator animator;

    // Animator Parameter ID를 미리 숫자로 바꿔두면 성능에 좋아!
    private readonly int rightGrabHash = Animator.StringToHash("RightGrab");
    private readonly int leftGrabHash = Animator.StringToHash("LeftGrab");

    // 아이템이 활성화될 때 자동으로 필요한 컴포넌트들을 찾아온다.
    void Awake()
    {
        // "Player" 태그가 붙은 오브젝트를 찾아서 필요한 컴포넌트를 가져옴
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            characterIK = player.GetComponent<CharacterIKController>();
            animator = player.GetComponent<Animator>();
        }
    }

    public void Grab()
    {
        if (characterIK == null || animator == null) return;

        if (rightHandGrip != null)
        {
            characterIK.SetHandTarget(AvatarIKGoal.RightHand, rightHandGrip);
            animator.SetBool(rightGrabHash, true); // "손 쥐는 애니메이션 켜!"
        }
        if (leftHandGrip != null)
        {
            characterIK.SetHandTarget(AvatarIKGoal.LeftHand, leftHandGrip);
            animator.SetBool(leftGrabHash, true);
        }
    }

    public void Release()
    {
        if (characterIK == null || animator == null) return;

        if (rightHandGrip != null)
        {
            characterIK.ClearHandTarget(AvatarIKGoal.RightHand);
            animator.SetBool(rightGrabHash, false); // "손 펴는 애니메이션 켜!"
        }
        if (leftHandGrip != null)
        {
            characterIK.ClearHandTarget(AvatarIKGoal.LeftHand);
            animator.SetBool(leftGrabHash, false);
        }
    }
}