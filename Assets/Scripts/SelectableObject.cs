using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public enum Team 
{
	Local, Opponent1
}

[RequireComponent(typeof(SpriteRenderer))]
public class SelectableObject : MonoBehaviour 
{
	public TerrainType[] availableTerrains = { TerrainType.Road, TerrainType.Forest, TerrainType.Plains };
	public float maxHealth = 100;
	public float health = 100;
	public Team team = Team.Local;

	protected bool selected = false;
	protected Field field = null;
	protected int terrainMask = 0;
	protected Text healthText;
	protected SpriteRenderer spriteRenderer;

	protected virtual void Awake() 
	{
		// create terrain mask
		foreach (TerrainType terrain in availableTerrains)
		{
			terrainMask |= (1 << NavMesh.GetAreaFromName(terrain.ToString()));
		}

		// place object on NavMesh
		NavMeshHit navMeshHit = new NavMeshHit();
		if (NavMesh.SamplePosition(transform.position, out navMeshHit, 0.5f, terrainMask))
		{
			transform.position = navMeshHit.position;
			transform.eulerAngles = new Vector3(90, 0, 0);
		}
		else
			Debug.LogWarning("Can't place '" + name + "' (" + GetType() + ") object on a NavMesh!");

		healthText = GetComponentInChildren<Text>();
		if (healthText)
		{
			if (health > maxHealth)
				health = maxHealth;
			healthText.text = health.ToString();
		}

		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected virtual void Update() 
	{
		if (Input.GetMouseButtonDown(1))
		{
			Deselect();
		}
	}

	public virtual void Select()
	{
		selected = true;
		spriteRenderer.color = Color.yellow;
	}

	public virtual void Deselect()
	{
		selected = false;
		spriteRenderer.color = Color.white;
	}

	public void TakeDamage(float amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Die();
		}

		if (healthText)
			healthText.text = health.ToString();
	}

	protected virtual void Die()
	{

	}
}
