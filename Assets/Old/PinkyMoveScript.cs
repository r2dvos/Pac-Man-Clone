using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyMoveScript : BlinkyMoveScript
{

    public override void Start()
    {
        scatterTile = new Vector2(1, 32);
        scatterMode = false;
        powerPelletEaten = false;
        startPos = new Vector2(12, 26);
        GameManager.GetComponent<GameManagerScriptOld>().ResetGame += StartGame;
        destination = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);
    }

    public override void FixedUpdate()
    {
        Vector2 currentDir = destination - (Vector2)transform.position.Round();
        List<Vector3> validTileList = new List<Vector3>();
        Vector3 targetTile = new Vector3();

        if (scatterMode)
        {
            targetTile = scatterTile;
        }
        else
        {
            targetTile = (Vector2)PacMan.transform.position.Round() + (4 * PacMan.GetComponent<PacManMoveScript>().moveDirection);
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
}
