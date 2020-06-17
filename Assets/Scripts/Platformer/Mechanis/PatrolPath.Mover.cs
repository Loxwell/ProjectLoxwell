using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Platformer.Mechanics {

    public partial class PatrolPath
    {
        public class Mover
        {
            public Vector2 Position
            {
                get {
                    p = Mathf.InverseLerp(0, m_duration, Mathf.PingPong(Time.time - stTime, m_duration));
                    return m_path.transform.TransformPoint(Vector2.Lerp(m_path.stPos, m_path.edPos, p));
                }
            }

            private PatrolPath m_path;
            private float m_duration;
            private float stTime, p;

            public Mover(PatrolPath path, float speed)
            {
                m_path = path;
                m_duration = (path.edPos - path.stPos).magnitude / speed;
                stTime = Time.time;
            }

        }
    }
}



