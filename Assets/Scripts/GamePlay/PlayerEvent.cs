
using ScheduleSystem.Core;
using Platformer.Mechanics;


namespace Platformer.GamePlay
{
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        public PlayerMovementController player;
        public override void Execute()
        {
            
        }
    }

    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
        public PlayerMovementController player;
        public override void Execute()
        {
            
        }
    }

    public class PlayerLaneded : Simulation.Event<PlayerLaneded>
    {
        public PlayerMovementController player;

        public override void Execute()
        {
            
        }
    }

    public class PlayerStopJump : Simulation.Event<PlayerStopJump>
    {
        public PlayerMovementController player;

        public override void Execute()
        {

        }
    }
}