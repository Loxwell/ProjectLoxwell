using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Light2D = UnityEngine.Experimental.Rendering.Universal.Light2D;

[RequireComponent(typeof(Light2D))]
public class LightLamp : MonoBehaviour
{
    Light2D m_light;
    float m_origin;
    float m_randomTime;
    private void Awake()
    {
        m_light = GetComponent<Light2D>();
        m_origin = m_light.intensity;
        m_randomTime = Random.Range(0.5f, 1.5f);
    }

    private void FixedUpdate()
    {
        m_light.intensity = Mathf.Sin(Time.time * m_randomTime * Mathf.PI) * Time.fixedDeltaTime * m_origin + m_origin;
    }
}
