
using ScheduleSystem.Core;
using Platformer.Mechanics;


namespace Platformer.GamePlay
{
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        public PlayerController player;
        public override void Execute()
        {
            
        }
    }

    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
        public PlayerController player;
        public override void Execute()
        {
            
        }
    }

    public class PlayerLaneded : Simulation.Event<PlayerLaneded>
    {
        public PlayerController player;

        public override void Execute()
        {
            
        }
    }
}