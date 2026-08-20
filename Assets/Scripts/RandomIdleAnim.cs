using UnityEngine;

public class RandomIdleAnim : StateMachineBehaviour
{
    [SerializeField] private int _maxIndex = 5;
    [SerializeField] private float _chanceOfIdle = 0.90f;
    [SerializeField] private float _currentChance = 0f;
    [SerializeField] private int _currentRandomIndex = 0;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _currentRandomIndex = 0;

        animator.SetFloat("IdleState", (float)_currentRandomIndex);

        _currentChance = Random.Range(0, 1f);

        if(_currentChance >= _chanceOfIdle)
        {
            _currentRandomIndex = Random.Range(0, _maxIndex + 1);
            animator.SetFloat("IdleState", (float)_currentRandomIndex);
        }
    }
}