using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
	#region Singleton
	public static Inventory instance;

	private void Awake()
	{
		if (instance != null)
		{
			if (instance != this)
			{
				Destroy(instance.gameObject);
			}
			else
			{
				return;
			}
		}
		instance = this;
		DontDestroyOnLoad(this.gameObject);
	}
	#endregion

	public List<InventoryItem> Items = new List<InventoryItem>();

	public void AddToInventory(GameObject Instigator, InventoryItem item, int amount, bool destroyObjectOnAdded)
	{
		Items[Items.IndexOf(item)].CurrentCount += amount;

		if (destroyObjectOnAdded && Instigator != null)
		{
			Destroy(Instigator);
		}
	}

}
