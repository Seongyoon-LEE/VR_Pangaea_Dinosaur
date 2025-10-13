using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    ParticleSystem ps;
    WaitForSeconds wait = new WaitForSeconds(0.3f);
    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }
    void OnEnable()
    {
        StartCoroutine(CheckIfAlive());
    }
    IEnumerator CheckIfAlive()
    {
        while(ps.IsAlive(true)) 
        {
            yield return wait; // 0.3초
        }
        gameObject.SetActive(false); // 재생이 끝나면 풀에 반납
    }
}
