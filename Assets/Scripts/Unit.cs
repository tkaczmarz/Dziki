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
	private bool hasMoved = false;

	protected override void Awake() 
	{
		base.Awake();

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
				if (markerPos != transform.position)
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

	/// <summary>Method calculates path from this unit's position to given position.</summary>
	/// <returns>Calculated path or null if unable to find.</returns>
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

	/// <summary>Moves unit to given position if possible.</summary>
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

	/// <summary>Coroutine that moves unit through given path.</summary>
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

	/// <summary>Method checks if enemy object is in attack range of this unit.</summary>
	/// <returns>True if there is an enemy in range.</returns>
	private bool EnemyInRange()
	{
		// get all enemy troops
		Team[] teams = GameController.Instance.Teams;
		foreach (Team team in teams)
		{
			if (team.nr != this.team)
			{
				foreach (SelectableObject o in team.Troops)
				{
					// check if enemy object is in attack range
					if (Vector3.Distance(transform.position, o.transform.position) <= attackRange)
						return true;
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
		StartCoroutine(DrawRangeNextFrame());

		return true;
	}

	public override bool Deselect()
	{
		if (isMoving)
			return false;

		if (!base.Deselect())
			return false;

		obstacle.carving = true;
		MapController.Instance.DrawPath(null);
		DisableRangeHighlight();
		return true;
	}

	/// <summary>Enables unit for use (move and attack).</summary>
	public override void Refresh()
	{
		base.Refresh();
		hasMoved = false;
	}

	/// <summary>Unit attacks given target if possible.</summary>
	public virtual void Attack(SelectableObject target)
	{
		// prevent attacking self or teammate
		if (target == this || target.team == team || target.IsDead)
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
		base.Die();
		Destroy(gameObject, 1);
	}

	#region Range drawing
	/// <summary>Method delays drawing of ranges to next frame.</summary>
	private IEnumerator DrawRangeNextFrame()
	{
		yield return null;
		if (!hasMoved)
			DrawMovementRange();
		DrawAttackRange();
	}

	/// <summary>Highlight fields that are available to move to.</summary>
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

				if (MapController.Instance.GetFieldAt(targetPos).Unit)
					continue;

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

	/// <summary>Highlight enemy targets in attack range.</summary>
	private void DrawAttackRange(bool drawFullRange = false)
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

				if (drawFullRange)
				{
					if (Vector3.Distance(f.transform.position, transform.position) <= this.attackRange)
					{
						f.EnableHighlight(Color.red);
					}
				}
				else
				{
					if (f.Selectable && f.Selectable.team != team && !f.Selectable.IsDead)
					{
						if (Vector3.Distance(f.Selectable.transform.position, transform.position) <= this.attackRange)
						{
							f.EnableHighlight(Color.red);
						}
					}
				}
			}
		}
	}

	private void DisableRangeHighlight()
	{
		int posX = (int)transform.position.x;
		int posY = (int)transform.position.z;
		int range = attackRange > movementRange ? Mathf.CeilToInt(attackRange) : movementRange;

		for (int y = posY + range; y >= posY - range; y--)
		{
			for (int x = posX - range; x <= posX + range; x++)
			{
				if (MapController.Instance.IsPointOnMap(x, y))
					MapController.Instance.GetFieldAt(new Vector3(x, 0, y)).DisableHighlight();
			}
		}
	}
	#endregion
}
