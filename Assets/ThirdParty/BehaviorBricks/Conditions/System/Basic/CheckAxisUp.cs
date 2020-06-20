using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    [Condition("Basic/CheckAxisUp")]
    public class CheckAxisUp : ConditionBase
    {
        public override bool Check()
        {
            return Input.GetAxis("Horizontal") > 0 ||
                Input.GetKey(KeyCode.UpArrow);
        }
    }
}

