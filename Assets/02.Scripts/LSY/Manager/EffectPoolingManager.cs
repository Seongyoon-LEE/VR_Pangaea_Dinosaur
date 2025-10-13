using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolingManager : MonoBehaviour
{
    public static EffectPoolingManager Instance;

    [Header("Hit Effect Pooling")]
    public GameObject MuzzleEffectPrefab;
    public int MuzzleEffectPoolSize = 5;
    public List<GameObject> MuzzleEffectPoolList = new List<GameObject>();

    [Header("Break Effect Pooling")]
    public GameObject bulletHitEffectPrefab;
    public int bulletHitEffectPoolSize = 3;
    public List<GameObject> bulletHitEffectPoolList = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 히트 이펙트 풀 초기화
        InitializePool(MuzzleEffectPrefab, MuzzleEffectPoolSize, MuzzleEffectPoolList);
        print(" 힛 이펙트 풀링");
        // 브레이크 이펙트 풀 초기화
        InitializePool(bulletHitEffectPrefab, bulletHitEffectPoolSize, bulletHitEffectPoolList);
    }
    void InitializePool(GameObject prefab, int poolSize, List<GameObject> poolList)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            poolList.Add(obj);
        }
    }
    public GameObject GetFromPool(List<GameObject> pool, GameObject prefab)
    {
        foreach (var obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        // 풀에 사용 가능한 오브젝트가 없으면 새로 생성
        GameObject newObj = Instantiate(prefab, transform);
        pool.Add(newObj);
        return newObj;
    }
}