using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonflyDamage : MonoBehaviour
{
    private readonly string bulletTag = "BULLET";

    void Start()
    {
    }

    public void OnDamage()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision col)
    {
        print("진입");
        if (col.gameObject.CompareTag(bulletTag))
        {
            print("일치");
            OnDamage();
        }
    }
}
