using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    [Condition("Input/CheckButtonDown")]
    [Help("Checks whether a button press")]
    public class CheckButtonDown : ConditionBase
    {
        [InParam("butoon", DefaultValue = "Attack")]
        [Help("Button expected to press")]
        public string button;

        public override bool Check()
        {
            return Input.GetButtonDown(button);
        }
    }
}

