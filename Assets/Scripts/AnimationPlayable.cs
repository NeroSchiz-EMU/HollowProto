
using UnityEngine;

public struct AnimationPlayable
{
    readonly public string animationStateName; // I hope the compiler is smart enough to not allocate/copy these all the time... We don't have Move semantics in C#
	readonly public uint priority;

	public AnimationPlayable(string a, uint p)  // we don't have record structs because Unity
    {
        animationStateName = a;
        priority = p;
    }

    public static bool operator==(AnimationPlayable a, AnimationPlayable b)
    {
        return a.animationStateName == b.animationStateName;
    }

    public static bool operator!=(AnimationPlayable a, AnimationPlayable b)
    {
        return !(a == b);
    }
}
