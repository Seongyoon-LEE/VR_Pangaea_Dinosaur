using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; 

[RequireComponent(typeof(XRSimpleInteractable), typeof(Collider))]
public class DinoEgg : MonoBehaviour
{
    private XRSimpleInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        // 누가 나를 Select(트리거 클릭)하면, OnCollected 함수를 실행시켜!
        interactable.selectEntered.AddListener(OnCollected);
    }

    private void OnDestroy()
    {
        // 오브젝트 파괴될 때 이벤트 연결도 깔끔하게 해제
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnCollected);
        }
    }

    // 'Select'(트리거 클릭) 당했을 때 실행될 함수!
    private void OnCollected(SelectEnterEventArgs args)
    {
        // ?? 혹시 모르니, '레이'로 클릭한 게 맞는지 확인!
        if (args.interactorObject is XRRayInteractor)
        {
            // 1. 총괄 매니저한테 "저 수집됐어요!" 하고 보고하기!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CollectEgg();
            }

            // 2. (선택) 수집 사운드 재생!
            // SoundManager.s_Instance.PlaySfx(...);

            // 3. 알 오브젝트 스스로 파괴! (수집됐으니까!)
            Destroy(gameObject);
        }
    }
}