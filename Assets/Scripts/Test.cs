using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void Awake()
    {
        TestClass t = ScheduleSystem.Core.Simulation.GetModel<TestClass>();
        if(!t)
        {
            UnityEngine.Debug.Log("TestClass is null");
        }
    }

    private void Start()
    {
        UnityEngine.Debug.Log(Physics2D.gravity);
    }

    void OnTriggerEnter2D(Collider2D Other)
    {
	    UnityEngine.Debug.Log(Other.transform.name);
    }

    class TestClass
    {
        public float a = 10;

        public TestClass()
        {
            UnityEngine.Debug.LogError("TestClass( ) constructor");
        }

        public static implicit operator bool(TestClass t)
        {
            return t != null;
        }
    }
}
