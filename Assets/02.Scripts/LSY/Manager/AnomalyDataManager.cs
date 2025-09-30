// 파일 이름: AnomalyDataManager.cs
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
            Debug.Log($"기믹 #{anomalyId}번을 처음 발견! 저장합니다.");
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
            Debug.Log("JSON 파일에서 기믹 데이터를 불러왔습니다.");
        }
        else
        {
            // 초기 데이터 생성 (실제 프로젝트에서는 여기에 모든 기믹 데이터를 넣어줘야 해!)
            List<AnomalyData> initialData = new List<AnomalyData>
            {
                new AnomalyData { anomalyId = 1, hasBeenSeen = false },
                new AnomalyData { anomalyId = 2, hasBeenSeen = false },
                new AnomalyData { anomalyId = 3, hasBeenSeen = false }
            };
            anomalyDatabase = initialData.ToDictionary(data => data.anomalyId, data => data);
            SaveAnomalies();
            Debug.Log("초기 기믹 데이터를 생성하고 JSON 파일을 저장했습니다.");
        }
    }

    void SaveAnomalies()
    {
        AnomalyList data = new AnomalyList { anomalies = anomalyDatabase.Values.ToList() };
        string jsonString = JsonUtility.ToJson(data, true);
        File.WriteAllText(jsonPath, jsonString);
    }
}