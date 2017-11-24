using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : SelectableObject 
{
	protected override void Awake()
	{
		base.Awake();

		field = MapController.Instance.GetFieldAt(transform.position);
		field.Structure = this;
	}
}
