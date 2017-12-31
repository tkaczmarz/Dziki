using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : SelectableObject 
{
	public float moveSpeed = 4;
	public float maxDamage = 50;
	public float attackRange = 1;
	public int movementRange = 3;

	private bool isMoving = false;
	private NavMeshObstacle obstacle;
	private bool hasMoved = false;

	protected override void Awake() 
	{
		base.Awake();

		obstacle = GetComponent<NavMeshObstacle>();
		obstacle.carving = true;

		field = MapController.Instance.GetFieldAt(transform.position);
		field.Unit = this;
	}

	protected override void Update() 
	{
		base.Update();

		if (!selected)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			// attack if pointing an object
			SelectableObject pointedObject = GameController.Instance.marker.PointedObject;
			if (pointedObject)
				Attack(pointedObject);

			// move if hasn't moved
			if (!hasMoved)
			{
				Vector3 markerPos = GameController.Instance.marker.transform.position;
				GoTo((int)markerPos.x, (int)markerPos.z);
			}
		}

		// drawing path to pointed target position
		if (GameController.Instance.marker.PositionChanged && !hasMoved)
		{
			if (!isMoving)
			{
				Vector3 marker = GameController.Instance.marker.transform.position;
				NavMeshPath path = CalculatePath((int)marker.x, (int)marker.z);
				if (MapController.Instance.PathLength(path) <= movementRange + 0.1f)
					MapController.Instance.DrawPath(path, this);
				else
					MapController.Instance.DrawPath(null);
			}
		}
	}

	private NavMeshPath CalculatePath(int x, int y)
	{
		NavMeshPath path = new NavMeshPath();
		Vector3 destination = new Vector3(x, 0, y);
		NavMesh.CalculatePath(transform.position, destination, terrainMask, path);
		if (path.status == NavMeshPathStatus.PathComplete)
			return path;
		else
			return null;
	}

	// calculates path to target location
	public void GoTo(int x, int y)
	{
		if (isMoving)
			return;

		MapController.Instance.DrawPath(null);
		Vector3 targetPos = new Vector3(x, 0, y);
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, targetPos, terrainMask, path);

		if (path.status == NavMeshPathStatus.PathComplete && MapController.Instance.PathLength(path) <= movementRange + 0.1f)
		{
			float cost = MapController.Instance.PathCost(path);
			if (cost <= movementRange)
			{
				field.Unit = null;
				StartCoroutine(Move(path));
			}
		}
	}

	// moves unit through given path
	private IEnumerator Move(NavMeshPath path)
	{
		isMoving = true;
		DisableRangeHighlight();

		// DEBUG: draw path corners
		for (int i = 0; i < path.corners.Length; i++)
		{
			Vector3 corner = path.corners[i];
			if (i > 0)
			{
				Debug.DrawLine(path.corners[i - 1], corner, Color.red, 3);
			}
		}
		
		foreach (Vector3 corner in path.corners)
		{
			while (Vector3.Distance(transform.position, corner) > 0.01f)
			{
				transform.position = Vector3.MoveTowards(transform.position, corner, Time.deltaTime * moveSpeed);
				yield return null;
			}

			transform.position = corner;
		}

		field = MapController.Instance.GetFieldAt(transform.position);
		field.Unit = this;
		isMoving = false;
		hasMoved = true;

		// finish move if can't attack
		if (!EnemyInRange())
			FinishMove();
		else
			DrawAttackRange();
	}

	/// <summary>
	/// Method checks if enemy unit is in attack range of this unit.
	/// <returns>True if there is an enemy in range.</returns>
	/// </summary>
	private bool EnemyInRange()
	{
		// get all enemy units
		Team[] teams = GameController.Instance.Teams;
		foreach (Team team in teams)
		{
			if (team.nr != this.team)
			{
				foreach (SelectableObject o in team.Troops)
				{
					// check if enemy unit is in attack range
					if (o is Unit)
					{
						if (Vector3.Distance(transform.position, o.transform.position) <= attackRange)
							return true;
					}
				}
			}
		}

		return false;
	}

	public override bool Select()
	{
		// can't select unit if it already finished its move
		if (isDone)
			return false;

		if (!base.Select())
			return false;

		obstacle.carving = false;
		if (!hasMoved)
			StartCoroutine(DrawRangeNextFrame());

		return true;
	}

	public override bool Deselect()
	{
		if (!base.Deselect())
			return false;

		obstacle.carving = true;
		MapController.Instance.DrawPath(null);
		DisableRangeHighlight();
		return true;
	}

	public override void Refresh()
	{
		base.Refresh();
		hasMoved = false;
	}

	public void Attack(SelectableObject target)
	{
		// prevent attacking self or teammate
		if (target == this || target.team == team)
			return;

		// can't attack if too far from target
		if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
			return;

		float dmg = (health / maxHealth) * maxDamage;
		target.TakeDamage(dmg);
		FinishMove();
	}

	protected override void Die()
	{
		Destroy(gameObject);
	}

	#region Range drawing
	private IEnumerator DrawRangeNextFrame()
	{
		yield return null;
		DrawMovementRange();
		DrawAttackRange();
	}

	/// <summary>
	/// Highlight fields that are available to move to.
	/// </summary>
	private void DrawMovementRange()
	{
		NavMeshPath path = new NavMeshPath();
		int posX = (int)transform.position.x;
		int posY = (int)transform.position.z;
		for (int y = posY + movementRange; y >= posY - movementRange; y--)
		{
			for (int x = posX - movementRange; x <= posX + movementRange; x++)
			{
				if (!MapController.Instance.IsPointOnMap(x, y))
					continue;

				// position points on a NavMesh
				Vector3 origin = transform.position;
				Vector3 targetPos = new Vector3(x, 0, y);
				NavMeshHit hit = new NavMeshHit();
				if (NavMesh.SamplePosition(origin, out hit, 0.5f, terrainMask))
					origin = hit.position;
				else
				{
					Debug.LogWarning("Can't position " + origin + " on a NavMesh!");
					continue;
				}
					
				if (NavMesh.SamplePosition(targetPos, out hit, 0.5f, terrainMask))
					targetPos = hit.position;
				else
					continue;
				
				NavMesh.CalculatePath(origin, targetPos, terrainMask, path);
				if (path == null)
					continue;
				
				if (path.status != NavMeshPathStatus.PathComplete)
					continue;
				
				// select field if its within range
				float length = MapController.Instance.PathLength(path);
				float cost = MapController.Instance.PathCost(path);
				if (length <= movementRange + 0.1f && length > 0 && cost <= movementRange)
				{
					MapController.Instance.GetFieldAt(new Vector3(x, 0, y)).EnableHighlight(Color.white);
				}
			}
		}
	}

	/// <summary>
	/// Highlight enemy targets in attack range.
	/// </summary>
	private void DrawAttackRange()
	{
		int posY = (int)transform.position.z;
		int posX = (int)transform.position.x;
		int range = Mathf.CeilToInt(attackRange);

		for (int y = posY + range; y >= posY - range; y--)
		{
			for (int x = posX - range; x <= posX + range; x++)
			{
				if (!MapController.Instance.IsPointOnMap(x, y))
					continue;
				
				Field f = MapController.Instance.GetFieldAt(new Vector3(x, 0, y));
				if (f.Unit && f.Unit.team != team)
				{
					if (Vector3.Distance(f.Unit.transform.position, transform.position) <= this.attackRange)
					{
						f.EnableHighlight(Color.red);
					}
				}
			}
		}
	}

	private void DisableRangeHighlight()
	{
		int posX = (int)transform.position.x;
		int posY = (int)transform.position.z;
		for (int y = posY + movementRange; y >= posY - movementRange; y--)
		{
			for (int x = posX - movementRange; x <= posX + movementRange; x++)
			{
				if (MapController.Instance.IsPointOnMap(x, y))
					MapController.Instance.GetFieldAt(new Vector3(x, 0, y)).DisableHighlight();
			}
		}
	}
	#endregion
}
