using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VectorRounder;

public class BlinkyMoveScript : MonoBehaviour
{
    public GameObject PacMan;
    public float movespeed = 0.3f;
    public LineRenderer linerenderer;
    public LayerMask layerMask;
    public GameObject GameManager;

    [HideInInspector]
    public bool scatterMode;
    public bool powerPelletEaten;
    public Vector2 destination = Vector2.zero;
    public Vector2 startPos;
    public Vector2 scatterTile;

    // Start is called before the first frame update
    public virtual void Start()
    {
        scatterTile = new Vector2(28, 32);
        scatterMode = false;
        powerPelletEaten = false;
        startPos = new Vector2(14, 20);
        GameManager.GetComponent<GameManagerScriptOld>().ResetGame += StartGame;
        destination = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);
    }

    public virtual void FixedUpdate()
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
            targetTile = PacMan.transform.position.Round();
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
        if((Vector2)transform.position.Round() != destination)
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

    public List<Vector3> CreateTileList()
    {
        List<Vector3> tileList = new List<Vector3>();
        Vector3 upTile = gameObject.transform.position.Round() + new Vector3(0f, 1f, 0f);
        Vector3 downTile = gameObject.transform.position.Round() + new Vector3(0f, -1f, 0f);
        Vector3 rightTile = gameObject.transform.position.Round() + new Vector3(1f, 0f, 0f);
        Vector3 leftTile = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);

        tileList.Add(upTile);
        tileList.Add(downTile);
        tileList.Add(leftTile);
        tileList.Add(rightTile);

        return tileList;
    }

    public List<Vector3> CreateLAValidTileList(List<Vector3> validTileList)
    {
        List<Vector3> LAValidTileList = new List<Vector3>();

        foreach (Vector3 tile in validTileList)
        {
            Vector3 LAupTile = tile + new Vector3(0f, 1f, 0f);
            Vector3 LAdownTile = tile + new Vector3(0f, -1f, 0f);
            Vector3 LArightTile = tile + new Vector3(1f, 0f, 0f);
            Vector3 LAleftTile = tile + new Vector3(-1f, 0f, 0f);

            if (ValidLATile(LAupTile, tile))
            {
                LAValidTileList.Add(LAupTile);
            }
            if (ValidLATile(LAdownTile, tile))
            {
                LAValidTileList.Add(LAdownTile);
            }
            if (ValidLATile(LArightTile, tile))
            {
                LAValidTileList.Add(LArightTile);
            }
            if (ValidLATile(LAleftTile, tile))
            {
                LAValidTileList.Add(LAleftTile);
            }
        }
        return LAValidTileList;
    }

    public void MoveTowardDestination(Vector3 currentPos, Vector3 destination, float movespeed, List<Vector3>validTileList)
    {
        Vector3 moveDestination = new Vector3();
        moveDestination = PickSmallestMagnitude(destination, validTileList);
        Vector2 p = Vector2.MoveTowards(currentPos, moveDestination, movespeed);
        GetComponent<Rigidbody2D>().MovePosition(p);
    }

    public virtual void SetDestination(List<Vector3> validTileList, List<Vector3> LAValidTileList, Vector3 targetTile)
    {
        if (LAValidTileList.Count == 1)
        {
            destination = LAValidTileList[0];
        }

        if (LAValidTileList.Count > 1)
        {
            destination = PickSmallestMagnitude(targetTile, LAValidTileList);
        }
    }

    public Vector2 PickSmallestMagnitude (Vector3 target, List<Vector3> potentialTiles)
    {
        Vector3 bestTile = new Vector3();
        float smallestMagnitude = Mathf.Infinity;
        foreach (Vector3 tile in potentialTiles)
        {
            Vector3 dir = target - tile;
            float magnitude = dir.magnitude;

            if (magnitude <= smallestMagnitude)
            {
                bestTile = tile;
                smallestMagnitude = magnitude;
            }
        }
        return bestTile;
    }  

    public bool Valid(Vector2 tile)
    {
        //True = hit blinky = valid path
        Vector2 pos = transform.position;
        Vector2 dir = tile - pos; 
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos, ~layerMask);
        return (hit.collider == GetComponent<Collider2D>());
    }

    public bool ValidLATile(Vector2 tile, Vector2 lastTile)
    {
        //return true when nothing is hit, ignores player and pacdots due to layermask
        Vector2 pos = lastTile;
        Vector2 dir = tile - pos;
        RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos, ~layerMask);
        return (hit.collider == null);
    }

    public IEnumerator Scatter(int time)
    {
        scatterMode = true;
        if (time == 8)
        {
            powerPelletEaten = true;
            GetComponent<Animator>().SetBool("Scared", true);
        }
        yield return new WaitForSeconds(time);
        scatterMode = false;
        powerPelletEaten = false;
        movespeed = 0.05f;
        GetComponent<Animator>().SetBool("Scared", false);
        StopCoroutine(Scatter(time));
    }

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "PacMan" && !powerPelletEaten)
        {
            GameManager.GetComponent<GameManagerScriptOld>().PacManDeath();
            GameManager.GetComponent<GameManagerScriptOld>().ResetGameFunction();
            //Destroy(co.gameObject);
            //end game
        }

        if (co.name == "PacMan" && powerPelletEaten)
        {
            movespeed = 0.2f;
        }

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

    //resets position of ghosts/pacman to original values
    public void StartGame()
    {
        gameObject.transform.position = startPos;
        destination = gameObject.transform.position.Round() + new Vector3(-1f, 0f, 0f);
    }
}
