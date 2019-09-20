using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FP_Inventory : MonoBehaviour
{
	[SerializeField] Animator anim;
	public bool hasRifle = false;

    void Start()
    {

    }
	
    void Update()
	{
		anim.SetBool("hasRifle", hasRifle);
	}
}
