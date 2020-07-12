using InGame;
using LSG;
using LSG.Interface;
using LSG.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace InGame
{
    public enum EEffectID { HIT = 0 }

    public class EffectManager : MonoBehaviour, LSG.Interface.IMessege
    {
        [SerializeField]
        GameObject[] effects;
        private void Awake()
        {
            Messenger<EInGameID>.Add(EInGameID.EFFECT_MANAGER, this);
        }

        void OnEnable()
        {
            for (int i = 0, len = effects.Length; i < len; ++i)
                effects[i].SetActive(false);
        }

        /// <summary>
        /// </summary>
        /// <param name="param"> 
        /// int id = param[0]
        /// Vector3 pos = param[1]
        /// </param>
        public void Send(params object[] param)
        {
            int id = (int)param[0];
            Vector3 pos = (Vector3)param[1];
            effects[id].SetActive(true);
            effects[(int)EEffectID.HIT].transform.position = pos;
        }
    }
}