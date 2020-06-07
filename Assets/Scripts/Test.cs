using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D Other)
    {
	    UnityEngine.Debug.Log(Other.transform.name);
    }

}
