using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// A: 왼쪽 회전 D 오른쪽으로 회전  W 전진  S 후진 
public class TankMove : MonoBehaviourPun, IPunObservable// 송수신
{
    [SerializeField]private Rigidbody rb;
    private float h = 0f, v = 0f;
    private Transform tr;
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    private Vector3 curpos = Vector3.zero; //벡터와 쿼터니언으로  동작을 변수로 수신받는다.
    private Quaternion currot = Quaternion.identity; 
    void Start()
    {
       
        tr = GetComponent<Transform>();
        // 데이터 전송 타입
        photonView.Synchronization = ViewSynchronization.Unreliable;
        //  photonView.ObservedComponents[0] 속성에 TankScript 연결
        photonView.ObservedComponents[0] = this;
        rb = GetComponent<Rigidbody>();
        // 다른 유저들도 자기 자신이기때문에 분류를 해야함
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        curpos = tr.position; currot = tr.rotation;
    }
    void Update()
    {
        // photonView나의것이면?
        if (photonView.IsMine)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        //else //  다른 네트워크 유저 즉 리모트
        //{
        //    tr.position = Vector3.Lerp(tr.position, curpos, moveSpeed * Time.deltaTime * 3);
        //    tr.rotation = Quaternion.Slerp(tr.rotation, currot, Time.deltaTime * 3f);

        //}

    }
    // 자신의 이동과 회전을 서버로 송신하고 다른네트워크 유저의 움직임을 수신 받는 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//로컬 (나의) 움직임을 송신함
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else // 다른 네트워크 유저의 움직임(리모트)을 수신 받는다.
        {
            curpos = (Vector3)stream.ReceiveNext();
            currot = (Quaternion)stream.ReceiveNext();
        }
    }
}
