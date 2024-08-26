// �ʿ��� ���ӽ����̽��� �����մϴ�.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI; // PUN ��Ʈ��ũ ���� ���� ���̺귯��

public class PhotonNetWorks : MonoBehaviourPunCallbacks
{
    public string Version = "V1.0"; // Photon ��Ʈ��ũ ����
    public InputField userid; // ����� ID �Է� �ʵ�
    public InputField roomName; // �� �̸� �Է� �ʵ�
    public GameObject roomitem; // �� ������ ������
    public GameObject scrollContants; // �� ����� ���� ��ũ�� �����̳�
    public Button button; // ��ư ������Ʈ

    void Awake()
    {
        // ��Ʈ��ũ�� ������� �ʾҴٸ�
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = Version; // ���� ���� ����
            PhotonNetwork.ConnectUsingSettings(); // ������ ������ ����
            roomName.text = "ROOM" + Random.Range(0, 999).ToString("000"); // �� �̸� �ʱ�ȭ
        }
    }

    public override void OnConnectedToMaster() // ������ ����Ǹ� �ڵ����� ȣ��
    {
        PhotonNetwork.JoinLobby(); // �κ� ����
    }

    public override void OnJoinedLobby()
    {
        // �κ� �������� ��
        Debug.Log("Lobby�� ����");
        userid.text = GetUserId(); // ����� ID�� �����ͼ� �Է� �ʵ忡 ����
    }

    public void OnclickJoinRanmdomRoom()
    {
        // ���� �濡 �����ϱ� ���� �޼���
        PhotonNetwork.NickName = userid.text; // ����� �г��� ����
        PlayerPrefs.SetString("USER_ID", userid.text); // ����� ID�� ����
        PhotonNetwork.JoinRandomRoom(); // ���� �濡 ����
    }

    public void OnClickCreateRoom()
    {
        // �� ���� �޼���
        string _roomName = roomName.text; // �Էµ� �� �̸�
        if (string.IsNullOrEmpty(roomName.text))
        {
            // �� �̸��� ��������� ������ �� �̸� ����
            _roomName = "Room_" + Random.Range(0, 999).ToString("000");
        }

        PhotonNetwork.NickName = userid.text; // ����� �г��� ����
        PlayerPrefs.SetString("USER_ID", userid.text); // ����� ID�� ����

        RoomOptions roomOptions = new RoomOptions(); // �� �ɼ� ����
        roomOptions.IsOpen = true; // �� ���� ����
        roomOptions.IsVisible = true; // �� ���ü� ����
        roomOptions.MaxPlayers = 20; // �ִ� �÷��̾� �� ����

        // ������ �������� �� ����
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }

    string GetUserId()
    {
        // ����� ID�� �������� �޼���
        string userid = PlayerPrefs.GetString("USER_ID"); // ����� ����� ID ��������
        if (string.IsNullOrEmpty(userid)) // ID�� ����ְų� null�� ���
        {
            // 0���� 998 ������ ���� ���ڸ� �����ϰ�, "USER" ���λ� �ٿ��� ��ȯ
            userid = "USER" + Random.Range(0, 999).ToString("000");
        }
        return userid;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // ���� �� ���� ���� �� ȣ��
    {
        print("���� �����ϴ�."); // ���� ������ ���
        PhotonNetwork.CreateRoom("MyRoom", new RoomOptions { MaxPlayers = 20 }); // ���ο� �� ����
    }

    public override void OnJoinedRoom() // �濡 ���������� �������� �� ȣ��
    {
        print("�� ����"); // ���� ���������� ���۵Ǿ����� ���
        StartCoroutine(loadTankMainScene()); // �� �ε��� ���� �ڷ�ƾ ����
    }

    IEnumerator loadTankMainScene() // �� �ε��� ó���ϴ� �ڷ�ƾ
    {
        // ���� �̵��ϴ� ���� ���� Ŭ���� �����κ��� ��Ʈ��ũ �޽��� ���� �ߴ�
        PhotonNetwork.IsMessageQueueRunning = false; // ��Ʈ��ũ �޽��� ������ �Ͻ� �����Ͽ� �� ��ȯ �� ���� ����

        // �񵿱������� �� �ε�
        AsyncOperation ao = SceneManager.LoadSceneAsync("TankMainScene"); // "TankMainScene" ���� �񵿱������� �ε�
        yield return ao; // �� �ε��� �Ϸ�� ������ ���
    }

    // ������ �� ����� ����Ǿ��� �� ȣ��Ǵ� �ݹ� �޼���
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ���� �� ������ ����
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("RoomItem"))
        {
            Destroy(obj);
        }

        // �� ����� ������Ʈ
        foreach (RoomInfo roomInfo in roomList)
        {
            // RoomItem ������ �������� ����
            GameObject room = (GameObject)Instantiate(roomitem);
            room.transform.SetParent(scrollContants.transform, false); // ��ũ�� �����̳ʿ� �ڽ����� �߰�
            RoomDeta roomData = room.GetComponent<RoomDeta>(); // RoomDeta ������Ʈ ��������
            roomData.roomName = roomInfo.Name; // �� �̸� ����
            roomData.connectPlayer = roomInfo.PlayerCount; // ���� �÷��̾� �� ����
            roomData.maxPlayers = roomInfo.MaxPlayers; // �ִ� �÷��̾� �� ����
            roomData.DisplayRoomData(); // �� ������ UI�� ǥ��
            // RoomItem�� ��ư Ŭ�� �̺�Ʈ�� �������� ����
            roomData.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(delegate { OnClickRoomItem(roomData.roomName); });
        }
    }

    public void OnClickRoomItem(string roomname)
    {
        // �� ������ Ŭ�� �� ȣ��Ǵ� �޼���
        PhotonNetwork.NickName = userid.text; // ����� �г��� ����
        PlayerPrefs.SetString("USER_ID", userid.text); // ����� ID ����
        PhotonNetwork.JoinRoom(roomname); // �ش� �濡 ����
    }

    private void OnGUI()
    {
        // ����Ƽ �ǽð� ȭ�鿡 ���̺��� �ݹ� �޼���� ������
        GUILayout.Label(PhotonNetwork.InRoom.ToString()); // ���� �濡 �ִ��� ���θ� ���̺�� ǥ��
    }
}
