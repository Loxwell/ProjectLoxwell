using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScheduleSystem.Core;
using Platformer.Mechanics;
using ScreenFader = Platformer.Helper.ScreenFader;
using UnityEngine.Rendering.Universal;

namespace InGame.Event
{
    public class EventCheckPoint : Simulation.Event<EventCheckPoint>
    {
        PlayerCharacter m_player;
        Checkpoint m_checkPoint;

        public override void Execute()
        {
            m_player?.SetChekpoint(m_checkPoint);
        }

        internal override void Initialize(params object[] param)
        {
            m_checkPoint = (Checkpoint)param[0];
            m_player = (PlayerCharacter)param[1];
        }

        internal override void Cleanup()
        {
            m_player = null;
            m_checkPoint = null;
        }
    }

    public class EventRespawn : Simulation.Event<EventRespawn>
    {
        public static bool DoingEvnet => g_doingEvent;
        static Coroutine currentCoroutine = null;
        static bool g_doingEvent;
        
        PlayerCharacter m_player;
        bool m_resetHealth = false;
        bool m_useCheckPoint = true;
        

        public EventRespawn()
        {
            g_doingEvent = false;
        }

        ~EventRespawn()
        {
            currentCoroutine = null;
            g_doingEvent = false;
        }

        public override void Execute()
        {
            if(!g_doingEvent && m_player)
            {
                g_doingEvent = true;
                if (currentCoroutine != null)
                    m_player.StopCoroutine(currentCoroutine);
                currentCoroutine = m_player.StartCoroutine(RespawnProcess());
            }
        }
        /// <summary>
        /// PlayerCharacter m_player = param[0]
        /// bool resetHealth = param[1]
        /// bool useCheckPoint = param[2]
        /// </summary>
        /// <param name="param">PlayerCharacter player, bool resetHealth, bool useCheckPoint</param>
        internal override void Initialize(params object[] param)
        {
            g_doingEvent = false;
            m_player = null;

            if (param.Length > 0)
            {
                m_player = (PlayerCharacter)param[0];
                m_resetHealth = (bool)param[1];
                m_useCheckPoint = (bool)param[2];
            }
        }

        void Respawn()
        {
            if (m_resetHealth)
                m_player.damageable.SetHealth(m_player.damageable.startingHealth);

            m_player.StopFlickering();
            m_player.UpdateFacing(m_player.spriteOriginallyFacesLeft);
            m_player.Teleport();
        }

        IEnumerator RespawnProcess()
        {
            m_player.inputManager.Disable();
            yield return new WaitForSeconds(ScreenFader.Instance.fadeDuration); //wait one second before respawing
            yield return m_player.StartCoroutine(ScreenFader.FadeSceneOut(m_useCheckPoint ? ScreenFader.FadeType.Black : ScreenFader.FadeType.GameOver));
            if (!m_useCheckPoint)
                yield return new WaitForSeconds(1f);
            Respawn();
            yield return new WaitForEndOfFrame();
            yield return m_player.StartCoroutine(ScreenFader.FadeSceneIn());
            m_player.inputManager.Enable();
            g_doingEvent = false;
        }
    }
}