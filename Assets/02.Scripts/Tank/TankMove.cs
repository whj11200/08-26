using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// A: ���� ȸ�� D ���������� ȸ��  W ����  S ���� 
public class TankMove : MonoBehaviourPun, IPunObservable// �ۼ���
{
    [SerializeField]private Rigidbody rb;
    private float h = 0f, v = 0f;
    private Transform tr;
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    private Vector3 curpos = Vector3.zero; //���Ϳ� ���ʹϾ�����  ������ ������ ���Ź޴´�.
    private Quaternion currot = Quaternion.identity; 
    void Start()
    {
       
        tr = GetComponent<Transform>();
        // ������ ���� Ÿ��
        photonView.Synchronization = ViewSynchronization.Unreliable;
        //  photonView.ObservedComponents[0] �Ӽ��� TankScript ����
        photonView.ObservedComponents[0] = this;
        rb = GetComponent<Rigidbody>();
        // �ٸ� �����鵵 �ڱ� �ڽ��̱⶧���� �з��� �ؾ���
        rb.centerOfMass = new Vector3(0, -0.5f, 0);
        curpos = tr.position; currot = tr.rotation;
    }
    void Update()
    {
        // photonView���ǰ��̸�?
        if (photonView.IsMine)
        {
            h = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");

            tr.Translate(Vector3.forward * v * Time.deltaTime * moveSpeed);
            tr.Rotate(Vector3.up * h * Time.deltaTime * rotSpeed);
        }
        //else //  �ٸ� ��Ʈ��ũ ���� �� ����Ʈ
        //{
        //    tr.position = Vector3.Lerp(tr.position, curpos, moveSpeed * Time.deltaTime * 3);
        //    tr.rotation = Quaternion.Slerp(tr.rotation, currot, Time.deltaTime * 3f);

        //}

    }
    // �ڽ��� �̵��� ȸ���� ������ �۽��ϰ� �ٸ���Ʈ��ũ ������ �������� ���� �޴� �޼���
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//���� (����) �������� �۽���
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else // �ٸ� ��Ʈ��ũ ������ ������(����Ʈ)�� ���� �޴´�.
        {
            curpos = (Vector3)stream.ReceiveNext();
            currot = (Quaternion)stream.ReceiveNext();
        }
    }
}
