using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        int st = 0;
        int end = 10;
        while (end > st)
        {
            int change = (end - 1) >> 1;
            UnityEngine.Debug.Log(change);
            end = change;
        }
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
	    UnityEngine.Debug.Log(Other.transform.name);
    }

}
