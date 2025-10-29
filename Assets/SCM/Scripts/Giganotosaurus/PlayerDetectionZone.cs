using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionZone : MonoBehaviour
{
    private readonly string playerTag = "Player";
    GiganotosaurusCtrl giganotosaurus;

    void Start()
    {
        giganotosaurus = GameObject.Find("Giganotosaurus").GetComponent<GiganotosaurusCtrl>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            giganotosaurus.FindOut(other.gameObject);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            giganotosaurus.FindOut(null);
        }
    }
}
