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
	public Weapon CurrentWeapon;
	public Transform WeaponParent;

	void EquipWeapon()
	{
		if (CurrentWeapon != null)
		{
			CurrentWeapon.gameObject.SetActive(true);
		}
	}

	public void AddToInventory(GameObject Instigator, InventoryItem item, int amount, bool destroyObjectOnAdded)
	{
		if (item.thisType == InventoryItem.ItemType.Weapon && !Inventory.instance.CurrentWeapon)
		{
			Inventory.instance.CurrentWeapon = FindWeaponOnPlayer(item.WeaponNameOnPlayer);
			EquipWeapon();
		}

		Items[Items.IndexOf(item)].CurrentCount += amount;

		if (destroyObjectOnAdded && Instigator != null)
		{
			Destroy(Instigator);
		}
	}

	Weapon FindWeaponOnPlayer(string name)
	{
		for (int i = 0; i < WeaponParent.childCount; i++)
		{
			if (WeaponParent.GetChild(i).name == name)
			{
				return WeaponParent.GetChild(i).GetComponent<Weapon>();
			}
		}

		Debug.LogError("Unable to find weapon with the name: " + name + " within the weapon parent.");
		return null;
	}
}
