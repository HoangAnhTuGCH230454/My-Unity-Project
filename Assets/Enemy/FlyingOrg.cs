using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingOrg : Enemy
{
    [SerializeField] private float chaseRange;
    [SerializeField] private float stunDuration;
    float timer;
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Flyingorg_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Flyingorg_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyStates)
        {
            case EnemyStates.Flyingorg_Idle:
                rb.velocity = new Vector2(0, 0);
                if (_dist < chaseRange)
                {
                    ChangeState(EnemyStates.Flyingorg_Chase);
                }
                break;

            case EnemyStates.Flyingorg_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                FlipOrg();
                if (_dist > chaseRange)
                {
                    ChangeState(EnemyStates.Flyingorg_Idle);
                }
                break;

            case EnemyStates.Flyingorg_Stun:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Flyingorg_Chase);
                    timer = 0;
                }
                break;

            case EnemyStates.Flyingorg_Died:
                Death(Random.Range(5, 10));
                break;
        }
    }

    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 1;
        base.Death(_destroyTime);
    }
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);
        if (health > 0)
        {
            ChangeState(EnemyStates.Flyingorg_Stun);
        }
        else
        {
            ChangeState(EnemyStates.Flyingorg_Died);
        }
    }

    protected override void ChangeCurrentAnim()
    {
        anim.SetBool("idle", GetCurrentEnemyStates == EnemyStates.Flyingorg_Idle);
        anim.SetBool("chase", GetCurrentEnemyStates == EnemyStates.Flyingorg_Chase);
        anim.SetBool("stunned", GetCurrentEnemyStates == EnemyStates.Flyingorg_Stun);

        if (GetCurrentEnemyStates == EnemyStates.Flyingorg_Died)
        {
            anim.SetTrigger("death");
            int LayerIgnore = LayerMask.NameToLayer("Ignore Player");
            gameObject.layer = LayerIgnore;
        }
    }
    void FlipOrg()
    {
        sr.flipX = PlayerController.Instance.transform.position.x > transform.position.x;
    }
}