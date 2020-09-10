using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField, Range(0, 1), Tooltip("BGMの音量")]
    private float _bgmVolume = 1;

    private AudioSource _bgmAudioSource;
    private List<AudioSource> _seAudioSourceList = new List<AudioSource>();

    [SerializeField]
    private int _seAudioSourceNum = 2;

    [System.Serializable]
    struct Audio
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField]
    private List<Audio> _bgmList;

    [SerializeField]
    private List<Audio> _seList;

    private void Awake()
    {
        base.Awake();

        _bgmAudioSource = gameObject.AddComponent<AudioSource>();

        for (int i = 0; i < _seAudioSourceNum; i++)
        {
            _seAudioSourceList.Add(gameObject.AddComponent<AudioSource>());
        }

        _bgmAudioSource.loop = true;
        _bgmAudioSource.volume = _bgmVolume;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBGM(string bgmName)
    {
        var bgmClip = _bgmList.Find(b => b.name == bgmName).clip;

        if (bgmClip == null)
        {
            Debug.LogError("bgmエラー " + bgmName);
            return;
        }

        _bgmAudioSource.clip = bgmClip;

        _bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }

    public void PlaySE(string seName)
    {
        var seClip = _seList.Find(s => s.name == seName).clip;

        if(seClip == null)
        {
            Debug.LogError("SEエラー " + seName);
            return;
        }

        foreach(var audioSource in _seAudioSourceList)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(seClip);
                break;
            }
        }
    }

    public void StopSE()
    {
        foreach (var audioSource in _seAudioSourceList)
        {
            audioSource.Stop();
        }
    }
}
