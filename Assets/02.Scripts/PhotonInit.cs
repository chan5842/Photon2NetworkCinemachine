using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;      // DB, �α���
using Photon.Realtime; // �ǽð�

public class PhotonInit : MonoBehaviourPunCallbacks
{
    readonly string version = "1.0"; // ���� ����
    string userId = "Rogame";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // ������ Ŭ���̾�Ʈ(����) �� �ڵ� ����ȭ �ɼ�
        PhotonNetwork.GameVersion = version; // ���� ����
        PhotonNetwork.NickName = userId; // ������ ������ �г���

        Debug.Log(PhotonNetwork.SendRate); // ���� ������ �����ϴ� �������� �ʴ� ���� Ƚ��
        PhotonNetwork.ConnectUsingSettings(); // ���� ���� ����
    }

    public override void OnConnectedToMaster() // ������ �Լ�
    {
        Debug.Log("Connected to Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // �κ� �����ϸ� ȣ��
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Room Failed {returnCode}:{message}"); // ���� �ڵ�, ���� �޽���
        RoomOptions room = new RoomOptions();
        room.MaxPlayers = 20;
        room.IsOpen = true;
        room.IsVisible = true;

        PhotonNetwork.CreateRoom("My Room", room);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Created Room");
        Debug.Log($"RoomName : {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"PhotonNetwork.InRoom = {PhotonNetwork.InRoom}");
        Debug.Log($"PlayerCount + : {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
        }

        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>(); // �÷��̾� ���� ��ġ ������ �迭�� ����
        int idx = Random.Range(1, points.Length);

        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // ��Ʈ��ũ �� ĳ���� ����
    }

}
