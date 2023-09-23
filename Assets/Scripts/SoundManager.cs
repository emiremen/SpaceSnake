using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource audioSource;


    [SerializeField] MyDictionary<string, AudioClip> audioList = new();

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        else
        {
            instance = this;
        }
    }


    public void PlayAudio(string soundName)
    {
        if (audioList.TryGetValue(soundName, out AudioClip sound))
        {

            audioSource.PlayOneShot(sound);
        }
        else
        {
            Debug.LogError("Ses dosyasý bulunamadý!");
        }
    }
}


[Serializable]
public class MyDictionary<TKey, TValue>
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();

    public void Add(TKey key, TValue value)
    {
        keys.Add(key);
        values.Add(value);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        int index = keys.IndexOf(key);
        if (index >= 0)
        {
            value = values[index];
            return true;
        }
        value = default(TValue);
        return false;
    }
}
