using System.Collections;
using UnityEngine;
using Path =  Platformer.Mechanics.PatrolPath;

namespace Platformer.Mechanics
{
    public interface IPatrolUtil
    {
        void SetPatroller(Patroller p);
    }

    public class Patroller : MonoBehaviour
    {
#pragma warning disable 
        [SerializeField]
        Transform m_target;

        [SerializeField, Range(0.1f, 10f)]
        float m_maxMovingSpeed = 1;

        [SerializeField, Header("class PatrolPath")]
        Path m_path = null;

        Coroutine m_currentProcess;

        private void OnEnable() => MovingBegins();
        
        private void OnDisable() => Stop();
        
        public bool MovingBegins()
        {
            Stop();

            if (m_path && m_target)
            {
                Path.Mover mover = m_path.CreateMover(m_maxMovingSpeed * 0.5f);
                if (m_target && m_target.GetComponent<IPatrolUtil>() != null)
                    m_target.GetComponent<IPatrolUtil>().SetPatroller(this);

                m_currentProcess = StartCoroutine(UpdateMovingProcess(mover));
                return true;
            }

            return false;
        }

        public void Stop() => this.SafeStopCoroutine(m_currentProcess);
        
        IEnumerator UpdateMovingProcess(Path.Mover mover)
        {
            
            Transform t = transform;

            while (true)
            {
                //float x = Mathf.Clamp(.x - t.position.x, -1, 1);
                m_target.position = mover.Position;
                yield return null;
            }
        }
    }
}
