
using ScheduleSystem.Core;
using Platformer.Mechanics;
using System;

namespace Platformer.GamePlay
{
    [Obsolete("사용안함")]
    public class EnablePlayerInput : Simulation.Event<EnablePlayerInput>
    {
        public PlayerMovementController player;
        public override void Execute()
        {
        }
    }
    [Obsolete("사용안함")]
    public class PlayerJumped : Simulation.Event<PlayerJumped>
    {
        public PlayerMovementController player;
        public override void Execute()
        {
        }
    }
    [Obsolete("사용안함")]
    public class PlayerLaneded : Simulation.Event<PlayerLaneded>
    {
        public PlayerMovementController player;

        public override void Execute()
        {
        }
    }

    [Obsolete("사용안함")]
    public class PlayerStopJump : Simulation.Event<PlayerStopJump>
    {
        public PlayerMovementController player;

        public override void Execute()
        {
        }
    }
}