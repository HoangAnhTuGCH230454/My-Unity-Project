using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLunge : StateMachineBehaviour
{
    Rigidbody2D rb;
    float defaultGravityScale = 1;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb = animator.GetComponentInParent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = 0;
        int _dir = TheBlindHuntress.instance.facingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * (TheBlindHuntress.instance.speed * 5), 0f);
        if (Vector2.Distance(PlayerController.Instance.transform.position, rb.position) <= TheBlindHuntress.instance.attackRange && !TheBlindHuntress.instance.damagePlayer && !PlayerController.Instance.pState.invincible)
        {
            PlayerController.Instance.TakeDamage(TheBlindHuntress.instance.damage);
            if (PlayerController.Instance.pState.alive)
            {
                GameManager.Stop();
            }
            TheBlindHuntress.instance.damagePlayer = true;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.gravityScale = defaultGravityScale;
    }
}
