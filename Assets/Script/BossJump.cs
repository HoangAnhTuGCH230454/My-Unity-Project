using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJump : StateMachineBehaviour
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
        DiveAttack();
    }
    void DiveAttack()
    {
        if (TheBlindHuntress.instance.DiveAttack)
        {
            TheBlindHuntress.instance.Flip();

            Vector2 _newPos = Vector2.MoveTowards(rb.position, TheBlindHuntress.instance.JumptoPosition, TheBlindHuntress.instance.speed * 3 * Time.deltaTime);
            rb.MovePosition(_newPos);
            float _distance = Vector2.Distance(rb.position, _newPos);
            if (_distance <= 0.1f)
            {
                TheBlindHuntress.instance.Dive();
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
