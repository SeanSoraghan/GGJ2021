using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    public enum NoteLength
    {
        Short,
        Medium,
        Long,
        NumLengths
    };

    public static int numNotes = 7;
    [Range(0.0f, 1.0f)]
    public float level = 0.6f;
    public int numSourcesPerNote = 3;
    private List<List<AudioSource>> audioSources = new List<List<AudioSource>>();
    private List<List<AudioClip>> noteClips = new List<List<AudioClip>>();
    List<List<int>> currentSourcePerNote = new List<List<int>>();

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

        for (int noteType = 0; noteType < (int)NoteLength.NumLengths; ++noteType)
        {
            noteClips.Add(new List<AudioClip>());
            audioSources.Add(new List<AudioSource>());
            currentSourcePerNote.Add(new List<int>());
            for (int i = 0; i < numNotes; ++i)
            {
                string clipName = "Note" + (i + 1);
                if (noteType == (int)NoteLength.Short)
                    clipName += "Short";
                else if (noteType == (int)NoteLength.Medium)
                    clipName += "Medium";

                noteClips[noteType].Add(Resources.Load("Audio/Music/" + clipName) as AudioClip);
                for (int s = 0; s < numSourcesPerNote; ++s)
                {
                    audioSources[noteType].Add(gameObject.AddComponent<AudioSource>());
                    audioSources[noteType][audioSources[noteType].Count - 1].playOnAwake = false;
                    audioSources[noteType][audioSources[noteType].Count - 1].loop = false;
                    audioSources[noteType][audioSources[noteType].Count - 1].clip = noteClips[noteType][noteClips[noteType].Count - 1];
                    audioSources[noteType][audioSources[noteType].Count - 1].volume = level;
                    if (noteType == (int)NoteLength.Short)
                        audioSources[noteType][audioSources[noteType].Count - 1].volume = 0.35f * level;
                }
                currentSourcePerNote[noteType].Add(0);
            }
        }
    }

    void Start()
    {
        
    }

    public void TriggerNote(int index, NoteLength noteLength, bool reverse = false)
    {
        int currentSource = currentSourcePerNote[(int)noteLength][index];
        int sourceIndex = index * numSourcesPerNote + currentSource;
        audioSources[(int)noteLength][sourceIndex].Stop();
        if (reverse)
        {
            audioSources[(int)noteLength][sourceIndex].pitch = -1.0f;
            audioSources[(int)noteLength][sourceIndex].time = audioSources[(int)noteLength][sourceIndex].clip.length - 0.01f;
        }
        else
        {
            audioSources[(int)noteLength][sourceIndex].pitch = 1.0f;
            audioSources[(int)noteLength][sourceIndex].time = 0.0f;
        }
        audioSources[(int)noteLength][sourceIndex].Play();

        currentSourcePerNote[(int)noteLength][index] = (currentSourcePerNote[(int)noteLength][index] + 1) % numSourcesPerNote;
    }
}
