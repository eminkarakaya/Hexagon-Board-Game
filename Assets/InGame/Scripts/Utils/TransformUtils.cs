using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtils : MonoBehaviour
{
    public static float GetGroundHeight = 1;
    public static Vector3 FixY(Vector3 pos)
    {
        return new Vector3(pos.x,GetGroundHeight,pos.z);
    }
}
