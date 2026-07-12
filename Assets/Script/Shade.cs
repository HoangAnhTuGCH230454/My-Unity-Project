using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : Enemy
{
    [SerializeField] private float chaseRange;
    [SerializeField] private float stunDuration;
    float timer;
    public static Shade Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        SaveData.saveinstance.SaveShadeData();
    }
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Shade_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Shade_Idle);
        }
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyStates)
        {
            case EnemyStates.Shade_Idle:
                rb.velocity = new Vector2(0, 0);
                if (_dist < chaseRange)
                {
                    ChangeState(EnemyStates.Shade_Chase);
                }
                break;

            case EnemyStates.Shade_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, speed * Time.deltaTime));
                FlipShade();
                if (_dist > chaseRange)
                {
                    ChangeState(EnemyStates.Shade_Idle);
                }
                break;

            case EnemyStates.Shade_Stun:
                timer += Time.deltaTime;
                if (timer > stunDuration)
                {
                    ChangeState(EnemyStates.Shade_Chase);
                    timer = 0;
                }
                break;

            case EnemyStates.Shade_Died:
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
            ChangeState(EnemyStates.Shade_Stun);
        }
        else
        {
            ChangeState(EnemyStates.Shade_Died);
        }
    }

    protected override void ChangeCurrentAnim()
    {
        if (GetCurrentEnemyStates == EnemyStates.Shade_Idle)
        {
            anim.Play("idle");
        }
        anim.SetBool("Walking", GetCurrentEnemyStates == EnemyStates.Shade_Chase);
        anim.SetBool("takeDamage", GetCurrentEnemyStates == EnemyStates.Shade_Stun);

        if (GetCurrentEnemyStates == EnemyStates.Shade_Died)
        {
            anim.SetTrigger("Death");
            PlayerController.Instance.RestoreMana();
            SaveData.saveinstance.SavePlayerData();
            Destroy(gameObject, 0.5f);
        }
    }
    protected override void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
    void FlipShade()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
}
