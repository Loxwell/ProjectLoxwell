using ScheduleSystem.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class GameController : MonoBehaviour
    {
        static GameController g_instance;
        public static GameController Instance
        {
            get
            {
                if (!g_instance)
                    g_instance = GameObject.FindObjectOfType<GameController>();
                return g_instance;
            }
        }

        public PlatformerModel model = Simulation.GetModel<PlatformerModel>();

        private void OnDisable()
        {
            if (g_instance == this)
                g_instance = null;
        }

        private void OnDestroy()
        {
            Simulation.Destroy();
        }

        // Update is called once per frame
        void Update()
        {
            if (g_instance == this)
                Simulation.Tick();
        }
    }
}

