using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Timeline;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using BookshelfCollider = BookshelfCollider;

public class Player : MonoBehaviour
{
    public float moveSpeed;
    Vector2 movement = Vector2.zero;
    private Rigidbody2D rb;
    private Animator animator;
    SpriteRenderer spriteRenderer;
    //tương tác với kệ sách
    public Collider2D col;
    public List<Collider2D> detectedObjs = new List<Collider2D>();
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        //di chuyển
        bool isPaused = PauseController.IsGamePaused;
        if (!isPaused)
        {
            movement.x = UnityEngine.Input.GetAxisRaw("Horizontal") * moveSpeed;
            movement.y = UnityEngine.Input.GetAxisRaw("Vertical") * moveSpeed;
            rb.velocity = new Vector2(movement.x, movement.y);
        }
        else
        {
            rb.velocity = Vector2.zero;
            movement = Vector2.zero;
        }

        if (movement != Vector2.zero)
        {
            animator.SetBool("isMoving", true);
            animator.SetFloat("X", movement.x);
            animator.SetFloat("Y", movement.y);
            if (movement.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movement.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        
    }
    

}

