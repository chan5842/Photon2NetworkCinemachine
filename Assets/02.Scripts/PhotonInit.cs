using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;      // DB, 로그인
using Photon.Realtime; // 실시간

public class PhotonInit : MonoBehaviourPunCallbacks
{
    readonly string version = "1.0"; // 게임 버전
    string userId = "Rogame";

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // 마스터 클라이언트(방장) 씬 자동 동기화 옵션
        PhotonNetwork.GameVersion = version; // 버전 설정
        PhotonNetwork.NickName = userId; // 접속한 유저의 닉네임

        Debug.Log(PhotonNetwork.SendRate); // 포톤 서버에 전송하는 데이터의 초당 전송 횟수
        PhotonNetwork.ConnectUsingSettings(); // 포톤 서버 접속
    }

    public override void OnConnectedToMaster() // 재정의 함수
    {
        Debug.Log("Connected to Master");
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() // 로비에 접속하면 호출
    {
        Debug.Log($"PhotonNetwork.InLobby = {PhotonNetwork.InLobby}");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Room Failed {returnCode}:{message}"); // 오류 코드, 에러 메시지
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

        Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>(); // 플레이어 출현 위치 정보를 배열에 저장
        int idx = Random.Range(1, points.Length);

        PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation, 0); // 네트워크 상 캐릭터 생성
    }

}
