using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AudioData
{
    public string id;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    class CacheInfo
    {
        public AudioSource audioSource;
        public bool active;
    }

    [SerializeField]
    private List<AudioData> _audioList = new List<AudioData>();
    private List<CacheInfo> _cacheList = new List<CacheInfo>();

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this);
    }

    public void PlayAudio(string id, bool loop, float time)
    {
        CacheInfo activeCache = null;
        bool hasActiveCache = false;
        foreach (var cache in _cacheList)
        {
            if(cache.active)
            {
                hasActiveCache = true;
                activeCache = cache;
            }
        }

        if(!hasActiveCache)
        {
            GameObject obj = new GameObject();
            obj.transform.parent = this.transform;
            AudioSource audioSource = obj.AddComponent<AudioSource>();
            activeCache = new CacheInfo() { audioSource = audioSource, active = false };
            _cacheList.Add(activeCache);
            audioSource.clip = _audioList.Find(item => item.id == id).audioClip;
            audioSource.loop = loop;
            audioSource.Play();
        }
        else
        {
            activeCache.audioSource.clip = _audioList.Find(item => item.id == id).audioClip;
            activeCache.audioSource.loop = loop;
            activeCache.audioSource.Play();
            activeCache.active = false;
        }

        if(!loop)
        {
            StartCoroutine(StopAudio(activeCache, time));
        }
    }

    IEnumerator StopAudio(CacheInfo target, float time)
    {
        yield return new WaitForSeconds(time);
        target.audioSource.Stop();
        target.active = true;
    }

    public void Clear()
    {
        _cacheList.Clear();
    }
}
