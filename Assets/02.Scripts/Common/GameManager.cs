using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // GameManager 클래스의 싱글턴 인스턴스
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
        set
        {
            // 인스턴스가 null일 경우만 설정
            if (instance == null)
                instance = value;
            // 이미 인스턴스가 있을 경우 새로 설정된 인스턴스를 파괴
            else if (instance != value)
                Destroy(value);
        }
    }

    [SerializeField] private List<Transform> spawnList; // 스폰 포인트 리스트
    [SerializeField] private GameObject apachePrefab; // 생성할 Apache 프리팹
    public Text txtConnect; // 연결 상태를 표시할 UI 텍스트
    [SerializeField] public Text textLogtext; // 로그 메시지를 표시할 UI 텍스트
    public bool isGameOver = false; // 게임 종료 여부
    public Text KillCounter;
    private int killscore = 0;

    private void Awake()
    {
        // 인스턴스를 설정하고 파괴되지 않도록 설정
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateTank(); // 탱크 생성
        PhotonNetwork.IsMessageQueueRunning = true; // 메시지 큐 활성화
        apachePrefab = Resources.Load<GameObject>("Apache"); // Apache 프리팹 로드
    }

    void CreateTank()
    {
        // 랜덤 위치에 탱크 생성
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5F, pos), Quaternion.identity);
    }

    void Start()
    {
        // 게임 시작 시 스폰 포인트 설정
        var spawnPoint = GameObject.Find("SpawnPoints").gameObject;
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList); // 자식 트랜스폼을 spawnList에 추가

        spawnList.RemoveAt(0); // 첫 번째 스폰 포인트 제거
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "]Connected</color>"; // 연결 메시지 생성
        photonView.RPC("LogMsg", RpcTarget.AllBuffered, msg); // 모든 클라이언트에 메시지 전송

        // 주석 처리된 부분은 Apache 생성 관련
        if (spawnList.Count > 0 && PhotonNetwork.IsMasterClient)
            StartCoroutine(CreateApache()); // Apache 생성 코루틴 시작
        //InvokeRepeating("CreateApaches", 0f, 3f); // 일정 간격으로 Apache 생성
    }

    IEnumerator CreateApache()
    {
        // 게임 오버가 아닐 때 Apache 생성
        while (isGameOver == false)
        {
            int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length; // 현재 Apache 수 체크
            if (count < 10) // Apache 수가 10개 미만일 경우
            {
                yield return new WaitForSeconds(3.0f); // 3초 대기
                int idx = Random.Range(0, spawnList.Count); // 랜덤한 스폰 포인트 선택
               PhotonNetwork.InstantiateRoomObject("Apache", spawnList[idx].position, spawnList[idx].rotation,0,null); // Apache 생성
            }
            else
            {
                yield return null; // Apache가 10개 이상일 경우 대기
            }
        }
    }
    [PunRPC]
    public void ApplyPlayerCountUpdate()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom; // 현재 방 정보 가져오기
        txtConnect.text = currentRoom.PlayerCount.ToString() + "/" + currentRoom.MaxPlayers.ToString(); // UI 업데이트
    }
    [PunRPC]
    void GetConnetPlayerCounter()
    {
        // 마스터 클라이언트일 경우 현재 방의 플레이어 수 업데이트
        if (PhotonNetwork.IsMasterClient)
        {
            //Room currentRoom = PhotonNetwork.CurrentRoom; // 현재 방 정보 가져오기
            //txtConnect.text = currentRoom.PlayerCount.ToString() + "/" + currentRoom.MaxPlayers.ToString(); // UI 업데이트
            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All); // 다른 클라이언트에 호출
        }
    }
    

    // 플레이어가 방에 입장했을 때 호출
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GetConnetPlayerCounter(); // 플레이어 수 업데이트
    }

    // 플레이어가 방에서 나갔을 때 호출
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnetPlayerCounter(); // 플레이어 수 업데이트
    }

    public void OnclickExit() // 룸으로 나가기 버튼 클릭 이벤트 함수
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>"; // 연결 종료 메시지 생성
        photonView.RPC("LogMsg", RpcTarget.AllBuffered, msg); // 모든 클라이언트에 메시지 전송
        PhotonNetwork.LeaveRoom(); // 방 떠나기
    }

    public override void OnLeftRoom() // 방에서 접속 종료 시 호출되는 콜백 함수
    {
        SceneManager.LoadScene("Lobby"); // 로비 씬으로 전환
    }

    [PunRPC]
    void LogMsg(string msg)
    {
        textLogtext.text += msg; // 로그 메시지 업데이트
    }
    [PunRPC]
    public void KillUp()
    {
        if(photonView.IsMine)
        {
            killscore= killscore+1;
            KillCounter.text = $"<color=#00ff00>kill:{killscore}</color>";
        }
       
    }

    // 주석 처리된 Apache 생성 코루틴
    //IEnumerator CreateApaches()
    //{
    //    int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length; // 현재 Apache 수 체크
    //    if (count < 10) // Apache 수가 10개 미만일 경우
    //    {
    //        int idx = Random.Range(0, spawnList.Count); // 랜덤 스폰 포인트 선택
    //        Instantiate(apachePrefab, spawnList[idx].position, spawnList[idx].rotation); // Apache 생성
    //    }
    //    else
    //    {
    //        yield return null; // Apache가 10개 이상일 경우 대기
    //    }
    //}
}
