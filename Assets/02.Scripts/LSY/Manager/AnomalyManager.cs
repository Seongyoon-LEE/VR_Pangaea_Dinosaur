using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public static AnomalyManager Instance;
    [Header("��� ���� Ȯ��")]
    [Range(0f, 1f)]
    public float anomalyChance = 0.75f;

    int anomalyIdForRound; // �̹� ���忡 ������ ��� ID (0 == ����)
    int stageNumberForRound; // �̹� ������ �������� ��ȣ
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
            print($"��� #{anomalyIdForRound}���� �������� {stageNumberForRound}���� �����մϴ�!");
        }
        else
        {
            print("�̹� ���忡�� ����� �������� �ʽ��ϴ�.");
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
                Debug.Log($"[����!] {currentPlayerStage}��° ��������! ��� #{anomalyIdForRound}���� Ȱ��ȭ�մϴ�!");
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

