using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Enemy
{
    [SerializeField] private float flipTime;
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private LayerMask whatIsGround;

    float timer;
    private Animator anim;

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
        anim = GetComponent<Animator>();
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        switch (GetCurrentEnemyStates)
        {
            case EnemyStates.Mushroom_Idle:
                anim.SetBool("isWalking", true);

                Vector3 _LedgeCheckStart = transform.localScale.x > 0
                    ? new Vector3(ledgeCheckX, 0)
                    : new Vector3(-ledgeCheckX, 0);
                Vector2 _wallCheckDir = transform.localScale.x > 0
                    ? transform.right
                    : -transform.right;

                if (!Physics2D.Raycast(transform.position + _LedgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    ChangeState(EnemyStates.Mushroom_Flip);
                }

                rb.velocity = transform.localScale.x > 0
                    ? new Vector2(speed, rb.velocity.y)
                    : new Vector2(-speed, rb.velocity.y);
                break;

            case EnemyStates.Mushroom_Flip:
                anim.SetBool("isWalking", false);

                timer += Time.deltaTime;
                if (timer > flipTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Mushroom_Idle);
                }
                break;
        }
    }
}