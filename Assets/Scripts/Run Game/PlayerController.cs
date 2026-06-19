using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : Entity
{


    // Control
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public InputAction InteractAction;
    private List<string> talkInteractions = new List<string>()
    {

    };
    private List<string> changeInteractions = new List<string>()
    {
        "Pedestal",
        "Minotaur"
    };
    private List<string> inspectInteractions = new List<string>()
    {

    };

    // I-Frames
    public float timeInvincible = 0.5f;
    bool isInvincible;
    float damageCooldown;

    public List<string> inventory = new List<string>();

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MoveAction.Enable();
        InteractAction.Enable();

        maxHealth = 3;
        currentHealth = maxHealth;
        speed = 5f;
    }

    public void resetRun()
    {
        RunReset();
    }

    protected override void RunReset()
    {
        UIHandler.instance.SetHealthValue(maxHealth);
        UIHandler.instance.UIReset();
        moveDirection = Vector2.up;
    }

    // Update is called once per frame
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            moveDirection.Set(move.x, move.y);
            moveDirection.Normalize();
        }
        animator.SetFloat("Look X", moveDirection.x);
        animator.SetFloat("Look Y", moveDirection.y);
        animator.SetFloat("Speed", move.magnitude);

        if (isInvincible)
        {
            damageCooldown -= Time.deltaTime;
            if (damageCooldown < 0)
            {
                isInvincible = false;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position, moveDirection, 1f, LayerMask.GetMask("Interactable") | LayerMask.GetMask("Collision"));
        if (hit)
        {
            if (talkInteractions.Contains(hit.transform.name))
            {
                UIHandler.instance.SetInteraction("Talk", "Talk");
            }
            if (changeInteractions.Contains(hit.transform.name))
            {
                Debug.Log(hit.transform.name);
                switch (hit.transform.name)
                {
                    case "Pedestal":
                        if (inventory.FindAll(y => y.Substring(0, 4) == "coin").Count == 4)
                        {
                            UIHandler.instance.SetInteraction("Change", "Assemble Coin");
                            UIHandler.instance.ElementSetVisible("Interaction");
                        }
                        break;
                    case "Minotaur":
                        Minotaur minotaurScript = hit.transform.GetComponent<Minotaur>();
                        Animator minotaurAnimator = hit.transform.GetComponent<Animator>();
                        Vector2 minotaurDirection = minotaurScript.chargeDirection;
                        if (minotaurScript.stunned && Vector2.Dot(minotaurDirection, (hit.transform.position - transform.position).normalized) >= 0.7f && inventory.Contains("sword"))
                        {
                            UIHandler.instance.SetInteraction("Change", "Stab Minotaur in the Back");
                            UIHandler.instance.ElementSetVisible("Interaction");
                        }
                        break;
                }
            }
            if (inspectInteractions.Contains(hit.transform.name))
            {
                UIHandler.instance.SetInteraction("Inspect", "Read");
            }

            if (InteractAction.WasPressedThisFrame())
            {
                switch (hit.transform.name)
                {
                    case "Pedestal":
                        if (inventory.FindAll(y => y.Substring(0, 4) == "coin").Count == 4)
                        {
                            inventory.RemoveAll(y => y.Substring(0, 4) == "coin");
                            StartCoroutine(hit.transform.GetComponent<Pedestal>().CreateCoin());
                            UIHandler.instance.ElementSetVisible("Interaction", false);
                            // assembled coin controller creates a path
                        }
                        break;
                    case "Minotaur":
                        Minotaur minotaurScript = hit.transform.GetComponent<Minotaur>();
                        Animator minotaurAnimator = hit.transform.GetComponent<Animator>();
                        Vector2 minotaurDirection = new Vector2(minotaurAnimator.GetFloat("Look X"), minotaurAnimator.GetFloat("Look Y"));
                        if (minotaurScript.stunned && Vector2.Dot(minotaurDirection, transform.position.normalized) >= 0.7f && inventory.Contains("sword"))
                        {
                            UIHandler.instance.DisplayEndScreen(true);
                        }
                        break;
                }
            }
        }
        else UIHandler.instance.ElementSetVisible("Interaction", false);
    }

    void FixedUpdate()
    {
        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }

    public void AddToInventory(GameObject other)
    {
        if (inventory.Count == 0) UIHandler.instance.ElementSetVisible("Inventory");
        inventory.Add(other.name);
        if (other.name == "sword")
        {
            animator.SetTrigger("HasSword");
        }
        UIHandler.instance.ElementSetVisible(other.name);
    }

    public override void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if (isInvincible) return;
            isInvincible = true;
            damageCooldown = timeInvincible;
            // animator.setTrigger("Hit");
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHandler.instance.SetHealthValue(currentHealth);

        if (currentHealth == 0)
        {
            UIHandler.instance.DisplayEndScreen(false);
        }
    }
}
