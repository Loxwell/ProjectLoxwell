using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameEffect : MonoBehaviour
{
    float m_duration;
    float m_elapsedTime;
    private void Awake()
    {
        m_duration = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
    }

    private void OnEnable()
    {
        m_elapsedTime = 0;
    }

    private void FixedUpdate()
    {
        m_elapsedTime += Time.fixedDeltaTime;
        if (m_duration <= m_elapsedTime)
            gameObject.SetActive(false);
    }
}
