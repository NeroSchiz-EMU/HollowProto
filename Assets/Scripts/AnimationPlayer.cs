using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    [SerializeField] Animator animator;

    AnimationPlayable nullState = new AnimationPlayable();

    AnimationPlayable currentlyPlaying = new AnimationPlayable();

    /// <summary>
    /// Play a Playable. The Playable will only play if it has an equal or higher priority.
    /// </summary>
    /// <param name="playable">the Playable to play</param>
    public void SetActiveAnimation(AnimationPlayable playable)
    {
        // can the current playable be played?
        if (playable.priority >= currentlyPlaying.priority)
        {
            currentlyPlaying = playable;
			animator.Play(playable.animationStateName);
		}
	}

    /// <summary>
    /// Play a Playable, overriding its priority. Don't use this unless you have a specific need. 
    /// </summary>
    /// <param name="playable">the Playable to play</param>
    /// <param name="priorityOverride">the priority to override</param>
    public void SetActiveAnimation(AnimationPlayable playable, uint priorityOverride)
    {
        SetActiveAnimation(new AnimationPlayable(playable.animationStateName, priorityOverride));
	}

    public void SetActiveAnimationAndResetPriority(AnimationPlayable playable)
    {
        currentlyPlaying = nullState;
        SetActiveAnimation(playable);
	}

    public void ResetActiveAnimation()
    {
        currentlyPlaying = nullState;
    }

    public AnimationPlayable GetActiveAnimation()
    {
        return currentlyPlaying;
    }
}
