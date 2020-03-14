using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Use this class for storing variables and managing events such as collected pellets and players deaths etc
public class GameManagerScriptOld : MonoBehaviour
{
    private static GameManagerScriptOld _instance;

    public static GameManagerScriptOld Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject pinkwall;
    public GameObject Blinky;
    public GameObject Inky;
    public GameObject Pinky;
    public GameObject Clyde;
    public GameObject PacMan;
    public GameObject life1, life2, life3;
    public List<GameObject> lifeList = new List<GameObject>();
    public float TimeBeforeDeath { get; set; }
    public GameObject model;
    public float ghostSpeed;

    [HideInInspector]
    public float pelletsCollected;
    [HideInInspector]
    public float powerPelletsCollected;
    public int Deaths { get; set; }
    public int lives = 3;

    public delegate void StartGame();
    public event StartGame ResetGame;

    // Start is called before the first frame update
    void Start()
    {
        ghostSpeed = CalcAvgGhostSpeed();
        lifeList.Add(life1);
        lifeList.Add(life2);
        lifeList.Add(life3);
        StartCoroutine(GhostBehaviour());
    }

    public void ResetGameFunction()
    {
        ResetGame();
    }

    //Switches ghosts from scatter behaviour to chase behaviour 4 times
    IEnumerator GhostBehaviour()
    {
        StartCoroutine(Blinky.GetComponent<BlinkyMoveScript>().Scatter(7));
        StartCoroutine(Inky.GetComponent<InkyMoveScript>().Scatter(7));
        StartCoroutine(Pinky.GetComponent<PinkyMoveScript>().Scatter(7));
        StartCoroutine(Clyde.GetComponent<ClydeMoveScript>().Scatter(7));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyMoveScript>().Scatter(7));
        StartCoroutine(Inky.GetComponent<InkyMoveScript>().Scatter(7));
        StartCoroutine(Pinky.GetComponent<PinkyMoveScript>().Scatter(7));
        StartCoroutine(Clyde.GetComponent<ClydeMoveScript>().Scatter(7));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyMoveScript>().Scatter(5));
        StartCoroutine(Inky.GetComponent<InkyMoveScript>().Scatter(5));
        StartCoroutine(Pinky.GetComponent<PinkyMoveScript>().Scatter(5));
        StartCoroutine(Clyde.GetComponent<ClydeMoveScript>().Scatter(5));

        yield return new WaitForSeconds(20);

        StartCoroutine(Blinky.GetComponent<BlinkyMoveScript>().Scatter(5));
        StartCoroutine(Inky.GetComponent<InkyMoveScript>().Scatter(5));
        StartCoroutine(Pinky.GetComponent<PinkyMoveScript>().Scatter(5));
        StartCoroutine(Clyde.GetComponent<ClydeMoveScript>().Scatter(5));

        yield break;
    }

    //could be done using get/set
    public void PelletCollected()
    {
        pelletsCollected += 1;
    }

    public void PowerPelletCollected()
    {
        powerPelletsCollected += 1;
    }

    public void InitializeLives()
    {
        foreach(GameObject GO in lifeList)
        {
            GO.SetActive(false);
        }
        for (int i = 0; i < lives; i++)
        {
            lifeList[i].SetActive(true);
        }
    }

    public void PacManDeath()
    {
        Deaths += 1;
        lives -= 1;
        InitializeLives();
        TimeBeforeDeath = Time.time - TimeBeforeDeath;   //only works for life 1 and 2
        model.GetComponent<ModelScript>().UpdateNumberOfSequenceBlocks(Deaths, TimeBeforeDeath);
    }

    //For later, delete the wall that keeps the ghosts in
    //IEnumerator DeletePinkWall()
    //{
    //    yield return new WaitForSeconds(3f);
    //    Destroy(pinkwall);
    //}

    //calculates avg distance, currently works without taking walls into effect
    public float CalcAvgDistance()
    {
        float avgDist = ((PacMan.transform.position - Clyde.transform.position).magnitude 
                        + (PacMan.transform.position - Inky.transform.position).magnitude
                        + (PacMan.transform.position - Pinky.transform.position).magnitude 
                        + (PacMan.transform.position - Blinky.transform.position).magnitude) / 4;
        return avgDist;
    }

    public float CalcNearestGhost()
    {
        float nearestGhost = Mathf.Min((PacMan.transform.position - Clyde.transform.position).magnitude,
                                      (PacMan.transform.position - Inky.transform.position).magnitude,
                                      (PacMan.transform.position - Pinky.transform.position).magnitude,
                                      (PacMan.transform.position - Blinky.transform.position).magnitude);

        return nearestGhost;
    }

    float CalcAvgGhostSpeed()
    {
        return (Blinky.GetComponent<BlinkyMoveScript>().movespeed
              + Inky.GetComponent<InkyMoveScript>().movespeed
              + Pinky.GetComponent<PinkyMoveScript>().movespeed
              + Clyde.GetComponent<ClydeMoveScript>().movespeed) / 4;
    }
}
