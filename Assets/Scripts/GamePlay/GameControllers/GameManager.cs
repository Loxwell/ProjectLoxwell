using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Platformer.Mechanics;
using PlayerMainController = LSG.PlayerMainController;

namespace LSG.GameControllers
{
#pragma warning disable
    public class GameManager : GameController
    {
        public static PlayerMainController hero;

        protected override void Awake()
        {
            base.Awake();

        }

        private void Start()
        {

#if UNITY_EDITOR
            // 동적 생성 해서 메모리 관리 할 것
            hero = GameObject.FindObjectOfType<PlayerMainController>();
#endif

            base.model.player = hero.GetComponent<KinematicObject>();
        }
    }
}


