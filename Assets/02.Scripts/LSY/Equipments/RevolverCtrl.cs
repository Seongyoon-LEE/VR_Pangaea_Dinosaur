using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RevolverCtrl : MonoBehaviour
{
    [Header("�� ������")]
    public GunData gunData;  // �� ������

    [Header("XRI �Է� �׼�")]
    public InputActionReference shootActionReference; // �߻� �Է� �׼�
    //public InputActionReference reloadActionReference; // ������ �Է� �׼�

    [Header("����")]
    public Transform firePoint; // �߻� ��ġ
    //public ParticleSystem muzzleFlash; // �ѱ� ȭ�� ȿ��
    //public GameObject hitEffectPrefab; // �ǰ� ȿ�� ������

    [Header("�ǰ� ����")]
    public LayerMask hittableLayers; // �ǰ� ����Ʈ�� �߻��� ���̾���� ����

    

    float nextFireTime = 0f; // ���� �߻� ���� �ð�
    Vector3 originalPosition; // ���� ���� ��ġ (�ݵ� ���)
    Quaternion originalRotation; // ���� ���� ȸ�� (�ݵ� ���)
    Vector3 originalEulerAngles; // ���� ������ ���Ϸ� �����ε� ����
    Coroutine recoilCoroutine; // �ݵ� �ڷ�ƾ ����
    XRBaseController controller; // XR ��Ʈ�ѷ� ������ ���� ��Ʈ�ѷ� ����
    //bool isReloading = false; // ������ ������ ����
    private void OnEnable()
    {
        // performed(Ʈ���� �������) �̺�Ʈ�� �߻� �Լ� ���
        shootActionReference.action.performed += OnShoot;
        //reloadActionReference.action.performed += OnReload;
    }
    private void OnDisable()
    {
        // performed(Ʈ���� �������) �̺�Ʈ���� �߻� �Լ� ����
        shootActionReference.action.performed -= OnShoot;
        //reloadActionReference.action.performed -= OnReload;
        
    }
    private void Awake()
    {
        controller = GetComponentInParent<XRBaseController>();
        
    }
    void OnShoot(InputAction.CallbackContext context)
    {
        // �������� �ƴҶ� 
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + gunData.fireRate; // ���� �߻� ���� �ð� ����
            // �߻� �Լ� ȣ��
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
            Debug.Log("ź�� ����");
            // ��! ��� ���� �Ҹ� 
            if(gunData.emptySound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.emptySound, false);
            return;
        }
        gunData.curAmmo--; // ź�� ����
        Debug.Log("�߻�! ���� ź��: " + gunData.curAmmo);
        // �߻� �Ҹ� ���
        if (gunData.shootSound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.shootSound, false);
        // ��Ʈ�ѷ� ����
        if (controller != null)
        {
            controller.SendHapticImpulse(0.7f, 0.1f);
        }
        // �ѱ� ȭ�� ȿ�� ���
        var muzzleEffect = EffectPoolingManager.Instance.GetFromPool(EffectPoolingManager.Instance.MuzzleEffectPoolList, EffectPoolingManager.Instance.MuzzleEffectPrefab);
        if (muzzleEffect != null)
        {
            muzzleEffect.transform.position = firePoint.position;
            muzzleEffect.transform.rotation = firePoint.rotation;
            muzzleEffect.SetActive(true);
        }
        if (recoilCoroutine != null) StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(Recoil());

        RaycastHit hit;
        Debug.DrawRay(firePoint.position, firePoint.forward * 100, Color.yellow, 2f);
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, gunData.range, hittableLayers))
        {
            Debug.Log(hit.transform.name + "��(��) ������ϴ�!");
            
            // �ǰ� ȿ�� ����
            var hitEffect = EffectPoolingManager.Instance.GetFromPool(EffectPoolingManager.Instance.bulletHitEffectPoolList, EffectPoolingManager.Instance.bulletHitEffectPrefab);
            if (hitEffect != null)
            {
                hitEffect.transform.position = hit.point;
                hitEffect.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                hitEffect.SetActive(true);
            }
            // ������ ó�� (�� ��ũ��Ʈ�� �ִٰ� ����)
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
    //    Debug.Log("������ ����");
    //    // ������ �Ҹ� ���
    //    if (gunData.reloadSound) SoundManager.s_Instance.PlaySfx(transform.position, gunData.reloadSound, false);
    //    yield return new WaitForSeconds(gunData.reloadTime);
    //    // ������ �Ϸ�
    //    gunData.curAmmo = gunData.maxAmmo; // ź���� �ִ�ġ�� ä��
    //    Debug.Log("������ �Ϸ�. ���� ź��: " + gunData.curAmmo);
    //    isReloading = false;
    //}
    IEnumerator Recoil()
    {
        // ��ǥ ���� = ���� �������� x������ -30�� ���ϱ�
        Vector3 targetEulerAngles = originalEulerAngles + new Vector3(30f, 0, 0);

        // 1. ��ǥ �������� �ݵ� (������)
        float elapsed = 0f;
        while (elapsed < gunData.recoilDuration) // 0.05��
        {
            // ���Ϸ� ������ Slerp�� �����ؼ� ȸ�� ����
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetEulerAngles), elapsed / gunData.recoilDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ȯ���ϰ� ��ǥ ������ ����
        transform.localRotation = Quaternion.Euler(targetEulerAngles);

        // 2. ���� ������ ���� (õõ��)
        elapsed = 0f;
        float returnDuration = 0.2f;
        Quaternion currentRotation = transform.localRotation;
        while (elapsed < returnDuration)
        {
            transform.localRotation = Quaternion.Slerp(currentRotation, originalRotation, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. ���� ȸ�������� �Ϻ��ϰ� ����
        transform.localRotation = originalRotation;
    }
    void Start()
    {
        // ���� �� �ִ� ź������ ����
        gunData.curAmmo = gunData.maxAmmo;
        // �ݵ� ����� ���� ���� ��ġ�� ȸ�� ����
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        originalEulerAngles = transform.localEulerAngles; // ���� ���� ����
    }
}
