using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer.Mechanics.View
{
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField]
        Vector3 m_movementScale = Vector3.one;

        Transform m_target;
        Transform m_cachedTransform;
        // Start is called before the first frame update
        void Start()
        {
            m_target = GameObject.FindObjectOfType<Camera>().transform;
            m_cachedTransform = transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            m_cachedTransform.position = Vector3.Scale(m_target.position, m_movementScale);
        }
    }

}

