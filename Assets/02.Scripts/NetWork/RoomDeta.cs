using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class RoomDeta : MonoBehaviourPun
{
                     
    [HideInInspector]
    public string roomName = string.Empty;//�ܺ������� ���� public �̶�� ���� ������ �ν����Ϳ� ���� ���� ����
    [HideInInspector]
    public int connectPlayer = 0;
    [HideInInspector]
    public int maxPlayers = 10;
    public Text textRoomName;
    public Text textConnectInfo;

    public void DisplayRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text ="("+ connectPlayer.ToString()+"/"+ maxPlayers.ToString()+")";
    }
}
