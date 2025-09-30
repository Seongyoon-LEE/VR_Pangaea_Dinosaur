using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public int currentStage = 0;
    void Start()
    {
        // ���� ���� ������ ��ȹ ��û
        AnomalyManager.Instance.PlanNewRound();

        // 0�� ���������� ���� ��� üũ 
        AnomalyManager.Instance.CheckAndActivateAnomaly(currentStage);
    }
    // ���� ���������� ���� �� �Լ� ȣ��
    public void GoToNextStage()
    {
        currentStage++;
        print($"�÷��̾ {currentStage}�� ���������� �̵��߽��ϴ�.");
        AnomalyManager.Instance.CheckAndActivateAnomaly(currentStage);

        if(currentStage >= 8)
        {
            Debug.Log("�Ѷ��� ��. �� ���� ����");
            currentStage = 0;
            AnomalyManager.Instance.PlanNewRound();
            AnomalyManager.Instance.CheckAndActivateAnomaly(currentStage);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N))
        {
            GoToNextStage();
        }
    }
}
