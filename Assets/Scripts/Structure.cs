using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : SelectableObject 
{
	public Sprite destroyedSprite;

	protected override void Awake()
	{
		base.Awake();

		field = MapController.Instance.GetFieldAt(transform.position);
		field.Structure = this;
	}

	protected override void Die()
	{
		base.Die();
		healthText.transform.parent.gameObject.SetActive(false);
		obstacle.carving = false;
		// change sprite to 'destroyed structure'
		if (destroyedSprite)
		{
			GetComponent<SpriteRenderer>().sprite = destroyedSprite;
		}
	}

	public override void Refresh()
	{
		if (isDead)
			return;

		base.Refresh();
		
		if (GameController.Instance.ActiveTeam.nr == team)
		{
			obstacle.carving = false;

			if (field.Unit && !isDead)
			{
				// heal 20% of unit's health
				field.Unit.Heal(0.2f * field.Unit.maxHealth);
			}
		}
		else
		{
			obstacle.carving = true;
		}
	}

	public override bool Select()
	{
		return false;
	}
}
