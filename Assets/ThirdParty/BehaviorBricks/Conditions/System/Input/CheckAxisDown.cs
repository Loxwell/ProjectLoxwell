using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;
using Pada1.BBCore.Tasks;

namespace BBCore.Conditions
{
    [Condition("Input/CheckAxisDown")]
    public class CheckAxisDown : ConditionBase
    {
        public override bool Check()
        {
            float a = Input.GetAxis("Vertical");
            
            TextMesh text = GameObject.FindObjectOfType<TextMesh>();
            text.text = a.ToString();


            return a < 0 || Input.GetKeyDown(KeyCode.DownArrow);
        }


        public override TaskStatus MonitorCompleteWhenTrue()
        {
            if (Check())
                return TaskStatus.COMPLETED;

            return TaskStatus.FAILED;
        }
    }
}


