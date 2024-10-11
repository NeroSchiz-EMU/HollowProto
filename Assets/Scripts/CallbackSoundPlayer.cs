using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackSoundPlayer : MonoBehaviour
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = 0.2f;
    public void PlaySound()
    {
		SoundFXManager.instance.PlaySoundFXClip(clip, transform, volume);
	}
}
