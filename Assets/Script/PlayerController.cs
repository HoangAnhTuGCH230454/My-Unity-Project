using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Terresquall;
public class PlayerController : PersistentObject
{
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f;
    private int jumpBufferCounter = 0;
    [SerializeField] private int jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    private float gravity;
    [SerializeField] private float maxFallSpeed;
    [Space(5)]

    [Header("Wall Jump Setting")]
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask walllayer;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower;
    float wallJumpingDirection;
    bool isSliding;
    bool isWallJumping;
    [Space(5)]

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask whatisGround;
    [Space(5)]

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    private bool canDash = true, dashed;
    [Space(5)]

    [Header("Attack Setting")]
    [SerializeField] private Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;
    [SerializeField] private Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage;
    private float timeBetweenAttack, timeSinceAttack;
    [SerializeField] private GameObject[] slashEffects;
    [SerializeField] GameObject Slash;

    private int currentSlashIndex = 0;

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Recoil")]
    [SerializeField] private int recoilXStep = 5;
    [SerializeField] private int recoilYStep = 5;
    [SerializeField] private float recoilXSpeed = 100;
    [SerializeField] private float recoilYSpeed = 100;
    private int stepXRecoiled, stepYRecoiled;
    [Space(5)]

    [Header("Health setting")]
    public int health;
    public int maxHealth;
    public int maxTotalHealth = 10;
    public int excessHealth = 0;
    public int heartShards;
    [Min(1)] public int heartShardsPerHealth = 4;
    [SerializeField] GameObject bloodSplit;
    [SerializeField] float hitFlashSpeed;
    [System.Obsolete] public delegate void OnHealthChangeDelegate();
    [System.Obsolete] public OnHealthChangeDelegate OnHealthChangedCallback;
    [SerializeField] GameObject HealingVFX;
    private GameObject activeHealingVFX;

    float healTimer;
    [SerializeField] float timetoHeal;
    [Space(5)]

    [Header("Mana Setting")]
    public float mana = 3;
    public float maxMana = 3;
    [Range(0, 1)] public float manaPenalty = 0f;

    [Header("Excess Mana Settings")]
    public float excessMana = 0;
    public int excessMaxManaUnits = 0, excessMaxManaUnitsLimit = 3;
    public float manaPerExcessUnit = 1f;
    [SerializeField] float excessManaRestoreDelay = 3f, excessManaRestoreRate = 1f;
    float excessManaRestoreCooldown = 0f;

    public int manaShards = 0;
    [Min(1)] public int manaShardsPerExcessUnit = 4;
    [Space(5)]

    [Header("Spell Setting")]
    [SerializeField] float attackManaGain = 0.34f;
    [SerializeField] float healManaCostPerSec = 1f;
    [SerializeField] float manaSpellCost = 0.3f;
    [SerializeField] float timeBetweenCast;
    [SerializeField] float spellDamage;
    [SerializeField] float downSpellForce;
    [SerializeField] GameObject UpSpellCast;
    [SerializeField] GameObject SideSpellCast;
    [SerializeField] GameObject DownSpellFinished;
    float timeSinceCast;
    float castorhealTimer;
    [Space(5)]

    [Header("Audio Setting")]
    [SerializeField] AudioClip landingSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip dashSound;
    [SerializeField] AudioClip attackSound;
    [SerializeField] AudioClip hurtSound;
    [SerializeField] AudioClip sideSpellSound;
    [SerializeField] AudioClip upSpellSound;
    [SerializeField] AudioClip deathSound;

    [Header("Camera Setting")]
    [SerializeField] private float playerFallSpeedTheshold = -10;

    [HideInInspector] public PlayerStateList pState;
    public Rigidbody2D rb;
    private float xAxis;
    private float yAxis;
    Animator anim;
    private bool attack = false;
    private SpriteRenderer sr;
    public ParticleSystem dust;

    private bool landingSoundisPlayed;
    private AudioSource audioSources;

    public static PlayerController Instance;
    bool openInventory;

    [System.Flags]
    public enum Abilities : byte
    {
        dash = 1,
        dbJump = 2,
        wallJump = 4,
        upCast = 8,
        sideCast = 16
    }
    [Header("Misc")]
    public Abilities abilities;

    [System.Flags]
    public enum State
    {
        jumping = 1, dashing = 2, recoilingX = 4, recoilingY = 8,
        lookingRight = 16, invincible = 32, healing = 64, casting = 128,
        cutscene = 256, alive = 512
    }

    public State state;
    public bool Is(State s) {  return state.HasFlag(s); }
    public void Set(State s, bool on)
    {
        if (on)
        {
            state |= s;
        }
        else
        {
            state &= ~s;
        }
    }
    public bool Toggle(State s)
    {
        state ^= s;
        return Is(s);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        { Destroy(gameObject); }
        else
        { Instance = this; }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        audioSources = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        pState.dashing = false;
        Mana = mana;
        Health = maxHealth;
        if (health > 0)
        {
            Set(State.alive, true);
        }

        UIManager.UpdateHealthUI(health, maxHealth, excessHealth);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    void HandleRestoreManaWithExcess()
    {
        if (excessManaRestoreCooldown > 0)
        {
            excessManaRestoreCooldown -= Time.deltaTime;
        }
        else if (Mana < maxMana && excessMana > 0f)
        {
            float restoreAmount = Mathf.Min(excessMana, excessManaRestoreRate * Time.deltaTime);
            Mana += restoreAmount;
            excessMana -= restoreAmount;
        }
    }

    void Update()
    {
        if (Is(State.cutscene) || GameManager.Instance.isPaused) return;

        if (Is(State.alive))
        {
            HandleRestoreManaWithExcess();
            GetInputs();
            ToggleInventory();
            UpdateJumpVariables();
            UpdateCameraYDampingforPlayerFall();
            FlashWhileInvincible();
        }
        else
        {
            return;
        }

        if (Is(State.dashing)) return;


        if (Is(State.alive))
        {
            Heal();
            CastSpell();
        }
        

        if (Is(State.healing)) return;
        if (Is(State.alive))
        {
            if (!isWallJumping)
            {
                Move();
                Flip();
                Jump();
            }
            if (abilities.HasFlag(Abilities.wallJump))
            {
                WallSlide();
                WallJump();
            }
            if (abilities.HasFlag(Abilities.dash))
            {
                StartDash();
            }
            Attack();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.GetComponent<Enemy>() != null && Is(State.casting))
        {
            _other.GetComponent<Enemy>().EnemyHit(spellDamage, (_other.transform.position - transform.position).normalized, -recoilYSpeed);
        }

    }

    private void FixedUpdate()
    {
        if (Is(State.cutscene)) return;
        if (Is(State.dashing)) return;
        Recoil();
    }

    void GetInputs()
    {
        if (GameManager.Instance.isPaused || GameManager.isStopped) 
        { 
            return;
        }
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
        openInventory = Input.GetButton("Inventory");

        if (Input.GetButton("Cast/Heal"))
        {
            castorhealTimer += Time.deltaTime;
        }
        else
        {
            castorhealTimer = 0;
        }
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    private void Move()
    {
        if (Is(State.healing)) rb.velocity = new Vector2(0, 0);
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);
        if (xAxis != 0) Flip();

        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    void UpdateCameraYDampingforPlayerFall()
    {
        if (rb.velocity.y < playerFallSpeedTheshold && !CameraManager.Instance.isLerpingYDamp && !CameraManager.Instance.hasLerpingYDamp)
        {
            StartCoroutine(CameraManager.Instance.LerpYDamping(true));
        }
        else if (rb.velocity.y >= 0 && !CameraManager.Instance.isLerpingYDamp && CameraManager.Instance.hasLerpingYDamp)
        {
            CameraManager.Instance.hasLerpingYDamp = false;
            StartCoroutine(CameraManager.Instance.LerpYDamping(false));
        }
    }

    void ToggleInventory()
    {
        if (openInventory)
        {
            UIManager.Instance.inventory.SetActive(true);
        }
        else
        {
            UIManager.Instance.inventory.SetActive(false);
        }
    }
    void Flip()
    {
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
            if (Grounded())
            {
                dust.Play();
            }
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
            if (Grounded())
            {
                dust.Play();
            }
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        anim.SetTrigger("Dashing");
        audioSources.PlayOneShot(dashSound);
        rb.gravityScale = 0;
        int dir_ = Is(State.lookingRight) ? 1 : -1;
        rb.velocity = new Vector2(dashSpeed * dir_, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pState.dashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    public IEnumerator WalktoScene(Vector2 _exitDir, float _delay)
    {
        if (_exitDir.y > 0)
        {
            rb.velocity = jumpForce * _exitDir;
        }

        if (_exitDir.x != 0)
        {
            xAxis = _exitDir.x > 0 ? 1 : -1;
            Move();
        }
        Flip();
        yield return new WaitForSeconds(_delay);
        pState.cutscene = false;
    }
    void Attack()
    {
        timeSinceAttack += Time.deltaTime;
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0;

            GameObject currentSlash = slashEffects[currentSlashIndex];
            audioSources.PlayOneShot(attackSound);

            currentSlashIndex++;
            if (currentSlashIndex >= slashEffects.Length)
            {
                currentSlashIndex = 0;
            }

            if (yAxis == 0 || (yAxis < 0 && Grounded()))
            {
                int recoilLeftorRight = Is(State.lookingRight) ? 1 : -1;
                Hit(SideAttackTransform, SideAttackArea, ref pState.recoilingX, Vector2.right * recoilLeftorRight, recoilXSpeed);
                Instantiate(currentSlash, SideAttackTransform);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea, ref pState.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(currentSlash, 80, UpAttackTransform);
            }
            else if (yAxis < 0 && !Grounded())
            {
                Hit(DownAttackTransform, DownAttackArea, ref pState.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(currentSlash, -90, DownAttackTransform);
            }
        }
    }

    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        List<Enemy> hitEnemies = new List<Enemy>();

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true;
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy e = objectsToHit[i].GetComponent<Enemy>();
            if (e && !hitEnemies.Contains(e))
            {
                e.EnemyHit(damage, _recoilDir, _recoilStrength);
                hitEnemies.Add(e);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    Mana += attackManaGain;
                }
            }
        }
    }

    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        GameObject spawnedSlash = Instantiate(_slashEffect, _attackTransform);
        spawnedSlash.transform.localEulerAngles = new Vector3(0, 0, _effectAngle);
    }

    void Recoil()
    {
        if (Is(State.recoilingX))
        {
            if (Is(State.lookingRight))
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            else
                rb.velocity = new Vector2(recoilXSpeed, 0);
        }
        if (Is(State.recoilingY))
        {
            rb.gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
            }
            airJumpCounter = 0;
        }
        else
        {
            rb.gravityScale = gravity;
        }
        if (Is(State.recoilingX) && stepXRecoiled < recoilXStep)
        {
            stepXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (Is(State.recoilingY) && stepYRecoiled < recoilYStep)
        {
            stepYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }
        if (Grounded())
        {
            StopRecoilY();
        }
    }

    void StopRecoilX()
    {
        stepXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepYRecoiled = 0;
        pState.recoilingY = false;
    }

    public void TakeDamage(float _damage)
    {
        if (Is(State.alive))
        {
            audioSources.PlayOneShot(hurtSound);

            if (excessHealth > 0)
            {
                if (excessHealth > -damage)
                {
                    excessHealth -= Mathf.RoundToInt(_damage);
                    _damage = 0;
                }
                else
                {
                    _damage -= excessHealth;
                    excessHealth = 0;
                }
            }

            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        anim.SetTrigger("takeDamage");
        GameObject _bloodSplitParticle = Instantiate(bloodSplit, transform.position, Quaternion.identity);
        Destroy(_bloodSplitParticle, 1f);
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }

    void FlashWhileInvincible()
    {
        sr.color = Is(State.invincible) ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }

    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        GameObject _bloodSplitParticle = Instantiate(bloodSplit, transform.position, Quaternion.identity);
        Destroy(_bloodSplitParticle, 1f);
        anim.SetTrigger("Death");
        audioSources.PlayOneShot(deathSound);
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        GetComponent<BoxCollider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        UIManager.Instance.deathScreen.Activate();
        yield return new WaitForSeconds(1f);
        Instantiate(GameManager.Instance.Shade, transform.position, Quaternion.identity);
        Terresquall.LightSpot.SaveGame();
    }
    public void Respawn(float manaPenalty = 0.5f)
    {
        if (!Is(State.alive))
        {
            if (rb)
            {
                rb.constraints = RigidbodyConstraints2D.None;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
            if (anim)
            {
                anim.Play("Player_Idle");
            }

            GetComponent<BoxCollider2D>().enabled = true;
            pState.alive = true;

            this.manaPenalty = manaPenalty;
            mana = excessMana = 0;
            UIManager.UpdateManaUI(mana, maxMana, excessMana, ExcessMaxMana, 1f - manaPenalty);

            Health = maxHealth;
        }
    }

    public void RestoreMana()
    {
        manaPenalty = 0f;
    }
    public int Health
    {
        get { return health; }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                UIManager.UpdateHealthUI(health, maxHealth, excessHealth);
            }
        }
    }

    public int ExcessHealth
    {
        get { return excessHealth; } 
        set 
        {
            if (excessHealth != value)
            {
                excessHealth = Mathf.Max(value, 0);
                UIManager.UpdateHealthUI(health, maxHealth, excessHealth);
            }
        }
    }

    public void ConvertHeartShards()
    {
        int remainingUnits = maxTotalHealth - maxHealth;
        if (heartShards >= heartShardsPerHealth && remainingUnits > 0)
        {
            int awardedUnits = Mathf.Min(remainingUnits, heartShards / heartShardsPerHealth);
            maxHealth += awardedUnits;
            heartShards -= awardedUnits * heartShardsPerHealth;

            UIManager.UpdateHealthUI(health, maxHealth, excessHealth);
        }
    }

    void Heal()
    {
        if (Input.GetButton("Cast/Heal") && castorhealTimer > 0.05 && Health < maxHealth && Mana > 0 && Grounded() && !Is(State.dashing))
        {
            pState.healing = true;
            rb.velocity = Vector2.zero;
            if (activeHealingVFX == null)
            {
                activeHealingVFX = Instantiate(HealingVFX, transform);
            }

            healTimer += Time.deltaTime;
            if (healTimer >= timetoHeal)
            {
                Health++;
                healTimer = 0;
            }

            Mana -= Time.deltaTime * healManaCostPerSec;
        }
        else
        {
            pState.healing = false;
            healTimer = 0;
            if (activeHealingVFX != null)
            {
                Destroy(activeHealingVFX);
                activeHealingVFX = null;
            }
        }
    }

    public float Mana
    {
        get { return mana; }
        set
        {
            float excess = value - MaxMana;
            if (excess > 0)
            {
                mana = MaxMana;
                excessMana += excess;
            }
            else
            {
                if (value < mana)
                {
                    excessManaRestoreCooldown = excessManaRestoreDelay;
                }
                mana = Mathf.Max(0, value);
            }
            UIManager.UpdateManaUI(mana, maxMana, excessMana, ExcessMaxMana, 1- manaPenalty);
        }
    }

    public float MaxMana
    {
        get { return maxMana * (1 - manaPenalty); }
    }

    public float ExcessMaxMana
    {
        get { return excessMaxManaUnits * manaPerExcessUnit; }
    }

    public void ConvertManaShards()
    {
        int remainingUnits = excessMaxManaUnitsLimit - excessMaxManaUnits;
        if (manaShards >= manaShardsPerExcessUnit && remainingUnits > 0)
        {
            int awardedUnits = Mathf.Min(remainingUnits, manaShards / manaShardsPerExcessUnit);

            excessMaxManaUnits += awardedUnits;
            manaShards -= awardedUnits * manaShardsPerExcessUnit;

            UIManager.UpdateManaUI(mana, maxMana, excessMana, ExcessMaxMana);
        }
    }

    void CastSpell()
    {
        if (Input.GetButtonUp("Cast/Heal") && castorhealTimer <= 0.05f && Mana >= manaSpellCost && timeSinceCast >= timeBetweenCast)
        {
            pState.casting = true;
            timeSinceCast = 0;
            StartCoroutine(CastCoroutine());
        }
        else
        {
            timeSinceCast += Time.deltaTime;
        }
    }

    IEnumerator CastCoroutine()
    {
        

        if ((yAxis == 0 || (yAxis < 0 && Grounded())) && abilities.HasFlag(Abilities.sideCast))
        {   
            audioSources.PlayOneShot(sideSpellSound);
            anim.SetBool("Casting", true);
            yield return new WaitForSeconds(0.15f);
            GameObject _fireBall = Instantiate(SideSpellCast, SideAttackTransform.position, Quaternion.identity);
            
            if (Is(State.lookingRight))
            {
                _fireBall.transform.eulerAngles = Vector3.zero;
            }
            else
            {
                _fireBall.transform.eulerAngles = new Vector2(_fireBall.transform.eulerAngles.x, 180);
            }
            pState.recoilingX = true;
            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.25f);

        }

        else if (yAxis > 0 && abilities.HasFlag(Abilities.upCast))
        {
            audioSources.PlayOneShot(upSpellSound);
            Instantiate(UpSpellCast, transform);
            rb.velocity = Vector2.zero;
            Mana -= manaSpellCost;
            yield return new WaitForSeconds(0.25f);
        }

        else if (yAxis < 0 && !Grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, -downSpellForce);
            if (Grounded())
            {
                Instantiate(DownSpellFinished, transform.position, Quaternion.identity);
            }
        }
        anim.SetBool("Casting", false);
        pState.casting = false;
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatisGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatisGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatisGround))
            return true;
        else
            return false;
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            if (!landingSoundisPlayed)
            {
                audioSources.PlayOneShot(landingSound);
                landingSoundisPlayed = true;
            }
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            landingSoundisPlayed = false;
        }

        if (Input.GetButtonDown("Jump"))
            jumpBufferCounter = jumpBufferFrames;
        else
            jumpBufferCounter--;
    }

    private bool Wall()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, walllayer);
    }

    void WallSlide()
    {
        if (Wall() && !Grounded() && xAxis != 0)
        {
            isSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlideSpeed, float.MaxValue));
        }
        else
        {
            isSliding = false;
        }
    }
    void WallJump()
    {
        if (isSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = !Is(State.lookingRight) ? 1 : -1;
            CancelInvoke(nameof(StopWallJump));
        }
        if (Input.GetButtonDown("Jump") && isSliding)
        {
            audioSources.PlayOneShot(jumpSound);
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingPower.x * wallJumpingDirection, wallJumpingPower.y);
            dashed = false;
            airJumpCounter = 0;
            float jumpDirection = Toggle(State.lookingRight) ? 0 : 180;
            transform.eulerAngles = new Vector2(transform.eulerAngles.x, jumpDirection);
            Invoke(nameof(StopWallJump), wallJumpingDuration);
        }
    }
    void StopWallJump()
    {
        isWallJumping = false;
        transform.eulerAngles = new Vector2(transform.eulerAngles.x, 0);
    }

    public override PersistentObject.SaveData Save() 
    {
        SaveData playerData = new SaveData();
        if (CanSave())
        {
            return new SaveData
            {
                saveID = saveID,
                Health = Health,
                maxHealth = maxHealth,
                maxTotalHealth = maxTotalHealth,
                heartShards = heartShards,

                Mana = Mana,
                manaPenalty = manaPenalty,
                manaOrbs = excessMaxManaUnits,
                orbShards = manaShards,

                unlocks = (byte)abilities,

                position = transform.position,

                lastScene = SceneManager.GetActiveScene().name,
            };
        }
        return null;
    }

    public override bool Load(PersistentObject.SaveData data)
    {
        if (data == null)
        {
            return false;
        }
        SaveData playerData = data as SaveData;
        if (playerData == null)
        {
            return false;
        }

        Health = playerData.Health;
        maxHealth = playerData.maxTotalHealth;
        heartShards = playerData.heartShards;

        manaPenalty = playerData.manaPenalty;
        excessMaxManaUnits = playerData.manaOrbs;
        manaShards = playerData.orbShards;
        Mana = playerData.Mana;

        abilities = (Abilities)playerData.unlocks;

        transform.position = playerData.position;
        return true;
    }

    [System.Serializable]
    public new class SaveData : PersistentObject.SaveData
    {
        public float positionX, positionY, positionZ;
        public float manaPenalty;
        public int Health;
        public int maxHealth;
        public int maxTotalHealth;
        public int heartShards;
        public float Mana;
        public int manaOrbs;
        public int orbShards;
        public byte unlocks;
        public string lastScene;

        public Vector3 position
        {
            get { return new Vector3(positionX, positionY, positionZ); }
            set
            {
                positionX = value.x;
                positionY = value.y;
                positionZ = value.z;
            }
        }
    }

    void Jump()
    {

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !Is(State.jumping))
        {
            if (Input.GetButtonDown("Jump"))
            {
                audioSources.PlayOneShot(jumpSound);
            }

            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            pState.jumping = true;
            jumpBufferCounter = 0;
        }
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump") && abilities.HasFlag(Abilities.dbJump))
        {
            audioSources.PlayOneShot(jumpSound);
            pState.jumping = true;
            airJumpCounter++;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            dust.Play();
            rb.velocity = new Vector2(rb.velocity.x, 0);
            pState.jumping = false;
        }
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallSpeed, rb.velocity.y));

        anim.SetBool("Jumping", !Grounded());
    }
}