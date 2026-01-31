using UnityEngine;

public class AnimatorStateSync : MonoBehaviour
{
    [SerializeField] private Animator baseAnimator;
    [SerializeField] private Animator blendAnimator;

    private void LateUpdate()
    {
        if (baseAnimator == null || blendAnimator == null)
            return;

        var state = baseAnimator.GetCurrentAnimatorStateInfo(0);

        int stateHash = state.fullPathHash;
        float time = state.normalizedTime;

        blendAnimator.Play(stateHash, 0, time % 1f);
        blendAnimator.Update(0f);

        blendAnimator.speed = 0f;
    }

    private void OnEnable()
    {
        if (blendAnimator != null)
            blendAnimator.speed = 0f;
    }
}
