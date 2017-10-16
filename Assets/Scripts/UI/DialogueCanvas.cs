using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DialogueInfo
{
    public DialogueInfo(float p_posX, float p_posY, string p_dialogue, NPCType p_nPCType = NPCType.None, Action p_callback = null)
    {
        Position = new Vector2(p_posX, p_posY);
        Dialogue = p_dialogue;
        NPCType = p_nPCType;
        Callback = p_callback;
    }

    public string Dialogue { get; set; }
    public Vector2 Position { get; set; }
    public NPCType NPCType { get; set; }
    public Action Callback { get; set; }

}

public class DialogueCanvas : MonoInstance<DialogueCanvas>
{
    [SerializeField] private RectTransform m_rectTrans;
    [SerializeField] private Image m_panel;
    [SerializeField] private Text m_dialogueText;
    [SerializeField] private TypewriterEffect m_typewriterEffect;

    private List<List<DialogueInfo>> m_dialogueBubbleSetList = new List<List<DialogueInfo>>();
    private Queue<DialogueInfo> m_dialogueBubbleQueue = new Queue<DialogueInfo>();

    private bool m_bIsCutscene = true;

    private Action m_callback = null;

    private Vector2 m_boxSizeOffset = new Vector2(100.0f, 150.0f);

    protected override void Awake()
    {
        base.Awake();
        List<DialogueInfo> dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "Sir Salisbury, honorable knight.Those worm infested barbarians will be here soon.", NPCType.BaronBacon));
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "Make sure they don't get through. Lest you be smoked.", NPCType.BaronBacon));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "You're doing well so far. Be wary of my grand daughter. She's a bit of trouble.", NPCType.BaronBacon));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "Our people must not mingle with their leafy hubris. Do not let them pass. They are dangerous.", NPCType.BaronBacon));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "I have heard of someone letting these... these... disgusting creatures through.", NPCType.BaronBacon));
        dialogueSet.Add(new DialogueInfo(-3.43f, -1.95f, "It better not had been you, Sir. Or I, Baron Bacon, will execute you myself.", NPCType.BaronBacon));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(2.61f, 2.93f, "....", NPCType.Bratwurst));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(2.61f, 2.93f, "These poor people.", NPCType.Bratwurst));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(2.61f, 2.93f, "The concept of compasion is alien to my grand father.", NPCType.Bratwurst));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(2.61f, 2.93f, "Every now and then, I will find a way to let them in. I hope you know what to do.", NPCType.Bratwurst));
        m_dialogueBubbleSetList.Add(dialogueSet);

        dialogueSet = new List<DialogueInfo>();
        dialogueSet.Add(new DialogueInfo(2.61f, 2.93f, "Do you enjoy this?", NPCType.Bratwurst));
        m_dialogueBubbleSetList.Add(dialogueSet);
    }

    public void UpdatePanelSize()
    {
        Vector2 newSize = m_dialogueText.rectTransform.sizeDelta + m_boxSizeOffset;
        newSize.y = Mathf.Clamp(newSize.y, 264.0f, float.PositiveInfinity);
        m_panel.rectTransform.sizeDelta = newSize;
        m_rectTrans.sizeDelta = newSize;
    }

    public void OpenDialogueBox(int p_index, bool p_bIsCutscene, Action p_callback = null)
    {
        m_bIsCutscene = p_bIsCutscene;

        m_callback = p_callback;

        m_panel.gameObject.SetActive(true);

        m_dialogueBubbleQueue.Clear();

        for (int i = 0; i < m_dialogueBubbleSetList[p_index].Count; i++)
        {
            m_dialogueBubbleQueue.Enqueue(m_dialogueBubbleSetList[p_index][i]);
        }

        NextDialogue();

        if (m_bIsCutscene)
        {
            PlayerPlatformerBehavior playerPlatformerBehavior = PlayerInput.Instance.GetComponent<PlayerPlatformerBehavior>();

            playerPlatformerBehavior.StopMoveX();
            playerPlatformerBehavior.PlayerDirInput = Vector2.zero;
            PlayerInput.Instance.enabled = false;
        }
        else
        {
            Invoke("NextDialogue", 5.0f);
        }
    }

    public void Update()
    {
        if (!m_panel.enabled) { return; }

        if(Input.GetButtonUp("Jump") && m_bIsCutscene)
        {
            DialogueCanvas.Instance.NextDialogue();
        }

        UpdatePanelSize();
    }

    public void NextDialogue()
    {
        AudioDialogueManager.Instance.StopAllVoices();
        m_typewriterEffect.ClearText();

        if (m_dialogueBubbleQueue == null) { return; }

        if (m_dialogueBubbleQueue.Count == 0)
        {
            m_panel.gameObject.SetActive(false);

            if(m_callback != null)
            {
                m_callback();
            }
            
            m_callback = null;
            return;
        }

        DialogueInfo dialogueInfo = m_dialogueBubbleQueue.Dequeue();
        transform.position = dialogueInfo.Position;
        m_dialogueText.text = dialogueInfo.Dialogue;
        m_typewriterEffect.ShowText(dialogueInfo.Dialogue);

        if(dialogueInfo.NPCType != NPCType.None)
        {
            AudioDialogueManager.Instance.PlayVoiceRandomLine(dialogueInfo.NPCType);
        }
        
        if (dialogueInfo.Callback != null)
        {
            dialogueInfo.Callback();
        }
    }
}
