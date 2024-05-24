using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    Transform target;
    public Transform borderCheck;
    public int ZombieHP = 100;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        FindTarget(); // Call to find the target
        IgnoreCollisions(); // Call to ignore collisions with the player
    }

    void FindTarget()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("Player not found. Zombie cannot detect player position.");
        }
    }

    void IgnoreCollisions()
    {
        Collider2D zombieCollider = GetComponent<Collider2D>();
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        if (zombieCollider != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(playerCollider, zombieCollider);
        }
        else
        {
            Debug.LogWarning("Zombie or player collider not found. Ignoring collision failed.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Update golem's scale based on player position
            transform.localScale = new Vector2(target.position.x > transform.position.x ? 1.5f : -1.5f, 1.5f);
        }
    }

    public void ZombieTakeDamage(int damage)
    {
        ZombieHP -= damage;
        if (ZombieHP <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("Damage3");
        }
    }

    void Die()
    {
        animator.SetTrigger("deth3");
        GetComponent<CapsuleCollider2D>().enabled = false;
        this.enabled = false;
    }

    public void PlayerDamage()
    {
        if (target != null)
        {
            PlayerCollision playerCollision = target.GetComponent<PlayerCollision>();
            if (HealthManager.health > 0)
            {
                playerCollision.PlayerTakeDamage();
            }
        }
        else
        {
            Debug.LogWarning("Player not found. Cannot damage player.");
        }
    }
}
