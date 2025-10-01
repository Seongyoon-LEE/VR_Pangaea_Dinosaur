using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanboaTrigger : MonoBehaviour
{
    private readonly string playerTag = "Player";
    
    TitanboaCtrl titanboaCtrl;
    void Start()
    {
        titanboaCtrl = GameObject.Find("Titanoboa").GetComponent<TitanboaCtrl>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            // �� �����̴� �޼��� �ҷ�����
            titanboaCtrl.OnBoaTigger();
        }
    }
}
