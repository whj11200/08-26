// 필요한 네임스페이스를 포함합니다.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI; // PUN 네트워크 관련 각종 라이브러리

public class PhotonNetWorks : MonoBehaviourPunCallbacks
{
    public string Version = "V1.0"; // Photon 네트워크 버전
    public InputField userid; // 사용자 ID 입력 필드
    public InputField roomName; // 방 이름 입력 필드
    public GameObject roomitem; // 방 아이템 프리팹
    public GameObject scrollContants; // 방 목록을 담을 스크롤 컨테이너
    public Button button; // 버튼 컴포넌트

    void Awake()
    {
        // 네트워크에 연결되지 않았다면
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = Version; // 현재 버전 설정
            PhotonNetwork.ConnectUsingSettings(); // 마스터 서버에 접속
            roomName.text = "ROOM" + Random.Range(0, 999).ToString("000"); // 방 이름 초기화
        }
    }

    public override void OnConnectedToMaster() // 서버에 연결되면 자동으로 호출
    {
        PhotonNetwork.JoinLobby(); // 로비에 연결
    }

    public override void OnJoinedLobby()
    {
        // 로비에 입장했을 때
        Debug.Log("Lobby에 입장");
        userid.text = GetUserId(); // 사용자 ID를 가져와서 입력 필드에 설정
    }

    public void OnclickJoinRanmdomRoom()
    {
        // 랜덤 방에 접속하기 위한 메서드
        PhotonNetwork.NickName = userid.text; // 사용자 닉네임 설정
        PlayerPrefs.SetString("USER_ID", userid.text); // 사용자 ID를 저장
        PhotonNetwork.JoinRandomRoom(); // 랜덤 방에 접속
    }

    public void OnClickCreateRoom()
    {
        // 방 생성 메서드
        string _roomName = roomName.text; // 입력된 방 이름
        if (string.IsNullOrEmpty(roomName.text))
        {
            // 방 이름이 비어있으면 무작위 방 이름 생성
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.NickName = userid.text; // 사용자 닉네임 설정
        PlayerPrefs.SetString("USER_ID", userid.text); // 사용자 ID를 저장

        RoomOptions roomOptions = new RoomOptions(); // 방 옵션 설정
        roomOptions.IsOpen = true; // 방 오픈 여부
        roomOptions.IsVisible = true; // 방 가시성 여부
        roomOptions.MaxPlayers = 20; // 최대 플레이어 수 설정

        // 지정한 조건으로 방 생성
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    string GetUserId()
    {
        // 사용자 ID를 가져오는 메서드
        string userid = PlayerPrefs.GetString("USER_ID"); // 저장된 사용자 ID 가져오기
        if (string.IsNullOrEmpty(userid)) // ID가 비어있거나 null일 경우
        {
            // 0에서 998 사이의 랜덤 숫자를 생성하고, "USER" 접두사 붙여서 반환
            userid = "USER" + Random.Range(0, 999).ToString("000");
        }
        return userid;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // 랜덤 방 접속 실패 시 호출
    {
        print("방이 없습니다."); // 방이 없음을 출력
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 }); // 새로운 방 생성
    }

    public override void OnJoinedRoom() // 방에 성공적으로 입장했을 때 호출
    {
        print("방 제작"); // 방이 성공적으로 제작되었음을 출력
        StartCoroutine(loadTankMainScene()); // 씬 로딩을 위한 코루틴 시작
    }

    IEnumerator loadTankMainScene() // 씬 로딩을 처리하는 코루틴
    {
        // 씬이 이동하는 동안 포톤 클라우드 서버로부터 네트워크 메시지 수신 중단
        PhotonNetwork.IsMessageQueueRunning = false; // 네트워크 메시지 수신을 일시 중지하여 씬 전환 중 오류 방지

        // 비동기적으로 씬 로딩
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene"); // "TankMainScene" 씬을 비동기적으로 로드
        yield return ao; // 씬 로딩이 완료될 때까지 대기
    }

    // 생성된 룸 목록이 변경되었을 때 호출되는 콜백 메서드
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 기존 방 아이템 제거
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem"))
        {
            Destroy(obj);
        }

        // 방 목록을 업데이트
        foreach (RoomInfo roomInfo in roomList)
        {
            // RoomItem 프리팹 동적으로 생성
            GameObject room = (GameObject)Instantiate(roomitem);
            room.transform.SetParent(scrollContants.transform, false); // 스크롤 컨테이너에 자식으로 추가
            RoomDeta roomData = room.GetComponent<RoomDeta>(); // RoomDeta 컴포넌트 가져오기
            roomData.roomName = roomInfo.Name; // 방 이름 설정
            roomData.connectPlayer = roomInfo.PlayerCount; // 현재 플레이어 수 설정
            roomData.maxPlayers = roomInfo.MaxPlayers; // 최대 플레이어 수 설정
            roomData.DisplayRoomData(); // 방 정보를 UI에 표시
            // RoomItem의 버튼 클릭 이벤트를 동적으로 연결
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });
        }
    }

    public void OnClickRoomItem(string roomname)
    {
        // 방 아이템 클릭 시 호출되는 메서드
        PhotonNetwork.NickName = userid.text; // 사용자 닉네임 설정
        PlayerPrefs.SetString("USER_ID", userid.text); // 사용자 ID 저장
        PhotonNetwork.JoinRoom(roomname); // 해당 방에 접속
    }

    private void OnGUI()
    {
        // 유니티 실시간 화면에 레이블을 콜백 메서드로 보여줌
        GUILayout.Label(PhotonNetwork.InRoom.ToString()); // 현재 방에 있는지 여부를 레이블로 표시
    }
}
