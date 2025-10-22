using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DinoFOV : MonoBehaviour
{
    protected readonly string playerTag = "Player";

    [Header("레이어 세팅")]
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    protected Light _light;
    WaitForSeconds ws;
    WaitForSeconds lightSeconds;
    protected Transform playerTr;
    protected bool isRandom = false;
    private Coroutine lightSetupCoroutine = null;
    protected virtual void Start()
    {
        ws = new WaitForSeconds(0.2f);
        lightSeconds = new WaitForSeconds(3f);
        _light = GetComponentInChildren<Light>();
        print(gameObject.name);
        StartCoroutine(FindTargetDelay());
    }


    IEnumerator FindTargetDelay()
    {
        print("인식시작");
        while(true)
        {
            yield return ws;

            if (isRandom && lightSetupCoroutine == null)
            {
                lightSetupCoroutine = StartCoroutine(LightSetup());
            }

            if (!_light.enabled) continue;

            if (IsPlayer()) // 플레이어 인식
            {
                OnPlayerRecognized();
            }
        }
    }

    IEnumerator LightSetup()
    {
        if (Random.value < 0.3f)
        {
            _light.enabled = false;
            OnPlayerLost();
        }
        else
        {
            _light.enabled = true;
        }

        yield return lightSeconds;

        lightSetupCoroutine = null;
    }

    private bool IsPlayer()
    {
        float viewRadius = _light.range;
        float viewAngle = _light.spotAngle;

        Collider[] colliders = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        if (colliders.Length == 0) return false;

        
        foreach (Collider target in colliders)
        {
            Transform targetTr = target.transform;

            if (targetTr.CompareTag(playerTag))
                playerTr = targetTr;

            Vector3 directionToTarget = (targetTr.position - _light.transform.position).normalized;

            // 시야각 확인
            if (Vector3.Angle(_light.transform.forward, directionToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTr.position);

                // Raycast: 장애물에 가려졌는지 확인
                // obstacleMask에 설정된 것(플레이어, 바닥 등 제외한 모든 것)에 부딪히는지 검사
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    // 모든 조건 충족: 플레이어를 인식함
                    return true;
                }
            }
        }
        return false;
    }

    protected virtual void OnPlayerRecognized()
    {
    }

    protected virtual void OnPlayerLost()
    {
    }

    void OnDrawGizmos()
    {
        // Gizmos 로직은 변경 없이 Spot Light과 시야를 일치시킵니다.
        if (_light == null)
        {
            _light = GetComponentInChildren<Light>();
            if (_light == null || _light.type != LightType.Spot) return;
        }

        float gizmoRadius = _light.range;
        float gizmoAngle = _light.spotAngle;

        Gizmos.color = Color.blue;

        // **1. DrawFrustum을 사용하여 원뿔을 그립니다 (가장 정확한 방법)**
        // DrawFrustum은 유니티 카메라의 절두체(Frustum)를 그리는 데 사용되지만,
        // FOV와 Near Clip Plane/Far Clip Plane을 이용하여 원뿔 모양을 정확히 표현할 수 있습니다.

        // DrawFrustum은 Camera FOV(수직 시야각)를 사용하므로, Spot Angle을 수직 FOV로 가정합니다.
        // Near Plane은 0.1f로, Far Plane은 Range로 설정합니다.

        // Gizmos 행렬을 Light의 위치와 회전에 맞춰 설정합니다.
        Gizmos.matrix = _light.transform.localToWorldMatrix;

        // DrawFrustum은 시야각을 전체 각도로 받으며, Light의 Range를 깊이로 사용합니다.
        // 위치는 로컬 원점(0,0,0)에서 시작합니다.
        Gizmos.DrawFrustum(Vector3.zero, gizmoAngle, gizmoRadius, 0.1f, 1f);

        // Gizmos 행렬을 원래대로 되돌립니다 (다른 Gizmos에 영향 방지).
        Gizmos.matrix = Matrix4x4.identity;
    }
}
