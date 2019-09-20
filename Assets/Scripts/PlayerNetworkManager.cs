using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerNetworkManager : MonoBehaviourPunCallbacks
{
	public UnityStandardAssets.Characters.FirstPerson.FirstPersonController FPC;
	public AudioSource AS;
	public CharacterController CC;
	public Camera Cam;
	public FP_Inventory inv;

	public SkinnedMeshRenderer PlayerModelMain;
	public List<MeshRenderer> PlayerObjects = new List<MeshRenderer>();

	void Start()
    {
		if (photonView.IsMine)
		{
			Cam.enabled = true;
			AS.enabled = true;
			CC.enabled = true;
			FPC.enabled = true;
			inv.enabled = true;

			PlayerModelMain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

			foreach (var thisPlayerObject in PlayerObjects)
			{
				thisPlayerObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
			}
		}
		else
		{
			PlayerModelMain.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

			foreach (var thisPlayerObject in PlayerObjects)
			{
				thisPlayerObject.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			}
		}
    }
}
