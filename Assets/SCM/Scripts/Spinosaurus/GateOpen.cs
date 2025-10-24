using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateOpen : MonoBehaviour
{
    private readonly int hashOpen = Animator.StringToHash("IsOpen");
    private readonly int hashClose = Animator.StringToHash("Close");
    Animator animator;

    WaitForSeconds openSeconds;
    WaitForSeconds ws;
    SpinosaurusCtrl spinosaurus;
    Coroutine upDownCoroutine;

    public bool isCloseTest = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        openSeconds = new WaitForSeconds(5f);
        ws = new WaitForSeconds(0.3f);
        spinosaurus = GameObject.FindFirstObjectByType<SpinosaurusCtrl>();
        StartCoroutine(GateOpon());
    }

    private void Update()
    {
        if (isCloseTest)
        {
            StartCoroutine(GateClose());
        }
    }
    IEnumerator GateOpon()
    {
        while (true)
        {
            print("열림?");
            yield return openSeconds;
            if (Random.value <= 0.8f)
            {
                print("열림!");
                // 오픈 애니메이션
                animator.SetBool(hashOpen, true);
                // 사이렌 소리
                // 스피노 등장
                upDownCoroutine = StartCoroutine(spinosaurus.DinoUpAndDown(true));
                yield break;
            }
        }
    }


    IEnumerator GateClose()
    {
        isCloseTest = false;
        if (upDownCoroutine != null)
        {
            StopCoroutine(upDownCoroutine);
            upDownCoroutine = null;
        }

        yield return null;

        upDownCoroutine = StartCoroutine(spinosaurus.DinoUpAndDown(false));

        yield return new WaitForSeconds(5f);

        // 철창 닫는 애니메이션
        animator.SetBool(hashOpen, false);
        animator.SetTrigger(hashClose);

        // 사이렌 종료

        StartCoroutine(GateOpon());
        
    }
}
