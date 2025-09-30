using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    public int currentStage = 0;
    void Start()
    {
        // 게임 시작 새라운드 기획 요청
        AnomalyManager.Instance.PlanNewRound();

        // 0번 스테이지에 대한 기믹 체크 
        AnomalyManager.Instance.CheckAndActivateAnomaly(currentStage);
    }
    // 다음 스테이지로 갈때 이 함수 호출
    public void GoToNextStage()
    {
        currentStage++;
        print($"플레이어가 {currentStage}번 스테이지로 이동했습니다.");
        AnomalyManager.Instance.CheckAndActivateAnomaly(currentStage);

        if(currentStage >= 8)
        {
            Debug.Log("한라운드 끝. 새 라운드 시작");
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
