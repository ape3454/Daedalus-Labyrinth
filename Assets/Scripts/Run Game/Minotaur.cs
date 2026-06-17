using System.Collections;
using UnityEngine;

public class Minotaur : Entity
{
    bool alive;
    public ParticleSystem smokeParticleEffect;
    public ParticleSystem stunnedParticleEffect;

    // Stun Time
    public float timeStunned = 2f;
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
    Vector2 chargeDirection;

    private void Awake()
    {
        playerDetection = transform.GetComponent<EnemyAwarenessOfPlayer>();
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = 2f;
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
            }
            else if (chargeTime > 0)
            {
                chargeTime -= Time.deltaTime;
            }
            else if (readyCharging)
            {
                RaycastHit2D rayToPlayer = Physics2D.Raycast(transform.position, chargeDirection, (chargeDestination - (Vector2)transform.position - chargeDirection * stoppingDistance).magnitude, 3);
                if (rayToPlayer)
                {
                    readyCharging = false;
                    StopCoroutine(Charge(chargePlayerPosition));
                }
            }
            else if (stunned)
            {
                stunnedParticleEffect.Play();
                stunCooldown -= Time.deltaTime;
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
        }
    }

    private IEnumerator Charge(Vector2 position)
    {
        readyCharging = true;
        smokeParticleEffect.Play();

        yield return new WaitForSeconds(1.5f);

        Debug.Log("CHARGE!");
        speed = 8f;
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

        Debug.Log("Fin");
        chargeTime = chargeCooldown;
        charging = false;
        speed = 2f;
        smokeParticleEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        
    }

    void FixedUpdate()
    {
        if (playerDetection.awareOfPlayer)
        {
            if (charging)
            {
                Vector2 position = Vector2.MoveTowards(transform.position, chargeDestination, speed * Time.deltaTime);
                rigidbody2d.MovePosition(position);
            }
            else if (!stunned && !readyCharging)
            {
                Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
                rigidbody2d.MovePosition(position);
                if ((Vector2)rigidbody2d.position != position)
                {

                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (charging && other.gameObject.layer == 3)
        {
            Debug.Log("ooh stunned");
            charging = false;
            stunned = true;
            stunCooldown = timeStunned;
            if (rigidbody2d.linearVelocity.sqrMagnitude == 0)
            {
                move *= -1;
                Debug.Log("do this");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().ChangeHealth(-2);
        }
    }
}