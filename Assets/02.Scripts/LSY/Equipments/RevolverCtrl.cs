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
    public ParticleSystem muzzleFlash; // �ѱ� ȭ�� ȿ��
    public GameObject hitEffectPrefab; // �ǰ� ȿ�� ������

    float nextFireTime = 0f; // ���� �߻� ���� �ð�
    Vector3 originalPosition; // ���� ���� ��ġ (�ݵ� ���)
    Quaternion originalRotation; // ���� ���� ȸ�� (�ݵ� ���)
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
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        if (recoilCoroutine != null) StopCoroutine(recoilCoroutine);
        recoilCoroutine = StartCoroutine(Recoil());

        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, firePoint.forward, out hit, gunData.range))
        {
            Debug.Log(hit.transform.name + "��(��) ������ϴ�!");
            // �ǰ� ȿ�� ����
            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
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
        Vector3 recoilPosition = new Vector3(0, 0, -0.2f) * gunData.recoilForce * 0.2f; // �ݵ� ��ġ ���
        Quaternion recoilRotation = Quaternion.Euler(5f * gunData.recoilForce, 0, 0); // �ݵ� ȸ�� ���

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
        float returnDuration = 0.2f; // ���� ��ġ�� ���ƿ��� �ð�
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
        // ���� �� �ִ� ź������ ����
        gunData.curAmmo = gunData.maxAmmo;
        // �ݵ� ����� ���� ���� ��ġ�� ȸ�� ����
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
}
