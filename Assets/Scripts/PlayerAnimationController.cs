using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Direction))]
[RequireComponent(typeof(PlayerInputController))]
public class PlayerAnimationController : MonoBehaviour
{
    Direction direction;
    PlayerInputController pic;

    public bool animationEnabled = true;

    [Header("Sprite Part References")]
    [SerializeField] AnimationPlayer singleAnimationPlayer;
    [SerializeField] AnimationPlayer topAnimationPlayer;
    [SerializeField] AnimationPlayer bottomAnimationPlayer;

    // Tracks if single sprite is enabled or if multiple sprites are enabled
    bool singleSpriteEnabled = true;

    // enum for animation type
    public enum AnimationType
    {
        Single,
        Top,
        Bottom
    }

    public void Awake()
    {
        direction = GetComponent<Direction>();
        pic = GetComponent<PlayerInputController>();
    }

    public void Update()
    {
        if (Time.timeScale < 0.0001) return;
        if (!animationEnabled) return;
        float xInput = pic.MoveInputX();

        if (xInput > 0) direction.SetFacing(Direction.Horizontal.Right);
        if (xInput < 0) direction.SetFacing(Direction.Horizontal.Left);
    }

    public void SetActiveAnimation(AnimationPlayable playable, AnimationType type)
    {
        // Set top animation
        if (type == AnimationType.Top)
        {
            // Enable top and bottom sprite and disable single sprite
            // Only do this if the priority is greater
            if (singleSpriteEnabled && playable.priority >= singleAnimationPlayer.GetActiveAnimation().priority)
            {
                singleSpriteEnabled = false;
                singleAnimationPlayer.gameObject.SetActive(false);
                topAnimationPlayer.gameObject.SetActive(true);
                bottomAnimationPlayer.gameObject.SetActive(true);
            }

            topAnimationPlayer.SetActiveAnimation(playable);
        }
        else if (type == AnimationType.Bottom && playable.priority >= singleAnimationPlayer.GetActiveAnimation().priority)
        {
            // Enable top and bottom sprite and disable single sprite
            if (singleSpriteEnabled)
            {
                singleSpriteEnabled = false;
                singleAnimationPlayer.gameObject.SetActive(false);
                topAnimationPlayer.gameObject.SetActive(true);
                bottomAnimationPlayer.gameObject.SetActive(true);
            }

            bottomAnimationPlayer.SetActiveAnimation(playable);
        }
        else
        {
            // Enable single and disable top and bottom sprite
            if (!singleSpriteEnabled && 
                playable.priority >= topAnimationPlayer.GetActiveAnimation().priority && 
                playable.priority >= bottomAnimationPlayer.GetActiveAnimation().priority)
            {
                singleSpriteEnabled = true;
                singleAnimationPlayer.gameObject.SetActive(true);
                topAnimationPlayer.gameObject.SetActive(false);
                bottomAnimationPlayer.gameObject.SetActive(false);
            }

            singleAnimationPlayer.SetActiveAnimation(playable);
        }
    }

    public AnimationPlayable GetActiveAnimation(AnimationType type)
    {
        if (type == AnimationType.Top)
        {
            return topAnimationPlayer.GetActiveAnimation();
        }
        else if (type == AnimationType.Bottom)
        {
            return bottomAnimationPlayer.GetActiveAnimation();
        }
        else
        {
            return singleAnimationPlayer.GetActiveAnimation();
        }
    }    

    public void SetActiveAnimationAndResetPriority(AnimationPlayable playable, AnimationType type)
    {
        // Set top animation
        if (type == AnimationType.Top)
        {
            // Enable top and bottom sprite and disable single sprite
            if (singleSpriteEnabled)
            {
                singleSpriteEnabled = false;
                singleAnimationPlayer.gameObject.SetActive(false);
                topAnimationPlayer.gameObject.SetActive(true);
                bottomAnimationPlayer.gameObject.SetActive(true);
            }

            topAnimationPlayer.SetActiveAnimationAndResetPriority(playable);
        }
        else if (type == AnimationType.Bottom)
        {
            // Enable top and bottom sprite and disable single sprite
            if (singleSpriteEnabled)
            {
                singleSpriteEnabled = false;
                singleAnimationPlayer.gameObject.SetActive(false);
                topAnimationPlayer.gameObject.SetActive(true);
                bottomAnimationPlayer.gameObject.SetActive(true);
            }

            bottomAnimationPlayer.SetActiveAnimationAndResetPriority(playable);
        }
        else
        {
            // Enable single and disable top and bottom sprite
            if (!singleSpriteEnabled)
            {
                singleSpriteEnabled = true;
                singleAnimationPlayer.gameObject.SetActive(true);
                topAnimationPlayer.gameObject.SetActive(false);
                bottomAnimationPlayer.gameObject.SetActive(false);
            }

            singleAnimationPlayer.SetActiveAnimationAndResetPriority(playable);
        }
    }

    public void ResetActiveAnimation(AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Top:
                topAnimationPlayer.ResetActiveAnimation();
                break;
            case AnimationType.Bottom:
                bottomAnimationPlayer.ResetActiveAnimation();
                break;
            case AnimationType.Single:
                singleAnimationPlayer.ResetActiveAnimation();
                break;
        }
    }
}
