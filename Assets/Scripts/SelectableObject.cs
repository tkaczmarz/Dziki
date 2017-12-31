using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(SpriteRenderer))]
public class SelectableObject : MonoBehaviour 
{
	public TerrainType[] availableTerrains = { TerrainType.Road, TerrainType.Forest, TerrainType.Plains };
	public float maxHealth = 100;
	public float health = 100;
	public int team = 0;
	public bool IsDone { get { return isDone; } }

	protected bool selected = false;
	protected Field field = null;
	protected int terrainMask = 0;
	protected Text healthText;
	protected SpriteRenderer spriteRenderer;
	protected bool isDone = false;

	private SpriteRenderer teamColor;
	private Color normalColor;
	private Color dimmedColor;

	protected virtual void Awake() 
	{
		// get components
		healthText = GetComponentInChildren<Text>();
		if (healthText)
		{
			if (health > maxHealth)
				health = maxHealth;
			healthText.text = (Mathf.RoundToInt(health / 10)).ToString();
		}

		spriteRenderer = GetComponent<SpriteRenderer>();

		teamColor = transform.GetChild(0).GetComponent<SpriteRenderer>();
		if (!teamColor)
			Debug.LogWarning("Can't find team color object of " + name);

		normalColor = spriteRenderer.color;
		dimmedColor = normalColor * 0.8f;
		// ///////////////////////////////////////////////////

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
	}

	protected virtual void Update() 
	{
		if (Input.GetMouseButtonDown(1))
		{
			Deselect();
		}

		if (Input.GetKeyDown(KeyCode.F) && selected)
			FinishMove();
	}

	public virtual bool Select()
	{
		if (GameController.Instance.ActiveTeam.nr != team)
			return false;

		selected = true;
		spriteRenderer.color = Color.yellow;
		return true;
	}

	public virtual bool Deselect()
	{
		if (!selected)
			return false;

		selected = false;
		spriteRenderer.color = Color.white;
		GameController.Instance.marker.DeselectObject();
		return true;
	}

	public void TakeDamage(float amount)
	{
		health -= amount;
		if (health <= 0)
		{
			Die();
		}

		if (healthText)
			healthText.text = (Mathf.RoundToInt(health / 10)).ToString();
	}

	protected virtual void Die()
	{

	}

	protected virtual void FinishMove()
	{
		isDone = true;
		Deselect();
		
		// dim done object
		spriteRenderer.color = dimmedColor;
	}

	public void AssignToTeam(Team team)
	{
		teamColor.color = team.color;
		this.team = team.nr;
	}

	public virtual void Refresh()
	{
		isDone = false;
		if (spriteRenderer)
			spriteRenderer.color = normalColor;
	}
}
