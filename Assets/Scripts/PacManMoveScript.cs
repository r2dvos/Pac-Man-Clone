using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PacManMoveScript : MonoBehaviour
{
    public float movespeed = 0.4f;
    Vector2 destination = Vector2.zero;
    public AudioSource chomp;
    public Vector2 moveDirection;
    public GameObject Blinky;
    public GameObject Pinky;
    public GameObject Inky;
    public GameObject Clyde;
    public TextMeshProUGUI score;
    public GameObject GameManager;
    private Vector2 startPos;

    [HideInInspector]
    public float timeSpent;

    void Start()
    {
        startPos = new Vector2(15, 8);
        GameManager.GetComponent<GameManagerScript>().ResetGame += StartGame;
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
        
        //works for now, counts moving in a wall as moving
        if ((Vector2)transform.position == destination && !Input.anyKey)
        {
            timeSpent += Time.deltaTime;
        }


        //To Fix: Plays on awake, stops/starts a lot (turned off because annoying)
            if ((Vector2)transform.position == destination && (Input.GetKey("up")))
        {
            //chomp.Play();
        }

        // Animation Parameters
        Vector2 dir = destination - (Vector2)transform.position;
        moveDirection = CheckMoveDirection(dir);
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);
    }

    Vector2 CheckMoveDirection(Vector2 dir)
    {
        if (dir.x > 0.01)
        {
            moveDirection = Vector2.right;
        }
        if (dir.x < -0.01)
        {
            moveDirection = Vector2.left;
        }
        if (dir.y > 0.01)
        {
            moveDirection = Vector2.up;
        }
        if (dir.y < -0.01)
        {
            moveDirection = Vector2.down;
        }
        return moveDirection;
    }

    bool valid(Vector2 dir)
    {
        // Cast Line from 'next square in movedirection to 'Pac-Man'. True = hit pac man
        Vector2 pos = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
        return (hit.collider == GetComponent<Collider2D>());
    }

    //Determines what happens when collided with. PacMan eats pellets, powers up due to power pellets and dies to ghosts
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "PowerPellet")
        {
            StartCoroutine(Blinky.GetComponent<BlinkyMoveScript>().Scatter(8));
            StartCoroutine(Inky.GetComponent<InkyMoveScript>().Scatter(8));
            StartCoroutine(Pinky.GetComponent<PinkyMoveScript>().Scatter(8));
            StartCoroutine(Clyde.GetComponent<ClydeMoveScript>().Scatter(8));

            GameManager.GetComponent<GameManagerScript>().PowerPelletCollected();
            score.GetComponent<ScoreScript>().ScorePowerPellet();
        }

        if (collision.tag == "Pellet")
        {
            score.GetComponent<ScoreScript>().ScorePellet();
            GameManager.GetComponent<GameManagerScript>().PelletCollected();
        }
        if (collision.tag == "Ghost")
        {
            score.GetComponent<ScoreScript>().ScoreGhost();
        }

        if (collision.name == "TeleportLeft")
        {
            Vector2 tp = new Vector2(27, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
        if (collision.name == "TeleportRight")
        {
            Vector2 tp = new Vector2(2, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
    }

    public void StartGame()
    {
        gameObject.transform.position = startPos;
        destination = startPos;
    }
}
