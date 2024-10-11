using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class Health : MonoBehaviour, HitHandler
{
    [SerializeField] float health = Mathf.Infinity;
    [SerializeField] float initialHealth = 1f;
    [SerializeField] float maxHealth = Mathf.Infinity;

    [SerializeField] AudioClip healSoundClip;
    [SerializeField] AudioClip damageSoundClip;
    [SerializeField] AudioClip deathSoundClip;

    [SerializeField] GameObject ItemToDrop;
    //100 dropchance = 100% chance of drop
    [SerializeField] float DropChance = 100f;
    
    //---------------------------------
    public Transform respawnPoint;
    //--------------------------------
    
    bool invulnerable = false;

    private HealthUI healthUI;

    public UnityEvent PlayerDeath;

    [SerializeField] PlayerAnimationController playerAnimationController;
    // [SerializeField] PlayerAttack attack;

    // Animations
    [SerializeField] float playerDeathTime = 5f;
    [SerializeField] float playerDamageTime = 1f;
    AnimationPlayable damageAnimation = new AnimationPlayable("enyd_damage", 3);
    AnimationPlayable deathAnimation = new AnimationPlayable("enyd_death", 4);

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        //only update UI if script parent is player
        if(gameObject.tag == "Player"){
            healthUI = FindObjectOfType<HealthUI>();
        }

        SetHealth(initialHealth);
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public void SetMaxHealth(float mh)
    {
        maxHealth = mh;
    }

    public void IncrementMaxHealth(float dmh)
    {
        SetMaxHealth(GetMaxHealth() + dmh);
    }

     public void SetHealth(float value)
    {
        if (health == 0 && value <= 0) return;
        health = Mathf.Clamp(value, 0, maxHealth);

        if (healthUI != null){

            if(maxHealth > healthUI.GetShowableHearts()){
                maxHealth = healthUI.GetShowableHearts();
                health = Mathf.Clamp(value, 0, maxHealth);
            }


            healthUI.UpdateHealthUI((int) health);
        }

        if (health <= 0)
        {
            Die();
        }
    }


    public void Heal(float amount)
    {
        if (amount == 0) return;
        SetHealth(GetHealth() + amount);

        //play sound FX
        SoundFXManager.instance.PlaySoundFXClip(healSoundClip, transform, 1f);
    }

    public void Damage(float amount)
    {
        if (amount == 0) return;
        SetHealth(GetHealth() - amount);
        
        //play sound FX
        SoundFXManager.instance.PlaySoundFXClip(damageSoundClip, transform, 1f);

        // Interrupt attack if necessary
        MeleeAttack meleeAttack = GetComponent<MeleeAttack>();
        if (meleeAttack)
            meleeAttack.AttackEnd();

        // Set damage animation
        if (playerAnimationController)
            StartCoroutine(PlayerHurtAnimation());
    }

    public void Hit(Vector2 force, float damage = 1)
    {
        if (invulnerable) return;
        Damage(damage);
    }

    public void Die()
    {
        SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1f);
        //**reloads scene from the starting point if player dies**
        if (gameObject.CompareTag("Player"))
        {
            SoundFXManager.instance.PlaySoundFXClip(deathSoundClip, transform, 1f);

            StartCoroutine(PlayerDeathAnimation());
            // Scene currentScene = SceneManager.GetActiveScene();
            // SceneManager.LoadScene(currentScene.name);
        }else{
            //item drop
            if(CalculateDropChance() && (ItemToDrop != null)){
                Instantiate(ItemToDrop,transform.position,Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

   private bool CalculateDropChance()
   {
        if(DropChance > 100f){
            DropChance = 100f;
        }

        float num = Random.Range(0f,100f);
        Debug.Log(num.ToString());
        if(num <= DropChance){
            return true;
        }

        return false;
   }

    //*** Work in progress ******************************************
    //
    // public void Respawn()
    // {
    //     if (gameObject.CompareTag("Player"))
    //     {
    //         transform.position = respawnPoint.position;
    //     }
    // }
    //************************************************************

    public IEnumerator InvincibleForTime(float seconds)
    {
        invulnerable = true;
        yield return new WaitForSeconds(seconds);
        invulnerable = false;
    }

    IEnumerator PlayerHurtAnimation()
    {
        playerAnimationController.SetActiveAnimation(damageAnimation, PlayerAnimationController.AnimationType.Single);

        yield return new WaitForSeconds(playerDamageTime);

        playerAnimationController.ResetActiveAnimation(PlayerAnimationController.AnimationType.Single);
    }

    IEnumerator PlayerDeathAnimation()
    {
        // Get components
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        PlayerAttackController playerAttackController = GetComponent<PlayerAttackController>();
        PlayerAnimationController playerAnimationController = GetComponent<PlayerAnimationController>();
        FadeToBlack fadeToBlack = GameObject.FindObjectOfType<FadeToBlack>();
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);

        // Disable movement and attacking
        playerMovement.movementEnabled = false;
        playerAttackController.attackingEnabled = false;
        playerAnimationController.animationEnabled = false;

        // Start fade to black
        fadeToBlack.StartFadeToBlack(playerDeathTime - 1f);

        // Store sprite layers
        int originalSortingOrder = sprites[0].sortingOrder;
        int originalLayer = sprites[0].sortingLayerID;

        // Set sprite to render over fade to black
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = 100;
            sprite.sortingLayerName = "Fadeout";
        }       

        // Play death animation
        float startTime = Time.time;
        while (Time.time - startTime < playerDeathTime)
        {
            playerAnimationController.SetActiveAnimation(deathAnimation, PlayerAnimationController.AnimationType.Single);
            yield return null;
        }

        // Reenable movement and attacking
        playerMovement.movementEnabled = true;
        playerAttackController.attackingEnabled = true;
        playerAnimationController.animationEnabled = true;

        // Disable fade to black
        fadeToBlack.ResetFadeToBlack();

        // Reset sprite layer
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = originalSortingOrder;
            sprite.sortingLayerID = originalLayer;
        }
        

        // Reset player animation
        playerAnimationController.ResetActiveAnimation(PlayerAnimationController.AnimationType.Single);
        //-----------------------------------------
        transform.position = respawnPoint.position;
        //-----------------------------------------
        SetHealth(initialHealth);
        PlayerDeath.Invoke();
    }



    public void OnDisable()
    {
        invulnerable = false;
    }
}

