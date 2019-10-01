using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
	public string Name;

	[ShowIf("IsPickup")] public InventoryItem Item;
	[ShowIf("IsPickup")] public int Amount;

	public enum InteractType { Pickup, Toggle }
	public InteractType InteractionType;

	#region InspectorChecks
	bool IsPickup()
	{
		if (InteractionType == InteractType.Pickup)
		{
			return true;
		}

		return false;
	}
	#endregion
}
