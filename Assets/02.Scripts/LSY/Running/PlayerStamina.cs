using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("UI ����")]
    public Image stamina; // ���¹̳� UI �̹���

    [Header("���¹̳� ����")]
    public float maxStamina = 100f; // �ִ� ���¹̳�
    public float staminaDrainRate = 20f; // �ʴ� �Ҹ�
    public float staminaRegenRate = 25f; // �ʴ� ȸ����
    public float regenDelay = 1.5f; // �޸��� ���߰� �� �ð��� ������ ȸ�� ����

    // ���� ���׹̳� ��
    private float currentStamina;
    private float regenDelayTimer;
    private bool isRunning;

    // ���� �޸� �� �ִ� ���׹̳ʰ� �ִ��� �˷��ִ� ������Ƽ
    public bool HasStamina => currentStamina > 0;
    void Start()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        if (isRunning)
        {
            // �޸��� ���̸� ���¹̳� �Ҹ�
            currentStamina -= staminaDrainRate * Time.deltaTime;
            // ȸ�� ������ Ÿ�̸Ӵ� ��� 0���� �ʱ�ȭ => �޸��� ������
            regenDelayTimer = 0f;
        }
        else
        {
            // ���������� ȸ�� ������ Ÿ�̸� ����
            regenDelayTimer += Time.deltaTime;

            // ������ �ð��� ����� ������,���¹̳��� �ִ�ġ���� ������ ȸ�� ����
            if(regenDelayTimer >= regenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
        // ���׹̳ʰ� 0�� �ִ�ġ ���̸� ����� �ʵ��� ����
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }
    // �޸��� ���� ���� �޼���
    public void SetRunningState(bool running)
    {
        this.isRunning = running;
    }
    // UI ������Ʈ �޼���
    private void UpdateUI()
    {
        stamina.fillAmount = currentStamina / maxStamina;
    }
}
