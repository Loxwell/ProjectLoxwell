using ScheduleSystem.Core;
using Platformer.Model;
using UnityEngine;

namespace Platformer.Mechanics
{
    public class SimulationController : MonoBehaviour
    {
        private void OnDestroy()
        {
            Simulation.Destroy();
        }

        // Update is called once per frame
        void Update()
        {
            
            Simulation.Tick();
        }
    }
}

