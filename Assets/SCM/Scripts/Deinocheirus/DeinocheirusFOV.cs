using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeinocheirusFOV : DinoFOV
{
    DeinocheirusCtrl deinocheirus;
    protected override void Start()
    {
        base.Start();
        deinocheirus = GetComponent<DeinocheirusCtrl>();
    }

    protected override void OnPlayerRecognized()
    {
        base.OnPlayerRecognized();
        deinocheirus.FindOut(playerTr);
    }
}
