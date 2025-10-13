using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("UI 설정")]
    public Image stamina; // 스태미나 UI 이미지

    [Header("스태미나 설정")]
    public float maxStamina = 100f; // 최대 스태미나
    public float staminaDrainRate = 20f; // 초당 소모량
    public float staminaRegenRate = 25f; // 초당 회복량
    public float regenDelay = 1.5f; // 달리기 멈추고 이 시간이 지나야 회복 시작

    // 현재 스테미너 값
    private float currentStamina;
    private float regenDelayTimer;
    private bool isRunning;

    // 현재 달릴 수 있는 스테미너가 있는지 알려주는 프로퍼티
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
            // 달리는 중이면 스태미나 소모
            currentStamina -= staminaDrainRate * Time.deltaTime;
            // 회복 딜레이 타이머는 계속 0으로 초기화 => 달리고 있으니
            regenDelayTimer = 0f;
        }
        else
        {
            // 멈춰있으면 회복 딜레이 타이머 증가
            regenDelayTimer += Time.deltaTime;

            // 딜레이 시간이 충분히 지나고,스태미나가 최대치보다 작으면 회복 시작
            if(regenDelayTimer >= regenDelay && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
        // 스테미너가 0과 최대치 사이를 벗어나지 않도록 고정
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateUI();
    }
    // 달리기 상태 설정 메서드
    public void SetRunningState(bool running)
    {
        this.isRunning = running;
    }
    // UI 업데이트 메서드
    private void UpdateUI()
    {
        stamina.fillAmount = currentStamina / maxStamina;
    }
}
