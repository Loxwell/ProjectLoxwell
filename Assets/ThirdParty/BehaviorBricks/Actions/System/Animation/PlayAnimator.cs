using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;

namespace BBUnity.Actions
{

    [Action("Animation/PlayAnimatorIntValue")]
    public class PlayAnimatorIntValue : GOAction
    {
        [InParam("animator")]
        public Animator animator;

        [InParam("paramter")]
        [Help("Int type variable name")]
        public string paramter;

        [InParam("fullPathStateName", DefaultValue = "")]
        [Help("Animator State Full Name")]
        public string fullPathStateName;

        [InParam("value", DefaultValue = 0)]
        [Help("Transite next state")]
        public int value;

        int m_hashName;

        public override void OnStart()
        {
            base.OnStart();

            if (m_hashName == 0 && animator && !string.IsNullOrEmpty(paramter))
            {
                m_hashName = Animator.StringToHash(paramter);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if(animator.GetInteger(paramter) != value)
                animator.SetInteger(paramter, value);

            if(string.IsNullOrEmpty(fullPathStateName))
            {
            }else
            {
                return TaskStatus.COMPLETED;
            }

            return TaskStatus.RUNNING;
        }
    }
}


