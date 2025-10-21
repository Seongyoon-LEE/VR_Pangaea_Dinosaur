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
        if (col.gameObject.CompareTag(bulletTag))
        {
            OnDamage();

            var dragonflies = GameObject.FindObjectsByType<DragonflyMove>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None
                );

            if (dragonflies != null)
            {
                foreach (var dragonfly in dragonflies)
                {
                    if (dragonfly != this)
                    {
                        dragonfly.status = DragonflyMove.Status.TRACE;
                    }
                }
            }
        }
    }
}
