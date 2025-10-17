using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinosaurusFOV : DinoFOV
{
    SpinosaurusCtrl spinosaurus;
    protected override void Start()
    {
        base.Start();
        spinosaurus = GetComponent<SpinosaurusCtrl>();
    }

    protected override void OnPlayerRecognized()
    {
        base.OnPlayerRecognized();
        print("ÀÎ½Ä");
        spinosaurus.FindOut(playerTr);
    }
}
