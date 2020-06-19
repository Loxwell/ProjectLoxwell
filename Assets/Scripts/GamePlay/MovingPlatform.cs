using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerMovementController = Platformer.Mechanics.PlayerMovementController;
using Patroller = Platformer.Mechanics.Patroller;

public class MovingPlatform : MonoBehaviour, Platformer.Mechanics.IPatrolUtil
{
    public TextMesh text;

    Transform m_transform;
    Vector3 m_prePos;
    float m_height;
    float m_preTime;

    Vector2 dir;

    private void Awake()
    {
        m_height = GetComponent<BoxCollider2D>().size.y;
        m_transform = transform;
        m_height *= m_transform.localScale.y;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            Initialize();
            dir = p.transform.position - m_transform.position;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        PlayerMovementController p = collision.transform.GetComponent<PlayerMovementController>();
        if(p)
        {
            Vector2 size = GetComponent<BoxCollider2D>().bounds.extents;
            size.x *= m_transform.localScale.x;
            size.y *= m_transform.localScale.y;

            if (p.transform.position.y  >= m_transform.position.y + size.y && (size.x * size.x > dir.x * dir.x))
            {
                text.text = "On";

                dir.x += p.Velocity.x * (Time.time - m_preTime);
                p.transform.position = m_transform.position + dir.x * Vector3.right + Vector3.up * size.y * 1.1f;
            }
            else
            {
                text.text = "Out";
            }


            Initialize();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        m_prePos = Vector2.zero;
    }

    public void SetPatroller(Patroller p)
    {

    }

    void Initialize()
    {
        

        m_prePos = m_transform.position;
        m_preTime = Time.time;
    }
}
