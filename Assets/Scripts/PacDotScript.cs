using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacDotScript : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D co)
    {
        if (co.name == "PacMan")
        {
            Destroy(gameObject);
        }
                
    }
}
