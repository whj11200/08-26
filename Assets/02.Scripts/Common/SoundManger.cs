using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManger : MonoBehaviour
{
    public bool isMute = false;//���Ұ�
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
    public void BackGroundSound(Vector3 pos, AudioClip bgm , bool isLoop) //  ��� ����
    {
        if (isMute) return;//���Ұ� ���̶�� ��������
        GameObject soundObj = new GameObject("backGroundSound");
        soundObj.transform.position = pos; // �Ҹ��� ���� ��ġ
                                    // ���� �ҽ� ���۳�Ʈ �߰�
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.loop = isLoop;
        audioSource.clip = bgm;
        audioSource.maxDistance = 30f;
        audioSource.minDistance = 10f;
        audioSource.volume = 1f;
        audioSource.Play();
    }
   public void OtherPlaySound(Vector3 pos, AudioClip sfx , bool isLoop) // ���� �Ҹ�
   {
        if (isLoop) return;//���Ұ� ���̶�� ��������
        GameObject soundObj = new GameObject("SFX");
        soundObj.transform.position = pos; // �Ҹ��� ���� ��ġ
                                           // ���� �ҽ� ���۳�Ʈ �߰�
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();

        audioSource.loop = sfx;
        audioSource.maxDistance = 30f;
        audioSource.minDistance = 10f;
        audioSource.volume = 1f;
        audioSource.Play();

    }
}
