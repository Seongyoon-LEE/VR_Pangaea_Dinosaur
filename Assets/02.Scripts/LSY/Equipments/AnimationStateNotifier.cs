using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateNotifier : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ZippoController zippo = animator.GetComponentInParent<ZippoController>();
        if (zippo != null)
        {
            zippo.OnAnimationComplete();
        }
    }
}
