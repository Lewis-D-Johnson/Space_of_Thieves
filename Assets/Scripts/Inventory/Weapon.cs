using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField] GameObject Projectile;
	[SerializeField] Transform Muzzle;
	
    void Start()
    {
        
    }
	
    void Update()
    {
		if (Input.GetMouseButtonDown(0))
		{
			GameObject thisProjectile = Instantiate(Projectile, Muzzle);
			//thisProjectile.transform.localPosition = Vector3.zero;
			//thisProjectile.transform
		}
    }
}
