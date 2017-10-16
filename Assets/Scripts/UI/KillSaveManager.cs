using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillSaveManager : MonoInstance<KillSaveManager>
{
    private int m_killCount = 0;
    private int m_saveCount = 0;
    
    [SerializeField] private Text m_killText;
    [SerializeField] private Text m_saveText;
    [SerializeField] private Transform m_saveFrame;
    private bool m_bIsSaveUIShown = false;

    [SerializeField]  private Transform m_saveShowPosRef;
    [SerializeField]  private Transform m_saveHidePosRef;

    protected override void Awake()
    {
        ResetKillSave();
    }

    private void UpdateKillText()
    {
        m_killText.text = m_killCount.ToString();
    }
    public void AddKill(int p_addKill)
    {
        m_killCount += p_addKill;
        UpdateKillText();
    }

    public void AddSave(int p_addSave)
    {
        m_saveCount += p_addSave;
        m_saveText.text = m_saveCount.ToString();

        m_bIsSaveUIShown = (m_saveCount > 1);

        Debug.Log(m_bIsSaveUIShown);

        if(!m_bIsSaveUIShown)
        {
            iTween.MoveTo(m_saveFrame.gameObject, iTween.Hash("y", m_saveShowPosRef.position.y, "time", 1.0f, "easeType", iTween.EaseType.easeOutBounce));
        }
    }

    public void ResetKillSave()
    {
        m_killCount = 0;
        UpdateKillText();

        m_saveCount = 0;
        m_saveText.text = m_saveCount.ToString();

        m_bIsSaveUIShown = false;
        m_saveFrame.position = m_saveHidePosRef.position;
    }
}
