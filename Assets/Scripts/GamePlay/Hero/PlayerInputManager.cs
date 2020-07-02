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
        public int HorizontaAxis => (keyRight.IsKeyDown || keyRight.IsPressing) ? 1 : (keyLeft.IsKeyDown || keyLeft.IsPressing) ? -1 : 0;
        public int VerticalAxis => (keyDown.IsKeyDown || keyRight.IsPressing) ? -1 : 0;

        const uint CROSS_KEY = 0x0000ff;
        const uint ACTION_BUTTON = 0xffff00;

        public InputState keyUp;
        public InputState keyDown;
        public InputState keyLeft;
        public InputState keyRight;
        public InputState keyJump;
        public InputState keyAction1;
        public InputState magicKey;

        uint m_curInputState;
        uint m_preInputState;

        private void Awake()
        {
            keyUp = new InputState(this, EInputState.UP_KEY);
            keyDown = new InputState(this, EInputState.DOWN_KEY);
            keyLeft = new InputState(this, EInputState.LEFT_KEY);
            keyRight = new InputState(this, EInputState.RIGHT_KEY);
            keyAction1 = new InputState(this, EInputState.ACTION_1);
            keyJump = new InputState(this, EInputState.JUMP);
        }

        private void OnEnable()
        {
            Initialize();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_preInputState = m_curInputState;

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
                ReleasedLeft();
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                PressedLeft();
                ReleasedRight();
            }
            else
            {
                ReleasedLeft();
                ReleasedRight();
            }
            
            if (Input.GetAxis("Vertical") < -0.3)
                PressedDown();
            else
                ReleasedDown();
        }

        void Initialize()
        {
            m_preInputState = m_curInputState = (int)EInputState.NONE;
        }

        #region DOWN
        public void PressedDown()
        {

            SetInputKey(ref m_curInputState, EInputState.DOWN_KEY);
        }

        public void ReleasedDown()
        {
            ReleaseFlag(ref m_curInputState, EInputState.DOWN_KEY);
        }
        #endregion

        #region RIGHT
        public void PressedRight()
        {
            ReleasedLeft();
            SetInputKey(ref m_curInputState, EInputState.RIGHT_KEY);
        }

        public void ReleasedRight()
        {
            ReleaseFlag(ref m_curInputState, EInputState.RIGHT_KEY);

        }
        #endregion


        #region Left
        public void PressedLeft()
        {
            ReleasedRight();
            SetInputKey(ref m_curInputState, EInputState.LEFT_KEY);
        }

        public void ReleasedLeft()
        {
            ReleaseFlag(ref m_curInputState, EInputState.LEFT_KEY);
        }
        #endregion


        #region ACTION
        public void PressedAction1()
        {
            SetInputKey(ref m_curInputState, EInputState.ACTION_1);
        }

        public void ReleasedAction1()
        {
            ReleaseFlag(ref m_curInputState, EInputState.ACTION_1);
        }
        #endregion

        #region JUMP
        public void PressedJump()
        {
            SetInputKey(ref m_curInputState, EInputState.JUMP);
        }

        public void ReleasedJump()
        {
            ReleaseFlag(ref m_curInputState, EInputState.JUMP);
        }
        #endregion

        //static void SetCrossKey(ref uint inputState, EInputState key)
        //{
        //    inputState = (0x01u << (int)key) | inputState;
        //}

        static void SetInputKey(ref uint inputState, EInputState key)
        {
            inputState = (0x01u << (int)key) | inputState;
        }

        public class InputState
        {
            public float Value
            {
                get
                {   return m_AixsValue; }

                set
                {   m_AixsValue = value;  }
            }

            public bool IsKeyDown => IsMarkedFlag(m_inputManager.m_curInputState, (int)m_assignedKey) && !IsMarkedFlag(m_inputManager.m_preInputState, (int)m_assignedKey);

            public bool IsPressing => IsMarkedFlag(m_inputManager.m_curInputState, (int)m_assignedKey);

            public bool IsUp => !IsMarkedFlag(m_inputManager.m_curInputState, (int)m_assignedKey)
                    && IsMarkedFlag(m_inputManager.m_preInputState, (int)m_assignedKey);

            PlayerInputManager m_inputManager;
            EInputState m_assignedKey;
            float m_AixsValue = 0;

            public InputState(PlayerInputManager inputManager,EInputState assignedKey)
            {
                m_assignedKey = assignedKey;
                m_inputManager = inputManager;
            }
        }
    }
}

