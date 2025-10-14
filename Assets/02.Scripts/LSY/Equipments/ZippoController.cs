using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))] // 이 스크립트는 Animator가 꼭 필요해!
public class ZippoController : MonoBehaviour
{
    [Header("핵심 부품")]
    [Tooltip("지포라이터의 애니메이터")]
    public Animator animator;
    [Tooltip("불꽃 역할을 할 Light 컴포넌트")]
    public Light flameLight;

    [Header("입력 액션")]
    [Tooltip("라이터를 켜고 끄는 액션 (예: 오른손 트리거)")]
    public InputActionReference useAction;

    // "IsOpen"이라는 애니메이터 파라미터 이름을 숫자로 바꿔서 기억해두면 성능에 좋아!
    private readonly int isOpenHash = Animator.StringToHash("IsOpen");

    // 현재 라이터가 켜져 있는지 기억하는 변수
    private bool isOpen = false;

    void Awake()
    {
        // 만약 인스펙터에서 연결 안 해줬으면, 스스로 찾아보게!
        if (animator == null) animator = GetComponent<Animator>();
        // 시작할 땐 무조건 불을 꺼둬야지!
        if (flameLight != null) flameLight.enabled = false;
    }

    // 이 오브젝트가 활성화될 때 (손에 쥐었을 때) 입력을 받기 시작
    void OnEnable()
    {
        useAction.action.performed += OnUsePressed;
        useAction.action.Enable();
    }

    // 비활성화될 때 (인벤토리에 넣었을 때) 입력 리스너를 깔끔하게 정리
    void OnDisable()
    {
        useAction.action.performed -= OnUsePressed;
        useAction.action.Disable();
    }

    // 오른손 트리거를 눌렀을 때 호출될 함수
    private void OnUsePressed(InputAction.CallbackContext context)
    {
        // 현재 상태를 반전시킨다 (켜져있었으면 끄고, 꺼져있었으면 켠다)
        isOpen = !isOpen;

        // 애니메이터에게 현재 상태를 알려줘서 알맞은 애니메이션을 재생하게 한다
        animator.SetBool(isOpenHash, isOpen);

        // 만약 라이터를 '닫는' 상황이라면?
        // 애니메이션 이벤트까지 기다릴 필요 없이 즉시 불을 끈다!
        if (!isOpen && flameLight != null)
        {
            flameLight.enabled = false;
        }
    }

    // ? 애니메이션 이벤트가 호출할 마법의 함수! ?
    // 이 함수는 public이어야 애니메이션 클립에서 찾을 수 있어!
    public void TurnOnFlame()
    {
        if (flameLight != null)
        {
            flameLight.enabled = true;
            Debug.Log("불꽃 점화!");
        }
    }
}