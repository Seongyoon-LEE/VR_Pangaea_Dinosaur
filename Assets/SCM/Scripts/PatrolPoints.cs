using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoints : MonoBehaviour
{

    public float radius = 0.1f;
    public Color color = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        for (int i = 0; i < GetWayCount(); i++)
        {
            int j = CurrentWayPoint(i);
            Gizmos.DrawSphere(GetWayPoint(i), radius);
            Gizmos.DrawLine(GetWayPoint(i), GetWayPoint(j));
        }
    }

    public Vector3 GetWayPoint(int idx)
    {
        return transform.GetChild(idx).position;
    }

    public int CurrentWayPoint(int idx)
    {
        if (transform.childCount == idx + 1)
            return 0;

        return idx + 1;
    }

    public int GetWayCount()
    {
        return transform.childCount;
    }
}

