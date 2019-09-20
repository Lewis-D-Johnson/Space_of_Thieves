using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	public GameObject PlayerPrefab;

    void Start()
    {
		PhotonNetwork.ConnectUsingSettings();
    }

	public override void OnConnectedToMaster()
	{
		Debug.Log("OnConnectedToMaster() was called by PUN.");

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.IsVisible = false;
		roomOptions.MaxPlayers = 4;

		PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
	}

	public override void OnJoinedRoom()
	{
		base.OnJoinedRoom();

		GameObject newPlayer = PhotonNetwork.Instantiate("Player/FPSController", Vector3.zero, Quaternion.identity);
	}
}
