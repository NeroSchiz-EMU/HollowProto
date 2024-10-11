using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Heals you when picked up (earth-shattering)
/// created by Ravi Bhatt 5/23/24
/// </summary>
public class ItenPickupHealth : ItemPickupBase
{
	[SerializeField] float healthToAdd = 1;

	//determines if pickup only heals or also increses max health
	[SerializeField] bool UpMaxHealth = true;
	protected override void OnPickedUp(GameObject player)
	{
		var health = player.GetComponent<Health>();
		Assert.IsNotNull(health);

		if(UpMaxHealth){
			health.IncrementMaxHealth(healthToAdd);
		}

		health.Heal(healthToAdd);
	}
}
