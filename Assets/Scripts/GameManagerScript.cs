using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//The Gamemanager is responsible for tracking a number of variables and managing game events suchs as player deaths etc
public class GameManagerScript : MonoBehaviour
{
    public GameObject Blinky;
    public GameObject Inky;
    public GameObject Pinky;
    public GameObject Clyde;
    public GameObject PacMan;

    [HideInInspector]
    public int pelletsCollected { get; set; }
    [HideInInspector]
    public int powerPelletsCollected { get; set; }
    private int lives;
    private List<GameObject> livesList;
    public GameObject life1, life2, life3;

    public AudioSource deathSound, startSound, victorySound;

    // Start is called before the first frame update
    void Start()
    {
        livesList = new List<GameObject>();
        lives = 3;
        livesList.Add(life1);
        livesList.Add(life2); 
        livesList.Add(life3);
        StartCoroutine(StartGame());
    }

    //Starts the sequence of ghosts moving from the ghosthouse. Ghosts aren't activated until they are outside the house. Time to release can be varied by changing the WaitForSeconds variable
    IEnumerator GhostHouseBehaviour()
    {
        yield return new WaitForSeconds(8);
        Vector2 waypoint1 = new Vector2(14.5f, 17f);
        Vector2 waypoint2 = new Vector2(14f, 20f);

        StartCoroutine(MoveFromHouse(Pinky, waypoint1, waypoint2));

        yield return new WaitForSeconds(8);

        StartCoroutine(MoveFromHouse(Inky, waypoint1, waypoint2));

        yield return new WaitForSeconds(8);

        StartCoroutine(MoveFromHouse(Clyde, waypoint1, waypoint2));

        yield break;
    }

    //This Coroutine moves the ghost in the input out of the ghost house via 2 waypoints, then activates their script
    IEnumerator MoveFromHouse(GameObject ghost, Vector2 waypoint1, Vector2 waypoint2)
    {
        while ((Vector2)ghost.transform.position != waypoint1)
        {
            Vector2 d = Vector2.MoveTowards(ghost.transform.position, waypoint1, 0.05f);
            ghost.GetComponent<Rigidbody2D>().MovePosition(d);
            yield return new WaitForEndOfFrame();
        }
        while ((Vector2)ghost.transform.position != waypoint2)
        {
            Vector2 d = Vector2.MoveTowards(ghost.transform.position, waypoint2, 0.05f);
            ghost.GetComponent<Rigidbody2D>().MovePosition(d);
            yield return new WaitForEndOfFrame();
        }

        if (ghost.name == "Pinky")
            ghost.GetComponent<PinkyScript>().StartGame();
        if (ghost.name == "Inky")
            ghost.GetComponent<InkyScript>().StartGame();
        if (ghost.name == "Clyde")
            ghost.GetComponent<ClydeScript>().StartGame();

        yield break;
    }

    //This coroutine manages ghostbehaviour. It cycles between scatter mode and chase mode for 4 cycles, then settles on chase mode forever
    IEnumerator DetermineGhostBehaviour()
    {
        StartCoroutine(Blinky.GetComponent<BlinkyScript>().Scatter(7));
        StartCoroutine(Inky.GetComponent<InkyScript>().Scatter(7));
        StartCoroutine(Pinky.GetComponent<PinkyScript>().Scatter(7));
        StartCoroutine(Clyde.GetComponent<ClydeScript>().Scatter(7));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyScript>().Scatter(7));
        StartCoroutine(Inky.GetComponent<InkyScript>().Scatter(7));
        StartCoroutine(Pinky.GetComponent<PinkyScript>().Scatter(7));
        StartCoroutine(Clyde.GetComponent<ClydeScript>().Scatter(7));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyScript>().Scatter(5));
        StartCoroutine(Inky.GetComponent<InkyScript>().Scatter(5));
        StartCoroutine(Pinky.GetComponent<PinkyScript>().Scatter(5));
        StartCoroutine(Clyde.GetComponent<ClydeScript>().Scatter(5));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyScript>().Scatter(5));
        StartCoroutine(Inky.GetComponent<InkyScript>().Scatter(5));
        StartCoroutine(Pinky.GetComponent<PinkyScript>().Scatter(5));
        StartCoroutine(Clyde.GetComponent<ClydeScript>().Scatter(5));

        yield break;
    }

    //Stops all current activity on player death and starts the sequences that restart the game. Resets the game if all lives are lost
    public void PlayerDeath()
    {
        lives -= 1;
        InitializeLives();
        deathSound.Play();
        StopAllCoroutines();
        if (lives > 0)
        {
            StartCoroutine(DeathSequence());
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    //Starts the deathsequence which resets all ghost positions and restarts the startgamesequence
    public IEnumerator DeathSequence()
    {
        PacMan.GetComponent<PacManMoveScript>().enabled = false;
        Blinky.GetComponent<BlinkyScript>().enabled = false;
        Pinky.GetComponent<PinkyScript>().enabled = false;
        Inky.GetComponent<InkyScript>().enabled = false;
        Clyde.GetComponent<ClydeScript>().enabled = false;

        yield return new WaitForSeconds(3);

        ResetGame();

        StartCoroutine(StartGame());
    }

    //Starts the startgame sequence, which enables all ghostbehaviourscripts
    IEnumerator StartGame()
    {
        startSound.Play();

        yield return new WaitForSeconds(4);
        PacMan.GetComponent<PacManMoveScript>().enabled = true;
        Blinky.GetComponent<CircleCollider2D>().enabled = true;
        Blinky.GetComponent<BlinkyScript>().StartGame();
        StartCoroutine(DetermineGhostBehaviour());
        StartCoroutine(GhostHouseBehaviour());

        yield break;
    }

    //Resets position of all ghosts and pac man to starting positions
    void ResetGame()
    {
        PacMan.GetComponent<PacManMoveScript>().ResetGame();
        Blinky.GetComponent<BlinkyScript>().ResetGame();
        Inky.GetComponent<InkyScript>().ResetGame();
        Pinky.GetComponent<PinkyScript>().ResetGame();
        Clyde.GetComponent<ClydeScript>().ResetGame();
    }

    //Sets ghosts to scared behaviour for 8 seconds
    public void PowerPelletCollected()
    {
        StartCoroutine(Blinky.GetComponent<BlinkyScript>().Frightened(8));
        StartCoroutine(Inky.GetComponent<InkyScript>().Frightened(8));
        StartCoroutine(Pinky.GetComponent<PinkyScript>().Frightened(8));
        StartCoroutine(Clyde.GetComponent<ClydeScript>().Frightened(8));
    }

    public void InitializeLives()
    {
        foreach (GameObject GO in livesList)
        {
            GO.SetActive(false);
        }
        for (int i = 0; i < lives; i++)
        {
            livesList[i].SetActive(true);
        }
    }

    public IEnumerator CheckForGameEnd()
    {
        if(pelletsCollected == 240 && powerPelletsCollected == 4)
        {
            //TODO: Stop all movement
            victorySound.Play();
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        yield break;
    }
}
