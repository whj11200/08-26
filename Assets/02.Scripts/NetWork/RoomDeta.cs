using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class RoomDeta : MonoBehaviourPun
{
                     
    [HideInInspector]
    public string roomName = string.Empty;//외부접근을 위해 public 이라고 선언 했지만 인스펙터에 노출 되지 않음
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
