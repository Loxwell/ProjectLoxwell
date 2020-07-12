using LSG;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using IMessege = LSG.Interface.IMessege;

/// <summary>
/// 현재 프로젝트 전용 클래스
/// </summary>
namespace InGame
{
    public class InGameUIHP : MonoBehaviour, IMessege
    {
        readonly int A_HEART = 4;
        int MaxHP { get => m_maxHP / A_HEART + (m_maxHP % A_HEART != 0 ? 1 : 0); }

        [SerializeField]
        EInGameID ID;
        [SerializeField]
        List<InGameUIHeart> m_hearts;
        /// <summary>
        /// 0 : Empty, 4 : Full State
        /// </summary>
        [SerializeField]
        Sprite[] m_heartImages;
        int m_maxHP;

        private void Awake()
        {
            LSG.Utilities.Messenger<EInGameID>.Add(ID, (IMessege)this);
            for (int i = 0, len = m_hearts.Count; i < len; ++i)
                m_hearts[i].Activate = false;
        }

        public void Send(params object[] param)
        {
            if (param.Length > 0)
            {
                if (param[0] is ECommand)
                {
                    switch ((ECommand)param[0])
                    {
                        case ECommand.ACTIVATE_WITH_PARAM:
                            {
                                m_maxHP = m_hearts.Count;
                                if (param.Length > 1)
                                    m_maxHP = (int)param[1];
                                else
                                    Debug.LogError("초기 HP 값 적용 할 것");

                                ChangeHP(m_maxHP);
                            }
                            break;
                        case ECommand.DEACTIVATE:
                            {
                                for (int i = 0, len = m_hearts.Count; i < len; ++i)
                                    m_hearts[i].Activate = false;
                            }
                            break;
                        case ECommand.SET:
                                ChangeHP((int)param[1]);
                            break;
                    }
                }
            }
        }


        
        void ChangeHP(int HP)
        {
            int rest = (HP % A_HEART);
            int last = (HP - 1) / A_HEART;

            if (last >= m_hearts.Count)
                last = m_hearts.Count - 1;

            for (int i = 0; i < m_hearts.Count; ++i)
            {
                int imgIdx = i < last ? A_HEART : (i > last || HP == 0) ? 0 : ((rest == 0) ? A_HEART : rest);
                m_hearts[i].Activate = (MaxHP > i);
                m_hearts[i].Value = m_heartImages[imgIdx];
                m_hearts[i].Popup(i == last);
            }
        }
    }
}

