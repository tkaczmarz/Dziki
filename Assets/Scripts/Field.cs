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

	/// <summary>
	/// First element is Unit and second element is Structure. Both may be null.
	/// </summary>
	private SelectableObject[] selectables = { null, null };

	public Unit Unit 
	{ 
		get { return selectables[0] as Unit; } 
		set 
		{
			if (value == null && selectables[0])
			{
				selectables[0].transform.SetParent(null);
			}
			
			if (value != null)
			{
				value.transform.SetParent(transform);
				value.transform.localPosition = Vector3.zero;
			}
			selectables[0] = value;
		}
	}
	
	public Structure Structure
	{
		get { return selectables[1] as Structure; }
		set
		{
			if (value == null && selectables[1])
			{
				selectables[1].transform.SetParent(null);
			}

			if (value != null)
			{
				value.transform.SetParent(transform);
				value.transform.localPosition = Vector3.zero;
			}
			selectables[1] = value;
		}
	}

	/// <summary>
	/// Returns first selectable object, that is Unit if not null or Structure which may be null.
	/// </summary>
	public SelectableObject Selectable
	{
		get
		{
			if (selectables[0])
				return selectables[0];
			else
				return selectables[1];
		}
	}
}
