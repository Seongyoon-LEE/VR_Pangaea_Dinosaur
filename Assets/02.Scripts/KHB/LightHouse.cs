using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHouse : MonoBehaviour
{

    [Header("회전 속도 (도/초)")]
    public float rotationSpeed = 300f; // 초당 회전 속도

    void Update()
    {
        // Y축 기준으로 일정 속도 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }
}
