using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMovementController = Platformer.Mechanics.PlayerMovementController;
using Patroller = Platformer.Mechanics.Patroller;
using BT.LSG;
using System;

public class MovingPlatform : MonoBehaviour, Platformer.Mechanics.IPatrolUtil
{
    Vector3 CurPosition
    {
        get
        {
            return m_transform.position + Vector3.up * m_boxScale.y * 1.1f;
        }
    }

    Transform m_transform;    
    float m_preTime;

    /// <summary>
    /// half scale
    /// </summary>
    Vector2 m_boxScale;
    Vector3 m_dir;
    Vector3 m_prePos;
    Vector3 m_movingDir;
    BoxCollider2D m_box;

    Transform target;

    private void Awake()
    {
        m_transform = transform;
        BoxCollider2D [] boxes = GetComponents<BoxCollider2D>();
        foreach(var b in boxes)
        {
            if (!b.isTrigger)
            {
                m_box = b;
                break;
            }
        }

        m_boxScale = m_box.size * 0.5f;
    }

    void OnEnable()
    {
        m_prePos = m_transform.position;
    }

    void LateUpdate()
    {
        m_movingDir = m_transform.position - m_prePos;
        m_prePos = m_transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            target = collision.transform;

            Initialize();
            m_dir = p.transform.position - m_transform.position;
            m_prePos = m_transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            if (p.transform.position.y  >= m_transform.position.y + m_box.offset.y
                && (m_boxScale.x * m_boxScale.x > m_dir.x * m_dir.x))
            {
                float deltaTime = (Time.time - m_preTime);
                m_dir.x += p.Velocity.x * deltaTime;
                m_dir.y = 0;
                p.transform.position = CurPosition + m_dir;
            }

            Initialize();
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        PlayerMovementController p = collider.gameObject.GetComponent<PlayerMovementController>();
        if (p != null)
        {
            
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
