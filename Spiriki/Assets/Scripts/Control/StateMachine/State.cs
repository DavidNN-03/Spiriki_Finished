using UnityEngine;

namespace Spiriki.Control
{
    public abstract class State /*The parent-class of all states.*/
    {
        public abstract void Enter(); /*Abstract Enter()-function for the child classes to override.*/
        public abstract void Tick(float deltaTime); /*Abstract Tick()-function for the child classes to override.*/
        public abstract void Exit(); /*Abstract Exit()-function for the child classes to override.*/

        protected float GetNormalizedTime(Animator animator, string tag) /*Returns how far an animator has gotten through an animation.*/
        {
            AnimatorStateInfo currentInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimatorStateInfo nextInfo = animator.GetNextAnimatorStateInfo(0);

            if (animator.IsInTransition(0) && nextInfo.IsTag(tag))
            {
                return nextInfo.normalizedTime;
            }
            else if (!animator.IsInTransition(0) && currentInfo.IsTag(tag))
            {
                return currentInfo.normalizedTime;
            }
            else
            {
                return 0f;
            }
        }
    }
}

