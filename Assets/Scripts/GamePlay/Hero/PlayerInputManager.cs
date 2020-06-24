using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LSG.Utilities.BitField;

namespace LSG
{
    public enum EInputState : byte
    {
        NONE = 0, LEFT_KEY = 1, RIGHT_KEY = 2, DOWN_KEY = 3, UP_KEY = 4, JUMP = 8, ACTION_1 = 9, ACTION_2 = 10, ACTION_3 = 11
    }

    public class PlayerInputManager : MonoBehaviour
    {
        const uint CROSS_KEY = 0x0000ff;
        const uint ACTION_BUTTON = 0xffff00;

#pragma warning disable
        [SerializeField]
        HeroBlackboard bb;

        uint m_inputState;
        uint m_preInputState;

        private void OnEnable()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {

#if UNITY_EDITOR
            if (Input.GetButton("Jump"))
                PressedJump();
            else
                ReleasedJump();

            if (Input.GetButton("Fire1"))
                PressedAction1();
            else
                ReleasedAction1();

            if (Input.GetAxis("Horizontal") > 0)
            {
                PressedRight();
            }
            else if (Input.GetAxis("Horizontal") < 0)
                PressedLeft();
            else
            {
                ReleasedLeft();
                ReleasedRight();
            }
            
            if (Input.GetAxis("Vertical") < -0.3)
                PressedDown();
            else
                ReleasedDown();
#endif

            bb.curFrameInputState = m_inputState;
            bb.preFrameInputState = m_preInputState;
            m_preInputState = m_inputState;
        }

        void Initialize()
        {
            m_preInputState = m_inputState = (int)EInputState.NONE;
        }

        #region DOWN
        public void PressedDown()
        {
            SetCrossKey(ref m_inputState, EInputState.DOWN_KEY);
        }

        public void ReleasedDown()
        {
            ReleaseFlag(ref m_inputState, EInputState.DOWN_KEY);
        }
        #endregion

        #region RIGHT
        public void PressedRight()
        {
            SetCrossKey(ref m_inputState, EInputState.RIGHT_KEY);
        }

        public void ReleasedRight()
        {
            ReleaseFlag(ref m_inputState, EInputState.RIGHT_KEY);

        }
        #endregion


        #region Left
        public void PressedLeft()
        {
            SetCrossKey(ref m_inputState, EInputState.LEFT_KEY);
        }

        public void ReleasedLeft()
        {
            ReleaseFlag(ref m_inputState, EInputState.LEFT_KEY);
        }
        #endregion


        #region ACTION
        public void PressedAction1()
        {
            SetActionButton(ref m_inputState, EInputState.ACTION_1);
        }

        public void ReleasedAction1()
        {
            ReleaseFlag(ref m_inputState, EInputState.ACTION_1);
        }
        #endregion

        #region JUMP
        public void PressedJump()
        {
            SetActionButton(ref m_inputState, EInputState.JUMP);
        }

        public void ReleasedJump()
        {
            ReleaseFlag(ref m_inputState, EInputState.JUMP);
        }
        #endregion

        static void SetCrossKey(ref uint inputState, EInputState key)
        {
            inputState = (0x01u << (int)key) | (ACTION_BUTTON & inputState);
        }

        static void SetActionButton(ref uint inputState, EInputState key)
        {
            inputState = (0x01u << (int)key) | inputState;
        }
    }

}

