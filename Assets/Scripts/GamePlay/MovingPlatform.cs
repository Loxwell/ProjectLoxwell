using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMovementController = Platformer.Mechanics.PlayerMovementController;
using Patroller = Platformer.Mechanics.Patroller;
using BT.LSG;

public class MovingPlatform : MonoBehaviour, Platformer.Mechanics.IPatrolUtil
{
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
        m_box = GetComponent<BoxCollider2D>();
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
            if (p.transform.position.y  >= m_boxScale.y + m_box.offset.y
                && (m_boxScale.x * m_boxScale.x > m_dir.x * m_dir.x))
            {
                float deltaTime = (Time.time - m_preTime);
                m_dir.x += p.Velocity.x * deltaTime;
                m_dir.y += (p.Velocity.y + m_movingDir.y) * deltaTime;
               
                p.transform.position = m_transform.position + Vector3.up * m_boxScale.y * 1.1f + m_dir;
            }

            Initialize();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        LSG.PlayerInputManager p = target.GetComponent<LSG.PlayerInputManager>();
        if(p != null)
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
