using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyMoveScript : BlinkyMoveScript
{
    public GameObject Blinky;

    public override void Start()
    {
        scatterTile = new Vector2(28, -1);
        scatterMode = false;
        powerPelletEaten = false;
        startPos = new Vector2(7, 23);
        GameManager.GetComponent<GameManagerScriptOld>().ResetGame += StartGame;
        destination = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);
    }

    public override void FixedUpdate()
    {
        Vector2 currentDir = destination - (Vector2)transform.position.Round();
        List<Vector3> validTileList = new List<Vector3>();
        Vector3 targetTile = new Vector3();
        // Take tile 2 in front of pac man
        // Draw line from blinky to this tile
        // Double line distance
        // Rounded = targettile
        Vector3 offsetTile = (Vector2)PacMan.transform.position.Round() + (2 * PacMan.GetComponent<PacManMoveScript>().moveDirection);
        if (scatterMode)
        {
            targetTile = scatterTile;
        }
        else
        {
            targetTile = (DrawOffsetLine(offsetTile));
        }

        List<Vector3> tileList = new List<Vector3>(CreateTileList());

        foreach (Vector3 tile in tileList)
        {
            Vector2 potentialDir = tile - transform.position.Round();
            if (Valid(tile) && Vector2.Dot(potentialDir.normalized, currentDir.normalized) != -1)
            {
                validTileList.Add(tile);
            }
        }

        List<Vector3> LAValidTileList = new List<Vector3>(CreateLAValidTileList(validTileList));

        //TODO: make it so blinky only moves after reaching destination (set destination if dest == transform.position like in pacmanscript)
        if ((Vector2)transform.position.Round() != destination)
        {
            MoveTowardDestination(transform.position, destination, movespeed, validTileList);
        }
        else
        {
            SetDestination(validTileList, LAValidTileList, targetTile);
        }

        Vector2 dir = destination - (Vector2)transform.position;
        GetComponent<Animator>().SetFloat("DirX", dir.x);
        GetComponent<Animator>().SetFloat("DirY", dir.y);

        linerenderer.SetPosition(0, gameObject.transform.position.Round());
        linerenderer.SetPosition(1, targetTile);
    }

    public Vector3 DrawOffsetLine(Vector3 offsetTile)
    {
        //LayerMask ignoreall;
        Vector2 pos = Blinky.transform.position.Round();
        Vector2 dir = (Vector2)offsetTile - pos;
        Vector2 target = (Vector2)offsetTile + dir;      
        return target;
    }

}
