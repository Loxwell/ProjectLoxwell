using LSG;
using Platformer.Mechanics;
using UnityEngine;
using static ScheduleSystem.Core.Simulation;

[RequireComponent(typeof(Checkpoint))]
public class CheckpointWithCollisionDetector2D : MonoBehaviour
{
    [SerializeField]
    CollisionDetector2D m_collisionTrigger;
    Checkpoint m_checkPoint;

    private void Awake()
    {
        if (m_collisionTrigger)
            m_collisionTrigger.onTriggerEnter += OntriggerEnter2D;
        m_checkPoint = GetComponent<Checkpoint>();
        m_checkPoint.onTriggerEnterEvent += OntriggerEnter2D;
    }

    private void OnDestroy()
    {
        if (m_collisionTrigger)
            m_collisionTrigger.onTriggerEnter -= OntriggerEnter2D;
        m_checkPoint.onTriggerEnterEvent -= OntriggerEnter2D;

        m_collisionTrigger = null;
        m_checkPoint = null;
    }

    void OntriggerEnter2D(Collider2D other)
    {
        Schedule<InGame.Event.EventCheckPoint>(0, m_checkPoint, other.GetComponent<PlayerCharacter>());
    }

    void OntriggerEnter2D(Checkpoint cc, Collider2D other)
    {
        Schedule<InGame.Event.EventCheckPoint>(0, m_checkPoint, other.GetComponent<PlayerCharacter>());
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.Label(transform.position, "Check Point_" + name);
        BoxCollider2D b = GetComponent<BoxCollider2D>();
        Gizmos.DrawWireCube(transform.position + (Vector3)b.offset, b.bounds.size);
        if(m_collisionTrigger)
            Gizmos.DrawWireCube(m_collisionTrigger.transform.position, m_collisionTrigger.GetComponent<BoxCollider2D>().bounds.size);
    }
#endif
}
