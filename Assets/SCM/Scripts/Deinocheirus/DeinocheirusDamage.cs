using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeinocheirusDamage : MonoBehaviour
{
    private readonly string bulletTag = "BULLET";
    Coroutine coroutine = null;
    DeinocheirusCtrl deinocheirus;
    void Start()
    {
        deinocheirus = GetComponent<DeinocheirusCtrl>();
    }


    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(bulletTag))
        {
            if (coroutine != null) return;

            coroutine = StartCoroutine(Stun());
        }
    }

    IEnumerator Stun()
    {
        DeinocheirusCtrl.Status initStatus = deinocheirus.status;
        deinocheirus.status = DeinocheirusCtrl.Status.STUN;

        yield return new WaitForSeconds(3f);

        deinocheirus.status = initStatus;
        coroutine = null;
    }
}
