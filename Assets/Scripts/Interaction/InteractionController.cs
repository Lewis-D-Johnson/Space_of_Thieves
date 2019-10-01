using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionController : MonoBehaviour
{
	public Text InteractText;
	RaycastHit hit;

	Interactable thisInteractable;

	void Update()
	{
		if (Physics.Raycast(transform.position, transform.forward, out hit, 3.5f))
		{
			if (hit.transform.GetComponent<Interactable>())
			{
				thisInteractable = hit.transform.GetComponent<Interactable>();

				switch (thisInteractable.InteractionType)
				{
					case Interactable.InteractType.Pickup:
						{
							InteractText.text = ("Press [F] to pickup " + thisInteractable.Name + ((thisInteractable.Amount > 0) ? " (" + thisInteractable.Amount + ")" : "")).ToUpper();
							break;
						}
					case Interactable.InteractType.Toggle:
						{
							InteractText.text = ("Press [F] to toggle " + thisInteractable.Name).ToUpper();
							break;
						}
				}

				InteractText.gameObject.SetActive(true);
			}
			else
			{
				thisInteractable = null;
				InteractText.gameObject.SetActive(false);
			}
		}
		else
		{
			thisInteractable = null;
			InteractText.gameObject.SetActive(false);
		}

		if (Input.GetButtonDown("Interact"))
		{
			if (thisInteractable != null)
			{
				Inventory.instance.AddToInventory(thisInteractable.gameObject, thisInteractable.Item, thisInteractable.Amount, true);
			}
		}

	}
}
