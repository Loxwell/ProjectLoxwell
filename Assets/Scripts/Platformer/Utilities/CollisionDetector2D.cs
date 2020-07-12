using UnityEngine;

namespace LSG
{
    [RequireComponent(typeof(Collider2D))]
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
            onTriggerEnter?.Invoke(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            onTrigger?.Invoke(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            onTriggerExit?.Invoke(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            onCollisionEnter?.Invoke(collision);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            onCollision?.Invoke(collision);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            onCollisionExit?.Invoke(collision);
        }
    }
}
