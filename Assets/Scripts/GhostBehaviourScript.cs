using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostBehaviourScript : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask layerMask;
    public LineRenderer lineRenderer;

    [HideInInspector]
    public Vector2 destination;
    public Vector2 targetTile;
    public Vector2 scatterTile;

    public virtual void Start()
    {
        //destination = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);
        moveSpeed = 0.2f;
        //initialize different values for different ghosts here
    }

    void Update()
    {
        PickDestination();
        Chase();
    }

    //Call this behaviour from the game manager to make the ghost Scatter
    public void Scatter()
    {
        targetTile = scatterTile;
    }

    //Call this behaviour from the game manager to make the ghost Chase
    public virtual void Chase()
    {
        //targettile = whatever chase mechanism the ghost has
    }

    void PickDestination()
    {
        //useful for checking if ghost wants to turn 180 degrees
        Vector2 currentDir = destination - (Vector2)transform.position.Round();
        //create list of possible tiles
        List<Vector2> tileList = new List<Vector2>(CreateTileList(gameObject.transform.position.Round()));
        //check all tiles for validity, returns only valid tiles (aka look ahead 1) (valid = no walls)
        List<Vector2> validTileList = new List<Vector2>(CreateValidTileList(tileList, gameObject.transform.position.Round()));
        //create list of valid destination tiles
        List<Vector2> validDestinations = new List<Vector2>(CreateValidDestinationList(validTileList));
        //Check valid destinations for smallest magnitude to targettile
        destination = PickSmallestMagnitude(targetTile, validDestinations);

        lineRenderer.SetPosition(0, transform.position.Round());
        lineRenderer.SetPosition(1, destination);

        //Move ghost towards destination
        Move(gameObject.transform.position, destination, validTileList);
    }

    //Move ghost towards the closest tile to the destination
    void Move(Vector2 currentPos, Vector2 destination, List<Vector2> validTileList)
    {
        Vector2 moveDestination = new Vector2();
        moveDestination = PickSmallestMagnitude(destination, validTileList);
        Vector2 p = Vector2.MoveTowards(currentPos, moveDestination, moveSpeed);
        GetComponent<Rigidbody2D>().MovePosition(p);
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
                if(ValidTileChecker(destination, tile))
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

    //Checks list for validity and returns only valid tiles
    List<Vector2> CreateValidTileList(List<Vector2> tileList, Vector2 origin)
    {
        List<Vector2> validTileList = new List<Vector2>();
        foreach (Vector2 tile in tileList)
        {
            //TODO: MAKE SURE GHOSTS NEVER TURN 180 DEGREES.
            Vector2 currentDir = destination - (Vector2)transform.position.Round();
            Vector2 potentialDir = tile - (Vector2)transform.position.Round();
            print(Vector2.Dot(potentialDir.normalized, currentDir.normalized));
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

}
