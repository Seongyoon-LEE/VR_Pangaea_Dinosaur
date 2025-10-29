using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanboaTrigger : MonoBehaviour
{
    private readonly string playerTag = "Player";
    public GameObject torch;
    public TitanboaCtrl titanboaCtrl;
    void Start()
    {
        titanboaCtrl = transform.parent.GetChild(0).GetComponent<TitanboaCtrl>();
        torch = GameObject.FindWithTag(playerTag).transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).gameObject;
    }

    private void OnTriggerStay(Collider other)
    {
        if (PlayerStateManager.Instance.CurState == PlayerState.Lighter)
        {
            titanboaCtrl.OnBoaTigger(false);
        }
        else if (other.CompareTag(playerTag))
        {
            titanboaCtrl.OnBoaTigger(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            titanboaCtrl.OnBoaTigger(false);
        }
    }

    private bool OnTorch()
    {
        return torch.activeInHierarchy;
    }
}
