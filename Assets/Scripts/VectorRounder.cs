using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class VectorRounder
{
    public static Vector3 Round(this Vector3 vector3)
    {
        return new Vector3(
            Mathf.Round(vector3.x),
            Mathf.Round(vector3.y),
            Mathf.Round(vector3.z));
    }
}

