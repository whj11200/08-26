using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // GameManager Ŭ������ �̱��� �ν��Ͻ�
    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
        set
        {
            // �ν��Ͻ��� null�� ��츸 ����
            if (instance == null)
                instance = value;
            // �̹� �ν��Ͻ��� ���� ��� ���� ������ �ν��Ͻ��� �ı�
            else if (instance != value)
                Destroy(value);
        }
    }

    [SerializeField] private List<Transform> spawnList; // ���� ����Ʈ ����Ʈ
    [SerializeField] private GameObject apachePrefab; // ������ Apache ������
    public Text txtConnect; // ���� ���¸� ǥ���� UI �ؽ�Ʈ
    [SerializeField] public Text textLogtext; // �α� �޽����� ǥ���� UI �ؽ�Ʈ
    public bool isGameOver = false; // ���� ���� ����
    public Text KillCounter;
    private int killscore = 0;

    private void Awake()
    {
        // �ν��Ͻ��� �����ϰ� �ı����� �ʵ��� ����
        Instance = this;
        DontDestroyOnLoad(gameObject);
        CreateTank(); // ��ũ ����
        PhotonNetwork.IsMessageQueueRunning = true; // �޽��� ť Ȱ��ȭ
        apachePrefab = Resources.Load<GameObject>("Apache"); // Apache ������ �ε�
    }

    void CreateTank()
    {
        // ���� ��ġ�� ��ũ ����
        float pos = Random.Range(-50f, 50f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 5F, pos), Quaternion.identity);
    }

    void Start()
    {
        // ���� ���� �� ���� ����Ʈ ����
        var spawnPoint = GameObject.Find("SpawnPoints").gameObject;
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(spawnList); // �ڽ� Ʈ�������� spawnList�� �߰�

        spawnList.RemoveAt(0); // ù ��° ���� ����Ʈ ����
        string msg = "\n<color=#00ff00>[" + PhotonNetwork.NickName + "]Connected</color>"; // ���� �޽��� ����
        photonView.RPC("LogMsg", RpcTarget.AllBuffered, msg); // ��� Ŭ���̾�Ʈ�� �޽��� ����

        // �ּ� ó���� �κ��� Apache ���� ����
        if (spawnList.Count > 0 && PhotonNetwork.IsMasterClient)
            StartCoroutine(CreateApache()); // Apache ���� �ڷ�ƾ ����
        //InvokeRepeating("CreateApaches", 0f, 3f); // ���� �������� Apache ����
    }

    IEnumerator CreateApache()
    {
        // ���� ������ �ƴ� �� Apache ����
        while (isGameOver == false)
        {
            int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length; // ���� Apache �� üũ
            if (count < 10) // Apache ���� 10�� �̸��� ���
            {
                yield return new WaitForSeconds(3.0f); // 3�� ���
                int idx = Random.Range(0, spawnList.Count); // ������ ���� ����Ʈ ����
               PhotonNetwork.InstantiateRoomObject("Apache", spawnList[idx].position, spawnList[idx].rotation,0,null); // Apache ����
            }
            else
            {
                yield return null; // Apache�� 10�� �̻��� ��� ���
            }
        }
    }
    [PunRPC]
    public void ApplyPlayerCountUpdate()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom; // ���� �� ���� ��������
        txtConnect.text = currentRoom.PlayerCount.ToString() + "/" + currentRoom.MaxPlayers.ToString(); // UI ������Ʈ
    }
    [PunRPC]
    void GetConnetPlayerCounter()
    {
        // ������ Ŭ���̾�Ʈ�� ��� ���� ���� �÷��̾� �� ������Ʈ
        if (PhotonNetwork.IsMasterClient)
        {
            //Room currentRoom = PhotonNetwork.CurrentRoom; // ���� �� ���� ��������
            //txtConnect.text = currentRoom.PlayerCount.ToString() + "/" + currentRoom.MaxPlayers.ToString(); // UI ������Ʈ
            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All); // �ٸ� Ŭ���̾�Ʈ�� ȣ��
        }
    }
    

    // �÷��̾ �濡 �������� �� ȣ��
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GetConnetPlayerCounter(); // �÷��̾� �� ������Ʈ
    }

    // �÷��̾ �濡�� ������ �� ȣ��
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnetPlayerCounter(); // �÷��̾� �� ������Ʈ
    }

    public void OnclickExit() // ������ ������ ��ư Ŭ�� �̺�Ʈ �Լ�
    {
        string msg = "\n<color=#ff0000>[" + PhotonNetwork.NickName + "]Disconnected</color>"; // ���� ���� �޽��� ����
        photonView.RPC("LogMsg", RpcTarget.AllBuffered, msg); // ��� Ŭ���̾�Ʈ�� �޽��� ����
        PhotonNetwork.LeaveRoom(); // �� ������
    }

    public override void OnLeftRoom() // �濡�� ���� ���� �� ȣ��Ǵ� �ݹ� �Լ�
    {
        SceneManager.LoadScene("Lobby"); // �κ� ������ ��ȯ
    }

    [PunRPC]
    void LogMsg(string msg)
    {
        textLogtext.text += msg; // �α� �޽��� ������Ʈ
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

    // �ּ� ó���� Apache ���� �ڷ�ƾ
    //IEnumerator CreateApaches()
    //{
    //    int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length; // ���� Apache �� üũ
    //    if (count < 10) // Apache ���� 10�� �̸��� ���
    //    {
    //        int idx = Random.Range(0, spawnList.Count); // ���� ���� ����Ʈ ����
    //        Instantiate(apachePrefab, spawnList[idx].position, spawnList[idx].rotation); // Apache ����
    //    }
    //    else
    //    {
    //        yield return null; // Apache�� 10�� �̻��� ��� ���
    //    }
    //}
}
