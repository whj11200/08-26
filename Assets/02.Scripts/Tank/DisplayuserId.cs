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
        // ���̵� ��ũ�� ǥ��
        userId.text = photonView.Owner.NickName;

    }
}
