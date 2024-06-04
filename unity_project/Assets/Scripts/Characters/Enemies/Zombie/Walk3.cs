using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk3 : StateMachineBehaviour
{
    Transform target;
    public float speed = 3;
    Transform borderCheck;
    public Animator animator;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       target = GameObject.FindGameObjectWithTag("Player").transform;
        borderCheck = animator.GetComponent<Zombie>().borderCheck;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       Vector2 newPos = new Vector2(target.position.x, animator.transform.position.y);
        animator.transform.position = Vector2.MoveTowards(animator.transform.position, newPos, speed*Time.deltaTime);
        if(Physics2D.Raycast(borderCheck.position, Vector2.down, 2) == false)
          animator.SetBool("isChasing3", false);
        
        
        float distance = Vector2.Distance(target.position, animator.transform.position);

        if(distance < 3)
            animator.SetBool("isAttack3", true);
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }


}
