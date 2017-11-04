using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour 
{
	public float moveSpeed = 4;
	public TerrainType[] walkableTerrains;

	private int walkableMask = 0;
	private bool isMoving = false;
	private NavMeshObstacle obstacle;
	private bool selected = false;
	private Field field = null;

	private void Awake() 
	{		
		foreach (TerrainType terrain in walkableTerrains)
		{
			walkableMask |= (1 << NavMesh.GetAreaFromName(terrain.ToString()));
		}

		// place agent on NavMesh
		NavMeshHit navMeshHit = new NavMeshHit();
		if (NavMesh.SamplePosition(transform.position, out navMeshHit, 5, walkableMask))
		{
			transform.position = navMeshHit.position;
			transform.eulerAngles = new Vector3(90, 0, 0);
		}
		else
			Debug.LogWarning("Can't place agent on a NavMesh!");

		obstacle = GetComponent<NavMeshObstacle>();
		obstacle.carving = true;

		field = MapController.Instance.GetFieldAt(transform.position);
		field.Unit = this;
	}

	private void Update() 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector2Int mousePos = MapController.GetMousePos();
			GoTo(mousePos.x, mousePos.y);
		}
	}

	// calculates path to target location
	public void GoTo(int x, int y)
	{
		if (isMoving || !selected)
			return;

		field.Unit = null;
		Vector3 targetPos = new Vector3(x, 0, y);
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, targetPos, walkableMask, path);
		StartCoroutine(Move(path));
	}

	// moves unit through given path
	private IEnumerator Move(NavMeshPath path)
	{
		isMoving = true;

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

	public void Select()
	{
		selected = true;
		obstacle.carving = false;
		GetComponent<SpriteRenderer>().color = Color.yellow;
	}

	public void Deselect()
	{
		selected = false;
		obstacle.carving = true;
		GetComponent<SpriteRenderer>().color = Color.white;
	}
}
