using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RevolverCtrl : MonoBehaviour
{
    [Header("총 데이터")]
    public GunData gunData;  // 총 데이터

    [Header("XRI 입력 액션")]
    public InputActionReference shootActionReference; // 발사 입력 액션
    //public InputActionReference reloadActionReference; // 재장전 입력 액션

    [Header("참조")]
    public Transform firePoint; // 발사 위치
    public ParticleSystem muzzleFlash; // 총구 화염 효과
    public GameObject hitEffectPrefab; // 피격 효과 프리팹

    float nextFireTime = 0f; // 다음 발사 가능 시간
    Vector3 originalPosition; // 총의 원래 위치 (반동 계산)
    Quaternion originalRotation; // 총의 원래 회전 (반동 계산)
    Coroutine recoilCoroutine; // 반동 코루틴 참조
    XRBaseController controller; // XR 컨트롤러 진동을 위한 컨트롤러 참조
    //bool isReloading = false; // 재장전 중인지 여부


    private void OnEnable()
    {
        // performed(트리거 당겼을때) 이벤트에 발사 함수 등록
        shootActionReference.action.performed += OnShoot;
        //reloadActionReference.action.performed += OnReload;
    }
    private void OnDisable()
    {
        // performed(트리거 당겼을때) 이벤트에서 발사 함수 해제
        shootActionReference.action.performed -= OnShoot;
        //reloadActionReference.action.performed -= OnReload;
    }
    private void Awake()
    {
        controller = GetComponentInParent<XRBaseController>();
        
    }
    void OnShoot(InputAction.CallbackContext context)
    {
        // 재장전이 아닐때 
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + gunData.fireRate; // 다음 발사 가능 시간 갱신
            // 발사 함수 호출
            Shoot();
        }
    }
    //void OnReload(InputAction.CallbackContext context)
    //{
    //    if (!isReloading && gunData.curAmmo < gunData.maxAmmo)
    //    {
    //        StartCoroutine(ReloadRoutine());
    //    }
    //}
    void Shoot()
    {
        if(gunData.curAmmo <= 0)
        {
            Debug.Log("탄약 부족");
            // 탕! 대신 딸깍 소리 
            if(gunData.emptySound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.emptySound, false);
            return;
        }
        gunData.curAmmo--; // 탄약 감소
        Debug.Log("발사! 남은 탄약: " + gunData.curAmmo);
        // 발사 소리 재생
        if (gunData.shootSound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.shootSound, false);
        // 컨트롤러 진동
        if (controller != null)
        {
            controller.SendHapticImpulse(0.7f, 0.1f);
        }
        // 총구 화염 효과 재생
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (recoilCoroutine != null) StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(Recoil());

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, gunData.range))
        {
            Debug.Log(hit.transform.name + "을(를) 맞췄습니다!");
            // 피격 효과 생성
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
            // 데미지 처리 (적 스크립트가 있다고 가정)
            // Enemy enemy = hit.collider.GetComponent<Enemy>();
            // if (enemy != null)
            // {
            //     enemy.TakeDamage(gunData.damage);
            // }
        }
    }
    //IEnumerator ReloadRoutine()
    //{
    //    isReloading = true;
    //    Debug.Log("재장전 시작");
    //    // 재장전 소리 재생
    //    if (gunData.reloadSound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.reloadSound, false);
    //    yield return new WaitForSeconds(gunData.reloadTime);
    //    // 재장전 완료
    //    gunData.curAmmo = gunData.maxAmmo; // 탄약을 최대치로 채움
    //    Debug.Log("재장전 완료. 현재 탄약: " + gunData.curAmmo);
    //    isReloading = false;
    //}
    IEnumerator Recoil()
    {
        Vector3 recoilPosition = new Vector3(0, 0, -0.2f) * gunData.recoilForce * 0.2f; // 반동 위치 계산
        Quaternion recoilRotation = Quaternion.Euler(5f * gunData.recoilForce, 0, 0); // 반동 회전 계산

        Vector3 targetPosition = originalPosition + recoilPosition;
        Quaternion targetRotation = originalRotation * recoilRotation;

        float elapsed = 0f;
        while (elapsed < gunData.recoilDuration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, elapsed / gunData.recoilDuration);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, elapsed / gunData.recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        float returnDuration = 0.2f; // 원래 위치로 돌아오는 시간
        Vector3 currentPosition = transform.localPosition;
        Quaternion currentRotation = transform.localRotation;
        while (elapsed < returnDuration)
        {
            transform.localPosition = Vector3.Lerp(currentPosition, originalPosition, elapsed / returnDuration);
            transform.localRotation = Quaternion.Slerp(currentRotation, originalRotation, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
    void Start()
    {
        // 시작 시 최대 탄약으로 설정
        gunData.curAmmo = gunData.maxAmmo;
        // 반동 계산을 위한 원래 위치와 회전 저장
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
}
