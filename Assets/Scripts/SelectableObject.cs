using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class SelectableObject : MonoBehaviour 
{
	public TerrainType[] availableTerrains = { TerrainType.Road, TerrainType.Forest, TerrainType.Plains };
	public float maxHealth = 100;
	public float health = 100;

	protected bool selected = false;
	protected Field field = null;
	protected int terrainMask = 0;
	protected Text healthText;

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
			Debug.LogWarning("Can't place object on a NavMesh!");

		healthText = GetComponentInChildren<Text>();
		if (healthText)
		{
			if (health > maxHealth)
				health = maxHealth;
			healthText.text = health.ToString();
		}
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
	}

	public virtual void Deselect()
	{
		selected = false;
	}
}
