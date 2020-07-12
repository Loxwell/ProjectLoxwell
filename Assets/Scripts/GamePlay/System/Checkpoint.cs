using LSG;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ScheduleSystem.Core;

namespace Platformer.Mechanics
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public Vector3 Position
        {
            get
            {
                if (m_checkPointT)
                    return m_checkPointT.position;
                return transform.position;
            }
        }

        public event System.Action<Checkpoint, Collider2D> onTriggerEnterEvent;
        [SerializeField]
        Transform m_checkPointT;

        private void Reset()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
        }

        private void OnDestroy()
        {
            onTriggerEnterEvent = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onTriggerEnterEvent?.Invoke(this, collision);
        }
    }
}