using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    [Condition("Basic/CheckAxisDown")]
    public class CheckAxisDown : ConditionBase
    {
        public override bool Check()
        {
            return Input.GetAxis("Vertical") < 0 || Input.GetKeyDown(KeyCode.DownArrow);
        }
    }
}


