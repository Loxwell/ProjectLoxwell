using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Platformer.Mechanics
{
    public partial class PatrolPath : MonoBehaviour
    {
        /// <summary>
        /// Patrol Start Point
        /// </summary>
        [SerializeField]
        internal Vector2 stPos;

        /// <summary>
        /// Patrol Destination Point
        /// </summary>
        [SerializeField]
        internal Vector2 edPos;

        /// <summary>
        /// Create a Mover instance which is used to move an entity along the path at a certain
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public Mover CreateMover(float speed = 1) => new Mover(this, speed);

        public void Reset()
        {
            stPos = Vector2.left;
            edPos = Vector2.right;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(stPos, edPos);
        }
#endif
    }
}

