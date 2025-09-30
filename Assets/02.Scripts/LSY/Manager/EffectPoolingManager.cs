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

        // ��Ʈ ����Ʈ Ǯ �ʱ�ȭ
        InitializePool(MuzzleEffectPrefab, MuzzleEffectPoolSize, MuzzleEffectPoolList);
        print(" �� ����Ʈ Ǯ��");
        // �극��ũ ����Ʈ Ǯ �ʱ�ȭ
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
        // Ǯ�� ��� ������ ������Ʈ�� ������ ���� ����
        GameObject newObj = Instantiate(prefab, transform);
        pool.Add(newObj);
        return newObj;
    }
}