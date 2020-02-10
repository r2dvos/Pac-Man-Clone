using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacManMoveScript : MonoBehaviour
{
    public float movespeed = 0.4f;
    Vector2 destination = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Come up with better name for 'p'
        Vector2 p = Vector2.MoveTowards(transform.position, destination, movespeed);
        GetComponent<Rigidbody2D>().MovePosition(p);

        // Check for Input if not moving
        if ((Vector2)transform.position == destination)
        {
            if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
                destination = (Vector2)transform.position + Vector2.up;
            if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right))
                destination = (Vector2)transform.position + Vector2.right;
            if (Input.GetKey(KeyCode.DownArrow) && valid(-Vector2.up))
                destination = (Vector2)transform.position - Vector2.up;
            if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right))
                destination = (Vector2)transform.position - Vector2.right;
        }

        // Animation Parameters
        Vector2 dir = destination - (Vector2)transform.position;
        print(dir.x);
        print(dir.y);
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    bool valid(Vector2 dir)
    {
        // Cast Line from 'next square in movedirection to 'Pac-Man'
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        return (hit.collider == GetComponent<Collider2D>());
    }
}
