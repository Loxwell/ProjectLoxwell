using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;

namespace BBCore.Conditions
{
    [Condition("Input/CheckAxis")]
    public class CheckAxis : ConditionBase
    {
        [InParam("axis", DefaultValue = "Horizontal")]
        public string axis;

        public override bool Check()
        {
            return Input.GetAxis(axis) != 0;
        }
    }
}
