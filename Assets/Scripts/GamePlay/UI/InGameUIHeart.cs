
using UnityEngine;
using UnityEngine.UI;

namespace InGame
{
    public class InGameUIHeart : MonoBehaviour
    {
        readonly int HASH_ACTIVATE = Animator.StringToHash("ChangeState");
        public bool Activate
        {
            set
            {  
                if(!m_image)
                    m_image = GetComponent<Image>();
                if(m_image.enabled != value)
                    m_image.enabled = value;  
            }

            get
            {
                if (!m_image)
                    m_image = GetComponent<Image>();
               return  m_image.enabled;
            }
        }

        public Sprite Value
        {
            set
            {
                m_image.sprite = value;
            }
        }

        Image m_image;
        Animator m_animator;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
        }

        public void Popup(bool bPopup)
        {
            if(bPopup)
                m_animator.SetTrigger(HASH_ACTIVATE);
        }
    }
}

