
using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    [Condition("Input/CheckButtonUp")]
    [Help("Checks wheter a button up")]
    public class CheckButtonUp : ConditionBase
    {
        // <Value> Input Button Name Parameter
        [InParam("button", DefaultValue = "Attack"), Help("Button expected to be released")]
        public string button;

        public override bool Check()
        {
            return !string.IsNullOrEmpty(button) && Input.GetButtonUp(button);
        }
    }
}