using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// ���콺 �����ǿ� ���� ����ĳ��Ʈ�� ��� �ͷ� ���� Ŭ����
public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    private Transform tr; // ���� ��ü�� Transform ������Ʈ
    private float rotSpeed = 5f; // ȸ�� �ӵ�
    RaycastHit hit; // ����ĳ��Ʈ ����� ������ ����
    private Quaternion qur = Quaternion.identity; // �ͷ��� ȸ�� ������ ������ ����

    void Start()
    {
        tr = transform; // ���� ��ü�� Transform�� ������
        qur = tr.localRotation; // �ʱ� ȸ�� ���� ����
        photonView.Synchronization = ViewSynchronization.Unreliable; // PhotonView�� ����ȭ ����� ����
        photonView.ObservedComponents[0] = this; // ���� ������Ʈ�� ���� ������� ���
    }

    // Photon ��Ʈ��ũ ������ ����ȭ ó��
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // ���� ��ü�� �����͸� �����ϴ� ���
        {
            stream.SendNext(tr.localRotation); // ���� ȸ�� ���¸� ����
        }
        else // �����͸� �����ϴ� ���
        {
            qur = (Quaternion)stream.ReceiveNext(); // ������ ȸ�� ���¸� ����
        }
    }

    // �� �����Ӹ��� ȣ��Ǵ� ������Ʈ �޼ҵ�
    void Update()
    {
        if (photonView.IsMine) // ���� ��ü�� ���� �÷��̾��� ���� ���
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // ���콺 �����ǿ��� ���� ����
            // ī�޶󿡼� ���콺 ������ �������� ������ �߻� 
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.green); // ������ ��θ� �ð������� ǥ��

            if (Physics.Raycast(ray, out hit, 100f, 1 << 8)) // ���̰� Ư�� ���̾�(���⼭�� ���̾� 8)�� �´��� �˻�
            {
                // ���̰� �ͷ��� Ÿ���� �Ǵ� ������Ʈ�� �¾Ҵٸ� 
                Vector3 relative = tr.InverseTransformPoint(hit.point); // ���� ������ �ͷ� ���� ��ǥ�� ��ȯ
                // Mathf.Deg2Rad; �Ϲ� ������ ���� ������ ��ȯ�ϴ� ���

                // �� �� ���� ������ ����Ͽ� ������ �Ϲ� ������ ��ȯ
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                // ��ź��Ʈ �Լ��� Atan2�� ����Ͽ� ȸ���� ������ ���
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f); // �ͷ��� ȸ��
            }
        }
        else // ����Ʈ �÷��̾��� ���
        {
            // �ٸ� �÷��̾��� �ͷ� ȸ���� �ε巴�� ���̵��� ����
            tr.localRotation = Quaternion.Slerp(tr.localRotation, qur, Time.deltaTime * 3f);
        }
    }
}
