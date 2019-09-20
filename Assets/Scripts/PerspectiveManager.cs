using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveManager : MonoBehaviour
{
	public enum Perspectives { FirstPerson, ThirdPerson, Cinematic }
	public Perspectives Perspective;

	public SkinnedMeshRenderer Head_MR;

	void Start()
	{
		switch (Perspective)
		{
			case Perspectives.FirstPerson:
				{
					Head_MR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
					break;
				}
		}
    }
}
