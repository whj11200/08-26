using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 마우스 포지션에 따라 레이캐스트를 쏘는 터렛 제어 클래스
public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr; // 현재 객체의 Transform 컴포넌트
    private float rotSpeed = 5f; // 회전 속도
    RaycastHit hit; // 레이캐스트 결과를 저장할 변수
    private Quaternion qur = Quaternion.identity; // 터렛의 회전 정보를 저장할 변수

    void Start()
    {
        tr = transform; // 현재 객체의 Transform을 가져옴
        qur = tr.localRotation; // 초기 회전 상태 저장
        photonView.Synchronization = ViewSynchronization.Unreliable; // PhotonView의 동기화 방식을 설정
        photonView.ObservedComponents[0] = this; // 현재 컴포넌트를 관찰 대상으로 등록
    }

    // Photon 네트워크 데이터 직렬화 처리
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 현재 객체가 데이터를 전송하는 경우
        {
            stream.SendNext(tr.localRotation); // 현재 회전 상태를 전송
        }
        else // 데이터를 수신하는 경우
        {
            qur = (Quaternion)stream.ReceiveNext(); // 수신한 회전 상태를 저장
        }
    }

    // 매 프레임마다 호출되는 업데이트 메소드
    void Update()
    {
        if (photonView.IsMine) // 현재 객체가 로컬 플레이어의 것인 경우
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 마우스 포지션에서 레이 생성
            // 카메라에서 마우스 포지션 방향으로 광선을 발사 
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green); // 레이의 경로를 시각적으로 표시

            if (Physics.Raycast(ray, out hit, 100f, 1 << 8)) // 레이가 특정 레이어(여기서는 레이어 8)에 맞는지 검사
            {
                // 레이가 터렛의 타겟이 되는 오브젝트에 맞았다면 
                Vector3 relative = tr.InverseTransformPoint(hit.point); // 맞은 지점을 터렛 로컬 좌표로 변환
                // Mathf.Deg2Rad; 일반 각도를 라디언 각도로 변환하는 상수

                // 두 점 간의 각도를 계산하여 라디언을 일반 각도로 변환
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                // 역탄젠트 함수인 Atan2를 사용하여 회전할 각도를 계산
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f); // 터렛을 회전
            }
        }
        else // 리모트 플레이어의 경우
        {
            // 다른 플레이어의 터렛 회전을 부드럽게 보이도록 보간
            tr.localRotation = Quaternion.Slerp(tr.localRotation, qur, Time.deltaTime * 3f);
        }
    }
}
