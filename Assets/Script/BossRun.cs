using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    Rigidbody2D rb;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        TargetPlayerPosition(animator);
        if (TheBlindHuntress.instance.countdownAttack <= 0)
        {
            TheBlindHuntress.instance.AttackHandle();
            TheBlindHuntress.instance.countdownAttack = Random.Range(TheBlindHuntress.instance.attackTimer - 1, TheBlindHuntress.instance.attackTimer + 1);
        }
    }

    void TargetPlayerPosition(Animator animator)
    {
        if (TheBlindHuntress.instance.Grounded())
        {
            TheBlindHuntress.instance.Flip();
            Vector2 _target = new Vector2(PlayerController.Instance.transform.position.x, rb.position.y);
            Vector2 _newPos = Vector2.MoveTowards(rb.position, _target, TheBlindHuntress.instance.runSpeed * Time.deltaTime);
            rb.MovePosition(_newPos);
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, -25);
        }
        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= TheBlindHuntress.instance.attackRange)
        {
            animator.SetBool("Run", false);
        }
        else
        {
            return;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("Run", false);
    }
}
