using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType
{
	Water, Mountains, Forest, Plains, Road
}

[RequireComponent(typeof(SpriteRenderer))]
public class Field : MonoBehaviour 
{
	public TerrainType terrain = TerrainType.Plains;

	public Unit Unit 
	{ 
		get { return unit; } 
		set 
		{
			if (value == null && unit)
			{
				unit.transform.SetParent(null);
			}
			else if (value != null && unit)
			{
				value.transform.SetParent(transform);
				value.transform.localPosition = Vector3.zero;
			}
			unit = value;
		}
	}

	private Unit unit;
}
