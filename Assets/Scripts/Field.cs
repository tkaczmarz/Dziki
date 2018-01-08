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

	private SpriteRenderer highlight;

	private void Awake() 
	{
		highlight = transform.GetChild(0).GetComponent<SpriteRenderer>();
		highlight.gameObject.SetActive(false);
	}

	/// <summary>
	/// First element is Unit and second element is Structure. Both may be null.
	/// </summary>
	private SelectableObject[] selectables = { null, null };

	/// <summary>
	/// Assigning unit object to this property moves it in map hierarchy.
	/// <returns>Unit which is standing on this field or null if none is available.</returns>
	/// </summary>
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
	
	/// <summary>
	/// Assigning structure object to this property moves it in map hierarchy.
	/// <returns>Structure which is standing on this field or null if none is available.</returns>
	/// </summary>
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

	public void EnableHighlight(Color color)
	{
		highlight.gameObject.SetActive(true);
		color.a = 0.5f;
		highlight.color = color;
	}

	public void DisableHighlight()
	{
		highlight.gameObject.SetActive(false);
	}
}
