// ���� �̸�: AnomalyData.cs
using System;
using System.Collections.Generic;

// ���� ������ �׸�
[Serializable]
public class AnomalyData
{
    public int anomalyId;
    public bool hasBeenSeen;
}

// ������ ����� ��� ����
[Serializable]
public class AnomalyList
{
    public List<AnomalyData> anomalies;
}