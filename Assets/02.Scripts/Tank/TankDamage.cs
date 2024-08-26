using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

// 탱크 hp가 0 이하 일 때 잠시 메쉬렌더러를 비활성화 해서 다시 5초후에 다시 활성화 시킬예정
public class TankDamage : MonoBehaviourPun
{
    [SerializeField] private MeshRenderer[] m_Renderer;
    [SerializeField] private GameObject expEffect;
    private float intithp = 100;
    private float currhp = 0;
    private float demage = 0.01f;
    public Canvas hubCanvas;
    public Image hpbar;
    private  string playerTag = "Player";
    void Start()
    {
       m_Renderer = GetComponentsInChildren<MeshRenderer>();
       expEffect = Resources.Load<GameObject>("Explosion");
       currhp = intithp;
        hpbar.color = Color.green;
    }
    [PunRPC]
     void OnDeamageRPC(string a)
    {
        
        if( currhp > 0 && a==playerTag)
        {

            currhp -= 25f;
            hpbar.fillAmount = currhp / intithp;
            if (hpbar.fillAmount <= 0.4)
                hpbar.color = Color.red;
            if (hpbar.fillAmount <= 0.6) 
                hpbar.color = Color.yellow;
            if(currhp <= 0)
            {
                StartCoroutine(ExplosionTank());
            }

        }

    }
    [PunRPC]
    void OnDeamageapacheRPC(string b)
    {

        if (currhp > 0 && b == playerTag)
        {

            currhp -= demage;
           
            hpbar.fillAmount = currhp / intithp;
            if (hpbar.fillAmount <= 0.4)
                hpbar.color = Color.red;
            if (hpbar.fillAmount <= 0.6)
                hpbar.color = Color.yellow;
            if (currhp <= 0)
            {
                StartCoroutine(ExplosionTank());
            }
        }

    }

    IEnumerator ExplosionTank()
    {
        GameManager.Instance.KillUp();
        Object effect = GameObject.Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect);
        SetTankvisible(false);
        hubCanvas.enabled = false;
        yield return new WaitForSeconds(5.0f);
        currhp = intithp;
        SetTankvisible(true);
        hpbar.fillAmount = 1.0f;
        hpbar.color = Color.green;
        hubCanvas.enabled  = true;  

    }
    public void OnDamage(string a)
    {
      
        if (photonView.IsMine)
        {
            photonView.RPC("OnDeamageRPC", RpcTarget.All, a);

        }
    }
    public void OnDamageApache(string b)
    {
        if(photonView.IsMine)
        {
            photonView.RPC("OnDeamageapacheRPC", RpcTarget.All, b);
        }
    }
    void SetTankvisible(bool isvisable)
    {
        foreach(var tank in m_Renderer)
        {
            tank.enabled = isvisable;
        }
    }
}
