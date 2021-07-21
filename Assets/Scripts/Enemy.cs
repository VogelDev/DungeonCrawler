using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public float maxSpeed;
    public float maxForce;

    public Rigidbody2D rb;

    public Transform attackCircle;
    public Transform attackPos;
    public float attackRadius;

    public Animator animator;

    public float maxHp = 50 ;
    public float currentHp;
    private static readonly int AttackAnimation = Animator.StringToHash("Attack");

    public Vector2 currentVelocity; 
    private bool _stopWalking = false;

    public float AttackAngle = 0;

    [FormerlySerializedAs("VisionRange")] public float EnemyVisionRange;
    public float MemoryTime;

    public float WallVisionRange = 2;

    private float playerLastSeen = float.MinValue;

    private float lastHorizontal = 0;
    private float lastVertical = 0;
    private float wanderTheta;

    public bool canSeePlayer;
    public bool canSeeWall;
    
    

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;
        _stopWalking = false;
        
        lastHorizontal = Random.Range(-1.0f, 1.0f);
        lastVertical = Random.Range(-1.0f, 1.0f);
        rb.velocity = new Vector2(lastHorizontal, lastVertical).normalized * maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_stopWalking)
        {
            // rb.velocity = Seek(GameScript.Player.transform.position);
            if (currentVelocity == Vector2.zero)
            {
                RandomizeVelocity();
            }
            if (CanSeeObject("Player", EnemyVisionRange))
            {
                canSeePlayer = true;
                playerLastSeen = Time.time;
                rb.velocity = Seek(GameScript.Player.transform.position);
            }
            else if (Time.time < playerLastSeen + MemoryTime)
            {
                canSeePlayer = false;
                // Debug.Log(Time.time);
                // Debug.Log(playerLastSeen);
                // still remembers seeing the player, keep walking strait
                // do I really not need to do anything here? We want the velocity to stay the same.
            }
            else if (CanSeeObject("Wall", WallVisionRange))
            {
                canSeeWall = true;
                // rb.velocity += Vector2.Perpendicular(rb.velocity) * (Random.value < .5 ? -1 : 1);
                
                rb.velocity += Vector2.Perpendicular(rb.velocity);
            }
            else
            {
                canSeeWall = false;
                Wander();
            }
            //
            // rb.velocity = rb.velocity.normalized * maxSpeed;
            
            currentVelocity = rb.velocity;
            
            // TODO: Refactor this
            lastHorizontal = rb.velocity.x;
            lastVertical = rb.velocity.y;
            
            AttackAngle = (float) (Math.Atan2(lastVertical, lastHorizontal) / (Math.PI) * 180) + 90;
            
            attackCircle.transform.rotation = Quaternion.Euler(0, 0, AttackAngle);
            animator.SetFloat("Horizontal", lastHorizontal);
            animator.SetFloat("Vertical", lastVertical);

        }

        Attack(GameScript.Player);
    }

    private void RandomizeVelocity()
    {
        lastHorizontal = Random.Range(-1.0f, 1.0f);
        lastVertical = Random.Range(-1.0f, 1.0f);
        rb.velocity = new Vector2(lastHorizontal, lastVertical).normalized * maxSpeed;
        currentVelocity = rb.velocity;
        
    }

    private void Wander()
    {
        var wanderR = 24;
        var wanderD = 120;
        var change = 0.2f;
        
        wanderTheta += Random.Range(-change, change);

        var pos = currentVelocity.normalized;
        pos *= wanderD;
        pos += pos;

        var h = Math.Atan2(currentVelocity.x,currentVelocity.y);
        var offSet = new Vector2((float)(wanderR * Math.Cos(wanderTheta * h)), (float)(wanderR * Math.Sin(wanderTheta * h)));

        var target = pos + offSet;
        var force = Seek(target).normalized * (maxForce / 2);

        rb.velocity = force;
    }

    private bool CanSeeObject(string tag, float range)
    {
        var hit = RayCastColliders(range);

        if (hit != null)
        {
            if (hit.gameObject.CompareTag(tag))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private Collider2D RayCastColliders(float range)
    {
        var endPos = (attackPos.position - attackCircle.position) * range + attackPos.position;

        var hit = Physics2D.Linecast(attackCircle.position, endPos, 1 << LayerMask.NameToLayer("Default"));
        
        Debug.DrawLine(attackCircle.position, endPos, Color.red);

        return hit.collider;

    }

    private void Attack(Player player)
    {
        
        var enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRadius, LayerMask.GetMask("Default"));
        if (enemiesToDamage.Length <= 0)
        {
            _stopWalking = false;
        }
        foreach (var enemy in enemiesToDamage)
        {
            if(!enemy.gameObject.CompareTag("Player")) continue;
            
            _stopWalking = true;
            enemy.GetComponent<Player>().TakeDamage(10);
            animator.SetTrigger(AttackAnimation);
        }
    }

    private Vector2 Seek(Vector3 target)
    {
        var force = target - transform.position;
        force.Normalize();
        force *= maxSpeed;
        var velocity = rb.velocity;
        force -= new Vector3(velocity.x, velocity.y);
        force.Normalize();
        force *= maxForce;
        
        return force;
    }
    
    private void Seek(Player player)
    {
        var force = player.transform.position - transform.position;
        force.Normalize();
        force *= maxSpeed;
        var velocity = rb.velocity;
        force -= new Vector3(velocity.x, velocity.y);
        force.Normalize();
        force *= maxForce;
        rb.velocity = force;

        currentVelocity = rb.velocity;

        var lastHorizontal = rb.velocity.x;
        var lastVertical = rb.velocity.y;
        animator.SetFloat("Horizontal", lastHorizontal);
        animator.SetFloat("Vertical", lastVertical);

        AttackAngle = (float) (Math.Atan2(lastVertical, lastHorizontal) / (Math.PI) * 180) + 90;

        attackCircle.transform.rotation = Quaternion.Euler(0, 0, AttackAngle);
        
        // // right
        // if (lastHorizontal > 0)
        // {
        // }
        // // up
        // else if (lastVertical > 0)
        // {
        //     attackCircle.transform.rotation = Quaternion.Euler(0, 0, 180);
        // }
        // // left
        // else if (lastHorizontal < 0)
        // {
        //     attackCircle.transform.rotation = Quaternion.Euler(0, 0, -90);
        // }
        // // down
        // else if (lastVertical < 0)
        // {
        //     attackCircle.transform.rotation = Quaternion.Euler(0, 0, 0);
        // }
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            GameScript.Player.IncreaseXP(20);
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackPos.position, attackRadius);
    }
}