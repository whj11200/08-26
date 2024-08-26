using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI; 

public class DisplayuserId : MonoBehaviourPun
{
    public Text userId;

    
    void Start()
    {
        // 아이디를 탱크에 표시
        userId.text = photonView.Owner.NickName;

    }
}
