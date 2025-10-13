using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Fov : MonoBehaviour
{
    // 시야 영역의 반지름과 시야 각도
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    // 마스크 2종
    public LayerMask targetMask, obstacleMask;

    // Target mask에 ray hit된 transform을 보관하는 리스트
    //public List<Transform> visibleTargets = new List<Transform>();
    public float meshResolution;

    Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        StartCoroutine(FindTargetsWithDelay(0.2f));
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    void FindVisibleTargets()
    {
        //visibleTargets.Clear();
        // viewRadius를 반지름으로 한 원 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.transform.position);

                // 타겟으로 가는 레이캐스트에 obstacleMask가 걸리지 않으면
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    //visibleTargets.Add(target);
                    //반복문 빠져나가고 해당 타겟을 추적 시작
                    break;
                    //아마 navMesh사용할거같은데 그거에 맞춰서 추적 시작하도록
                }
            }
        }

    }
    void LateUpdate()
    {
        DrawFieldOfView();
    }
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    void DrawFieldOfView()
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution); //보는 각에 mesh해상도로 잡은 값 곱하기, 그리고 반올림 : 그려질 삼각형의 개수
        float stepAngleSize = viewAngle / stepCount; // 그걸 보는각에다 나눔 : 그려질 삼각형의 각도
        List<Vector3> viewPoints = new List<Vector3>(); // 정점들 모음
        //ViewCastInfo prevViewCast = new ViewCastInfo(); // 이전에 본 각도, 미리 만들어놓음

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i; // 인스펙터 창 각도에 y, 그리고 보는 각에 절반을 뺀거
                                                                                       // -> 전체 보는각의 맨 왼쪽, 시작각도
                                                                                       // 거기에 그려질 삼각형의 각도의 i배 만큼 오른쪽으로 옮겨서 각도를 구한다
            ViewCastInfo newViewCast = ViewCast(angle); // 해당 각도를 통해 ViewCastInfo, 선의 형태를 가져온다

            // i가 0이면 prevViewCast에 아무 값이 없어 정점 보간을 할 수 없으므로 건너뛴다.
            /*if (i != 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold; // 이전 선과 이번 선의 길이가 미리 잡아놓은
                                                                                                                // 보정용 수치보다 크면 서로 다른 장애물에 맞았다고
                                                                                                                // 판단

                // 둘 중 한 raycast가 장애물을 만나지 않았거나 두 raycast가 서로 다른 장애물에 hit 된 것이라면(edgeDstThresholdExceed 여부로 계산)
                if (prevViewCast.hit != newViewCast.hit || (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                {
                    Edge e = FindEdge(prevViewCast, newViewCast);

                    // zero가 아닌 정점을 추가함
                    if (e.PointA != Vector3.zero)
                    {
                        viewPoints.Add(e.PointA);
                    }

                    if (e.PointB != Vector3.zero)
                    {
                        viewPoints.Add(e.PointB);
                    }
                }
            }*/

            viewPoints.Add(newViewCast.point);
            //prevViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1; //내 좌표, 삼각형 시작점을 포함해야해서 1개 추가
        Vector3[] vertices = new Vector3[vertexCount]; // 정점 모음, 꼭짓점들
        int[] triangles = new int[(vertexCount - 2) * 3]; //삼각형의 꼭짓점 index들 모음(반드시 3배수 여야함)
        vertices[0] = Vector3.zero; // 0,0,0 : mesh시작점, 내 좌표
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]); // 위에서 그려진 선 viewPoint를 이용해 정점의 위치를 구함
            if (i < vertexCount - 2) // 삼각형을 그릴건데 정점 모음(vertices)에서 삼각형의 꼭짓점의 index를 넣음
            {
                triangles[i * 3] = 0; // 맨 시작점, 내 좌표
                triangles[i * 3 + 1] = i + 1; //왼쪽거
                triangles[i * 3 + 2] = i + 2; //오른쪽거
            }
        }
        viewMesh.Clear(); //매쉬 지웠다가
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles; // 위에서 잡아놓은거 넣고
        viewMesh.RecalculateNormals(); // 다시 그림

        // 그 후 이걸 update등에서 반복해서 돌리면 시야가 표현됨
    }
    /*public struct Edge
    {
        public Vector3 PointA, PointB;
        public Edge(Vector3 _PointA, Vector3 _PointB)
        {
            PointA = _PointA;
            PointB = _PointB;
        }
    }
    Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = minAngle + (maxAngle - minAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new Edge(minPoint, maxPoint);
    }*/
    public struct ViewCastInfo // 그려질 선의 정보
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }
    ViewCastInfo ViewCast(float globalAngle)// 각도를 받아 삼각형의 형태를 뽑아내는 함수
    {
        Vector3 dir = DirFromAngle(globalAngle/*, true*/); //각도를 넘겨 해당 각도의 선을 가져온다
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask)) // 그 선을 통해 ray를 쏴본다,
                                                                                         // 맞는 경우와 안맞는 경우를 bool값으로 미리 구분해놓는다
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle); // 어디에 맞으면 딱 거기까지만
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle); // 안맞으면 잡아놨던 반지름 길이 전부 다
        }
        
    }
    // y축 오일러 각을 3차원 방향 벡터로 변환한다.
    public Vector3 DirFromAngle(float angleDegrees/*, bool angleIsGlobal*/)
    {
        /*if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }*/

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
}
