using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanboaTrigger : MonoBehaviour
{
    private readonly string playerTag = "Player";
    private GameObject torch;
    TitanboaCtrl titanboaCtrl;
    void Start()
    {
        titanboaCtrl = GameObject.Find("Titanoboa").GetComponent<TitanboaCtrl>();
        torch = GameObject.FindWithTag(playerTag).transform.GetChild(0).gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnTorch())
        {
            titanboaCtrl.OnBoaTigger(false);
        }
        else if (other.CompareTag(playerTag))
        {
            titanboaCtrl.OnBoaTigger(true);
        }
    }

    private bool OnTorch()
    {
        return torch.activeInHierarchy;
    }
}
