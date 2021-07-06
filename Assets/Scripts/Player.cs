using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    public SpriteRenderer sprite;

    public int CurrentLevel = 1;
    public float CurrentXP;
    
    public float moveSpeed;
    public Rigidbody2D rb;

    private Vector2 moveDirection;

    public int MaxHP;
    public int CurrentHP;

    public Animator animator;
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private static readonly int LastHorizontal = Animator.StringToHash("LastHorizontal");
    private static readonly int LastVertical = Animator.StringToHash("LastVertical");
    private static readonly int Magnitude = Animator.StringToHash("Magnitude");
    private static readonly int IsPushing = Animator.StringToHash("IsPushing");

    public float lastHorizontal;
    public float lastVertical;

    public Transform AttackCircle;
    public Transform AttackPos;

    public Weapon EquippedWeapon;
    
    [FormerlySerializedAs("attackRange")] public float attackRadius = .5f;
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int WeaponType = Animator.StringToHash("WeaponType");

    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 50;
        CurrentHP = MaxHP;
        
        EquipWeapon(new Weapon());
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    private void FixedUpdate()
    {
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            IncreaseXP(1);

            lastHorizontal = moveX;
            lastVertical = moveY;
            
            animator.SetFloat(LastHorizontal, lastHorizontal);
            animator.SetFloat(LastVertical, lastVertical);
        }

        // right
        if (lastHorizontal > 0)
        {
            AttackCircle.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        // up
        else if (lastVertical > 0)
        {
            AttackCircle.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        // left
        else if (lastHorizontal < 0)
        {
            AttackCircle.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        // down
        else if (lastVertical < 0)
        {
            AttackCircle.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        moveDirection = new Vector2(moveX, moveY);
        animator.SetFloat(Horizontal, moveX);
        animator.SetFloat(Vertical, moveY);
        animator.SetFloat(Magnitude, moveDirection.magnitude);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger(Attack);
            var enemiesToDamage = Physics2D.OverlapCircleAll(AttackPos.position, attackRadius, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemiesToDamage)
            {
                Debug.Log(enemy.transform.name + " was hit");
                enemy.GetComponent<Enemy>().TakeDamage(10);
            }
        }

    }

    public void TakeDamage(int damage)
    {
        CurrentHP -= damage;        
    }

    public void EquipWeapon(Weapon weapon)
    {
        EquippedWeapon = weapon;
        animator.SetInteger(WeaponType, (int)weapon.WeaponType);
        
        // calculate stat boosts from weapon
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPos.position, attackRadius);
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    public void IncreaseXP(int xpToAdd)
    {
        CurrentXP += xpToAdd;
        CheckLevelUp();
    }

    public void CheckLevelUp()
    {
        if (CurrentXP >= CurrentLevel * 100)
        {
            CurrentXP = 0;
            CurrentLevel++;

            MaxHP = CurrentLevel * 50;
            CurrentHP = MaxHP;
        }
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if (moveDirection.magnitude > 0.01)
        {
            // Debug.Log($"rb.position {rb.position}");
            // Debug.Log($"other.transform.position {other.transform.position}");
            // Debug.Log($"rb.velocity {rb.velocity}");
            // Debug.Log($"rb.velocity.magnitude {rb.velocity.magnitude}");

            if (rb.position.x <= other.transform.position.x && moveDirection.x > 0 ||
                rb.position.x > other.transform.position.x && moveDirection.x < 0 ||
                rb.position.y <= other.transform.position.y && moveDirection.y > 0 ||
                rb.position.y > other.transform.position.y && moveDirection.y < 0)
            {
                animator.SetBool(IsPushing, true);
            }
            else
            {
                animator.SetBool(IsPushing, false);
            }
        }
        else
        {
            animator.SetBool(IsPushing, false);
        }

    }

    public void OnCollisionExit2D(Collision2D other)
    {
        animator.SetBool(IsPushing, false);
    }
}
