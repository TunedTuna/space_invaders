using UnityEngine;

public class EnemyShockedSGHandler : StateMachineBehaviour
{
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //= animator.GetComponent<SpriteRenderer>().material;
        ShaderController control = animator.GetComponent<ShaderController>();
        if (control != null)
        {
            control.EnableGradient();
        }

    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Material mat = animator.GetComponent<SpriteRenderer>().material;
        ShaderController control = animator.GetComponent<ShaderController>();
        if (control != null)
        {
            control.DisableGradient();
        }

    }
}
