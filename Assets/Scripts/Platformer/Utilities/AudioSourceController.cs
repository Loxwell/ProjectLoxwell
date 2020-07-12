using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AudioSourceController : MonoBehaviour
{
    public AudioClip[] audioClips;
    public AudioSource[] audioSources;

    static AudioSourceController instance;
    public static AudioSourceController Instance
    {
        get
        {
            if (!instance)
                Initialize();
            return instance;
        }
    }

    Dictionary<int, AudioSource> dicOfAudioSources;
    List<AudioSource> listWillBeDestroyedThing;

    void Awake()
    {
        Initialize();
    }

    void OnEnable()
    {
        StartCoroutine(CheckPlayingSourceProcess());
    }

    void OnDisable()
    {
        if (audioSources == null) return;
        for (int i = 0; i < audioSources.Length; ++i)
        {
            if (audioSources[i] == null || !audioSources[i].isPlaying) continue;
            audioSources[i].Stop();
        }
        
        dicOfAudioSources.Clear();
    }

    static void Initialize()
    {
        if (!instance) instance = FindObjectOfType<AudioSourceController>();
        if (instance != null)
        {
            if (instance.dicOfAudioSources == null)
                instance.dicOfAudioSources = new Dictionary<int, AudioSource>();
            if (instance.audioSources == null || instance.audioSources.Length == 0)
            {
                instance.audioSources = (from AudioSource a in instance.gameObject.GetComponentsInChildren<AudioSource>() where a.gameObject != instance.gameObject select a).ToArray();
                if (instance.audioSources.Length == 0)
                {
                    int _num = 10;
                    instance.audioSources = new AudioSource[_num];
                    for (int i = 0; i < _num; ++i)
                    {
                        GameObject _go = new GameObject("AudioSource");
                        _go.transform.parent = instance.transform;
                        _go.transform.localPosition = Vector3.zero;
                        instance.audioSources[i] = _go.AddComponent<AudioSource>();
                    }
                }
            }

            if (instance.listWillBeDestroyedThing == null)
                instance.listWillBeDestroyedThing = new List<AudioSource>();
        }
    }

    public AudioSource Play(int _ID, int _clipIndex)
    {
        if (audioClips.Length <= _clipIndex || dicOfAudioSources.ContainsKey(_ID)) 
            return null;

        for (int i = 0, len = audioSources.Length; i < len; ++i)
        {
            if (!audioSources[i].isPlaying)
            {
                audioSources[i].PlayOneShot(audioClips[_clipIndex]);
                if (!dicOfAudioSources.ContainsKey(_ID))
                    dicOfAudioSources.Add(_ID, audioSources[i]);
                
                return dicOfAudioSources[_ID];
            }
        }

        return null;
    }

    public void Play(int _id, int _clipIndex, bool _loop, float _vloume = 1f)
    {
        if (audioClips.Length <= _clipIndex) return;

        AudioClip _clip = audioClips[_clipIndex];
        AudioSource _source = Play(_id, _clip);
        if (_source != null)
        {
            _source.volume = _vloume;
            _source.loop = _loop;
            if (!_source.isPlaying)
                _source.Play();
        }
    }

    public void Play(int _id, AudioClip _clip, bool _loop, float _vloume = 1f)
    {
        if (_clip == null) return;
        AudioSource _source = Play(_id, _clip);
        if (_source != null)
        {
            _source.volume = _vloume;
            _source.loop = _loop;
            if (!_source.isPlaying)
                _source.Play();
        }
    }

    public AudioSource Play(int _id, AudioClip _clip)
    {
        if (_clip == null) return null;

        if (dicOfAudioSources.ContainsKey(_id))
        {
            return dicOfAudioSources[_id];
        }

        for (int i = 0, len = audioSources.Length; i < len; ++i)
        {
            if (!audioSources[i].isPlaying)
            {
                //   audioSources[i].clip = audioClips[_touchID];
                audioSources[i].clip = _clip;
                audioSources[i].Play();
                dicOfAudioSources.Add(_id, audioSources[i]);
                return audioSources[i];
            }
        }

        return null;
    }

    public void Play(AudioClip _clip)
    {
        if (_clip == null) return;
        AudioSource.PlayClipAtPoint(_clip, Vector3.zero);
    }

    public void Play(int i)
    {
        if (i >= audioClips.Length) return;
        Play(audioClips[i]);
    }

    public void Play(int i, Vector3 worldPos)
    {
        if (i >= audioClips.Length) return;
        AudioSource.PlayClipAtPoint(audioClips[i], worldPos);
    }

    public bool IsPlaying(int ID)
    {
        if (!dicOfAudioSources.ContainsKey(ID)) return false;
        return dicOfAudioSources[ID].isPlaying;
    }

    public void Stop(int ID, bool _adjust = true)
    {
        if (!dicOfAudioSources.ContainsKey(ID)) return;
        if(_adjust)
        {
            AudioSource _as = dicOfAudioSources[ID];
            StartCoroutine(StopProcess(_as));
        }else
        {
            dicOfAudioSources[ID].Stop();
        }
        dicOfAudioSources.Remove(ID);
    }

    IEnumerator StopProcess(AudioSource _as)
    {
        float _volume = _as.volume;
        do
        {

            _volume -= Time.deltaTime * 5f;
           
            if (_volume <= 0f) _volume = 0f;
            if (_as.clip == null) break;
            else
                _as.volume = _volume;
            yield return null;
        } while (_volume > 0f);

        if (_as.clip != null)
        {
            _as.Stop();
            _as.volume = 1f;
            _as.clip = null;
        }
    }

    IEnumerator CheckPlayingSourceProcess()
    {
        GameObject _go = this.gameObject;
        if (dicOfAudioSources != null)
        {
            do
            {
                foreach (KeyValuePair<int, AudioSource> _kv in dicOfAudioSources)
                {
                    if (!_kv.Value.isPlaying)
                    {
                        //    _kv.Value.clip = null;
                        dicOfAudioSources.Remove(_kv.Key);
                        break;
                    }
                }
                yield return null;
            } while (_go.activeInHierarchy);
        }
    }

    public static IEnumerator VolumeProcess(AudioSource _as, float from, float to, float duration)
    {
        float _elapsedTime = 0;
        do
        {
            _elapsedTime += Time.deltaTime;
            _as.volume = Mathf.Lerp(from, to, Mathf.Clamp01(_elapsedTime / duration));
            yield return null;
        } while (_elapsedTime < duration);
        _as.volume = to;
    }

    public static IEnumerator PlayingClipsProcess(AudioClip [] clips, Vector3 pos)
    {
        for(int i = 0; i< clips.Length; ++i)
        {
            AudioSource.PlayClipAtPoint(clips[i], pos);
            yield return new WaitForSeconds(clips[i].length + 0.001f);
        }
    }
}