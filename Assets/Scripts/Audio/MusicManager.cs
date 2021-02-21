using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public int numSourcesPerNote = 3;
    private List<AudioSource> audioSources = new List<AudioSource>();
    public static int numNotes = 7;
    private List<AudioClip> noteClips = new List<AudioClip>();
    List<int> currentSourcePerNote = new List<int>();

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
        for (int i = 0; i < numNotes; ++i)
        {
            string clipName = "Note" + (i + 1);
            noteClips.Add(Resources.Load("Audio/Music/" + clipName) as AudioClip);
            for (int s = 0; s < numSourcesPerNote; ++s)
            {
                audioSources.Add(gameObject.AddComponent<AudioSource>());
                audioSources[audioSources.Count - 1].playOnAwake = false;
                audioSources[audioSources.Count - 1].loop = false;
                audioSources[audioSources.Count - 1].clip = noteClips[noteClips.Count - 1];
            }
            currentSourcePerNote.Add(0);
        }
    }

    public void TriggerNote(int index)
    {
        int currentSource = currentSourcePerNote[index];
        int sourceIndex = index * numSourcesPerNote + currentSource;
        audioSources[sourceIndex].Stop();
        audioSources[sourceIndex].Play();
        currentSourcePerNote[index] = (currentSourcePerNote[index] + 1) % numSourcesPerNote;
    }
}
