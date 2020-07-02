using UnityEngine;
using System.Collections.Generic;

namespace LSG.Utilities
{
    public class AnimatorHelper
    {
        public Animator Animator => m_animator;

        AnimatorOverrideController m_aoc;
        AnimationClipOverrides m_clipOverrides;
        Animator m_animator;

        public AnimatorHelper(Animator ani)
        {
            this.m_animator = ani;
            m_aoc = new AnimatorOverrideController(m_animator.runtimeAnimatorController);
            m_clipOverrides = new AnimationClipOverrides(m_aoc.overridesCount);
            foreach (AnimationClip clip in m_aoc.animationClips)
                m_clipOverrides.Add(new KeyValuePair<AnimationClip, AnimationClip>(clip, clip));
            m_animator.runtimeAnimatorController = m_aoc;
        }
        ~AnimatorHelper()
        {
            Object.Destroy(m_animator.runtimeAnimatorController);
            m_clipOverrides.Clear();
            m_clipOverrides.TrimExcess();

            m_animator.runtimeAnimatorController = null;
            m_clipOverrides = null;
            m_animator = null;
            m_aoc = null;
        }

        public void ChangeClip(string key, AnimationClip newClip)
        {
            if (m_clipOverrides == null || m_aoc == null)
                return;

            m_clipOverrides[key] = newClip;
            m_aoc.ApplyOverrides(m_clipOverrides);
        }

        internal class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
        {
            public AnimationClipOverrides(int capacity) : base(capacity) { }

            public AnimationClip this[string name]
            {
                get { return this.Find(x => x.Key.name.Equals(name)).Value; }
                set
                {
                    int index = this.FindIndex(x => x.Key.name.Equals(name));
                    if (index != -1)
                        this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
                }
            }
        }
    }
}


//https://docs.unity3d.com/kr/current/Manual/AnimatorOverrideController.html
/* animator 
 public class WeaponTemplate : MonoBehaviour {
     
     public int damage;
     //weapon animations override
     public AnimatorOverrideController animationsOverride;
 
     //character animator
     public Animator anim;
 
     public void Equip(){
       anim.runtimeAnimatorController = animationsOverride;
     }
 
 }
 */


/*
 https://docs.unity3d.com/ScriptReference/AnimatorOverrideController.html
동적으로 애니메이터의 상태에 애니 클립을 적용 하는 예제
https://support.unity3d.com/hc/en-us/articles/205845885-Animator-state-is-reset-when-AnimationClips-are-replaced-using-an-AnimatorControllerOverride
 */
