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
			SelectableObject pointedObject = GameController.Instance.marker.PointedObject;
			if (pointedObject)
				Attack(pointedObject);

			Vector3 markerPos = GameController.Instance.marker.transform.position;
			GoTo((int)markerPos.x, (int)markerPos.z);
		}

		if (GameController.Instance.marker.PositionChanged)
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
	}

	public override void Select()
	{
		base.Select();
		obstacle.carving = false;
		StartCoroutine(DrawRangeNextFrame());
	}

	public override void Deselect()
	{
		base.Deselect();
		obstacle.carving = true;
		MapController.Instance.DrawPath(null);
		DisableRangeHighlight();
	}

	public void Attack(SelectableObject target)
	{
		if (target == this || target.team == team)
			return;

		// can't attack if too far from target
		if (Vector3.Distance(transform.position, target.transform.position) > attackRange)
			return;

		float dmg = (health / maxHealth) * maxDamage;
		target.TakeDamage(dmg);
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
	}

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
					MapController.Instance.GetFieldAt(new Vector3(x, 0, y)).EnableHighlight();
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
