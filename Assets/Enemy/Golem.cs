using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float LungeSpeedMultiplier;
    [SerializeField] private float LungeLength;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;

    float timer;

    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Golem_Idle);
        rb.gravityScale = 12f;
    }

    protected override void Update()
    {
        base.Update();

        if (!PlayerController.Instance.Is(PlayerController.State.alive))
        {
            ChangeState(EnemyStates.Golem_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
        Vector3 _LedgeCheckStart = transform.localScale.x > 0
                    ? new Vector3(ledgeCheckX, 0)
                    : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0
                    ? transform.right
                    : -transform.right;
        switch (GetCurrentEnemyStates)
        {
            case EnemyStates.Golem_Idle:
                if (!Physics2D.Raycast(transform.position + _LedgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {

                }

                rb.velocity = transform.localScale.x > 0
                    ? new Vector2(speed, rb.velocity.y)
                    : new Vector2(-speed, rb.velocity.y);
                break;

            case EnemyStates.Golem_Suprise:
                rb.velocity = new Vector2(0, jumpForce);

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _LedgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Golem_Attack);
                }

                break;

            case EnemyStates.Golem_Attack:
                timer += Time.deltaTime;

                if(timer < LungeLength)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * LungeSpeedMultiplier, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * LungeSpeedMultiplier, rb.velocity.y);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Golem_Idle);
                }
                break;
        }
    }

    protected override void ChangeCurrentAnim()
    {
        if (GetCurrentEnemyStates == EnemyStates.Golem_Idle)
        {
            anim.speed = 1;
        }

        if (GetCurrentEnemyStates == EnemyStates.Golem_Attack)
        {
            anim.speed = LungeSpeedMultiplier;
        }
    }
}
