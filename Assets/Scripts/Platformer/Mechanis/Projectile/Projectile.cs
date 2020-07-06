using UnityEngine;


namespace Platformer.Mechanics
{
    public class Projectile : MonoBehaviour
    {

        Rigidbody2D m_rigidbody;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();
        }

        public void SetProjectileVeloicty(Vector3 target, Vector3 origin)
        {
            m_rigidbody.velocity = GetProjectilVelocity(target, origin);
        }

        private Vector3 GetProjectilVelocity(Vector3 target, Vector3 origin)
        {
            const float projectileSpeed = 30.0f;

            Vector3 velocity = Vector3.zero;
            Vector3 toTarget = target - origin;

            float gSquared = Physics.gravity.sqrMagnitude;
            float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
            float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

            // Check whether the target is reachable at max speed or less.
            if (discriminant < 0)
            {
                velocity = toTarget;
                velocity.y = 0;
                velocity.Normalize();
                velocity.y = 0.7f;

                velocity *= projectileSpeed;
                return velocity;
            }

            float discRoot = Mathf.Sqrt(discriminant);

            // Highest
            float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

            // Lowest speed arc
            float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

            // Most direct with max speed
            float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

            float T = 0;

            // 0 = highest, 1 = lowest, 2 = most direct
            int shotType = 1;

            switch (shotType)
            {
                case 0:
                    T = T_max;
                    break;
                case 1:
                    T = T_lowEnergy;
                    break;
                case 2:
                    T = T_min;
                    break;
                default:
                    break;
            }

            velocity = toTarget / T - Physics.gravity * T / 2f;

            return velocity;
        }
    }
}
