using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyScript : GhostBehaviourScript
{
    public GameObject PacMan;

    // Start is called before the first frame update
    public override void Start()
    {
        startPos = new Vector2(14f, 20f);
        scatterTile = new Vector2(28, 32);
        destination = gameObject.transform.position.Round()/* + new Vector3(-1f, 0f, 0f)*/;
        currentDir = new Vector2(-1f, 0f);
    }

    public override void Chase()
    {
        targetTile = PacMan.transform.position.Round();
    }

    public override void StartGame()
    {
        enabled = true;
        destination = transform.position;
        UpdateLists();
    }
}
