using UnityEngine;

namespace Entities.Player
{
    public class Player_Dash : StateMachineBehaviour
    {
        private Player _player;
    
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player = animator.GetComponentInParent<Player>();
            
            _player.LoseControl();
            _player.SetDashing(true);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _player.GainControl();
            _player.SetDashing(false);
        }
    }
}
