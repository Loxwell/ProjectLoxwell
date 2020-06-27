using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSG.Utilities 
{

    public class ParallaxLayer : MonoBehaviour
    {

        [SerializeField]
        Vector3 m_scale = Vector3.zero;

        Transform m_target;
        Transform m_cachedTransform;
        Vector3 m_originPos;

        private void Start()
        {
            m_target = GameObject.FindObjectOfType<Camera>().transform;
            m_cachedTransform = transform;
            m_originPos = transform.position;
        }

        void LateUpdate()
        {
            Vector3 dir =( m_target.position - m_cachedTransform.position).normalized;
            m_cachedTransform.position = Vector3.Scale(dir, m_scale) + m_originPos;
        }
    }

}
