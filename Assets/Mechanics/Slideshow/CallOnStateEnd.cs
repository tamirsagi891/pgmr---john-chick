using UnityEngine;

namespace Mechanics.Slideshow
{
    public class CallOnStateEnd : StateMachineBehaviour
    {
        [SerializeField]
        private string funcName = "NextSlide";

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.SendMessage(funcName);
        }
        
    }
}