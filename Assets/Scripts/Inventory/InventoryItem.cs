using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
	public string ItemName;
	public string ItemDescription;
	public int CurrentCount;

	public enum ItemType { Item, Weapon }
}
