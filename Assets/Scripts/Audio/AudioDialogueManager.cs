using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NPCType
{
    None = -1,
    BaronBacon,
    Bratwurst
}

[System.Serializable]
public class AudioDialogueInfo
{
    [SerializeField] private NPCType m_npcType = NPCType.None;
    [SerializeField] private List<AudioClip> m_audioList = null;

    public NPCType NPCType
    {
        get { return m_npcType; }
    }
    public List<AudioClip> AudioList
    {
        get { return m_audioList; }
    }
}

[RequireComponent(typeof(AudioSource))]
public class AudioDialogueManager : MonoInstance<AudioDialogueManager>
{
    private AudioSource m_audioSource;

    [SerializeField] private List<AudioDialogueInfo> m_audioDialogueInfo;

    protected override void Awake()
    {
        base.Awake();
        m_audioSource = GetComponent<AudioSource>();
    }

    public void StopAllVoices()
    {
        m_audioSource.Stop();
    }

    public void PlayVoiceRandomLine(NPCType m_nPCType)
    {
        AudioDialogueInfo audioDialogueInfo = m_audioDialogueInfo.Where(adi => adi.NPCType == m_nPCType).SingleOrDefault();

        if(audioDialogueInfo == null) { return; }

        m_audioSource.PlayOneShot(audioDialogueInfo.AudioList[Random.Range(0, audioDialogueInfo.AudioList.Count)]);
    }

    public void PlayVoiceLine(NPCType m_nPCType, int p_index)
    {
        AudioDialogueInfo audioDialogueInfo = m_audioDialogueInfo.Where(adi => adi.NPCType == m_nPCType).SingleOrDefault();

        if (audioDialogueInfo == null) { return; }

        m_audioSource.PlayOneShot(audioDialogueInfo.AudioList[p_index]);
    }
}
