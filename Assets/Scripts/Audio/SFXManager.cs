using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class SFXManager : MonoBehaviour
{
    public enum SFXType
    {
        PlacementIncorrect,
        PlacementCorrect,
        Tilt,
        Erase,
        NumTypes
    }

    static string SFXTypeString(SFXType type)
    {
        switch (type)
        {
            case SFXType.PlacementIncorrect: return "PlacementIncorrect";
            case SFXType.PlacementCorrect: return "PlacementCorrect";
            case SFXType.Tilt: return "Tilt";
            case SFXType.Erase: return "Erase";
        }
        Debug.LogError("Audio SFX Manager: Unrecognized SFX Type");
        return "Unrecognized";
    }

    public static SFXManager Instance;

    [Range(0.0f, 1.0f)]
    public float level = 0.6f;

    public int numSources = 3;
    private List<AudioSource> audioSources = new List<AudioSource>();
    private Dictionary<string, AudioClip> Clips = new Dictionary<string, AudioClip>();
    int currentSourceIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    void Start()
    {
        for (int i = 0; i < (int)SFXType.NumTypes; ++i)
        {
            string clipName = SFXTypeString((SFXType)i);
            Clips.Add(clipName, Resources.Load("Audio/SFX/" + clipName) as AudioClip);
        }
        for (int i = 0; i < numSources; ++i)
        {
            audioSources.Add(gameObject.AddComponent<AudioSource>());
            audioSources[audioSources.Count - 1].playOnAwake = false;
            audioSources[audioSources.Count - 1].loop = false;
            audioSources[audioSources.Count - 1].volume = level;
        }
    }

    public void PlayClip(SFXType type)
    {
        audioSources[currentSourceIndex].Stop();
        audioSources[currentSourceIndex].clip = Clips[SFXTypeString(type)];
        audioSources[currentSourceIndex].Play();
        currentSourceIndex = (currentSourceIndex + 1) % numSources;
    }
}
