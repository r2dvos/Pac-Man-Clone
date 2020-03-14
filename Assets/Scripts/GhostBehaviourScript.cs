using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehaviourScript : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask layerMask;
    public LineRenderer lineRenderer;
    public GameObject GameManager;
    public AudioSource eatGhost;

    [HideInInspector]
    public Vector2 destination, targetTile, scatterTile, currentDir, startPos;
    [HideInInspector]
    public List<Vector2> validTileList, tileList, validDestinations, path;
    [HideInInspector]
    private int waypointIndex = 0;
    [HideInInspector]
    public bool scatter { get; set; }
    public bool frightened { get; set; }

    //Awake is called when the object is created
    private void Awake()
    {
        path = new List<Vector2>(2);
        scatter = false;
        UpdateLists();
    }

    //Start is called before the first frameupdate, Initialize variables that you want to use for all ghosts here
    public virtual void Start()
    {
        moveSpeed = 0.2f;
        //initialize different values for all ghosts here, use the ghost specific override to set values for specific ghost types
    }

    //Update gets called every frame, if not at his current destination, the ghost will move towards it.
    //If at current destination. It will set a new update all relevant variables and set a new destination according to its pathfinding logic
    void Update()
    {
        PickTargetTile();
        if ((Vector2)gameObject.transform.position == destination)
        {
            waypointIndex = 0;
            UpdateLists();
            SetDestination();
        }
        else
        {
            Move(gameObject.transform.position, destination, validTileList);
        }
        
        //Sets variables so that the correct animation shows
        GetComponent<Animator>().SetFloat("DirX", currentDir.x);
        GetComponent<Animator>().SetFloat("DirY", currentDir.y);

        //Renders lines towards targetile for testing purposes, disable in actual gameplay
        //lineRenderer.SetPosition(0, transform.position.Round());
        //lineRenderer.SetPosition(1, targetTile);
    }
    
    //Picks the desired targettile based on if the ghost is in scattermode or not
    public void PickTargetTile()
    {
        if (scatter || frightened)
        {
            targetTile = scatterTile;
        }
        else
        {
            Chase();
        }
    }

    //Sets ghost behaviour to scatter, can be overriden to make each ghost scatter behaviour different
    public IEnumerator Scatter(int time)
    {
        ReverseDirection();
        scatter = true;
        yield return new WaitForSeconds(time);
        scatter = false;
        ReverseDirection();
    }

    //This method is called whenever PacMan eats a power pellet, turns ghosts 180 degrees and plays their scared animation. Returns their movespeed to normal afterwards in case pacman hits them in this state
    public IEnumerator Frightened(int time)
    {
        //turn around
        ReverseDirection();
        frightened = true;
        GetComponent<Animator>().SetBool("Scared", true);
        yield return new WaitForSeconds(time);
        frightened = false;
        moveSpeed = 0.1f;
        GetComponent<Animator>().SetBool("Scared", false);
    }

    //Ghosts reverse direction whenever changing behaviour mode, except when they go from Frightenend to any other mode
    void ReverseDirection()
    {
        currentDir = -currentDir;
        UpdateLists();
        SetDestination();
    }

    //Sets ghostbehaviour to chase, the targettile is different for each ghosttype and set in their respective scripts
    public virtual void Chase()
    {
        //targettile = whatever chase mechanism the ghost has
    }

    //Updates all lists that determine which tiles are valid to move to
    protected void UpdateLists()
    {
        //create list of possible tiles
        tileList = new List<Vector2>(CreateTileList(gameObject.transform.position.Round()));
        //check all tiles for validity, returns only valid tiles (aka look ahead 1) (valid = no walls)
        validTileList = new List<Vector2>(CreateValidTileList(tileList, gameObject.transform.position.Round()));
        //create list of valid destination tiles. Destinations are tiles located 2 tiles away from the Ghost.
        validDestinations = new List<Vector2>(CreateValidDestinationList(validTileList));
        //Check valid destinations for smallest magnitude to targettile
    }

    //Clears current path, adds 2 waypoints, the destination tile and the closest valid tile to that destination to be used as waypoints
    void SetDestination()
    {
        path.Clear();
        destination = PickSmallestMagnitude(targetTile, validDestinations);
        path.Add(PickSmallestMagnitude(destination, validTileList));      
        path.Add(destination);
    }

    //Moves the ghost along the two waypoints
    void Move(Vector2 currentPos, Vector2 destination, List<Vector2> validTileList)
    {
        currentDir = path[waypointIndex] - (Vector2)transform.position;
        if ((Vector2)transform.position != path[waypointIndex])
        {
            Vector2 p = Vector2.MoveTowards(currentPos, path[waypointIndex], moveSpeed);
            GetComponent<Rigidbody2D>().MovePosition(p);
        }
        else if (waypointIndex == 0)
        {
            waypointIndex += 1;
        }
    }

    //Helpfunctions

    //Returns the tile closest to the targettile 
    public Vector2 PickSmallestMagnitude(Vector2 target, List<Vector2> potentialTiles)
    {
        Vector2 bestTile = new Vector2();
        float smallestMagnitude = Mathf.Infinity;
        foreach (Vector2 tile in potentialTiles)
        {
            Vector2 dir = target - tile;
            float magnitude = dir.magnitude;

            if (magnitude <= smallestMagnitude)
            {
                bestTile = tile;
                smallestMagnitude = magnitude;
            }
        }
        return bestTile;
    }

    //Checks all squares around all valid tiles for validity and returns a list of all valid destinations (destinations are valid tiles 2 tiles away from the ghost)
    public List<Vector2> CreateValidDestinationList(List<Vector2> validTileList)
    {
        List<Vector2> validDestinationsList = new List<Vector2>();
        foreach (Vector2 tile in validTileList)
        {
            List<Vector2> possibleDestinations = new List<Vector2>(CreateTileList(tile));
            foreach(Vector2 destination in possibleDestinations)
            {
                if (ValidTileChecker(destination, tile) && destination != (Vector2)transform.position.Round())
                {
                    validDestinationsList.Add(destination);
                }
            }
        }
        return validDestinationsList;
    }

    //Returns a list of all tiles adjacent to the input tile
    public List<Vector2> CreateTileList(Vector2 startTile)
    {
        List<Vector2> tileList = new List<Vector2>();
        Vector2 upTile = startTile + new Vector2(0f, 1f);
        Vector2 downTile = startTile + new Vector2(0f, -1f);
        Vector2 rightTile = startTile + new Vector2(1f, 0f);
        Vector2 leftTile = startTile + new Vector2(-1f, 0f);

        tileList.Add(upTile);
        tileList.Add(downTile);
        tileList.Add(leftTile);
        tileList.Add(rightTile);

        return tileList;
    }

    //Checks list for validity and returns only valid tiles. Valid tiles are tiles that are not in a wall and not in the opposite direction the ghost is moving in
    List<Vector2> CreateValidTileList(List<Vector2> tileList, Vector2 origin)
    {
        List<Vector2> validTileList = new List<Vector2>();
        foreach (Vector2 tile in tileList)
        {
            Vector2 potentialDir = tile - origin;
            if (ValidTileChecker(tile, origin) && Vector2.Dot(potentialDir.normalized, currentDir.normalized) != -1)
            {
                validTileList.Add(tile);
            }
        }
        return validTileList;
    }

    //Draws a line from the origintile to the tile to be checked, returns valid if no wall is hit, ignores everything that isn't a wall
    bool ValidTileChecker(Vector2 tileToCheck, Vector2 origin)
    {
        RaycastHit2D hit = Physics2D.Linecast(origin, tileToCheck, ~layerMask);
        return (hit.collider == null);
    }

    public void ResetGame()
    {
        transform.position = startPos;
    }

    public virtual void StartGame()
    {
        //Enables the pathfinding logic scripts
    }

    //Determines what happens if the ghost collides with numerous objects
    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "PacMan" && !frightened)
        {
            //Call some method in gamemanager 
            GameManager.GetComponent<GameManagerScript>().PlayerDeath();
        }

        if (co.name == "PacMan" && frightened)
        {
            eatGhost.Play();
            moveSpeed = 0.2f;
        }

        //Teleports the ghost to the other side when a teleporter is hit
        if (co.name == "TeleportLeft")
        {
            Vector2 tp = new Vector2(27, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
        if (co.name == "TeleportRight")
        {
            Vector2 tp = new Vector2(2, 17);
            destination = tp;
            gameObject.transform.position = tp;
        }
    }
}
