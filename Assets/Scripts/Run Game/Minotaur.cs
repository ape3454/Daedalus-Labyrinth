using System.Collections;
using UnityEngine;

public class Minotaur : Entity
{
    bool alive;
    public ParticleSystem smokeParticleEffect;
    public ParticleSystem stunnedParticleEffect;

    // Stun Time
    public float timeStunned = 4f;
    public bool stunned;
    float stunCooldown;

    // Player interaction
    Rigidbody2D rigidbody2d;
    Vector2 move;

    // Charging
    [SerializeField]
    int chargeRange = 2;
    float chargeTime;
    bool readyCharging = false;
    bool charging = false;
    float chargeTimer = 0;
    [SerializeField]
    float chargeCooldown = 2f;
    int stoppingDistance = 2;
    Vector2 chargePlayerPosition;
    Vector2 chargeDestination;
    public Vector2 chargeDirection;

    private void Awake()
    {
        playerDetection = transform.GetComponent<EnemyAwarenessOfPlayer>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = 3f;
        maxHealth = 60;
        currentHealth = maxHealth;
        smokeParticleEffect.Pause();
        stunnedParticleEffect.Pause();
    }

    public void resetRun()
    {
        RunReset();
    }

    protected override void RunReset()
    {
        moveDirection = Vector2Int.up;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerDetection.awareOfPlayer)
        {

        }
        else
        {
            move = playerDetection.enemyToPlayerVector;

            if (move.magnitude <= chargeRange && chargeTimer <= 0 && !readyCharging && !charging)
            {
                StartCoroutine(Charge(move));
                chargePlayerPosition = move;
                move = Vector2.zero;
            }
            else if (readyCharging)
            {
                RaycastHit2D rayToPlayer = Physics2D.Raycast(transform.position, chargeDirection, (chargeDestination - (Vector2)transform.position - chargeDirection * stoppingDistance).magnitude, 3);
                if (rayToPlayer)
                {
                    readyCharging = false;
                    StopCoroutine(Charge(chargePlayerPosition));
                    StopCharging();
                }
            }
            else if (stunned)
            {
                stunnedParticleEffect.Play();
                stunCooldown -= Time.deltaTime;
                charging = false;
                if (stunCooldown < 0)
                {
                    stunned = false;
                    stunnedParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
            }
            else if (!charging)
            {
                if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
                {
                    moveDirection.Set(move.x, move.y);
                    moveDirection.Normalize();
                }
                animator.SetFloat("Look X", moveDirection.x);
                animator.SetFloat("Look Y", moveDirection.y);
                animator.SetFloat("Speed", move.magnitude);
            }

            if (chargeTime > 0)
            {
                chargeTime -= Time.deltaTime;
            }
        }
    }

    private IEnumerator Charge(Vector2 position)
    {
        readyCharging = true;
        smokeParticleEffect.Play();

        yield return new WaitForSeconds(1.5f);

        speed = 6f;
        charging = true;
        readyCharging = false;
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
            chargeDirection = moveDirection;
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        chargeDestination = (Vector2)transform.position + move + chargeDirection * stoppingDistance;

        yield return new WaitUntil(() => (Vector2)transform.position == chargeDestination | !charging);
        animator.SetFloat("Speed", 0f);

        StopCharging();
        speed = 3f;
        smokeParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
    }

    void FixedUpdate()
    {
        if (playerDetection.awareOfPlayer | charging)
        {
            if (charging && !stunned)
            {
                Vector2 position = Vector2.MoveTowards(transform.position, chargeDestination, speed * Time.deltaTime);
                rigidbody2d.MovePosition(position);
            }
            else if (!stunned && !readyCharging)
            {
                Vector2 position = rigidbody2d.position + move.normalized * speed * Time.deltaTime * ((chargeTime < chargeCooldown - 1f) ? 1 : -1);
                rigidbody2d.MovePosition(position);
                animator.SetBool("Stunned", false);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (charging && other.gameObject.layer == 3)
        {
            StopCharging();
            stunned = true;
            stunCooldown = timeStunned;
            animator.SetBool("Stunned", true);
        }
        if (rigidbody2d.linearVelocity.sqrMagnitude == 0)
        {
            move = ((stunned) ? chargeDirection : Vector2.one) * -1 * speed;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && charging)
        {
            if (other.name != "Player")
            {
                other.GetComponentInParent<PlayerController>().ChangeHealth(-1);
            }
            else
            {
                other.GetComponent<PlayerController>().ChangeHealth(-1);
            }
            StopCharging();
        }
    }

    private void StopCharging()
    {
        chargeTime = chargeCooldown;
        charging = false;
    }
}