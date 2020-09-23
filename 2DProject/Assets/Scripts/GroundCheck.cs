using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            player.isGrounded = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            player.isGrounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        player.isGrounded = false;
    }
        
void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            player.isGrounded = true;
        }
        else
        {
            player.isGrounded = false;
        }
    }
    private void OnDrawGizmos()
    {
        if (player.isGrounded)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Vector2 boxCenter = (Vector2)transform.position + (Vector2.down * 0.5f);
        Gizmos.DrawWireCube(transform.position,GetComponent<BoxCollider2D>().size);
    }
}
