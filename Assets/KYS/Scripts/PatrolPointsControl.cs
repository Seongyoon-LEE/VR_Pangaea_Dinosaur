using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

public class PatrolPointsControl : MonoBehaviour
{
    List<Transform> points;
    public int idx;
    private void Awake()
    {
        points = GetComponentsInChildren<Transform>().ToList();
        this.transform.parent = null;
        points.RemoveAt(0);
        this.idx = 0;
    }
    public Vector3 GetPoint()
    {
        return points[this.idx].position;
    }
    public Vector3 GetNextPoint()
    {
        this.idx++;
        if (this.idx >= points.Count)
        {
            this.idx = 0;
        }
        return points[this.idx].position;
    }
}
