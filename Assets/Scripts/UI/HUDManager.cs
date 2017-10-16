using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoInstance<HUDManager>
{
    [SerializeField] GameObject m_panel;
    [SerializeField] Text m_healthText;

    [SerializeField] private Transform m_showPosRef;
    [SerializeField] private Transform m_hidePosRef;

    private bool m_bIsVisible = false;

    public Text HealthText
    {
        get { return m_healthText; }
    }

    public void ToggleHUD(bool p_bIsActive)
    {
        m_bIsVisible = p_bIsActive;
        if (m_bIsVisible)
        {
            iTween.MoveTo(m_panel.gameObject, iTween.Hash("y", m_showPosRef.position.y, "time", 0.5f, "easeType", iTween.EaseType.easeOutQuad));
        }
        else
        {
            iTween.MoveTo(m_panel.gameObject, iTween.Hash("y", m_hidePosRef.position.y, "time", 0.5f, "easeType", iTween.EaseType.easeInQuad));
        }
    }
}
