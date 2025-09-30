using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;
    [Header("기믹 등장 확률")]
    [Range(0f, 1f)]
    public float anomalyChance = 0.75f;

    int anomalyIdForRound; // 이번 라운드에 등장할 기믹 ID (0 == 없음)
    int stageNumberForRound; // 이번 라운드의 스테이지 번호
    BaseAnomaly currentActivatedAnomaly;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void PlanNewRound()
    {
        anomalyIdForRound = 0;
        stageNumberForRound = -1;

        if (Random.value < anomalyChance)
        {
            var unseenIds = AnomalyDataManager.Instance.anomalyDatabase.Values
                                 .Where(a => !a.hasBeenSeen)
                                 .Select(a => a.anomalyId)
                                 .ToList();
            if (unseenIds.Count > 0)
            {
                anomalyIdForRound = unseenIds[Random.Range(0, unseenIds.Count)];
            }
            else
            {
                var allIds = AnomalyDataManager.Instance.anomalyDatabase.Keys.ToList();
                anomalyIdForRound = allIds[Random.Range(0, allIds.Count)];
            }
            stageNumberForRound = Random.Range(0, 8);
            print($"기믹 #{anomalyIdForRound}번이 스테이지 {stageNumberForRound}에서 등장합니다!");
        }
        else
        {
            print("이번 라운드에는 기믹이 등장하지 않습니다.");
        }
    }
    public void CheckAndActivateAnomaly(int currentPlayerStage)
    {
        if (currentActivatedAnomaly != null)
        {
            currentActivatedAnomaly.EndAnomaly();
            currentActivatedAnomaly.gameObject.SetActive(false);
            currentActivatedAnomaly = null;
        }

        if (anomalyIdForRound != 0 && currentPlayerStage == stageNumberForRound)
        {
            BaseAnomaly[] allAnomaliesInScene = FindObjectsOfType<BaseAnomaly>(true);
            currentActivatedAnomaly = allAnomaliesInScene.FirstOrDefault(a => a.anomalyId == anomalyIdForRound);

            if (currentActivatedAnomaly != null)
            {
                currentActivatedAnomaly.gameObject.SetActive(true);
                currentActivatedAnomaly.StartAnomaly();
                Debug.Log($"[실행!] {currentPlayerStage}번째 스테이지! 기믹 #{anomalyIdForRound}번을 활성화합니다!");
            }
        }
    }

    public void MarkCurrentAnomalyAsSeen()
    {
        if (anomalyIdForRound != 0)
        {
            AnomalyDataManager.Instance.MarkAnomalyAsSeen(anomalyIdForRound);
        }
    }
}

