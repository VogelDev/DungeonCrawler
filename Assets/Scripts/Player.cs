using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public SpriteRenderer sprite;

    public int CurrentLevel = 1;
    public float CurrentXP;
    
    public float moveSpeed;
    public Rigidbody2D rb;

    private Vector2 moveDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
        if (CurrentXP >= CurrentLevel * 100)
        {
            CurrentXP = 0;
            CurrentLevel++;
        }
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
        }

        moveDirection = new Vector2(moveX, moveY);
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
    }

    public void IncreaseXP(int xpToAdd)
    {
        CurrentXP += xpToAdd;
    }
}
