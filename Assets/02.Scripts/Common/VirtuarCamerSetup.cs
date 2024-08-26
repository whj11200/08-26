using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VirtuarCamerSetup : MonoBehaviourPun
{
    CinemachineVirtualCamera virtualCamera;
    void Start()
    {
       virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

    }
    private void Update()
    {
        if (photonView.IsMine)
        {
            virtualCamera.LookAt = transform;
        }
   

    }

}
