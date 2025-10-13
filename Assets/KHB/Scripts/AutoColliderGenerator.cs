using UnityEngine;
using UnityEditor;

/// <summary>
/// 씬 내 오브젝트에 자동으로 Collider를 추가하는 유틸리티
/// - 벽, 바닥, 천장은 BoxCollider
/// - 계단, 복잡한 구조물은 MeshCollider
/// - 난간, 문 등은 BoxCollider + isTrigger 가능
/// </summary>
public class AutoColliderGenerator : EditorWindow
{
    [MenuItem("Tools/Auto Collider Generator")]
    public static void ShowWindow()
    {
        GetWindow<AutoColliderGenerator>("Auto Collider Generator");
    }

    private bool includeInactive = false;
    private bool overwriteExisting = false;

    void OnGUI()
    {
        GUILayout.Label(" Auto Collider Generator", EditorStyles.boldLabel);
        includeInactive = EditorGUILayout.Toggle("Include Inactive Objects", includeInactive);
        overwriteExisting = EditorGUILayout.Toggle("Overwrite Existing Colliders", overwriteExisting);

        if (GUILayout.Button("Generate Colliders"))
        {
            GenerateColliders();
        }
    }

    private void GenerateColliders()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(includeInactive);
        int addedCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<MeshFilter>() == null) continue; // 메시 없는 오브젝트는 스킵
            if (!overwriteExisting && obj.GetComponent<Collider>() != null) continue;

            string name = obj.name.ToLower();
            Collider col = obj.GetComponent<Collider>();

            if (col != null && overwriteExisting)
                DestroyImmediate(col);

            // 벽, 바닥, 천장
            if (name.Contains("wall") || name.Contains("floor") || name.Contains("ceiling"))
            {
                obj.AddComponent<BoxCollider>();
                addedCount++;
                continue;
            }

            // 계단 / 브릿지 등 복잡한 구조물
            if (name.Contains("stair") || name.Contains("bridge") || name.Contains("ramp"))
            {
                MeshCollider meshCol = obj.AddComponent<MeshCollider>();
                meshCol.convex = false;
                addedCount++;
                continue;
            }

            // 난간 / 문 / 트리거 객체
            if (name.Contains("rail") || name.Contains("door"))
            {
                BoxCollider triggerCol = obj.AddComponent<BoxCollider>();
                triggerCol.isTrigger = true;
                addedCount++;
                continue;
            }

            // 기본값: BoxCollider
            obj.AddComponent<BoxCollider>();
            addedCount++;
        }

        Debug.Log($" Auto Collider Generator 완료! 총 {addedCount}개 Collider 추가됨.");
    }
}
