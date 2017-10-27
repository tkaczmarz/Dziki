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
}
