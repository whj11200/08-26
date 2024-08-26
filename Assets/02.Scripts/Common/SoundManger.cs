using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManger : MonoBehaviour
{
    public bool isMute = false;//음소거
    public static SoundManger S_instance;
    private void Awake()
    {
        if(S_instance == null)
        {
            S_instance = this;
        }
        else if(S_instance != null)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    public void BackGroundSound(Vector3 pos, AudioClip bgm , bool isLoop) //  배경 음악
    {
        if (isMute) return;//음소거 중이라면 빠져나감
        GameObject soundObj = new GameObject("backGroundSound");
        soundObj.transform.position = pos; // 소리가 몰릴 위치
                                    // 오디서 소스 컴퍼넌트 추가
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.loop = isLoop;
        audioSource.clip = bgm;
        audioSource.maxDistance = 30f;
        audioSource.minDistance = 10f;
        audioSource.volume = 1f;
        audioSource.Play();
    }
   public void OtherPlaySound(Vector3 pos, AudioClip sfx , bool isLoop) // 폭파 소리
   {
        if (isLoop) return;//음소거 중이라면 빠져나감
        GameObject soundObj = new GameObject("SFX");
        soundObj.transform.position = pos; // 소리가 몰릴 위치
                                           // 오디서 소스 컴퍼넌트 추가
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.loop = sfx;
        audioSource.maxDistance = 30f;
        audioSource.minDistance = 10f;
        audioSource.volume = 1f;
        audioSource.Play();

    }
}
