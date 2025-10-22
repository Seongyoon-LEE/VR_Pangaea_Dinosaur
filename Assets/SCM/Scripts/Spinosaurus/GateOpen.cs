using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    Animator animator;
    WaitForSeconds openSeconds;
    WaitForSeconds ws;
    SpinosaurusCtrl spinosaurus;
    Vector3 initPosition;
    float curTime = 0f;
    void Start()
    {
        animator = GetComponent<Animator>();
        openSeconds = new WaitForSeconds(180f);
        ws = new WaitForSeconds(0.3f);
        spinosaurus = GetComponent<SpinosaurusCtrl>();
        initPosition = transform.position;
        StartCoroutine(GateOepn());
    }

    IEnumerator GateOepn()
    {
        yield return openSeconds;

        if (Random.value >= 0.3f)
        {
            StartCoroutine(GateOepn());
            yield break;
        }
        // 오픈 애니메이션
        // 사이렌 소리
        // 스피노 등장
        StartCoroutine(RiseDinoWater());
    }

    IEnumerator RiseDinoWater()
    {
        while(true)
        {
            curTime += Time.deltaTime;
            yield return ws;

            // 서서히 올라오게

            // 스피노 물 밖 등장
            if (curTime - Time.deltaTime >= 30f)
            {
                spinosaurus.DinoAppeared();
                yield break;
            }
        }
    }

    public void GateClose()
    {
        curTime = 0f;
        StopAllCoroutines();
        StartCoroutine(GateOepn());
        transform.position = initPosition;
        // 철창 닫는 애니메이션
        // 사이렌 종료
    }
}
