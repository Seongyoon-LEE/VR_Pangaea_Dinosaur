// 파일 이름: AnomalyData.cs
using System;
using System.Collections.Generic;

// 개별 데이터 그릇
[Serializable]
public class AnomalyData
{
    public int anomalyId;
    public bool hasBeenSeen;
}

// 데이터 목록을 담는 상자
[Serializable]
public class AnomalyList
{
    public List<AnomalyData> anomalies;
}