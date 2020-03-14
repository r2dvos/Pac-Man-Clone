using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayCollider : MonoBehaviour
{
    public Collider2D pinkwall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ghost")
        {
            print("entered");
            Physics2D.IgnoreCollision(collision, pinkwall, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Ghost")
        {
            print("exited");
            Physics2D.IgnoreCollision(other, pinkwall, false);
        }
    }
}
