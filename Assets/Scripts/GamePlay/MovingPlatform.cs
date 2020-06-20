using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMovementController = Platformer.Mechanics.PlayerMovementController;
using Patroller = Platformer.Mechanics.Patroller;

public class MovingPlatform : MonoBehaviour, Platformer.Mechanics.IPatrolUtil
{
    public TextMesh text;

    Transform m_transform;    
    float m_preTime;

    Vector2 m_dir;
    Vector2 m_size;

    private void Awake()
    {
        m_transform = transform;

        m_size = GetComponent<BoxCollider2D>().bounds.extents;
        m_size.x *= m_transform.localScale.x;
        m_size.y *= m_transform.localScale.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            Initialize();
            m_dir = p.transform.position - m_transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            if (p.transform.position.y  >= m_transform.position.y + m_size.y
                && (m_size.x * m_size.x > m_dir.x * m_dir.x))
            {
                m_dir.x += p.Velocity.x * (Time.time - m_preTime);
                p.transform.position = m_transform.position + m_dir.x * Vector3.right + Vector3.up * m_size.y * 1.1f;
            }

            Initialize();
        }
    }

    public void SetPatroller(Patroller p)
    {

    }

    void Initialize()
    {
        m_preTime = Time.time;
    }
}
