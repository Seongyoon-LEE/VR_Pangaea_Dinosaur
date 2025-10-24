using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuetzalcoatlusFOV : DinoFOV
{
    

    QuetzalcoatlusCtrl quetzalcoatlus;
    protected override void Start()
    {
        base.Start();
        quetzalcoatlus = transform.root.GetComponent<QuetzalcoatlusCtrl>();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag(playerTag))
    //    {
    //        player.FindOut(other.transform);
    //        gameObject.SetActive(false);
    //    }
    //}

    protected override void OnPlayerRecognized()
    {
        base.OnPlayerRecognized();
        print("Ã£À½");
        quetzalcoatlus.FindOut(playerTr);
        _light.enabled = false;
    }
}
