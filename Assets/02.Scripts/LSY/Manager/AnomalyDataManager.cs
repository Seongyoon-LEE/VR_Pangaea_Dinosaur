// ���� �̸�: AnomalyDataManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AnomalyDataManager : MonoBehaviour
{
    public static AnomalyDataManager Instance;

    public Dictionary<int, AnomalyData> anomalyDatabase = new Dictionary<int, AnomalyData>();
    private string jsonPath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            jsonPath = Path.Combine(Application.persistentDataPath, "Anomalies.json");
            LoadAnomalies();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MarkAnomalyAsSeen(int anomalyId)
    {
        if (anomalyDatabase.ContainsKey(anomalyId) && !anomalyDatabase[anomalyId].hasBeenSeen)
        {
            anomalyDatabase[anomalyId].hasBeenSeen = true;
            Debug.Log($"��� #{anomalyId}���� ó�� �߰�! �����մϴ�.");
            SaveAnomalies();
        }
    }

    void LoadAnomalies()
    {
        if (File.Exists(jsonPath))
        {
            string jsonString = File.ReadAllText(jsonPath);
            List<AnomalyData> dataList = JsonUtility.FromJson<AnomalyList>(jsonString).anomalies;
            anomalyDatabase = dataList.ToDictionary(data => data.anomalyId, data => data);
            Debug.Log("JSON ���Ͽ��� ��� �����͸� �ҷ��Խ��ϴ�.");
        }
        else
        {
            // �ʱ� ������ ���� (���� ������Ʈ������ ���⿡ ��� ��� �����͸� �־���� ��!)
            List<AnomalyData> initialData = new List<AnomalyData>
            {
                new AnomalyData { anomalyId = 1, hasBeenSeen = false },
                new AnomalyData { anomalyId = 2, hasBeenSeen = false },
                new AnomalyData { anomalyId = 3, hasBeenSeen = false }
            };
            anomalyDatabase = initialData.ToDictionary(data => data.anomalyId, data => data);
            SaveAnomalies();
            Debug.Log("�ʱ� ��� �����͸� �����ϰ� JSON ������ �����߽��ϴ�.");
        }
    }

    void SaveAnomalies()
    {
        AnomalyList data = new AnomalyList { anomalies = anomalyDatabase.Values.ToList() };
        string jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonPath, jsonString);
    }
}