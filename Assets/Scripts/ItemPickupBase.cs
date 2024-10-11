using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Base class for item pickups. Item is automatically destroyed when picked up
/// created by Ravi Bhatt 5/23/24
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public abstract class ItemPickupBase : MonoBehaviour
{
	// put reusable logic for all pickups here.

	/// <summary>
	/// Called when the player picks up the item. 
	/// </summary>
	/// <param name="player">Player gameobject</param>
	protected abstract void OnPickedUp(GameObject player);

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == GlobalConstants.PlayerTag)
		{
			OnPickedUp(collision.gameObject);
			Destroy(gameObject);
		}
	}
}
