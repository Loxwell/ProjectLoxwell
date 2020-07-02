using UnityEngine;

namespace LSG
{
    public class CollisionDetector2D : MonoBehaviour
    {
        public event System.Action<Collider2D> onTriggerEnter;
        public event System.Action<Collider2D> onTrigger;
        public event System.Action<Collider2D> onTriggerExit;

        public event System.Action<Collision2D> onCollisionEnter;
        public event System.Action<Collision2D> onCollision;
        public event System.Action<Collision2D> onCollisionExit;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (onTriggerEnter != null)
                onTriggerEnter(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (onTrigger != null)
                onTrigger(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (onTriggerExit != null)
                onTriggerExit(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (onCollisionEnter != null)
                onCollisionEnter(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (onCollision != null)
                onCollision(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (onCollisionExit != null)
                onCollisionExit(collision);
        }
    }
}
