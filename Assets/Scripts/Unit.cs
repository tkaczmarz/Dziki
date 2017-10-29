using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour 
{
	public float moveSpeed = 4;
	public TerrainType[] walkableTerrains;

	private int walkableMask = 0;

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
	}

	private void Update() 
	{
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int x = Mathf.RoundToInt(mousePos.x);
			int y = Mathf.RoundToInt(mousePos.z);
			GoTo(x, y);
		}	
	}

	// calculates path to target location
	public void GoTo(int x, int y)
	{
		Vector3 targetPos = new Vector3(x, 0, y);
		NavMeshPath path = new NavMeshPath();
		NavMesh.CalculatePath(transform.position, targetPos, walkableMask, path);
		StopAllCoroutines();
		StartCoroutine(Move(path));
	}

	// moves unit through given path
	private IEnumerator Move(NavMeshPath path)
	{
		foreach (Vector3 corner in path.corners)
		{
			while (Vector3.Distance(transform.position, corner) > 0.05f)
			{
				transform.position = Vector3.MoveTowards(transform.position, corner, Time.deltaTime * moveSpeed);
				yield return null;
			}

			transform.position = corner;
		}
	}
}
