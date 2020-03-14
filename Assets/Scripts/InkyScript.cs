using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkyScript : GhostBehaviourScript
{
    public GameObject PacMan;
    public GameObject Blinky;

    private void Awake()
    {
        startPos = new Vector2(13f, 17f);
    }

    public override void Start()
    {
        scatterTile = new Vector2(28, -1);
        destination = gameObject.transform.position.Round()/* + new Vector3(-1f, 0f, 0f)*/;
        currentDir = new Vector2(-1f, 0f);
    }

    public override void Chase()
    {
        Vector2 offsetTile = (Vector2)PacMan.transform.position.Round() + (2 * PacMan.GetComponent<PacManMoveScript>().moveDirection);
        targetTile = (DrawOffsetLine(offsetTile));
    }

    public Vector2 DrawOffsetLine(Vector2 offsetTile)
    {
        //LayerMask ignoreall;
        Vector2 pos = Blinky.transform.position.Round();
        Vector2 dir = offsetTile - pos;
        Vector2 target = offsetTile + dir;
        return target;
    }

    public override void StartGame()
    {
        enabled = true;
        destination = transform.position;
        UpdateLists();
    }

}
