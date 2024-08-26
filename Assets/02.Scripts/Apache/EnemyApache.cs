using System.Collections;
using System.Collections.Generic;
//using UnityEditor.iOS;
using UnityEngine;
using Photon.Pun;

public class EnemyApache : MonoBehaviourPun
{
    [Header("Patrol")]
    public List<Transform>patrolList = new List<Transform>();
    private Transform tr=null;
    public float moveSpeed = 10.0f;
    public float rotSpeed = 15f;
    bool isSearch = true;
    int wayPointCount = 0;
    [SerializeField] Transform FirePos1;
    [SerializeField] Transform FirePos2;
    [SerializeField] GameObject A_bullet;
    [SerializeField] LeaserBeam[] leaserBeams;
    public GameObject expEffect;
    [SerializeField] MeshRenderer[] m_redner;
    float curDelay = 0f;
    float maxDelay =1f;
    //시작 후 적이 수십 명일때 네트워크 플레이어도 복수의 갯수 일떄
     // 적 개인 은 플레이엊우 가장 가까운 거리르 탐색해서 공격로직
    GameObject[] playertanks = null;
    private string apachtag = "Player";
    private string Tanktag = "Player";
    

    void Start()
    {
        m_redner = GetComponentsInChildren<MeshRenderer>();
        leaserBeams[0] = GetComponentsInChildren<LeaserBeam>()[0];
        leaserBeams[1] = GetComponentsInChildren<LeaserBeam>()[1];
        var patrolPoint = GameObject.Find("PatrolPoint");
        if(patrolPoint != null )
            patrolPoint.GetComponentsInChildren<Transform>(patrolList);
        patrolList.RemoveAt(0);
        tr = transform;
        A_bullet = Resources.Load<GameObject>("A_Bullet");
        curDelay = maxDelay;
        expEffect = Resources.Load<GameObject>("Explosion");
    }

    private void Awake()
    {
       photonView.Synchronization = ViewSynchronization.Unreliable;
        photonView.ObservedComponents[0] = this;
    }
    
    void Update()
    {
        if(isSearch)
        {
           
                WayPointMove();
            
            
        }
        else
        {
            Attack();
        }
        
    }
    void WayPointMove()
    {
        Vector3 PointDist = Vector3.zero;
        float dist = 0f;
       if (wayPointCount ==0)
        {
            PointDist = patrolList[0].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation,Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed *Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[0].position);
            if (dist <= 5.5f)
                wayPointCount = 1;
        }
        
        else if(wayPointCount ==1)
        {
            PointDist = patrolList[1].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[1].position);
            if (dist <= 5.5f)
                wayPointCount = 2;
        }
        
        else if (wayPointCount == 2)
        {
            PointDist = patrolList[2].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[2].position);
            if (dist <= 5.5f)
                wayPointCount = 3;
        }
        else if (wayPointCount == 3)
        {
            PointDist = patrolList[3].position - tr.position;
            tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(PointDist),
                Time.deltaTime * rotSpeed);
            tr.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            dist = Vector3.Distance(tr.position, patrolList[3].position);
            if (dist <= 5.5f)
                wayPointCount = 0;
        }

        Search();
    }
    void Search()
    {

        playertanks = GameObject.FindGameObjectsWithTag(Tanktag);
        Transform target = playertanks[0].transform;
        //첫번째 배열요소에 있는 탱크의 위치와 아파치 헬기의 거리를 잰다
        //첫번째 배열 요소의 탱크의 포지션은 거리를 재기 위한 기준이 될 뿐이다.

        float dist = (target.position - tr.position).sqrMagnitude;
        float dist2D;

        foreach (GameObject _tank in playertanks)
        {

            //첫번째 요소의 탱크 포지션 기준으로 아파치 헬기와 탱크들의 전체거리를 잰다.

            dist2D = (_tank.transform.position - tr.position).sqrMagnitude;

            if (dist2D < dist)
            {

                target = _tank.transform;
                dist = (_tank.transform.position - tr.position).sqrMagnitude;

            }


        }

        if (Vector3.Distance(target.transform.position,tr.position)<80f)
        {

            isSearch = false;

        }

    }
    void Attack()
    {
        playertanks = GameObject.FindGameObjectsWithTag(Tanktag);
        Transform target = playertanks[0].transform;
        //첫번째 배열요소에 있는 탱크의 위치와 아파치 헬기의 거리를 잰다
        //첫번째 배열 요소의 탱크의 포지션은 거리를 재기 위한 기준이 될 뿐이다.
        float dist = (target.position - tr.position).sqrMagnitude;
        float dist2D;

        foreach (GameObject _tank in playertanks)
        {

            //첫번째 요소의 탱크 포지션 기준으로 아파치 헬기와 탱크들의 전체거리를 잰다.

            dist2D = (_tank.transform.position - tr.position).sqrMagnitude;

            if (dist2D < dist)
            {
                target = _tank.transform;
                dist = (_tank.transform.position - tr.position).sqrMagnitude;
            }

        }

        Vector3 _normal = target.position - tr.position;

        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(_normal), Time.deltaTime * 3.0f);
        FireRay();
        if (Vector3.Distance(target.transform.position,tr.position)>80f)
        {

            isSearch = true;

        }
    }

    private void FireRay()
    {
        Ray ray = new Ray(FirePos1.position, FirePos1.forward * 100f);
        Ray ray1 = new Ray(FirePos2.position, FirePos2.forward * 100f);
        RaycastHit hit;
        //RaycastHit hit1;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8) ||
              Physics.Raycast(ray1, out hit, Mathf.Infinity, 1 << 8))
        {
            if(hit.collider.tag == Tanktag)
            {
                string tag = hit.collider.tag;

                hit.collider.transform.parent.SendMessage("OnDamageApache", tag, SendMessageOptions.DontRequireReceiver);
                
            }
            curDelay -= 0.01f;
            if (curDelay <= 0)
            {
                curDelay = maxDelay;
                leaserBeams[0].FireRay();
                leaserBeams[1].FireRay();
                ShowEffect(hit);
               
            }
        }
       
    }

    void ShowEffect(RaycastHit hit)
    {
        Vector3 hitPos = hit.point;
        Vector3 _normal =(FirePos1.position - hitPos).normalized;
        Quaternion rot  = Quaternion.FromToRotation(-Vector3.forward, _normal);
        GameObject hitEff = Instantiate(expEffect, hitPos, rot);
        Destroy(hitEff, 1.0f);

    }
    private void Fire()
    {
        //Instantiate(A_bullet, FirePos1.position, FirePos1.rotation);
        //Instantiate(A_bullet,FirePos2.position, FirePos2.rotation);
    }
    public void Die(string d)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("OnDeamageapacheRPC", RpcTarget.All, d);
        }
    }
    [PunRPC]
    void OnDeamageapacheRPC(string d)
    {
        GameManager.Instance.KillUp();
        Object effect = GameObject.Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect);
        Destroy(gameObject);

    }
   
   
    
}
