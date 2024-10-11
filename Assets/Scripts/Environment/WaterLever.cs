using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterLever : MonoBehaviour, HitHandler
{
    [SerializeField] bool isTriggered = false;
    private bool initialState = false;
    // Reference to on and off sprites
    [SerializeField] GameObject offSprite;
    [SerializeField] GameObject onSprite;
    [SerializeField] AudioClip leverPulledClip;
    // Event published when the water lever is hit
    public UnityEvent leverHitOnEvent;
    public UnityEvent leverHitOffEvent;
    
    

    // Store last time hit so this doesn't freak out
    float lastTimeHit = -99f;
    float timeBetweenHits = 1f;

    public void Awake()
    {
        initialState = isTriggered;
        SetOnOff(isTriggered);
    }
    public void SetOnOff(bool on)
    {
        isTriggered = on;
        offSprite.SetActive(!on);
        onSprite.SetActive(on);
    }

    public void Hit(Vector2 force, float damage = 1)
    {
        if (Time.time - lastTimeHit < timeBetweenHits)
        {
            return;
        }

        if (!isTriggered)
        {
            leverHitOnEvent.Invoke();
            SoundFXManager.instance.PlaySoundFXClip(leverPulledClip, transform, 1.2f);
            SetOnOff(true);
            
        }
        else
        {
            leverHitOffEvent.Invoke();
            SoundFXManager.instance.PlaySoundFXClip(leverPulledClip, transform, 1.2f);
            SetOnOff(false);
            
        }

        // Set hit time
        lastTimeHit = Time.time;
        
        
    }

    //sets lever back to initial state
    public void Reset(){
        SetOnOff(initialState);
    }
}
