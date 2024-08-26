using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMPlay : MonoBehaviour
{
    private Transform tr;
    public AudioClip bgm;
    void Start()
    {
        tr = transform;
        SoundManger.S_instance.BackGroundSound(tr.position, bgm, true);
    }
    //public void NextScene()
    //{
       
    // SceneManager.LoadScene("TankMainScene");
        
    //}
}
