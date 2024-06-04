using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack1 : StateMachineBehaviour
{
    Transform target;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      target = GameObject.FindGameObjectWithTag("Player").transform;    //find the player
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       float distance = Vector2.Distance(target.position, animator.transform.position);
       if(distance > 4)
            animator.SetBool("isAttack1", false);          //if the distance is greater than 4, the goblin will stop attacking
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }


}
