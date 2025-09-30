using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzalcoatlusFOV : MonoBehaviour
{
    private readonly string playerTag = "Player";

    QuetzalcoatlusCtrl player;
    void Start()
    {
        player = transform.root.GetComponent<QuetzalcoatlusCtrl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            player.FindOut(other.transform);
            gameObject.SetActive(false);
        }
    }
}
