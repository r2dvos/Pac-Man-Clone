using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyScript : GhostBehaviourScript
{
    public GameObject PacMan;

    private void Awake()
    {
        startPos = new Vector2(14.5f, 17f);
    }

    public override void Start()
    {
        scatterTile = new Vector2(1, 32);
        destination = gameObject.transform.position.Round()/* + new Vector3(-1f, 0f, 0f)*/;
        currentDir = new Vector2(-1f, 0f);
    }

    public override void Chase()
    {
        targetTile = (Vector2)PacMan.transform.position.Round() + (4 * PacMan.GetComponent<PacManMoveScript>().moveDirection);
    }

    public override void StartGame()
    {
        enabled = true;
        destination = transform.position;
        UpdateLists(); 
    }
}
