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
