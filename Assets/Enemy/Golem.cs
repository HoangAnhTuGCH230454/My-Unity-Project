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
            anim.Play("Died");
            Death(0.83f);
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
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                rb.velocity = transform.localScale.x > 0
                    ? new Vector2(speed, rb.velocity.y)
                    : new Vector2(-speed, rb.velocity.y);

                RaycastHit2D _playerDetect = Physics2D.Raycast(transform.position + _LedgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_playerDetect.collider != null && _playerDetect.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Golem_Suprise);
                }
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

                if (timer < LungeLength)
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
            anim.Play("Walk");
            anim.speed = 1;
        }
        else if (GetCurrentEnemyStates == EnemyStates.Golem_Suprise)
        {
            anim.Play("Golem");
            anim.speed = 1;
        }
        else if (GetCurrentEnemyStates == EnemyStates.Golem_Attack)
        {
            anim.Play("Attack");
            anim.speed = LungeSpeedMultiplier;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            anim.Play("Hit");
            StopCoroutine(nameof(ResumeAnimAfterHit));
            StartCoroutine(ResumeAnimAfterHit());
        }
    }

    private IEnumerator ResumeAnimAfterHit()
    {
        yield return new WaitForSeconds(recoilLength);
        ChangeCurrentAnim();
    }

    protected override void Death(float _destroyTime)
    {
        base.Death(0.83f);
    }
}