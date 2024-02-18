using UnityEngine;

public class ArcherAnimator : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _aimAnimationTime;

    private static readonly int AimAnimationStateHash = Animator.StringToHash("Aim");
    private static readonly int IdleAnimationStateHash = Animator.StringToHash("Idle");
    private static readonly int WalkAnimationStateHash = Animator.StringToHash("Walk");

    public void PlayAimAnimation()
    {
        _animator.Play(AimAnimationStateHash, 0, _aimAnimationTime);
    }

    public void PlayIdleAnimation()
    {
        _animator.Play(IdleAnimationStateHash);
    }
    
    public void PlayWalkAnimation()
    {
        _animator.Play(WalkAnimationStateHash);
    }

    public void SetActive(bool value)
    {
        _animator.enabled = value;
    }
}